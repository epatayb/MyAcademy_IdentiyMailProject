using IdentiyMail.Web.Context;
using IdentiyMail.Web.DTOs.UserMessageDtos;
using IdentiyMail.Web.Entities;
using IdentiyMail.Web.ViewModels.MessageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace IdentiyMail.Web.Controllers
{
    [Authorize]
    public class MessageController(UserManager<AppUser> _userManager, AppDbContext _context) : Controller
    {
        #region Gelen kutusu listeleme ve filtreleme
        public async Task<IActionResult> Index(string status = "all")
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null) 
            {
                return RedirectToAction("Login", "Auth");
            }

            #region Gelen kutusu temel sorgusu

            var inboxQuery = _context.UserMessages
                .AsNoTracking()
                .Where(x => x.ReceiverId == user.Id);

            var totalCount = await inboxQuery.CountAsync();

            var unreadCount = await inboxQuery.CountAsync(x => !x.IsRead);

            #endregion

            #region Okunma durumuna göre filtreleme

            status = status?.ToLowerInvariant() switch
            {
                "unread" => "unread",
                "read" => "read",
                _ => "all"
            };

            var filteredQuery = status switch
            {
                "unread" => inboxQuery.Where(x => !x.IsRead),
                "read" => inboxQuery.Where(x => x.IsRead),
                _ => inboxQuery
            };

            #endregion

            #region Mesajları ViewModele dönüştürme

            var messages = await filteredQuery
                .OrderByDescending(x => x.SendDate)
                .Select(x => new
                {
                    x.Id,

                    SenderFirstName = x.Sender.FirstName,
                    SenderLastName = x.Sender.LastName,
                    SenderEmail = x.Sender.Email,
                    x.Sender.ProfileImageUrl,

                    x.Subject,
                    x.Body,
                    x.SendDate,
                    x.IsRead,
                    x.IsImportant
                })
                .ToListAsync();

            var messageViewModels = messages
                .Select(x => new InboxMessageViewModel
                {
                    Id = x.Id,
                    SenderFullName = $"{x.SenderFirstName} {x.SenderLastName}".Trim(),
                    SenderEmail = x.SenderEmail ?? string.Empty,
                    SenderProfileImageUrl = x.ProfileImageUrl,
                    SenderInitials = CreateInitials(x.SenderFirstName, x.SenderLastName),
                    Subject = x.Subject,
                    Preview = CreateMessagePreview(x.Body),
                    SendDate = x.SendDate,
                    IsRead = x.IsRead,
                    IsImportant = x.IsImportant
                })
                .ToList();

            #endregion

            #region Gelen kutusu ekran modelini hazırlama

            var model = new InboxViewModel
            {
                Messages = messageViewModels,
                TotalCount = totalCount,
                UnreadCount = unreadCount,
                Status = status
            };

            #endregion

            return View(model);
        }
        #endregion

        public IActionResult SendMail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMail(SendMailDto sendMailDto)
        {
            if (!ModelState.IsValid)
            {
                return View(sendMailDto);
            }

            var sender = await _userManager.GetUserAsync(User);

            if (sender is null)
            {
                return RedirectToAction("Login", "Auth");
            }

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
                IsRead = false,
                IsImportant = false
            };

            _context.UserMessages.Add(newMessage);
            await _context.SaveChangesAsync();

            TempData["MessageSuccess"] = "Mesajınız başarıyla gönderildi.";

            return RedirectToAction(nameof(Sent));
        }

        public async Task<IActionResult> MailDetail(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var message = await _context.UserMessages
                .Include(x => x.Sender)
                .FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == user.Id);

            if (message is null)
            {
                return NotFound();
            }

            if (!message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
            return View(message);
        }

        public async Task<IActionResult> Sent()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var messages = await _context.UserMessages
                .Include(x => x.Receiver)
                .Where(x => x.SenderId == user.Id)
                .OrderByDescending(x => x.SendDate)
                .ToListAsync();

            return View(messages);
        }

        [HttpGet]
        public async Task<IActionResult> SentMailDetail(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var message = await _context.UserMessages
                .Include(x => x.Receiver)
                .FirstOrDefaultAsync(x => x.Id == id && x.SenderId == user.Id);
            if (message is null)
            {
                return NotFound();
            }

            return View(message);
        }

        #region Mesaj Listeleme Yardımcı Metotları
        private static string CreateInitials(string firstName, string lastName)
        {
            var firstNameInitial = string.IsNullOrWhiteSpace(firstName)
                ? string.Empty
                : firstName[..1].ToUpper();

            var lastNameInitial = string.IsNullOrWhiteSpace(lastName)
                ? string.Empty
                : lastName[..1].ToUpper();

            return $"{firstNameInitial}{lastNameInitial}";
        }

        private static string CreateMessagePreview(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return "Mesaj içeriği bulunmuyor.";
            }
            
            var normalizedBody = string.Join(" ", body.Split(new[] { ' ', '\r', '\n', 't'}, StringSplitOptions.RemoveEmptyEntries));

            const int maxLength = 110;

            return normalizedBody.Length <= maxLength
                ? normalizedBody
                : $"{normalizedBody[..maxLength]}...";
        }
        #endregion
    }
}
