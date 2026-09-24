using IdentiyMail.Web.DTOs.UserDtos;
using IdentiyMail.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentiyMail.Web.Controllers
{
    [Authorize]
    public class AccountController(UserManager<AppUser> _userManager,
                                   SignInManager<AppUser> _signInManager,
                                   IWebHostEnvironment _environment) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new AccountDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                ProfileImageUrl = user.ProfileImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AccountDto model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                model.Email = user.Email ?? string.Empty;
                model.UserName = user.UserName ?? string.Empty;
                model.ProfileImageUrl = user.ProfileImageUrl;

                return View(model);
            }

            string? newImageUrl = null;
            string? newImagePath = null;

            if (model.ProfileImage is not null && model.ProfileImage.Length > 0)
            {
                const long maxFileSize = 2 * 1024 * 1024; // 2 MB

                if (model.ProfileImage.Length > maxFileSize)
                {
                    ModelState.AddModelError(nameof(model.ProfileImage), "Profil resmi 2 MB'den büyük olamaz.");

                    model.Email = user.Email ?? string.Empty;
                    model.UserName = user.UserName ?? string.Empty;
                    model.ProfileImageUrl = user.ProfileImageUrl;

                    return View(model);
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                var extension = Path.GetExtension(model.ProfileImage.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.ProfileImage), "Sadece .jpg, .jpeg, .png ve .webp uzantılı dosyalar yüklenebilir.");

                    model.Email = user.Email ?? string.Empty;
                    model.UserName = user.UserName ?? string.Empty;
                    model.ProfileImageUrl = user.ProfileImageUrl;

                    return View(model);
                }

                var allowedContentTypes = new[]
                {
                    "image/jpeg",
                    "image/png",
                    "image/webp"
                };

                if (!allowedContentTypes.Contains(model.ProfileImage.ContentType.ToLowerInvariant()))
                {
                    ModelState.AddModelError(nameof(model.ProfileImage), "Geçersiz dosya türü. Sadece .jpg, .jpeg, .png ve .webp uzantılı dosyalar yüklenebilir.");
                    model.Email = user.Email ?? string.Empty;
                    model.UserName = user.UserName ?? string.Empty;
                    model.ProfileImageUrl = user.ProfileImageUrl;
                    return View(model);
                }

                var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "profile-images");

                Directory.CreateDirectory(uploadFolder);

                var fileName = $"{Guid.NewGuid():N}{extension}";

                newImagePath = Path.Combine(uploadFolder, fileName);

                await using var stream = new FileStream(newImagePath, FileMode.Create);
                await model.ProfileImage.CopyToAsync(stream);

                newImageUrl = $"/uploads/profile-images/{fileName}";
            }

            var oldImageUrl = user.ProfileImageUrl;

            user.FirstName = model.FirstName.Trim();
            user.LastName = model.LastName.Trim();

            if (newImageUrl is not null)
            {
                user.ProfileImageUrl = newImageUrl;
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                if (newImagePath is not null && System.IO.File.Exists(newImagePath))
                {
                    System.IO.File.Delete(newImagePath);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                model.Email = user.Email ?? string.Empty;
                model.UserName = user.UserName ?? string.Empty;
                model.ProfileImageUrl = user.ProfileImageUrl;

                return View(model);
            }

            if (newImageUrl is not null &&
                !string.IsNullOrWhiteSpace(oldImageUrl) &&
                oldImageUrl.StartsWith("/uploads/profile-images/"))
            {
                var oldFileName = Path.GetFileName(oldImageUrl);

                var oldImagePath = Path.Combine(_environment.WebRootPath, "uploads", "profile-images", oldFileName);

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            TempData["AccountSuccess"] = "Profil bilgileriniz başarıyla güncellendi.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            // Şifre değişiminden sonra mevcut oturumun devam etmesini sağlar.
            await _signInManager.RefreshSignInAsync(user);

            TempData["AccountSuccess"] = "Şifreniz başarıyla değiştirildi.";

            return RedirectToAction("Index");
        }
    }
}
