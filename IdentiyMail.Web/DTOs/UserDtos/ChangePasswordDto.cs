using System.ComponentModel.DataAnnotations;

namespace IdentiyMail.Web.DTOs.UserDtos
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Mevcut şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni şifre tekrar alanı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Yeni şifre ve tekrar şifre eşleşmiyor.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
