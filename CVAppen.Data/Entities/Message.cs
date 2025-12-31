using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CvAppen.Data
{
    public class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage ="Meddelandet måste innehålla text.")]
        [MaxLength(1000, ErrorMessage = "Meddelandet får inte överstiga 1000 tecken.")]
        public string Text {  get; set; }
        public string? FromUserId { get; set; }

        public User? FromUser { get; set; }
        public string SenderName { get; set; }
        [Required]
        public string ToUserId { get; set; } = null!;

        public User ToUser { get; set; }

        public bool IsRead { get; set; }
        public DateTime SentAt {  get; set; }
    }
}
