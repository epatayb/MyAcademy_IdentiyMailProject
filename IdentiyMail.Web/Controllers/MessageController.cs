using IdentiyMail.Web.Context;
using IdentiyMail.Web.DTOs.UserMessageDtos;
using IdentiyMail.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace IdentiyMail.Web.Controllers
{
    [Authorize]
    public class MessageController(UserManager<AppUser> _userManager,
                                    AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            ViewBag.fullName = user.FirstName + " " + user.LastName;

            var messages = await _context.UserMessages
                .Include(x => x.Sender)
                .Where(x => x.ReceiverId == user.Id)
                .ToListAsync();

            return View(messages);
        }

        public IActionResult SendMail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMail(SendMailDto sendMailDto)
        {
            var sender = await _userManager.FindByNameAsync(User.Identity.Name);
            var receiver = await _userManager.FindByEmailAsync(sendMailDto.ReceiverMail);
            if (receiver is null)
            {
                ModelState.AddModelError(string.Empty, "Alıcı maili bulunamadı.");
                return View(sendMailDto);
            }
            var newMessage = new UserMessage
            {
                SendDate = DateTime.Now,
                ReceiverId = receiver.Id,
                SenderId = sender.Id,
                Subject = sendMailDto.Subject,
                Body = sendMailDto.Body,
            };

            _context.UserMessages.Add(newMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MailDetail(int id)
        {
            var message = await _context.UserMessages
                            .AsNoTracking()
                            .Include(x => x.Sender)
                            .FirstOrDefaultAsync(x => x.Id == id);

            message.IsRead = true;
            await _context.SaveChangesAsync();

            return View(message);
        }
    }
}
