using Microsoft.AspNetCore.Mvc;
using ClinicWebsite.Models;

namespace ClinicWebsite.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AppointmentController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Appointment
        public IActionResult Index()
        {
            var appointments = _db.Appointments.ToList();
            return View(appointments);
        }

        // GET: /Appointment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Appointment model)
        {
            if (ModelState.IsValid)
            {
                _db.Appointments.Add(model);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
