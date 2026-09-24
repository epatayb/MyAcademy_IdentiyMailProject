using IdentiyMail.Web.DTOs.UserDtos;
using IdentiyMail.Web.Entities;
using IdentiyMail.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IdentiyMail.Web.Controllers
{
    public class AuthController(UserManager<AppUser> _userManager,
                                SignInManager<AppUser> _signInManager,
                                IMailService _mailService,
                                ILogger<AuthController> _logger) : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Message");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new AppUser
            {
                Email = model.Email.Trim(),
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                UserName = model.UserName.Trim(),
                ProfileImageUrl = model.ProfileImageUrl
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return View(model);
            }    
            
            var roleResult = await _userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return View(model);
            }

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Message");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var email = model.Email.Trim();

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null) 
            {
                ModelState.AddModelError(string.Empty, "Bu email sistemde kayıtlı değil.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, 
                model.Password, 
                isPersistent: false, 
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty,"Email veya şifre hatalı");
                return View(model);
            }
            return RedirectToAction("Index", "Message");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = model.Email.Trim();
            var user = await _userManager.FindByEmailAsync(email);

            if (user is not null)
            {
                try
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    var resetLink = Url.Action(
                        nameof(ResetPassword),
                        "Auth",
                        new
                        {
                            email = user.Email,
                            token = encodedToken
                        },
                        Request.Scheme);

                    if (!string.IsNullOrWhiteSpace(resetLink))
                    {
                        var safeFirstName = WebUtility.HtmlEncode(user.FirstName);
                        var safeResetLink = WebUtility.HtmlEncode(resetLink);

                        var body = $"""                                                           
                            
                            <h2>Şifre Yenileme İsteği</h2>                            

                            <p>Merhaba {safeFirstName},</p>

                            <p>
                                Hesabınız için bir şifre yenileme talebi aldık.
                            </p>

                            <p>
                                <a href="{safeResetLink}">
                                    Şifremi Yenile
                                </a>
                            </p>
                            <p>
                                Bu işlemi siz yapmadıysanız bu e-postayı
                                dikkate almayabilirsiniz.
                            </p>
                            """;

                        await _mailService.SendAsync(email, "Şifre Yenileme Talebi", body);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Şifre yenileme maili gönderilirken bir hata oluştu.");
                }
            }

            TempData["ForgotPasswordSuccess"] = "Email adresiniz sistemde kayıtlıysa şifre yenileme bağlantısı gönderildi.";

            return RedirectToAction(nameof(ForgotPassword));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest();
            }

            var model = new ResetPasswordDto
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null) {
                TempData["AuthSuccess"] = "Şifreniz güncellendiyse yeni şifrenizle giriş yapabilirsiniz.";
                return RedirectToAction(nameof(Login));
            }

            string token;

            try
            {
                token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            }
            catch 
            {
                ModelState.AddModelError(string.Empty, "Şifre yenileme bağlantısı geçersiz.");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return View(model);
            }

            TempData["AuthSuccess"] = "Şifreniz başarıyla yenilendi. Yeni şifrenizle giriş yapabilirsiniz.";

            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
