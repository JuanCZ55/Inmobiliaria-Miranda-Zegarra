using Inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly IRepositorioPropietario repositorio;
        private readonly IConfiguration config;

        // GET: Propietario
        public PropietarioController(IRepositorioPropietario repositorio, IConfiguration config)
        {
            this.repositorio = repositorio;
            this.config = config;
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
                if (ModelState.IsValid)
                {
                    var existe = repositorio.ObtenerPorDni(propietario.Dni);
                    if (existe.IdPropietario > 0)
                    {
                        TempData["Error"] = "El DNI ya existe";
                        return View(propietario);
                    }
                    repositorio.Crear(propietario);
                    TempData["Success"] =
                        $"Se agrego correctamente el propietario {propietario.Dni} {propietario.Nombre} {propietario.Apellido}";
                    return RedirectToAction(nameof(Listar));
                }
                else
                {
                    TempData["Error"] = "Error con el modelo";
                    return View(propietario);
                }
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
                if (ModelState.IsValid)
                {
                    var existe = repositorio.ObtenerPorDni(propietario.Dni);
                    if (existe.IdPropietario <= 0)
                    {
                        TempData["Error"] = "No hay propiertario con ese DNI para modificar";
                        return View(propietario);
                    }
                    repositorio.Modificar(propietario);
                    TempData["Success"] =
                        $"Se modifico correctamente el propietario {propietario.Dni} {propietario.Nombre} {propietario.Apellido}";
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
                    TempData["Success"] = $"Se eliminó correctamente el propietario";
                    return RedirectToAction(nameof(Listar));
                }

                TempData["Error"] = "No se pudo eliminar el propietario";
                return RedirectToAction(nameof(Listar));
            }
            catch (System.Exception)
            {
                return RedirectToAction(nameof(Listar));
            }
        }

        // GET: Propietario/Lista
        public IActionResult Listar(string Dni, int PaginaActual = 1)
        {
            int registrosPorPagina = 9;
            int offset = (PaginaActual - 1) * registrosPorPagina;
            int total = repositorio.ContarFiltro(Dni);
            int limit = Math.Max(0, Math.Min(registrosPorPagina, total - offset));

            var propietarios = repositorio.Filtro(Dni, limit, offset);

            int totalPaginas = (int)Math.Ceiling((double)total / registrosPorPagina);

            ViewBag.PaginaActual = PaginaActual;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.Dni = Dni;

            return View(propietarios);
        }
    }
}
