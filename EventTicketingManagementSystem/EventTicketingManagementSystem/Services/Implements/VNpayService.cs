using EventTicketingManagementSystem.Models;
using EventTicketingManagementSystem.Utilities;

namespace EventTicketingManagementSystem.Services.Implements
{
    public class VnPayService : IVNPayService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VnPayService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public string CreatePaymentUrl(Booking booking, string locale, string bankCode, string orderType)
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

            vnpay.AddRequestData("vnp_Version", "2.1.1");
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
    }
}