using EventTicketingManagementSystem.Data.Data.Repository.Implement;
using EventTicketingMananagementSystem.Core.Models;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingManagementSystem.Response;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystemTests.Data.Repository
{
    public class PaymentRepositoryTests : BaseRepositoryTests
    {
        private readonly PaymentRepository _paymentRepository;

        public PaymentRepositoryTests()
        {
            _paymentRepository = new PaymentRepository(Context);
            SeedData();
        }

        private void SeedData()
        {
            var booking = new Booking
            {
                Id = 1,
                EventId = 1,
                UserId = 1,
                BookingDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddHours(1),
            };

            var payment = new Payment
            {
                Id = 1,
                BookingId = 1,
                Amount = 100000,
                PaymentMethod = "VnPay",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                TransactionId = "123456"
            };

            Context.Bookings.Add(booking);
            Context.Payments.Add(payment);
            Context.SaveChanges();
        }

        [Fact]
        public async Task UpdatePaymentStatusAsync_ShouldUpdatePayment_WhenValidRequest()
        {
            // Arrange
            var paymentId = 1;
            var updateDto = new UpdatePaymentDto
            {
                Status = "Success",
                TransactionId = "123456",
                PaymentMethod = "VnPay"
            };

            // Act
            var result = await _paymentRepository.UpdatePaymentStatusAsync(paymentId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateDto.Status, result.Status);
            Assert.Equal(updateDto.TransactionId, result.TransactionId);
            Assert.Equal(updateDto.PaymentMethod, result.PaymentMethod);
        }

        [Fact]
        public async Task UpdatePaymentStatusAsync_ShouldThrowKeyNotFoundException_WhenPaymentNotFound()
        {
            // Arrange
            var nonExistentPaymentId = 999;
            var updateDto = new UpdatePaymentDto
            {
                Status = "Success",
                TransactionId = "123456",
                PaymentMethod = "VnPay"
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                async () => await _paymentRepository.UpdatePaymentStatusAsync(nonExistentPaymentId, updateDto)
            );
        }

        [Fact]
        public async Task DeleteExpiredBookingAsync_ShouldDeletePaymentAndBooking_WhenExpired()
        {
            // Arrange
            var expiredBooking = new Booking
            {
                Id = 2,
                EventId = 1,
                UserId = 1,
                BookingDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddHours(-1), // Expired booking
                TotalAmount = 100000,
                Quantity = 1,
                Subtotal = 100000
            };

            var expiredPayment = new Payment
            {
                Id = 2,
                BookingId = 2,
                Amount = 100000,
                PaymentMethod = "VnPay", // Required property
                Status = "Pending",
                TransactionId = "123456", // Required property
                CreatedAt = DateTime.UtcNow
            };

            Context.Bookings.Add(expiredBooking);
            Context.Payments.Add(expiredPayment);
            await Context.SaveChangesAsync();

            // Act
            var result = await _paymentRepository.DeleteExpiredBookingAsync(expiredPayment.Id);

            // Assert
            Assert.True(result);
            Assert.Null(await Context.Payments.FindAsync(expiredPayment.Id));
            Assert.Null(await Context.Bookings.FindAsync(expiredBooking.Id));
        }
        
        [Fact]
        public async Task CreatePaymentAsync_ShouldCreateNewPayment_WhenValidRequest()
        {
            // Arrange
            var paymentResponse = new PaymentResponse
            {
                BookingId = 1,
                Amount = 200000,
                TransactionStatus = "00",
                VnPayTranId = 123456
            };

            // Act
            var result = await _paymentRepository.CreatePaymentAsync(paymentResponse);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paymentResponse.BookingId, result.BookingId);
            Assert.Equal(paymentResponse.Amount, result.Amount);
            Assert.Equal("Success", result.Status);
            Assert.Equal("VnPay", result.PaymentMethod);
            Assert.Equal(paymentResponse.VnPayTranId.ToString(), result.TransactionId);
        }

        [Fact]
        public async Task DeleteExpiredBookingAsync_ShouldReturnFalse_WhenBookingNotExpired()
        {
            // Arrange
            var paymentId = 1; // Using the non-expired payment from SeedData

            // Act
            var result = await _paymentRepository.DeleteExpiredBookingAsync(paymentId);

            // Assert
            Assert.False(result);
            Assert.NotNull(await Context.Payments.FindAsync(paymentId));
            Assert.NotNull(await Context.Bookings.FindAsync(1));
        }
    }
}