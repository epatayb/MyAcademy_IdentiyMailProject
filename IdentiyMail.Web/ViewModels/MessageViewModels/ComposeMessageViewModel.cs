using IdentiyMail.Web.DTOs.CategoryDtos;
using IdentiyMail.Web.DTOs.UserMessageDtos;

namespace IdentiyMail.Web.ViewModels.MessageViewModels
{
    public class ComposeMessageViewModel
    {
        public SendMailDto Form { get; set; } = new();

        public List<CategoryOptionDto> Categories { get; set; } = new();
    }
}
