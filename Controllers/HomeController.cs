using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClinicWebsite.Models;


namespace ClinicWebsite.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Doctors()
        {
            return View();
        }

        public IActionResult Appointment()
        {
            return View();
        }

        public IActionResult Testimonials()
        {
            return View();
        }

        public IActionResult Facilities()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
