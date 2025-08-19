using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
namespace Inmobiliaria.Controllers
{

    public class PropietarioController : Controller
    {
        // Sin inyección de dependencias (crear dentro del ctor)
        private readonly RepositorioPropietario repositorio;
        // GET: Propietario
        public PropietarioController(IConfiguration config)
        {
            // Sin inyección de dependencias y sin usar el config (quitar el parámetro repo del ctor)
            this.repositorio = new RepositorioPropietario(config);
        }
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        // GET: Propietario/Crear
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        //POST: Propietario/Crear
        [HttpPost]
        public IActionResult Crear(Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid) // Verifiaca que propietario tenga el formato valido
                {
                    repositorio.Crear(propietario);
                    TempData["Id"] = propietario.IdPropietario;
                    return RedirectToAction(nameof(Index));
                }
                else
                    return View(propietario);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        // GET: Propietario/Modificar/5
        [HttpGet]
        public IActionResult Modificar(int id)
        {
            try
            {
                Propietario p = repositorio.ObtenerPorID(id);
                if (p == null)
                { 
                    return RedirectToAction(nameof(Index));
                }
                return View(p);
            }
            catch (System.Exception)
            {
                throw;
            }
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
