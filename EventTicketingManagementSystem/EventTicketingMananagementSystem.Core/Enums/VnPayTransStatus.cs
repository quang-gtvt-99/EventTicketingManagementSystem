namespace EventTicketingManagementSystem.Enums
{
    public enum VnPayTransStatus
    {
        Success = 00,                  // Giao dịch thành công
        Pending = 01,                  // Giao dịch chưa hoàn tất
        Error = 02,                    // Giao dịch bị lỗi
        Reversed = 04,                 // Giao dịch đảo
        ProcessingRefund = 05,         // VNPAY đang xử lý giao dịch hoàn tiền
        RefundRequested = 06,          // VNPAY đã gửi yêu cầu hoàn tiền sang Ngân hàng
        FraudSuspected = 07,           // Giao dịch bị nghi ngờ gian lận
        RefundRejected = 09            // GD Hoàn trả bị từ chối
    }
}
