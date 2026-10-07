using IdentiyMail.Web.Entities;
using IdentiyMail.Web.Services;
using IdentiyMail.Web.ViewModels.CategoryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentiyMail.Web.Controllers
{
    [Authorize]
    public class CategoryController(UserManager<AppUser> _userManager, ICategoryService _categoryService) : Controller
    {
        #region Kategori yönetim ekranı
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await BuildPageModelAsync();

            return View(model);
        }
        #endregion

        #region Kategori kaydetme ve güncelleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var pageModel = await BuildPageModelAsync(model);

                ViewData["OpenCategoryModal"] = true;

                return View("Index", pageModel);
            }

            var userId = GetCurrentUserId();

            CategoryServiceResult result;

            if (model.Id.HasValue)
            {
                result = await _categoryService.UpdateAsync(userId, model.Id.Value, model.Name, model.Description, model.ColorHex);
            }
            else
            {
                result = await _categoryService.CreateAsync(userId, model.Name, model.Description, model.ColorHex);
            }

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Kategori işlemi gerçekleştirilemedi.");

                var pageModel = await BuildPageModelAsync(model);

                ViewData["OpenCategoryModal"] = true;

                return View("Index", pageModel);
            }

            TempData["CategorySuccess"] = model.Id.HasValue
                ? "Kategori başarıyla güncellendi."
                : "Kategori başarıyla oluşturuldu.";

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region kategori silme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();

            var result = await _categoryService.DeleteAsync(userId, id);

            if (!result.Success)
            {
                TempData["CategoryError"] = result.ErrorMessage ?? "Kategori silinemedi.";

                return RedirectToAction(nameof(Index));
            }

            TempData["CategorySuccess"] = "Kategori başarıyla silindi.";

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Kategori ekran modeli oluşturma

        private async Task<CategoryManagementViewModel> BuildPageModelAsync(CategoryFormViewModel? form = null)
        {
            var userId = GetCurrentUserId();

            return new CategoryManagementViewModel
            {
                Categories = await _categoryService.GetCategoriesAsync(userId),

                Form = form ?? new CategoryFormViewModel()
            };
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
    }
}
