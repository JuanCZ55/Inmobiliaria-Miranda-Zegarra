using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    public class InquilinoController : Controller
    {
        // GET: Inquilino
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        // GET: Inquilino/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // GET: Inquilino/Modificar/5
        public IActionResult Modificar(int id)
        {
            return View();
        }

        // GET: Inquilino/Alta/5
        public IActionResult Alta(int id)
        {
            return View();
        }

        // GET: Inquilino/Baja/5
        public IActionResult Baja(int id)
        {
            return View();
        }

        // Espacio para métodos POST
    }
}
