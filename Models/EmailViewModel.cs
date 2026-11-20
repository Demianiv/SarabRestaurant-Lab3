
using System.ComponentModel.DataAnnotations;

namespace SarabRestaurant.Models
{
    public class EmailViewModel
    {
        [Required(ErrorMessage = "Enter email address")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Enter your message")]
        public string Message { get; set; } = "";
    }
}
