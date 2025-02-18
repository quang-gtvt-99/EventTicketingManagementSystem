using EventTicketingManagementSystem.Response;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.AspNetCore.Http;

public interface IVNPayService
{
    Task<string> CreatePaymentUrl(Booking booking, string locale, string bankCode, string orderType);
    Task<PaymentResponse> ProcessVnPayReturn(IQueryCollection query);
}
