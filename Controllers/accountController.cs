using Microsoft.AspNetCore.Mvc;
using ClinicWebsite.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ClinicWebsite.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly string _jwtKey = "SuperSecretKeyForJWT123!"; // Replace with strong key

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                // Generate salt
                byte[] salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                // Hash password with salt
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: model.Password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                // Store password as salt:hash
                model.Password = $"{Convert.ToBase64String(salt)}:{hashed}";

                // Assign default role if not set
                if (string.IsNullOrWhiteSpace(model.Role))
                    model.Role = "User";

                _db.Users.Add(model);
                _db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || !VerifyPassword(password, user.Password))
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View();
            }

            // Create JWT token with role claim
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role) // <-- role included
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Store JWT in HttpOnly cookie
            Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // set true in production with HTTPS
                Expires = DateTime.UtcNow.AddHours(2)
            });

            return RedirectToAction("Index", "Home");
        }

        // Helper: Verify password
        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            try
            {
                var parts = storedPassword.Split(':');
                if (parts.Length != 2) return false;

                var salt = Convert.FromBase64String(parts[0]);
                var hash = parts[1];

                var enteredHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: enteredPassword,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                return hash == enteredHash;
            }
            catch
            {
                return false;
            }
        }

        // Example protected action with role
        [Authorize(Roles = "Admin", AuthenticationSchemes = "JwtBearer")]
        public IActionResult AdminSecret()
        {
            return Content($"Welcome Admin {User.Identity.Name}! You are authenticated.");
        }

        // Example protected action for all logged-in users
        [Authorize(AuthenticationSchemes = "JwtBearer")]
        public IActionResult Secret()
        {
            return Content($"Welcome {User.Identity.Name}! You are authenticated.");
        }
    }
}
