using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;

public interface IVNPayService
{
    string CreatePaymentUrl(Booking booking, string locale, string bankCode, string orderType);
}
