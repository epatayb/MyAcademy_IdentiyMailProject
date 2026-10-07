using IdentiyMail.Web.Context;
using IdentiyMail.Web.ViewModels.CategoryViewModels;
using IdentiyMail.Web.ViewModels.MessageViewModels;
using Microsoft.EntityFrameworkCore;

namespace IdentiyMail.Web.Services
{
    public class MessageQueryService(AppDbContext _context) : IMessageQueryService
    {
        public async Task<List<MessageSearchItemViewModel>> SearchAsync(int userId, string query)
        {
            query = query.Trim();

            if (string.IsNullOrWhiteSpace(query))
            {
                return new();
            }

            var messages = await _context.UserMessages
                .AsNoTracking()
                .Where(x =>
                    (
                        x.ReceiverId == userId &&
                        !x.IsDeletedByReceiver &&
                        !x.IsPermanentlyDeletedByReceiver
                    )
                    ||
                    (
                        x.SenderId == userId &&
                        !x.IsDeletedBySender &&
                        !x.IsPermanentlyDeletedBySender
                    ))
                .Where(x =>
                    x.Subject.Contains(query) ||
                    x.Body.Contains(query) ||

                    x.Sender.FirstName.Contains(query) ||
                    x.Sender.LastName.Contains(query) ||
                    (x.Sender.Email != null &&
                     x.Sender.Email.Contains(query)) ||

                    x.Receiver.FirstName.Contains(query) ||
                    x.Receiver.LastName.Contains(query) ||
                    (x.Receiver.Email != null &&
                     x.Receiver.Email.Contains(query)) ||

                    x.CategoryAssignments.Any(a =>
                        a.UserId == userId &&
                        a.Category.Name.Contains(query)))
                .OrderByDescending(x => x.SendDate)
                .Select(x => new
                {
                    x.Id,
                    x.Subject,
                    x.Body,
                    x.SendDate,

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

                    CategoryName = x.CategoryAssignments
                        .Where(a => a.UserId == userId)
                        .Select(a => a.Category.Name)
                        .FirstOrDefault(),

                    CategoryColor = x.CategoryAssignments
                        .Where(a => a.UserId == userId)
                        .Select(a => a.Category.ColorHex)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return messages.Select(x =>
            {
                var isIncoming = x.ReceiverId == userId;

                var firstName = isIncoming
                    ? x.SenderFirstName
                    : x.ReceiverFirstName;

                var lastName = isIncoming
                    ? x.SenderLastName
                    : x.ReceiverLastName;

                return new MessageSearchItemViewModel
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

                    Category = string.IsNullOrWhiteSpace(x.CategoryName)
                        ? null
                        : new CategoryBadgeViewModel
                        {
                            Name = x.CategoryName,
                            ColorHex = x.CategoryColor ?? "#667085"
                        }
                };
            })
            .ToList();
        }

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

        private static string CreateMessagePreview(string? body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return "Mesaj içeriği bulunmuyor.";
            }

            var normalizedBody = string.Join(" ", body.Split(new[]
            {
                ' ',
                '\r',
                '\n',                         
                '\t'
            },
            StringSplitOptions.RemoveEmptyEntries));

            const int maxLength = 110;

            return normalizedBody.Length <= maxLength
                ? normalizedBody
                : $"{normalizedBody[..maxLength]}...";
        }
    }
}