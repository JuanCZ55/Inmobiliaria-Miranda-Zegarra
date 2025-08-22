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
                    TempData["PropietarioCreado"] = $"Se agrego correctamente el propietario {propietario.Dni} {propietario.Nombre} {propietario.Apellido}";
                    return RedirectToAction(nameof(Listar));
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
                    return RedirectToAction(nameof(Listar));
                }
                return View(p);
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        // POST: Propietario/Modificar/5
        [HttpPost]
        public IActionResult Modificar(Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid) // Verifiaca que propietario tenga el formato valido
                {
                    repositorio.Modificar(propietario);
                    TempData["PropietarioCreado"] = $"Se modifico correctamente el propietario {propietario.Dni} {propietario.Nombre} {propietario.Apellido}";
                    return RedirectToAction(nameof(Listar));
                }
                else
                    return View(propietario);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            try
            {
                if (repositorio.Eliminar(id) > 0)
                {
                    TempData["Mensaje"] = $"Se eliminó correctamente el propietario";
                    return RedirectToAction(nameof(Listar));
                }

                TempData["Mensaje"] = "No se pudo eliminar el propietario";
                return RedirectToAction(nameof(Listar));
            }
            catch (System.Exception)
            {
                return RedirectToAction(nameof(Listar));
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

        // GET: Propietario
        public IActionResult Listar()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }
        [HttpGet]
        public IActionResult Listartodos()
        {
            var lista = repositorio.ObtenerTodos();
            return Ok(lista);
        }
        [HttpGet]
        public IActionResult Buscar(string dni)
        {
            var lista = repositorio.filtrarDNI(dni);
            return Ok(lista);
        }
    }
}
