using EventTicketingMananagementSystem.Core.Models;

public interface IVNPayService
{
    string CreatePaymentUrl(Booking booking, string locale, string bankCode, string orderType);
}
