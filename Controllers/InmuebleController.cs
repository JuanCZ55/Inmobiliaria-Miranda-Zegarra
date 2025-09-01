using Inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly RepositorioInmueble repositorio;
        private readonly RepositorioTipoInmueble repoTipo;
        private readonly RepositorioPropietario repoPropietario;

        public InmuebleController(IConfiguration config)
        {
            this.repositorio = new RepositorioInmueble(config);
            this.repoTipo = new RepositorioTipoInmueble(config);
            this.repoPropietario = new RepositorioPropietario(config);
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
                if (repositorio.SeEstaUsando(id) > 0)
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

        // GET: Inmueble/Listar
        public IActionResult Listar()
        {
            var lista = repositorio.ObtenerTodos();
            var tipos = repoTipo.TenerTodos();
            ViewBag.Tipos = tipos;
            // Crear diccionario para acceso rápido en la vista
            ViewBag.TipoDict = tipos.ToDictionary(t => t.IdTipoInmueble, t => t.Nombre);
            return View(lista);
        }

        // GET: Inmueble/ListarTodos
        [HttpGet]
        public IActionResult ListarTodos()
        {
            var lista = repositorio.ObtenerTodos();
            return Ok(lista);
        }

        // GET: Inmueble/BuscarPorPropietario
        [HttpGet]
        public IActionResult BuscarPorPropietario(int idPropietario)
        {
            var lista = repositorio.ObtenerPorPropietario(idPropietario);
            return Ok(lista);
        }

        // GET: Inmueble/BuscarPorTipo
        [HttpGet]
        public IActionResult BuscarPorTipo(int idTipo)
        {
            var lista = repositorio.ObtenerPorTipo(idTipo);
            return Ok(lista);
        }

        // GET: Inmueble/BuscarPorUso
        [HttpGet]
        public IActionResult BuscarPorUso(int uso)
        {
            var lista = repositorio.ObtenerPorUso(uso);
            return Ok(lista);
        }

        // GET: Inmueble/BuscarPorCantidadAmbientes
        [HttpGet]
        public IActionResult BuscarPorCantidadAmbientes(int cantidad)
        {
            var lista = repositorio.ObtenerPorCantidadAmbientes(cantidad);
            return Ok(lista);
        }

        [HttpGet]
        public IActionResult SupaFiltro(
            string direccion,
            string dni,
            int tipo,
            int uso,
            int ambientes,
            int precioMin,
            int precioMax,
            int estado
        )
        {
            var lista = repositorio.SupaFiltro(
                direccion,
                dni,
                tipo,
                uso,
                ambientes,
                precioMin,
                precioMax,
                estado
            );
            return Ok(lista);
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

        // GET: Inquilino/Inquilino
        [HttpGet]
        public IActionResult Inmueble(int idInmueble)
        {
            var inmueble = repositorio.ObtenerPorID(idInmueble);
            return Ok(inmueble);
        }
    }
}
