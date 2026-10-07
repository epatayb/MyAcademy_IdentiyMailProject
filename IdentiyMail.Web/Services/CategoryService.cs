using IdentiyMail.Web.Context;
using IdentiyMail.Web.DTOs.CategoryDtos;
using IdentiyMail.Web.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace IdentiyMail.Web.Services
{
    public class CategoryService(AppDbContext _context) : ICategoryService
    {
        #region Kategori seçeneklerini listeleme
        public async Task<List<CategoryOptionDto>> GetOptionsAsync(int userId)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Name)
                .Select(x => new CategoryOptionDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ColorHex = x.ColorHex,
                })
                .ToListAsync();
        }
        #endregion

        #region Kategorileri kullanım sayılarına göre listeleme
        public async Task<List<CategoryListItemDto>> GetCategoriesAsync(int userId)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => new CategoryListItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    ColorHex = x.ColorHex,
                    CreatedAt = x.CreatedAt,

                    MessageCount = x.Assignments.Count(a => 
                        a.UserId == userId &&                                                            
                        (
                            (a.Message.SenderId == userId &&
                             !a.Message.IsDeletedBySender &&
                             !a.Message.IsPermanentlyDeletedBySender)                                                             
                             ||
                             (a.Message.ReceiverId == userId &&
                              !a.Message.IsDeletedByReceiver &&
                              !a.Message.IsPermanentlyDeletedByReceiver)
                        ))                        
                })
                .OrderByDescending(x => x.MessageCount)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }
        #endregion

        #region Mesajın mevcut kategorisini getirme

        public async Task<int?> GetMessageCategoryIdAsync(int userId, int messageId)
        {
            return await _context.MessageCategoryAssignments
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.MessageId == messageId)
                .Select(x => (int?)x.CategoryId)
                .FirstOrDefaultAsync();
        }

        #endregion

        #region Kategori oluşturma

        public async Task<CategoryServiceResult> CreateAsync(int userId, string name, string? description, string colorHex)
        {
            name = name.Trim();
            description = string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim();

            colorHex = NormalizeColor(colorHex);

            var validationResult = ValidateCategory(name, description, colorHex);

            if (!validationResult.Success)
            {
                return validationResult;
            }

            var categoryExists = await _context.Categories
                .AnyAsync(x => x.UserId == userId && x.Name == name);

            if (categoryExists)
            {
                return CategoryServiceResult.Failure("Bu isimde bir kategoriniz zaten bulunuyor.");
            }

            var category = new Category
            {
                UserId = userId,
                Name = name,
                Description = description,
                ColorHex = colorHex,
                CreatedAt = DateTime.Now
            };

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            return CategoryServiceResult.Successful();
        }

        #endregion

        #region Kategori güncelleme

        public async Task<CategoryServiceResult> UpdateAsync(int userId, int categoryId, string name, string? description, string colorHex)
        {
            name = name.Trim();

            description = string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim();

            colorHex = NormalizeColor(colorHex);

            var validationResult = ValidateCategory(name, description, colorHex);

            if (!validationResult.Success)
            {
                return validationResult;
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == categoryId && x.UserId == userId);

            if (category is null)
            {
                return CategoryServiceResult.Failure("Kategori bulunamadı.");
            }

            var duplicateExists = await _context.Categories
                .AnyAsync(x => x.UserId == userId && x.Id != categoryId && x.Name == name);

            if (duplicateExists)
            {
                return CategoryServiceResult.Failure("Bu isimde bir kategoriniz zaten bulunuyor.");
            }

            category.Name = name;
            category.Description = description;
            category.ColorHex = colorHex;

            await _context.SaveChangesAsync();

            return CategoryServiceResult.Successful();
        }

        #endregion

        #region Kategori silme

        public async Task<CategoryServiceResult> DeleteAsync(int userId, int categoryId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == categoryId && x.UserId == userId);

            if (category is null)
            {
                return CategoryServiceResult.Failure("Kategori bulunamadı.");
            }

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return CategoryServiceResult.Successful();
        }

        #endregion

        #region Mesaja kategori atama değiştirme ve kaldırma

        public async Task<CategoryServiceResult> AssignToMessageAsync(int userId, int messageId, int? categoryId)
        {
            var messageExists = await _context.UserMessages
                .AnyAsync(x =>
                    x.Id == messageId &&
                    (
                        (x.SenderId == userId && !x.IsDeletedBySender && !x.IsPermanentlyDeletedBySender) ||
                        (x.ReceiverId == userId && !x.IsDeletedByReceiver && !x.IsPermanentlyDeletedByReceiver)
                    ));

            if (!messageExists)
            {
                return CategoryServiceResult.Failure("Mesaj bulunamadı veya bu mesaj üzerinde işlem yetkiniz yok.");
            }


            var assignment = await _context.MessageCategoryAssignments
                .FirstOrDefaultAsync(x => x.UserId == userId && x.MessageId == messageId);


            // Kategori Yok seçildi.
            if (!categoryId.HasValue)
            {
                if (assignment is not null)
                {
                    _context.MessageCategoryAssignments.Remove(assignment);

                    await _context.SaveChangesAsync();
                }

                return CategoryServiceResult.Successful();
            }


            var categoryExists = await _context.Categories
                .AnyAsync(x => x.Id == categoryId.Value && x.UserId == userId);

            if (!categoryExists)
            {
                return CategoryServiceResult.Failure("Seçilen kategori size ait değil veya bulunamadı.");
            }


            if (assignment is null)
            {
                assignment = new MessageCategoryAssignment
                {
                    UserId = userId,
                    MessageId = messageId,
                    CategoryId = categoryId.Value
                };

                _context.MessageCategoryAssignments.Add(assignment);
            }
            else
            {
                assignment.CategoryId = categoryId.Value;
            }


            await _context.SaveChangesAsync();

            return CategoryServiceResult.Successful();
        }

        #endregion

        #region Kategori doğrulama yardımcı metotları

        private static CategoryServiceResult ValidateCategory(string name, string? description, string colorHex)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return CategoryServiceResult.Failure("Kategori adı boş bırakılamaz.");
            }

            if (name.Length > 50)
            {
                return CategoryServiceResult.Failure("Kategori adı en fazla 50 karakter olabilir.");
            }

            if (description?.Length > 250)
            {
                return CategoryServiceResult.Failure("Kategori açıklaması en fazla 250 karakter olabilir.");
            }

            if (!IsValidColorHex(colorHex))
            {
                return CategoryServiceResult.Failure("Geçerli bir kategori rengi seçiniz.");
            }

            return CategoryServiceResult.Successful();
        }


        private static bool IsValidColorHex(string colorHex)
        {
            return Regex.IsMatch(colorHex, "^#[0-9A-Fa-f]{6}$");
        }


        private static string NormalizeColor(string colorHex)
        {
            return string.IsNullOrWhiteSpace(colorHex)
                ? string.Empty
                : colorHex.Trim().ToUpperInvariant();
        }

        #endregion

        #region Kategori sahiplik kontrolü
        public async Task<bool> IsOwnedByUserAsync(int userId, int categoryId)
        {
            return await _context.Categories
                .AsNoTracking()
                .AnyAsync(x => x.Id == categoryId && x.UserId == userId);
        }
        #endregion
    }
}