namespace EventTicketingManagementSystem.Enums
{
    public enum VnPayResponseCode
    {
        Success = 00,                     // Giao dịch thành công
        Suspicious = 07,                  // Giao dịch bị nghi ngờ (lừa đảo, giao dịch bất thường)
        NotRegisteredInternetBanking = 09, // Thẻ/Tài khoản chưa đăng ký InternetBanking
        AuthenticationFailed = 10,         // Xác thực thông tin thẻ/tài khoản không đúng quá 3 lần
        Timeout = 11,                      // Hết hạn chờ thanh toán
        CardOrAccountLocked = 12,          // Thẻ/Tài khoản bị khóa
        WrongOtp = 13,                     // Nhập sai mật khẩu OTP
        CanceledByCustomer = 24,           // Khách hàng hủy giao dịch
        InsufficientFunds = 51,            // Tài khoản không đủ số dư
        ExceedDailyLimit = 65,             // Vượt quá hạn mức giao dịch trong ngày
        BankMaintenance = 75,              // Ngân hàng đang bảo trì
        ExceededWrongPasswordLimit = 79,   // Nhập sai mật khẩu thanh toán quá số lần quy định
        OtherError = 99                    // Lỗi khác (không có trong danh sách mã lỗi)
    }
}
