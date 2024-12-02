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
            return Json(new { token });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Kullanıcı var mı kontrol et
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (existingUser != null)
            {
                return BadRequest("Kullanıcı zaten var.");
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Şifreler eşleşmiyor.");
                return View(model);
            }

            // Yeni kullanıcı oluştur
            var user = new User
            {
                Username = model.Username,
                // Şifreyi hashle
                Password = _passwordHasher.HashPassword(null, model.Password),
                Role = "User" // Varsayılan rol
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Token oluşturuluyor
            var token = _tokenService.GenerateToken(user);
            return Json(new { token });
        }
    }
}
