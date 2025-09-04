using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using System.Text.Json;
namespace Inmobiliaria.Controllers
{

  public class ContratoController : Controller
  {
    // Sin inyección de dependencias (crear dentro del ctor)
    private readonly RepositorioContraro repositorio;
    private readonly RepositorioPago repositorioPago;
    private readonly RepositorioInmueble repositorioInmueble;
    private readonly RepositorioInquilino repositorioInquilino;
    // GET: Contrato
    public ContratoController(IConfiguration config)
    {
      // Sin inyección de dependencias y sin usar el config (quitar el parámetro repo del ctor)
      this.repositorio = new RepositorioContraro(config);
      this.repositorioPago = new RepositorioPago(config);
      this.repositorioInmueble = new RepositorioInmueble(config);
      this.repositorioInquilino = new RepositorioInquilino(config);
    }
    public IActionResult Index()
    {
      return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Crear(int? idInmueble)
    {
      Contrato contrato = new Contrato();
      if (idInmueble == null)
      {
        string? contratoJson = TempData["Contrato"] as string;
        if (!string.IsNullOrEmpty(contratoJson))
        {
          contrato = JsonSerializer.Deserialize<Contrato>(contratoJson) ?? new Contrato();
        }
        return View("Gestion", contrato);
      }
      else
      {
        contrato = new Contrato
        {
          IdInmueble = idInmueble.Value,
          Inmueble = repositorioInmueble.ObtenerPorID(idInmueble.Value)
        };
        return View("Gestion", contrato);
      }
    }


    [HttpGet]
    public IActionResult Ver(int id)
    {
      Contrato contrato;
      try
      {
        contrato = repositorio.ObtenerPorID(id);
        return View("Gestion", contrato);
      }
      catch (System.Exception)
      {
        contrato = new Contrato();
        return View("Gestion", contrato);
      }
    }

    //POST: Contrato/Crear
    [HttpPost]
    public IActionResult Crear(Contrato contrato)
    {
      try
      {
        if (ModelState.IsValid)
        {
          if (repositorioInmueble.ObtenerPorID(contrato.IdInmueble).Estado != 1)
          {
            TempData["MensajeError"] = "Inmueble inactivo";
            TempData["Contrato"] = JsonSerializer.Serialize(contrato);
            return RedirectToAction("Crear");
          }

          Inquilino inqui = repositorioInquilino.ObtenerPorID(contrato.IdInquilino);
          if (inqui.IdInquilino == 0)
          {
            TempData["MensajeError"] = "Inquilino inactivo";
            TempData["Contrato"] = JsonSerializer.Serialize(contrato);
            return RedirectToAction("Crear");
          }

          if (contrato.FechaDesde < DateTime.Now || contrato.FechaHasta < contrato.FechaDesde)
          {
            TempData["MensajeError"] = "Fachas Invalidas";
            TempData["Contrato"] = JsonSerializer.Serialize(contrato);
            return RedirectToAction("Crear");
          }

          int cont = repositorio.ExisteSolapamiento(contrato.IdInmueble, contrato.FechaDesde, contrato.FechaHasta);
          if (cont != 0)
          {
            TempData["MensajeError"] = "Se solapa con otro contrato";
            TempData["Contrato"] = JsonSerializer.Serialize(contrato);
            return RedirectToAction("Crear");
          }

          Contrato nuevo = repositorio.ObtenerPorID(repositorio.Crear(contrato));
          TempData["MensajeError"] = "Contrato Creado";
          return RedirectToAction("Ver", new { id = nuevo.IdContrato });
        }
        TempData["MensajeError"] = "Modelo invalido";
        TempData["Contrato"] = JsonSerializer.Serialize(contrato);
        return RedirectToAction("Crear");
      }
      catch (System.Exception)
      {
        TempData["MensajeError"] = "Modelo invalido";
        TempData["Contrato"] = JsonSerializer.Serialize(contrato);
        return RedirectToAction("Crear");
      }
    }

    // POST: Contrato/Cancelar
    [HttpPost]
    public IActionResult Cancelar(Contrato contrato)
    {
      try
      {
        if (ModelState.IsValid)
        {
          if (contrato.FechaFin?.Date != DateTime.Today)
          {
            TempData["MensajeError"] = "Fecha invalida";
            return View("Gestion", contrato);
          }
          if (contrato.FechaFin != null && repositorio.validarContratoCancelar(contrato.IdContrato, contrato.FechaFin) != 1)
          {
            TempData["MensajeError"] = "Contrato no cancelable";
            return View("Gestion", contrato);
          }
          if (repositorio.validarFechaMayorMulta(contrato.IdContrato, contrato.FechaFin) == 1)
          {
            TempData["MensajeError"] = "Multa de un mes";
            contrato.Multa = contrato.MontoMensual;
          }
          else
          {
            TempData["MensajeError"] = "Multa de dos meses";
            contrato.Multa = contrato.MontoMensual * 2;
          }
          repositorio.Cancelado(contrato);
          Contrato nuevo = repositorio.ObtenerPorID(contrato.IdContrato);
          return RedirectToAction("Ver", new { id = nuevo.IdContrato });
        }
        else
          return View("Gestion", contrato);
      }
      catch (System.Exception)
      {
        TempData["MensajeError"] = JsonSerializer.Serialize(contrato);
        return View("Gestion", contrato);
      }
    }

    // POST: Contrato/Eliminar/5
    [HttpPost]
    public IActionResult Eliminar(int id)
    {
      try
      {
        repositorioPago.Eliminar(id);
        if (repositorio.Eliminar(id) > 0)
        {
          TempData["Mensaje"] = "Se eliminó correctamente el contrato";
          return RedirectToAction(nameof(Listar));
        }
        TempData["Mensaje"] = "No se pudo eliminar el contrato";
        return RedirectToAction(nameof(Listar));
      }
      catch (System.Exception)
      {
        return RedirectToAction(nameof(Listar));
      }
    }

    // GET: Contrato/Listar
    [HttpGet]
    public IActionResult Listar(string? idContrato, string? dniInquilino, string? idInmueble, string? estado, string? Fecha_desde, string? Fecha_hasta, int PaginaActual = 1)
    {
      int registrosPorPagina = 7;
      int total = 0;
      int offset = (PaginaActual - 1) * registrosPorPagina;
      int limite = registrosPorPagina;
      List<Contrato> lista;
      try
      {
        total = repositorio.CantidadFiltro(idContrato, dniInquilino, idInmueble, estado, Fecha_desde, Fecha_hasta);
        limite = Math.Min(registrosPorPagina, total - offset);
        lista = repositorio.Filtrar(idContrato, dniInquilino, idInmueble, estado, Fecha_desde, Fecha_hasta, offset, limite);

        int totalPaginas = (int)Math.Ceiling((double)total / registrosPorPagina);

        ViewBag.PaginaActual = PaginaActual;
        ViewBag.TotalPaginas = totalPaginas;
        ViewBag.IdContrato = idContrato;
        ViewBag.DniInquilino = dniInquilino;
        ViewBag.IdInmueble = idInmueble;
        ViewBag.Estado = estado;
        ViewBag.FechaDesde = Fecha_desde;
        ViewBag.FechaHasta = Fecha_hasta;

        return View(lista);
      }
      catch (System.Exception)
      {
        return View(new List<Contrato>());
      }
    }
    [HttpGet]
    public IActionResult Contrato(int idContrato)
    {
      Contrato contrato = new Contrato();
      try
      {
        contrato = repositorio.ObtenerPorID(idContrato);
        return Ok(contrato);
      }
      catch (System.Exception)
      {
        return Ok(contrato);
      }
    }

  }

}
