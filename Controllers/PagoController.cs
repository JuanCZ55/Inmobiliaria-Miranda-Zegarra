using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using System.Text.Json;
namespace Inmobiliaria.Controllers
{

  public class PagoController : Controller
  {
    // Sin inyección de dependencias (crear dentro del ctor)
    private readonly RepositorioPago repositorio;
    private readonly RepositorioPago repositorioPago;
    private readonly RepositorioInmueble repositorioInmueble;
    private readonly RepositorioInquilino repositorioInquilino;
    private readonly RepositorioContraro repositorioContraro;

    // GET: Contrato
    public PagoController(IConfiguration config)
    {
      // Sin inyección de dependencias y sin usar el config (quitar el parámetro repo del ctor)
      this.repositorio = new RepositorioPago(config);
      this.repositorioPago = new RepositorioPago(config);
      this.repositorioInmueble = new RepositorioInmueble(config);
      this.repositorioInquilino = new RepositorioInquilino(config);
      this.repositorioContraro = new RepositorioContraro(config);
    }
    public IActionResult Index()
    {
      return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Crear(int? idContrato)
    {
      Pago pago = new Pago();
      pago.FechaPago = DateTime.Today;
      if (idContrato == null)
      {
        string? pagoJson = TempData["Pago"] as string;
        if (!string.IsNullOrEmpty(pagoJson))
        {
          pago = JsonSerializer.Deserialize<Pago>(pagoJson) ?? new Pago();
        }
        return View("Gestion", pago);
      }
      else
      {
        try
        {
          pago = new Pago
          {
            IdContrato = idContrato.Value,
            contrato = repositorioContraro.ObtenerPorID(idContrato.Value)
          };
          return View("Gestion", pago);
        }
        catch (System.Exception)
        {
          TempData["MensajeError"] = "Error al obtener el contrato";
          return View("Gestion", pago);
        }
      }
    }


    [HttpGet]
    public IActionResult Ver(int id)
    {
      Pago pago;
      try
      {
        pago = repositorio.ObtenerPorID(id);
        pago.contrato = repositorioContraro.ObtenerPorID(pago.IdContrato);
        return View("Gestion", pago);
      }
      catch (System.Exception)
      {
        pago = new Pago();
        return View("Gestion", pago);
      }
    }

    //POST: Contrato/Crear
    [HttpPost]
    public IActionResult Crear(Pago pago)
    {
      if (!ModelState.IsValid)
      {
        TempData["MensajeError"] = "Modelo invalido";
        TempData["Pago"] = JsonSerializer.Serialize(pago);
        return RedirectToAction("Crear");
      }
      try
      {
        if (repositorioContraro.ObtenerPorID(pago.IdContrato).Estado != 1)
        {
          TempData["MensajeError"] = "Contrato inactivo";
          TempData["Pago"] = JsonSerializer.Serialize(pago);
          return RedirectToAction("Crear");
        }

        if (pago.FechaPago != DateTime.Today)
        {
          TempData["MensajeError"] = "Fachas Invalidas";
          TempData["Pago"] = JsonSerializer.Serialize(pago);
          return RedirectToAction("Crear");
        }

        Pago nuevo = repositorio.ObtenerPorID(repositorio.Crear(pago));
        TempData["MensajeError"] = "Pago Creado";
        return RedirectToAction("Ver", new { id = nuevo.IdContrato });
      }

      catch (System.Exception)
      {
        TempData["MensajeError"] = "Modelo invalido";
        TempData["Pago"] = JsonSerializer.Serialize(pago);
        return RedirectToAction("Crear");
      }
    }

    // GET: Pago/Listar
    [HttpGet]
    public IActionResult Listar(string? idPago, string? MontoMenor, string? MontoMayor, string? estado, string? Fecha_desde, string? Fecha_hasta, int PaginaActual = 1)
    {
      int registrosPorPagina = 7;
      int total = 0;
      int offset = (PaginaActual - 1) * registrosPorPagina;
      int limite = registrosPorPagina;
      List<Pago> lista;
      try
      {
        total = repositorio.CantidadFiltro(idPago, MontoMenor, MontoMayor, estado, Fecha_desde, Fecha_hasta);
        limite = Math.Min(registrosPorPagina, total - offset);
        lista = repositorio.Filtrar(idPago, MontoMenor, MontoMayor, estado, Fecha_desde, Fecha_hasta, offset, limite);

        int totalPaginas = (int)Math.Ceiling((double)total / registrosPorPagina);

        ViewBag.PaginaActual = PaginaActual;
        ViewBag.TotalPaginas = totalPaginas;
        ViewBag.IdPago = idPago;
        ViewBag.MontoMenor = MontoMenor;
        ViewBag.MontoMayor = MontoMayor;
        ViewBag.estado = estado;
        ViewBag.Fecha_desde = Fecha_desde;
        ViewBag.Fecha_hasta = Fecha_hasta;

        return View(lista);
      }
      catch (System.Exception)
      {
        return View(new List<Pago>());
      }
    }
  }
}
