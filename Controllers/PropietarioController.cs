namespace Inmobiliaria.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class PropietarioController : Controller
    {
        // GET: Propietario

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        // GET: Propietario/Crear

        public IActionResult Crear()
        {
            return View();
        }

        // GET: Propietario/Modificar/5

        public IActionResult Modificar(int id)
        {
            return View();
        }

        // GET: Propietario/Alta/5

        public IActionResult Alta(int id)
        {
            return View();
        }

        // GET: Propietario/Baja/5

        public IActionResult Baja(int id)
        {
            return View();
        }
    }
}
