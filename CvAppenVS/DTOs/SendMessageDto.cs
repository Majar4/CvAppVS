using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CvAppen.Web.DTOs
{
    public class SendMessageDto
    {
        [Required(ErrorMessage = "Du måste välja en mottagare.")]
        public string ToUserId { get; set; } = null!;
        [Required(ErrorMessage = "Meddelandet får inte lämnas tomt.")]
        [MaxLength(1000, ErrorMessage = "Meddelandet får inte vara längre än 1000 tecken.")]
        public string Text { get; set; } = null!;
        public string? SenderName { get; set; }
        public bool IsAuthenticated { get; set; }
        }
}
