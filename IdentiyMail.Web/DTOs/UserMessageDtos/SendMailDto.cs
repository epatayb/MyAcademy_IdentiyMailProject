using System.ComponentModel.DataAnnotations;

namespace IdentiyMail.Web.DTOs.UserMessageDtos
{
    public class SendMailDto
    {
        [Required(ErrorMessage = "Alıcı mail alanı boş geçilemez.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string ReceiverMail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Konu alanı boş geçilemez.")]
        [StringLength(200, ErrorMessage = "Konu alanı en fazla 200 karakter olabilir.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mesaj içeriği zorunludur.")]
        public string Body { get; set; } = string.Empty;      
    }
}
