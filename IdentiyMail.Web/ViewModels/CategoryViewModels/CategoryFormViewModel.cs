using System.ComponentModel.DataAnnotations;

namespace IdentiyMail.Web.ViewModels.CategoryViewModels
{
    public class CategoryFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Kategori adı en fazla 50 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Açıklama en fazla 250 karakter olabilir.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Kategori rengi zorunludur.")]
        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Geçerli bir renk seçiniz.")]
        public string ColorHex { get; set; } = "#1D4ED8";
    }
}
