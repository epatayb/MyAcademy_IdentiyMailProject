using System.ComponentModel.DataAnnotations;

namespace IdentiyMail.Web.DTOs.UserDtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Ad alanı boş geçilemez.")]
        [StringLength(50, ErrorMessage = "Ad alanı en fazla 50 karakter olabilir.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı boş geçilemez.")]
        [StringLength(50, ErrorMessage = "Soyad alanı en fazla 50 karakter olabilir.")]
        public string LastName { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }

        [Required(ErrorMessage = "Email alanı boş geçilemez.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kullanıcı adı alanı boş geçilemez.")]
        [StringLength(20, ErrorMessage = "Kullanıcı adı alanı en fazla 20 karakter olabilir.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrar alanı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
