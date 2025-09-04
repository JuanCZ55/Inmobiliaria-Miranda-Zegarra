using Inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mysqlx.Crud;

namespace Inmobiliaria.Controllers
{
    public class TipoInmuebleController : Controller
    {
        private readonly IRepositorioTipoInmueble repositorio;
        private readonly IConfiguration config;
        public TipoInmuebleController(IRepositorioTipoInmueble repositorio, IConfiguration config)
        {
            this.repositorio = repositorio;
            this.config = config;
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
                            $"El tipo de inmueble {tipoInmueble.Nombre} ya existe, no se creo.";
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
                var tipo = repositorio.ObtenerPorID(id);
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
                    var ti = repositorio.ObtenerPorID(tipoInmueble.IdTipoInmueble);
                    if (ti.Nombre == tipoInmueble.Nombre)
                    {
                        TempData["Mensaje"] =
                            $"Ya tiene ese nombre este tipo de inmueble: {tipoInmueble.Nombre}, no se modifico.";
                        return RedirectToAction(nameof(Listar));
                    }
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
                if (repositorio.SeEstaUsando(id) > 0)
                {
                    TempData["Warning"] =
                        "No se puede eliminar el tipo de inmueble porque está en uso.";
                    return RedirectToAction(nameof(Listar));
                }
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
        [HttpGet]
        public IActionResult Listar()
        {
            var lista = repositorio.TenerTodos();
            return View(lista);
        }

        // GET: TipoInmueble/ListarTodos
        [HttpGet]
        public IActionResult ListarTodos(int PaginaActual = 1)
        {
            int registrosPorPagina = 5;
            var total = repositorio.ContarPorTodos();
            int offset = (PaginaActual - 1) * registrosPorPagina;
            int limite = Math.Min(registrosPorPagina, total - offset);

            var lista = repositorio.ObtenerTodos(limite, offset);

            return Ok(
                new
                {
                    lista = lista,
                    totalPaginas = (int)Math.Ceiling((double)total / registrosPorPagina),
                }
            );
        }

        // GET: TipoInmueble/ListarPorNombre
        [HttpGet]
        public IActionResult ListarPorNombre(string nombre, int PaginaActual = 1)
        {
            int registrosPorPagina = 5;
            var total = repositorio.ContarPorNombre(nombre);
            int offset = (PaginaActual - 1) * registrosPorPagina;
            int limite = Math.Min(registrosPorPagina, total - offset);

            var lista = repositorio.ListarPorNombre(nombre, limite, offset);

            return Ok(
                new
                {
                    lista = lista,
                    totalPaginas = (int)Math.Ceiling((double)total / registrosPorPagina),
                }
            );
        }
    }
}
