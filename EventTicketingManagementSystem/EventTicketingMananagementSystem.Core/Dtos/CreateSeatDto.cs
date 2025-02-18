using System.ComponentModel.DataAnnotations;

namespace EventTicketingMananagementSystem.Core.Dtos
{
    public class CreateSeatDto
    {
        [Required]
        public int EventId { get; set; }
        [Required]
        public decimal Price { get; set; }

    }
}
