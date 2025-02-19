using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingManagementSystem.Response;
using EventTicketingMananagementSystem.Core.Models;
using EventTicketingMananagementSystem.Core.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace EventTicketingManagementSystem.Services.Services.Implements
{
    public class VnPayService : IVNPayService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBookingRepository _bookingRepository;

        public VnPayService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IBookingRepository bookingRepository)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _bookingRepository = bookingRepository;
        }
        public async Task<string> CreatePaymentUrl(Booking booking, string locale, string bankCode, string orderType)
        {
            var vnp_Url = _configuration["vnp_Url"];
            var vnp_TmnCode = _configuration["vnp_TmnCode"];
            var vnp_HashSecret = _configuration["vnp_HashSecret"];
            var vnp_Returnurl = _configuration["vnp_ReturnUrl"];

            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
            {
                throw new Exception("Vui lòng cấu hình các tham số: vnp_TmnCode, vnp_HashSecret");
            }

            var vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (booking.TotalAmount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", booking.BookingDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(_httpContextAccessor.HttpContext));
            vnpay.AddRequestData("vnp_Locale", string.IsNullOrEmpty(locale) ? "vn" : locale);
            vnpay.AddRequestData("vnp_OrderInfo", "thanh toan don hang: " + booking.Id);
            vnpay.AddRequestData("vnp_OrderType", orderType);
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", booking.Id.ToString());

            if (!string.IsNullOrEmpty(bankCode))
            {
                vnpay.AddRequestData("vnp_BankCode", bankCode);
            }

            return vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        }

        public async Task<PaymentResponse> ProcessVnPayReturn(IQueryCollection query)
        {

            if (query.Count > 0)
            {
                var vnp_HashSecret = _configuration["vnp_HashSecret"];
                var vnpayData = query.ToDictionary(q => q.Key, q => q.Value.ToString());
                var vnpay = new VnPayLibrary();
                foreach (var key in vnpayData.Keys)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, vnpayData[key]);
                    }
                }

                long bookingId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                string vnp_SecureHash = query["vnp_SecureHash"].ToString();
                string terminalId = query["vnp_TmnCode"].ToString();
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount"));
                string bankCode = query["vnp_BankCode"].ToString();
                string payDate = vnpay.GetResponseData("vnp_PayDate");

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    var model = new PaymentResponse
                    {
                        BookingId = bookingId,
                        VnPayTranId = vnpayTranId,
                        ResponseCode = vnp_ResponseCode,
                        TransactionStatus = vnp_TransactionStatus,
                        Amount = vnp_Amount / 100,
                        TerminalId = terminalId,
                        BankCode = bankCode,
                        PayDate =  DateTime.ParseExact(payDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                    };

                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        model.Message = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                    }
                    else
                    {
                        model.Message = $"Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: {vnp_ResponseCode}";
                    }

                    return model;
                }
                else
                {
                    return new PaymentResponse { Message = "Có lỗi xảy ra trong quá trình xử lý" };
                }
            }

            return new PaymentResponse { Message = "Không có dữ liệu trả về từ VNPAY" };
        }
    }
}