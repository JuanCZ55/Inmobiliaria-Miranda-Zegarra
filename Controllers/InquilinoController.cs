using Inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    public class InquilinoController : Controller
    {
        // Sin inyección de dependencias (crear dentro del ctor)
        private readonly RepositorioInquilino repositorio;
        // GET: Propietario
        public InquilinoController(IConfiguration config)
        {
            // Sin inyección de dependencias y sin usar el config (quitar el parámetro repo del ctor)
            this.repositorio = new RepositorioInquilino(config);
        }
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

        // GET: Inquilino/Lista
        public IActionResult Listar()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }
        // Espacio para m�todos POST
    }
}
