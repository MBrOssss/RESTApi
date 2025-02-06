using System.ComponentModel.DataAnnotations;

namespace RESTApi.Models.DTOs
{
    public class RegisterModel
    {
        [Required]
        [MaxLength(30)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Password { get; set; } = string.Empty;
    }
}
