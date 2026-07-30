using IdentiyMail.Web.Entities;

namespace IdentiyMail.Web.DTOs.UserMessageDtos
{
    public class SendMailDto
    {
        public string ReceiverMail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }      
    }
}
