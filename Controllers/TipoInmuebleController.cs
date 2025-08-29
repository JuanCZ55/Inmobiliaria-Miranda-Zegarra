using Inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Controllers
{
    public class TipoInmuebleController : Controller
    {
        private readonly RepositorioTipoInmueble repositorio;

        public TipoInmuebleController(IConfiguration config)
        {
            this.repositorio = new RepositorioTipoInmueble(config);
        }

        // GET: TipoInmueble
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        // GET: TipoInmueble/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: TipoInmueble/Crear
        [HttpPost]
        public IActionResult Crear(TipoInmueble tipoInmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (repositorio.ExisteTipoInmueble(tipoInmueble.Nombre) == 0)
                    {
                        repositorio.Crear(tipoInmueble);
                        TempData["Mensaje"] =
                            $"Se agrego correctamente el tipo de inmueble: {tipoInmueble.Nombre}";
                    }
                    else
                    {
                        TempData["Warning"] =
                            $"El tipo de inmueble {tipoInmueble.Nombre} ya existe.";
                    }
                    return RedirectToAction(nameof(Listar));
                }
                else
                    return View(tipoInmueble);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        // GET: TipoInmueble/Modificar/5
        public IActionResult Modificar(int id)
        {
            try
            {
                var tipo = repositorio.ObtenerTodos().FirstOrDefault(x => x.IdTipoInmueble == id);
                if (tipo == null)
                    return RedirectToAction(nameof(Listar));

                return View(tipo);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        // POST: TipoInmueble/Modificar
        [HttpPost]
        public IActionResult Modificar(TipoInmueble tipoInmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Modificar(tipoInmueble);
                    TempData["Mensaje"] =
                        $"Se modifico correctamente el tipo de inmueble: {tipoInmueble.Nombre}";
                    return RedirectToAction(nameof(Listar));
                }
                else
                    return View(tipoInmueble);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        // POST: TipoInmueble/Eliminar/5
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            try
            {
                if (repositorio.Eliminar(id) > 0)
                {
                    TempData["Mensaje"] =
                        $"Se elimino correctamente el id {id} de tipo de inmueble";
                    return RedirectToAction(nameof(Listar));
                }

                TempData["Mensaje"] = "No se pudo eliminar el tipo de inmueble";
                return RedirectToAction(nameof(Listar));
            }
            catch (System.Exception)
            {
                return RedirectToAction(nameof(Listar));
            }
        }

        // GET: TipoInmueble/Listar
        public IActionResult Listar()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: TipoInmueble/ListarTodos
        [HttpGet]
        public IActionResult ListarTodos()
        {
            var lista = repositorio.ObtenerTodos();
            return Ok(lista);
        }
    }
}
