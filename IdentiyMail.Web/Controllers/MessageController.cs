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
            var userId = GetCurrentUserId();

            #region Gelen kutusu temel sorgusu

            var inboxQuery = _context.UserMessages
                .Where(x => x.ReceiverId == userId && !x.IsDeletedByReceiver);

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

            var messageViewModels = await GetInboxMessageItemsAsync(filteredQuery);

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

        #region Mesaj gönderme işlemi
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

            var senderId = GetCurrentUserId();

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
                SenderId = senderId,
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
        #endregion

        #region Gelen mesaj detayı
        [HttpGet]
        public async Task<IActionResult> MailDetail(int id)
        {
            var userId = GetCurrentUserId();

            var message = await _context.UserMessages
                .Include(x => x.Sender)
                .FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == userId && !x.IsDeletedByReceiver && !x.IsPermanentlyDeletedByReceiver);

            if (message is null)
            {
                return NotFound();
            }

            if (!message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }

            var model = new MessageDetailViewModel
            {
                Id = message.Id,
                ContactFullName = $"{message.Sender.FirstName} {message.Sender.LastName}".Trim(),
                ContactEmail = message.Sender.Email ?? string.Empty,
                ContactProfileImageUrl = message.Sender.ProfileImageUrl,
                ContactInitials = CreateInitials(message.Sender.FirstName, message.Sender.LastName),
                Subject = message.Subject,
                Body = message.Body,
                SendDate = message.SendDate,
                IsIncoming = true,
                IsRead = message.IsRead,
                IsImportant = message.IsImportant
            };

            return View(model);
        }
        #endregion

        #region Gönderilen mesajları listeleme
        [HttpGet]
        public async Task<IActionResult> Sent()
        {
            var userId = GetCurrentUserId();

            var messages = await _context.UserMessages
                .AsNoTracking()
                .Where(x => x.SenderId == userId && !x.IsDeletedBySender && !x.IsPermanentlyDeletedBySender)
                .OrderByDescending(x => x.SendDate)
                .Select(x => new
                {
                    x.Id,
                    ReceiverFirstName = x.Receiver.FirstName,
                    ReceiverLastName = x.Receiver.LastName,
                    ReceiverEmail = x.Receiver.Email,
                    x.Receiver.ProfileImageUrl,
                    x.Subject,
                    x.Body,
                    x.SendDate,
                    x.IsRead
                })
                .ToListAsync();

            var model = messages
                .Select(x => new SentMessageViewModel
                {
                    Id = x.Id,
                    ReceiverFullName = $"{x.ReceiverFirstName} {x.ReceiverLastName}".Trim(),
                    ReceiverEmail = x.ReceiverEmail ?? string.Empty,
                    ReceiverProfileImageUrl = x.ProfileImageUrl,
                    ReceiverInitials = CreateInitials(x.ReceiverFirstName, x.ReceiverLastName),
                    Subject = x.Subject,
                    Preview = CreateMessagePreview(x.Body),
                    SendDate = x.SendDate,
                    IsRead = x.IsRead,
                })
                .ToList();

            return View(model);
        }
        #endregion

        #region Gönderilen mesaj detayı
        [HttpGet]
        public async Task<IActionResult> SentMailDetail(int id)
        {
            var userId = GetCurrentUserId();

            var message = await _context.UserMessages
                .AsNoTracking()
                .Include(x => x.Receiver)
                .FirstOrDefaultAsync(x => x.Id == id && x.SenderId == userId && !x.IsDeletedBySender && !x.IsPermanentlyDeletedBySender);

            if (message is null)
            {
                return NotFound();
            }

            var model = new MessageDetailViewModel
            {
                Id = message.Id,
                ContactFullName = $"{message.Receiver.FirstName} {message.Receiver.LastName}".Trim(),
                ContactEmail = message.Receiver.Email ?? string.Empty,
                ContactProfileImageUrl = message.Receiver.ProfileImageUrl,
                ContactInitials = CreateInitials(message.Receiver.FirstName, message.Receiver.LastName),
                Subject = message.Subject,
                Body = message.Body,
                SendDate = message.SendDate,
                IsIncoming = false,
                IsRead = message.IsRead,
            };

            return View(model);
        }
        #endregion

        #region Mesaj yanıtlama işlemi
        [HttpGet]
        public async Task<IActionResult> Reply(int id)
        {
            var userId = GetCurrentUserId();

            var message = await _context.UserMessages
                .AsNoTracking()
                .Include(x => x.Sender)
                .FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == userId && !x.IsDeletedByReceiver && !x.IsPermanentlyDeletedByReceiver);

            if (message is null)
            { return NotFound(); }

            var subject = message.Subject.StartsWith("RE:", StringComparison.OrdinalIgnoreCase) ? message.Subject : $"RE: {message.Subject}";

            var model = new SendMailDto
            {
                ReceiverMail = message.Sender.Email ?? string.Empty,
                Subject = subject
            };

            return View("SendMail", model);
        }
        #endregion

        #region Çöp kutusundaki mesajları listeleme
        [HttpGet]
        public async Task<IActionResult> Trash()
        {
            var userId = GetCurrentUserId();

            var messages = await _context.UserMessages
                .AsNoTracking()
                .Where(x => (x.ReceiverId == userId && x.IsDeletedByReceiver && !x.IsPermanentlyDeletedByReceiver) ||
                            (x.SenderId == userId && x.IsDeletedBySender && !x.IsPermanentlyDeletedBySender))
                .OrderByDescending(x => x.SendDate)
                .Select(x => new
                {
                    x.Id,
                    x.Subject,
                    x.Body,
                    x.SendDate,
                    x.IsRead,

                    x.SenderId,
                    x.ReceiverId,

                    SenderFirstName = x.Sender.FirstName,
                    SenderLastName = x.Sender.LastName,
                    SenderEmail = x.Sender.Email,
                    SenderProfileImageUrl = x.Sender.ProfileImageUrl,

                    ReceiverFirstName = x.Receiver.FirstName,
                    ReceiverLastName = x.Receiver.LastName,
                    ReceiverEmail = x.Receiver.Email,
                    ReceiverProfileImageUrl = x.Receiver.ProfileImageUrl,

                    x.DeletedBySenderAt,
                    x.DeletedByReceiverAt,
                    x.IsDeletedBySender,
                    x.IsDeletedByReceiver,
                    x.IsPermanentlyDeletedBySender,
                    x.IsPermanentlyDeletedByReceiver
                })
                .ToListAsync();

            var model = messages
                .Select(x =>
                {
                    var isIncoming = x.ReceiverId == userId && x.IsDeletedByReceiver && !x.IsPermanentlyDeletedByReceiver;

                    var firstName = isIncoming
                        ? x.SenderFirstName
                        : x.ReceiverFirstName;

                    var lastName = isIncoming
                        ? x.SenderLastName
                        : x.ReceiverLastName;

                    var deletedAt = isIncoming
                            ? x.DeletedByReceiverAt
                            : x.DeletedBySenderAt;

                    int? daysUntilDeletion = null;

                    if (deletedAt.HasValue)
                    {
                        var permanentDeleteDate = deletedAt.Value.Date.AddDays(30);
                        daysUntilDeletion = (permanentDeleteDate - DateTime.Now.Date).Days;
                    }

                    return new TrashMessageViewModel
                    {
                        Id = x.Id,

                        ContactFullName = $"{firstName} {lastName}".Trim(),

                        ContactEmail = isIncoming
                            ? x.SenderEmail ?? string.Empty
                            : x.ReceiverEmail ?? string.Empty,

                        ContactProfileImageUrl = isIncoming
                            ? x.SenderProfileImageUrl
                            : x.ReceiverProfileImageUrl,

                        ContactInitials = CreateInitials(firstName, lastName),

                        Subject = x.Subject,
                        Preview = CreateMessagePreview(x.Body),
                        SendDate = x.SendDate,
                        IsIncoming = isIncoming,
                        IsRead = x.IsRead,
                        DeletedAt = deletedAt,
                        DaysUntilDeletion = daysUntilDeletion,
                    };
                })
                .ToList();

            return View(model);
        }
        #endregion

        #region Mesaj önemli durumunu değiştirme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleImportant(int id, string? returnUrl)
        {
            var userId = GetCurrentUserId();

            var message = await _context.UserMessages
                .FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == userId && !x.IsDeletedByReceiver);

            if (message is null)
            {
                return NotFound();
            }

            message.IsImportant = !message.IsImportant;

            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Önemli mesajları listeleme
        public async Task<IActionResult> Important()
        {
            var userId = GetCurrentUserId();

            var query = _context.UserMessages
                .Where(x => x.ReceiverId == userId && x.IsImportant && !x.IsDeletedByReceiver);

            var model = await GetInboxMessageItemsAsync(query);

            return View(model);
        }
        #endregion

        #region Mesajı çöp kutusuna taşıma
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveToTrash(int id, string side, string? returnUrl)
        {
            var userId = GetCurrentUserId();

            var message = await _context.UserMessages
                .FirstOrDefaultAsync(x => x.Id == id);

            if (message is null) { return NotFound(); }

            var deletedAt = DateTime.Now;

            if (side == "receiver")
            {
                if (message.ReceiverId != userId || message.IsPermanentlyDeletedByReceiver)
                { return NotFound(); }

                if (!message.IsDeletedByReceiver)
                {
                    message.IsDeletedByReceiver = true;
                    message.DeletedByReceiverAt = deletedAt;
                }
            }
            else if (side == "sender")
            {
                if (message.SenderId != userId || message.IsPermanentlyDeletedBySender)
                { return NotFound(); }

                if (!message.IsDeletedBySender)
                {
                    message.IsDeletedBySender = true;
                    message.DeletedBySenderAt = deletedAt;
                }
            }
            else
            {
                return BadRequest();
            }

            await _context.SaveChangesAsync();
            
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Mesajı çöp kutusundan geri yükleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreFromTrash(int id, string side)
        {
            var userId = GetCurrentUserId();

            var message = await _context.UserMessages
                .FirstOrDefaultAsync(x => x.Id == id);

            if (message is null)
            {
                return NotFound();
            }

            if (side == "receiver")
            {
                if (message.ReceiverId != userId || !message.IsDeletedByReceiver || message.IsPermanentlyDeletedByReceiver)
                { return NotFound(); }

                message.IsDeletedByReceiver = false;
                message.DeletedByReceiverAt = null;
            }
            else if (side == "sender")
            {
                if (message.SenderId != userId || !message.IsDeletedBySender || message.IsPermanentlyDeletedBySender)
                { return NotFound(); }

                message.IsDeletedBySender = false;
                message.DeletedBySenderAt = null;
            }
            else
            {
                return BadRequest();
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Trash));
        }
        #endregion

        #region Mesajı çöp kutusundan silme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFromTrash(int id, string side)
        {
            var userId = GetCurrentUserId();

            var message = await _context.UserMessages
                .FirstOrDefaultAsync(x => x.Id == id);

            if (message is null) { return NotFound(); }

            if (side == "receiver")
            {
                if (message.ReceiverId != userId || !message.IsDeletedByReceiver || message.IsPermanentlyDeletedByReceiver)
                {
                    return NotFound();
                }

                message.IsPermanentlyDeletedByReceiver = true;
            }
            else if (side == "sender")
            {
                if (message.SenderId != userId || !message.IsDeletedBySender || message.IsPermanentlyDeletedBySender)
                {
                    return NotFound();
                }

                message.IsPermanentlyDeletedBySender = true;
            }
            else
            {
                return BadRequest();
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Trash));
        }
        #endregion

        #region Giriş yapan kullanıcı bilgileri
        private int GetCurrentUserId()
        {
            var userId = _userManager.GetUserId(User);

            if (!int.TryParse(userId, out var currentUserId))
            {
                throw new InvalidOperationException("Giriş yapan kullanıcının kimliği alınamadı.");
            }

            return currentUserId;
        }
        #endregion

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
            
            var normalizedBody = string.Join(" ", body.Split(new[] { ' ', '\r', '\n', '\t'}, StringSplitOptions.RemoveEmptyEntries));

            const int maxLength = 110;

            return normalizedBody.Length <= maxLength
                ? normalizedBody
                : $"{normalizedBody[..maxLength]}...";
        }

        private async Task<List<InboxMessageViewModel>> GetInboxMessageItemsAsync(IQueryable<UserMessage> query)
        {
            var messages = await query
                .AsNoTracking()
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
                    x.IsImportant,
                    x.IsRead
                })
                .ToListAsync();

            return messages
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
        }

        #endregion
    }
}
