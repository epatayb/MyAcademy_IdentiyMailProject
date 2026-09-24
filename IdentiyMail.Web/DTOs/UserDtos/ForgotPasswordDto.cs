using System.ComponentModel.DataAnnotations;

namespace IdentiyMail.Web.DTOs.UserDtos
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; } = string.Empty;
    }
}
