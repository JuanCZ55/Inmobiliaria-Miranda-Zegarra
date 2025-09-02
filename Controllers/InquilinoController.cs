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

        //POST: Inquilino/Crear
        [HttpPost]
        public IActionResult Crear(Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid) // Verifiaca que inquilino tenga el formato valido
                {
                    repositorio.Crear(inquilino);
                    TempData["InquilinoCreado"] =
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
                    repositorio.Modificar(inquilino);
                    TempData["InquilinoCreado"] =
                        $"Se modificado correctamente el inquilino {inquilino.Dni} {inquilino.Nombre} {inquilino.Apellido}";
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

        // GET: Inquilino/Lista
        public IActionResult Listar(string dni, int PaginaActual = 1)
        {
            int registrosPorPagina = 9;
            int offset = (PaginaActual - 1) * registrosPorPagina;
            int total = repositorio.ContarFiltro(dni);
            int limit = Math.Max(0, Math.Min(registrosPorPagina, total - offset));

            var inquilinos = repositorio.Filtro(dni, limit, offset);

            int totalPaginas = (int)Math.Ceiling((double)total / registrosPorPagina);

            ViewBag.PaginaActual = PaginaActual;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.dni = dni;

            return View(inquilinos);
        }

        [HttpGet]
        public IActionResult Listartodos()
        {
            var lista = repositorio.ObtenerTodos();
            return Ok(lista);
        }

        // GET: Inquilino/Buscar
        [HttpGet]
        public IActionResult Buscar(string dni)
        {
            var lista = repositorio.filtrarDNI(dni);
            return Ok(lista);
        }

        // GET: Inquilino/Inquilino
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
    }
}
