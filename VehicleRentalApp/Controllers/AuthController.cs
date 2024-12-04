using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleRentalApp.Data;
using VehicleRentalApp.Models;
using VehicleRentalApp.Services;

namespace VehicleRentalApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthController(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = new PasswordHasher<User>(); // PasswordHasher kullanımı
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Kullanıcı adı ve şifre doğrulaması
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password) != PasswordVerificationResult.Success)
            {
                return Unauthorized("Geçersiz giriş.");
            }

            // Token oluşturuluyor
            var token = _tokenService.GenerateToken(user);

            // Token'ı Cookie veya Header'da saklayabilirsiniz, örneğin:
            Response.Cookies.Append("auth_token", token, new CookieOptions { HttpOnly = true });

            // Kullanıcıyı home sayfasına yönlendiriyoruz
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Register()
        {
            // Modeli null olarak değil, boş bir nesne ile başlatıyoruz
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı adı kontrolü
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Bu kullanıcı adı zaten alınmış.");
                    return View(model);
                }

                //şifre eşleşmesi kontrolü
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Şifreler eşleşmiyor.");
                    return View(model);
                }

                //kullanıcı oluşturma
                var user = new User
                {
                    Username = model.Username,
                    Password = _passwordHasher.HashPassword(null, model.Password), // Şifreyi hashle
                    Role = "User" // Varsayılan rol "User"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            // Eğer model geçersizse, formu tekrar göster
            return View(model);
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("auth_token"); // Auth token'ı sil
            return RedirectToAction("Index", "Home"); // Home sayfasına yönlendir
        }


    }
}
