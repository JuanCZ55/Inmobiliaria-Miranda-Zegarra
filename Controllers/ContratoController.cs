using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using System.Text.Json;
namespace Inmobiliaria.Controllers
{

  public class ContratoController : Controller
  {
    private readonly IRepositorioContrato repositorio;
    private readonly IRepositorioPago repositorioPago;
    private readonly IRepositorioInmueble repositorioInmueble;
    private readonly IRepositorioInquilino repositorioInquilino;

    private readonly IConfiguration config;

    // GET: Contrato
    public ContratoController(IRepositorioContrato repositorio, IRepositorioPago repositorioPago, IRepositorioInmueble repositorioInmueble, IRepositorioInquilino repositorioInquilino, IConfiguration config)
    {
      this.repositorio = repositorio;
      this.repositorioPago = repositorioPago;
      this.repositorioInmueble = repositorioInmueble;
      this.repositorioInquilino = repositorioInquilino;
      this.config = config;
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
        contrato.FechaDesde = DateTime.Today;
        contrato.FechaHasta = DateTime.Today.AddMonths(6);
        return View("Gestion", contrato);
      }
      else
      {
        contrato = new Contrato
        {
          IdInmueble = idInmueble.Value,
          Inmueble = repositorioInmueble.ObtenerPorID(idInmueble.Value),
          FechaDesde = DateTime.Today,
          FechaHasta = DateTime.Today.AddMonths(6)
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

        ViewBag.Renovar = contrato.Estado != 1 ? "Renovar" : null;
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
        if (!ModelState.IsValid)
        {
          TempData["MensajeError"] = "Modelo invalido";
          TempData["Contrato"] = JsonSerializer.Serialize(contrato);
          return RedirectToAction("Crear");
        }
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

        if (contrato.FechaDesde < DateTime.Today || contrato.FechaHasta < contrato.FechaDesde.AddMonths(6))
        {
          TempData["MensajeError"] = "Fechas Invalidas";
          TempData["Contrato"] = JsonSerializer.Serialize(contrato);
          return RedirectToAction("Crear");
        }

        Contrato nuevo = repositorio.ObtenerPorID(repositorio.Crear(contrato));
        repositorioInmueble.SeEstaUsando(contrato.IdInmueble);
        TempData["MensajeError"] = "Contrato Creado";
        return RedirectToAction("Ver", new { id = nuevo.IdContrato });
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
