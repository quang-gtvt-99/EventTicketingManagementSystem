using System.ComponentModel.DataAnnotations;

namespace EventTicketingManagementSystem.Dtos
{
    public class CreateSeatDto
    {
        [Required]
        public int EventId { get; set; }
        [Required]
        public decimal Price { get; set; }

    }
}
