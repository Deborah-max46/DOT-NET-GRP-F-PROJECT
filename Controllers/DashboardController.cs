using Microsoft.AspNetCore.Mvc;

namespace ConsumersVoiceSystemPrototype.Controllers
{
    public class DashboardController : Controller
    {
        // Check if user is logged in
        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("UserRole") != null;
        }

        // Consumer Dashboard
        public IActionResult Consumer()
        {
            if (!IsLoggedIn()) return Redirect("/Home/Login");

            if (HttpContext.Session.GetString("UserRole") != "Consumer")
                return Redirect("/Home/Login");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            return View();
        }

        // Advocate Dashboard
        public IActionResult Advocate()
        {
            if (!IsLoggedIn()) return Redirect("/Home/Login");

            if (HttpContext.Session.GetString("UserRole") != "Advocate")
                return Redirect("/Home/Login");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            return View();
        }

        // Admin Dashboard
        public IActionResult Admin()
        {
            if (!IsLoggedIn()) return Redirect("/Home/Login");

            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return Redirect("/Home/Login");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            return View();
        }

        // Business Dashboard
        public IActionResult Business()
        {
            if (!IsLoggedIn()) return Redirect("/Home/Login");

            if (HttpContext.Session.GetString("UserRole") != "Business")
                return Redirect("/Home/Login");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            return View();
        }
    }
}
