using Inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioTipoInmueble repoTipo;
        private readonly IRepositorioPropietario repoPropietario;
        private readonly IConfiguration config;

        public InmuebleController(
            IRepositorioInmueble repositorio,
            IRepositorioTipoInmueble repoTipo,
            IRepositorioPropietario repoPropietario,
            IConfiguration config
        )
        {
            this.repositorio = repositorio;
            this.repoTipo = repoTipo;
            this.repoPropietario = repoPropietario;
            this.config = config;
        }

        // GET: Inmueble
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        // GET: Inmueble/Crear
        public IActionResult Crear()
        {
            ViewBag.Tipos = repoTipo.TenerTodos();
            ViewBag.Propietarios = repoPropietario
                .ObtenerTodos()
                .Select(p => new
                {
                    p.IdPropietario,
                    NombreCompleto = p.Nombre + " " + p.Apellido + " - " + p.Dni,
                })
                .ToList();
            return View();
        }

        // POST: Inmueble/Crear
        [HttpPost]
        public IActionResult Crear(Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Crear(inmueble);
                    TempData["Mensaje"] =
                        $"Se agrego correctamente el inmueble en {inmueble.Direccion}";
                    return RedirectToAction(nameof(Listar));
                }
                ViewBag.Tipos = repoTipo.TenerTodos();
                return View(inmueble);
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = "Error al crear inmueble: " + ex.Message;
                ViewBag.Tipos = repoTipo.TenerTodos();
                return View(inmueble);
            }
        }

        // GET: Inmueble/Modificar/5
        [HttpGet]
        public IActionResult Modificar(int id)
        {
            try
            {
                Inmueble i = repositorio.ObtenerPorID(id);
                if (i == null)
                {
                    return RedirectToAction(nameof(Listar));
                }
                ViewBag.Propietarios = repoPropietario
                    .ObtenerTodos()
                    .Select(p => new
                    {
                        p.IdPropietario,
                        NombreCompleto = p.Nombre + " " + p.Apellido + " - " + p.Dni,
                    })
                    .ToList();
                ViewBag.Tipos = repoTipo.TenerTodos();
                return View(i);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Error = "Error al cargar inmueble: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        // POST: Inmueble/Modificar
        [HttpPost]
        public IActionResult Modificar(Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Modificar(inmueble);
                    TempData["Mensaje"] =
                        $"Se modifico correctamente el inmueble de {inmueble.Direccion}";
                    return RedirectToAction(nameof(Listar));
                }
                ViewBag.Propietarios = repoPropietario
                    .ObtenerTodos()
                    .Select(p => new
                    {
                        p.IdPropietario,
                        NombreCompleto = p.Nombre + " " + p.Apellido + " - " + p.Dni,
                    })
                    .ToList();
                ViewBag.Tipos = repoTipo.TenerTodos();
                return View(inmueble);
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = "Error al modificar inmueble: " + ex.Message;
                ViewBag.Tipos = repoTipo.TenerTodos();
                return View(inmueble);
            }
        }

        // POST: Inmueble/Eliminar/5
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            try
            {
                if (repositorio.SeEstaUsando(id))
                {
                    TempData["Mensaje"] = "No se puede eliminar el inmueble porque está en uso";
                    return RedirectToAction(nameof(Listar));
                }
                if (repositorio.Eliminar(id) > 0)
                {
                    TempData["Mensaje"] = "Se elimino correctamente el inmueble";
                    return RedirectToAction(nameof(Listar));
                }

                TempData["Mensaje"] = "No se pudo eliminar el inmueble";
                return RedirectToAction(nameof(Listar));
            }
            catch (System.Exception)
            {
                return RedirectToAction(nameof(Listar));
            }
        }

        // POST: Inmueble/SetEstado/5
        [HttpPost]
        public IActionResult SetEstado(int id, int estado)
        {
            try
            {
                repositorio.SetEstado(id, estado);
                TempData["Mensaje"] = "Se actualizo el estado del inmueble correctamente";
                return RedirectToAction(nameof(Listar));
            }
            catch
            {
                TempData["Mensaje"] = "No se pudo actualizar el estado";
                return RedirectToAction(nameof(Listar));
            }
        }

        // GET: Inmueble/Listar
        [HttpGet]
        public IActionResult Listar(
            string? direccion,
            string? dni,
            int? idTipoInmueble,
            int? uso,
            int? cantidadAmbientesMin,
            decimal? precioMin,
            decimal? precioMax,
            int? estado,
            int paginaActual = 1
        )
        {
            int registrosPorPagina = 9;
            int offset = (paginaActual - 1) * registrosPorPagina;
            int total = repositorio.ContarFiltro(
                direccion,
                dni,
                idTipoInmueble,
                uso,
                cantidadAmbientesMin,
                precioMin,
                precioMax,
                estado
            );
            int limit = Math.Max(0, Math.Min(registrosPorPagina, total - offset));

            var inmuebles = repositorio.Filtro(
                direccion,
                dni,
                idTipoInmueble,
                uso,
                cantidadAmbientesMin,
                precioMin,
                precioMax,
                estado,
                limit,
                offset
            );
            var tipos = repoTipo.TenerTodos();
            int totalPaginas = (int)Math.Ceiling((double)total / registrosPorPagina);
            Console.WriteLine("Total paginas: " + inmuebles);
            ViewBag.PaginaActual = paginaActual;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.Direccion = direccion;
            ViewBag.Dni = dni;
            ViewBag.IdTipoInmueble = idTipoInmueble;
            ViewBag.Uso = uso;
            ViewBag.CantidadAmbientesMin = cantidadAmbientesMin;
            ViewBag.PrecioMin = precioMin;
            ViewBag.PrecioMax = precioMax;
            ViewBag.Estado = estado;
            ViewBag.Tipos = tipos;

            return View(inmuebles);
        }
    }
}
