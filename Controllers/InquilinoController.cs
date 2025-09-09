using Inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly IRepositorioInquilino repositorio;
        private readonly IConfiguration config;

        // GET: Propietario
        public InquilinoController(IRepositorioInquilino repositorio, IConfiguration config)
        {
            this.repositorio = repositorio;
            this.config = config;
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

        //POST: Inquilino/Crear
        [HttpPost]
        public IActionResult Crear(Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid) // Verifiaca que inquilino tenga el formato valido
                {
                    repositorio.Crear(inquilino);
                    TempData["Success"] =
                        $"Se agrego correctamente el inquilino {inquilino.Dni} {inquilino.Nombre} {inquilino.Apellido}";
                    return RedirectToAction(nameof(Listar));
                }
                else
                    return View(inquilino);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        // GET: Inquilino/Modificar/5
        public IActionResult Modificar(int id)
        {
            try
            {
                Inquilino i = repositorio.ObtenerPorID(id);
                if (i == null)
                {
                    return RedirectToAction(nameof(Listar));
                }
                return View(i);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult Modificar(Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid) // Verifiaca que inquilino tenga el formato valido
                {
                    var existe = repositorio.ObtenerPorDni(inquilino.Dni);
                    if (existe.IdInquilino <= 0)
                    {
                        TempData["Error"] = "No hay inquilino con ese DNI para modificar";
                        return View(inquilino);
                    }
                    repositorio.Modificar(inquilino);
                    TempData["Success"] =
                        $"Se modifico correctamente el inquilino {inquilino.Dni} {inquilino.Nombre} {inquilino.Apellido}";
                    return RedirectToAction(nameof(Listar));
                }
                else
                    return View(inquilino);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        // POST: Inquilino/Eliminar/5
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            try
            {
                if (repositorio.Eliminar(id) > 0)
                {
                    TempData["Success"] = $"Se eliminó correctamente el inquilino";
                    return RedirectToAction(nameof(Listar));
                }

                TempData["Success"] = "No se pudo eliminar el inquilino";
                return RedirectToAction(nameof(Listar));
            }
            catch (System.Exception)
            {
                return RedirectToAction(nameof(Listar));
            }
        }

        [HttpGet]
        public IActionResult Inquilino(string dni)
        {
            var inquilino = repositorio.ObtenerPorDni(dni);
            if (inquilino.Estado == 1)
            {
                return Ok(inquilino);
            }
            return Ok(null);
        }

        // GET: Inquilino/Lista
        [HttpGet]
        public IActionResult Listar(string Dni, int PaginaActual = 1)
        {
            int registrosPorPagina = 9;
            int offset = (PaginaActual - 1) * registrosPorPagina;
            int total = repositorio.ContarFiltro(Dni);
            int limit = Math.Max(0, Math.Min(registrosPorPagina, total - offset));

            var inquilinos = repositorio.Filtro(Dni, limit, offset);

            int totalPaginas = (int)Math.Ceiling((double)total / registrosPorPagina);

            ViewBag.PaginaActual = PaginaActual;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.Dni = Dni;

            return View(inquilinos);
        }
    }
}
