using System.ComponentModel.DataAnnotations;

namespace IdentiyMail.Web.DTOs.UserDtos
{
    public class ResetPasswordDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Yeni Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni Şifre Tekrar alanı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Yeni Şifre ve Yeni Şifre Tekrar eşleşmiyor.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
