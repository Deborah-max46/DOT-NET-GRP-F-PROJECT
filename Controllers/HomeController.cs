using ConsumersVoiceSystemPrototype.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConsumersVoiceSystemPrototype.Controllers
{
    public class HomeController : Controller
    {
        // Landing Page
        public IActionResult Index()
        {
            return View();
        }

        // Login Page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // Mock users - simulating a database
            var users = GetMockUsers();
            var user = users.FirstOrDefault(u =>
                u.Email == model.Email &&
                u.Password == model.Password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View(model);
            }

            // Store user info in session
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());
            HttpContext.Session.SetString("UserEmail", user.Email);

            // Redirect based on role
            return user.Role switch
            {
                UserRole.Consumer => Redirect("/Dashboard/Consumer"),
                UserRole.Advocate => Redirect("/Dashboard/Advocate"),
                UserRole.Admin => Redirect("/Dashboard/Admin"),
                UserRole.Business => Redirect("/Dashboard/Business"),
                _ => Redirect("/")
            };
        }

        // Register Page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            // Store in session and redirect to login
            HttpContext.Session.SetString("UserName", model.FullName);
            HttpContext.Session.SetString("UserRole", model.Role.ToString());
            HttpContext.Session.SetString("UserEmail", model.Email);

            // Redirect based on role immediately after register
            return model.Role switch
            {
                UserRole.Consumer => Redirect("/Dashboard/Consumer"),
                UserRole.Advocate => Redirect("/Dashboard/Advocate"),
                UserRole.Admin => Redirect("/Dashboard/Admin"),
                UserRole.Business => Redirect("/Dashboard/Business"),
                _ => Redirect("/")
            };
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }

        // Mock users simulating a database
        private List<ApplicationUser> GetMockUsers()
        {
            return new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = 1,
                    FullName = "Uwase Ketsia",
                    Email = "consumer@test.com",
                    Password = "password123",
                    Role = UserRole.Consumer
                },
                new ApplicationUser
                {
                    Id = 2,
                    FullName = "Jean Bosco",
                    Email = "advocate@test.com",
                    Password = "password123",
                    Role = UserRole.Advocate
                },
                new ApplicationUser
                {
                    Id = 3,
                    FullName = "Admin User",
                    Email = "admin@test.com",
                    Password = "password123",
                    Role = UserRole.Admin
                },
                new ApplicationUser
                {
                    Id = 4,
                    FullName = "Shop Owner",
                    Email = "business@test.com",
                    Password = "password123",
                    Role = UserRole.Business
                }
            };
        }
    }
}
