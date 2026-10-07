namespace IdentiyMail.Web.ViewModels.CategoryViewModels
{
    public class CategoryDistributionItemViewModel
    {
        public string Name { get; set; } = string.Empty;

        public string ColorHex { get; set; } = string.Empty;

        public int MessageCount { get; set; }

        public double Percentage { get; set; }
    }
}
