using EventTicketingManagementSystem.API.Request;
using EventTicketingManagementSystem.API.Response;
using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingManagementSystem.Response;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;
using System.Globalization;
using System.Text;

namespace EventTicketingManagementSystem.Services.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ISendMailService _sendEmailService;
        private readonly ICacheService _cacheService;

        public UserService(IUserRepository userRepository, IBookingRepository bookingRepository, ICurrentUserService currentUserService, IPaymentRepository paymentRepository, ITicketRepository ticketRepository, ISendMailService sendEmailService, ICacheService cacheService)
        {
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _paymentRepository = paymentRepository;
            _ticketRepository = ticketRepository;
            _sendEmailService = sendEmailService;
            _cacheService = cacheService;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<UserInfoDto> GetUserProfileAsync()
        {
            if (string.IsNullOrEmpty(_currentUserService.Id)
                || !int.TryParse(_currentUserService.Id, out int userId))
            {
                throw new Exception("User id not found.");
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var bookings = await _bookingRepository.GetBookingInfosByUserIdAsync(user.Id);

            return new UserInfoDto()
            {
                Bookings = bookings,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userRepository.UserEmailExisted(request.Email);
            if (existingUser == true)
            {
                throw new Exception("Email already exists.");
            }

            var user = new User
            {
                Email = request.Email.ToLower(),
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Status = "Active"
            };
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangeAsync();

            var userAdded = await _userRepository.FindByEmailAsync(user.Email);
            if (userAdded == null) throw new Exception($"User not found with email {user.Email}.");

            await _userRepository.AssignRoleAsync(userAdded.Id, RoleConsts.User);

            await _userRepository.SaveChangeAsync();

            return new RegisterResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName
            };
        }
        public async Task<Booking> CreateBookingAsync(CreateBookingDto bookingRequestDto, int loggedInUserId)
        {
            await _cacheService.InvalidCacheAsync(CacheKeyConsts.UPCOMING_EVENTS);
            return await _bookingRepository.CreateBookingAsync(bookingRequestDto, loggedInUserId);
        }
        public async Task<Payment> UpdatePaymentStatusAsync(int paymentId, UpdatePaymentDto requestDto)
        {
            return await _paymentRepository.UpdatePaymentStatusAsync(paymentId, requestDto);
        }
        public async Task<bool> DeleteExpiredBookingAsync(int paymentId)
        {
            return await _paymentRepository.DeleteExpiredBookingAsync(paymentId);
        }
        public async Task<List<Ticket>> CreateTicketsAsync(int bookingId)
        {
            return await _ticketRepository.CreateTicketsAsync(bookingId);
        }
        private async Task<CommonMessageResponse> ChangePassword(ChangePasswordDto request)
        {
            var currentUser = request.User;

            string? otpCache = null;
            if (await _cacheService.IsCacheKeyExistAsync($"{CacheKeyConsts.OneTimePassword}:{currentUser.Email}"))
            {
                otpCache = await _cacheService.GetAsync<string>($"{CacheKeyConsts.OneTimePassword}:{currentUser.Email}");
            }

            bool isPasswordValid = VerifyPasswordHash(request.OldPassword, currentUser.PasswordHash);
            bool isOtpValid = otpCache != null && VerifyPasswordHash(request.OldPassword, otpCache);

            if (!isPasswordValid && !isOtpValid)
            {
                return new CommonMessageResponse
                {
                    IsSuccess = false,
                    Message = "Mật khẩu cũ không đúng."
                };
            }

            if (isOtpValid)
            {
                // Invalidate the OTP cache
                await _cacheService.InvalidCacheAsync($"{CacheKeyConsts.OneTimePassword}:{currentUser.Email}");
            }

            if (request.NewPassword != request.ConfirmedNewPassword)
            {
                return new CommonMessageResponse
                {
                    IsSuccess = false,
                    Message = "Mật khẩu mới và xác nhận mật khẩu không khớp."
                };
            }

            currentUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            return new CommonMessageResponse
            {
                IsSuccess = true,
                Message = "Đổi mật khẩu thành công."
            };
        }

        public async Task<CommonMessageResponse> UpdateUserProfileAsync(UpdateUserProfileRequest request)
        {
            if (!int.TryParse(_currentUserService.Id, out int userId))
            {
                return new CommonMessageResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy ID người dùng."
                };
            }

            var currentUser = await _userRepository.GetByIdAsync(userId);

            if (currentUser == null)
            {
                return new CommonMessageResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy người dùng."
                };
            }

            currentUser.FullName = request.FullName;
            currentUser.PhoneNumber = request.PhoneNumber;
            currentUser.Email = request.Email;

            if (request.AllowChangePassword)
            {
                var changePasswordRequest = new ChangePasswordDto
                {
                    OldPassword = request.OldPassword,
                    NewPassword = request.NewPassword,
                    ConfirmedNewPassword = request.ConfirmedNewPassword,
                    User = currentUser
                };
                var changePasswordResponse = await ChangePassword(changePasswordRequest);
                if (!changePasswordResponse.IsSuccess)
                {
                    return changePasswordResponse;
                }
            }

            _userRepository.Update(currentUser);
            await _userRepository.SaveChangeAsync();

            return new CommonMessageResponse
            {
                IsSuccess = true,
                Message = "Cập nhật hồ sơ người dùng thành công!"
            };
        }

        public async Task<CommonMessageResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var isUserExist = await _userRepository.UserEmailExisted(request.Email);

            if (isUserExist == false)
            {
                return new CommonMessageResponse
                {
                    Message = "Email không tồn tại trong hệ thống!",
                    IsSuccess = false
                };
            }

            var (otp, hashedOtp) = GeneratePassword();

            var emailBody = $@"
                <html>
                <body>
                    <h2>Yêu cầu đặt lại mật khẩu</h2>
                    <p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu của bạn.</p>
                    <p>Mật khẩu dùng một lần (OTP) của bạn là: <strong>{otp}</strong></p>
                    <p>Vui lòng sử dụng OTP này để đăng nhập trong vòng 30 phút kể từ khi nhận được email này. Sau khi đăng nhập, vui lòng thay đổi mật khẩu của bạn trong phần 'Hồ sơ cá nhân'.</p>
                    <p>Nếu mật khẩu không hoạt động, vui lòng thực hiện lại quy trình đặt lại mật khẩu để đảm bảo tính bảo mật của ứng dụng.</p>
                </body>
                </html>";

            await _sendEmailService.SendEmailAsync(
                request.Email,
                "Đặt lại mật khẩu",
                emailBody,
                true // Set isHtml to true
            );

            await _cacheService.SetAsync(
                $"{CacheKeyConsts.OneTimePassword}:{request.Email}",
                hashedOtp,
                30 * 60);

            return new CommonMessageResponse
            {
                Message = "OTP đã được gửi đến email của bạn!",
                IsSuccess = true
            };
        }

        private (string Password, string HashedPassword) GeneratePassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var random = new Random();
            var password = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }

            var plainPassword = password.ToString();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            return (plainPassword, hashedPassword);
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
        public async Task SendEmailToId(PaymentResponse response, int userId)
        {
            var toEmail = _userRepository.GetEmailByIdAsync(userId);

            if (string.IsNullOrEmpty(toEmail))
            {
                throw new Exception($"Không tìm thấy email cho người dùng có ID: {userId}");
            }

            var body = $@"
                    <html>
                    <body>
                        <h2>Thông tin giao dịch</h2>
                        <table border='1' cellpadding='5' cellspacing='0'>
                            <tr>
                                <td><strong>Mã đặt chỗ</strong></td>
                                <td>{response.BookingId}</td>
                            </tr>
                            <tr>
                                <td><strong>Mã giao dịch VNPAY</strong></td>
                                <td>{response.VnPayTranId}</td>
                            </tr>
                            <tr>
                                <td><strong>Thời gian thanh toán</strong></td>
                                <td>{response.PayDate}</td>
                            </tr>
                            <tr>
                                <td><strong>Trạng thái giao dịch</strong></td>
                                <td>{response.TransactionStatus}</td>
                            </tr>
                            <tr>
                                <td><strong>Số tiền</strong></td>
                                <td>{response.Amount.ToString("C", new CultureInfo("vi-VN"))}</td>
                            </tr>
                            <tr>
                                <td><strong>Ngân hàng thanh toán</strong></td>
                                <td>{response.BankCode}</td>
                            </tr>
                            <tr>
                                <td><strong>Thông báo</strong></td>
                                <td>{response.Message}</td>
                            </tr>
                        </table>
                        <br/>
                        <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>
                    </body>
                    </html>";

            await _sendEmailService.SendEmailAsync(
                    toEmail,
                    $"Finiko - Đơn hàng mới: {response.BookingId}",
                    body,
                    true // Set isHtml to true
                );
            }
        }
    }
