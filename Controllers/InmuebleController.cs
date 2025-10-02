using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Inmobiliaria.Services;
using Microsoft.AspNetCore.Http;
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
        private readonly IFileService _fileService;
        private const string RUTA_CARPETA_IMAGENES = "Uploads/Inmuebles";

        public InmuebleController(
            IRepositorioInmueble repositorio,
            IRepositorioTipoInmueble repoTipo,
            IRepositorioPropietario repoPropietario,
            IConfiguration config,
            IFileService fileService
        )
        {
            this.repositorio = repositorio;
            this.repoTipo = repoTipo;
            this.repoPropietario = repoPropietario;
            this.config = config;
            this._fileService = fileService;
        }

        // GET: Inmueble/Crear
        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Tipos = repoTipo.TenerTodos();
            return View();
        }

        // POST: Inmueble/Crear
        [HttpPost]
        public async Task<IActionResult> Crear(Inmueble inmueble)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Tipos = repoTipo.TenerTodos();
                return View(inmueble);
            }

            var nombresArchivosGuardados = new List<string>();
            try
            {
                inmueble.listImagenes = new List<Imagen>();
                if (inmueble.FilePortada != null)
                {
                    string nombreArchivo = await _fileService.GuardarArchivoAsync(
                        inmueble.FilePortada,
                        RUTA_CARPETA_IMAGENES
                    );
                    nombresArchivosGuardados.Add(nombreArchivo);
                    inmueble.listImagenes.Add(
                        new Imagen { Url = $"/{RUTA_CARPETA_IMAGENES}/{nombreArchivo}", Tipo = 1 }
                    );
                }
                if (inmueble.FileGaleria != null)
                {
                    foreach (var archivo in inmueble.FileGaleria)
                    {
                        string nombreArchivo = await _fileService.GuardarArchivoAsync(
                            archivo,
                            RUTA_CARPETA_IMAGENES
                        );
                        nombresArchivosGuardados.Add(nombreArchivo);
                        inmueble.listImagenes.Add(
                            new Imagen
                            {
                                Url = $"/{RUTA_CARPETA_IMAGENES}/{nombreArchivo}",
                                Tipo = 2,
                            }
                        );
                    }
                }
                await repositorio.CrearAsync(inmueble);
                TempData["Success"] =
                    $"Se agregó correctamente el inmueble en {inmueble.Direccion}";
                return RedirectToAction(nameof(Listar));
            }
            catch (Exception ex)
            {
                foreach (var nombreArchivo in nombresArchivosGuardados)
                {
                    _fileService.BorrarArchivo(nombreArchivo, RUTA_CARPETA_IMAGENES);
                }
                TempData["Error"] = "Error al crear inmueble: " + ex.Message;
                ViewBag.Tipos = repoTipo.TenerTodos();
                return View(inmueble);
            }
        }

        // GET: Inmueble/Modificar/{id}
        [HttpGet]
        public IActionResult Modificar(int id)
        {
            try
            {
                var inmueble = repositorio.ObtenerPorID(id);
                if (inmueble == null || inmueble.IdInmueble == 0)
                {
                    TempData["Error"] = "El inmueble no existe.";
                    return RedirectToAction(nameof(Listar));
                }
                ViewBag.Tipos = repoTipo.TenerTodos();
                return View(inmueble);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el inmueble para modificar: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        // POST: Inmueble/Modificar
        [HttpPost]
        public async Task<IActionResult> Modificar(Inmueble inmueble)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var original = repositorio.ObtenerPorID(inmueble.IdInmueble);
                    inmueble.listImagenes = original.listImagenes;
                    ViewBag.TipoInmuebles = repoTipo.TenerTodos();
                    return View(inmueble);
                }

                await ProcesarImagenesModificacionAsync(inmueble);

                await repositorio.ModificarAsync(inmueble);

                TempData["Success"] =
                    $"Se modificó correctamente el inmueble de {inmueble.Direccion}";
                return RedirectToAction(nameof(Listar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al modificar inmueble: " + ex.Message;
                return View(inmueble);
            }
        }

        // POST: Inmueble/Eliminar
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            try
            {
                if (repositorio.SeEstaUsando(id))
                {
                    TempData["Warning"] = "No se puede eliminar el inmueble porque está en uso";
                    return RedirectToAction(nameof(Listar));
                }

                var imagenesAEliminar = repositorio.ObtenerImagenesPorInmueble(id);
                foreach (var imagen in imagenesAEliminar)
                {
                    _fileService.BorrarArchivo(Path.GetFileName(imagen.Url), RUTA_CARPETA_IMAGENES);
                }

                if (repositorio.Eliminar(id) > 0)
                {
                    TempData["Success"] = "Se eliminó correctamente el inmueble";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el inmueble";
                }
                return RedirectToAction(nameof(Listar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al eliminar el inmueble: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        [NonAction]
        private async Task ProcesarImagenesModificacionAsync(Inmueble inmueble)
        {
            if (inmueble.EliminarIDs != null && inmueble.EliminarIDs.Any())
            {
                foreach (var id in inmueble.EliminarIDs)
                {
                    var imagen = repositorio.ObtenerImagenPorId(id);
                    if (imagen != null)
                    {
                        _fileService.BorrarArchivo(
                            Path.GetFileName(imagen.Url),
                            RUTA_CARPETA_IMAGENES
                        );
                        repositorio.EliminarImagen(id);
                    }
                }
            }

            if (inmueble.FilePortada != null)
            {
                var portadaActual = repositorio
                    .ObtenerImagenesPorInmueble(inmueble.IdInmueble)
                    .FirstOrDefault(i => i.Tipo == 1);

                if (portadaActual != null)
                {
                    _fileService.BorrarArchivo(
                        Path.GetFileName(portadaActual.Url),
                        RUTA_CARPETA_IMAGENES
                    );
                    repositorio.EliminarImagen(portadaActual.IdImagen);
                }

                var nombreArchivo = await _fileService.GuardarArchivoAsync(
                    inmueble.FilePortada,
                    RUTA_CARPETA_IMAGENES
                );
                inmueble.listImagenes = inmueble.listImagenes ?? new List<Imagen>();
                inmueble.listImagenes.Add(
                    new Imagen { Url = $"/{RUTA_CARPETA_IMAGENES}/{nombreArchivo}", Tipo = 1 }
                );
            }

            if (inmueble.FileGaleria != null && inmueble.FileGaleria.Any())
            {
                inmueble.listImagenes = inmueble.listImagenes ?? new List<Imagen>();
                foreach (var archivo in inmueble.FileGaleria)
                {
                    var nombreArchivo = await _fileService.GuardarArchivoAsync(
                        archivo,
                        RUTA_CARPETA_IMAGENES
                    );
                    inmueble.listImagenes.Add(
                        new Imagen { Url = $"/{RUTA_CARPETA_IMAGENES}/{nombreArchivo}", Tipo = 2 }
                    );
                }
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

        // GET: Inmueble/Inmueble/{idInmueble}
        public IActionResult Inmueble(int idInmueble)
        {
            var inmueble = repositorio.ObtenerPorID(idInmueble);
            if (inmueble != null && inmueble.Estado == 1)
            {
                return Ok(inmueble);
            }
            return Ok(null);
        }

        [HttpGet]
        public IActionResult Calendario(int id)
        {
            var inmueble = repositorio.ObtenerPorID(id);
            if (inmueble == null || inmueble.IdInmueble <= 0)
            {
                TempData["Error"] = "Error al cargar el Calendario, no exite ese inmueble";
                return RedirectToAction(nameof(Listar));
            }
            return View(inmueble);
        }
    }
}
