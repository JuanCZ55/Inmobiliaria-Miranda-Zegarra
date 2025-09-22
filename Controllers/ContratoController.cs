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
      return View("~/Views/Home/Index.cshtml");
    }

    [HttpGet]
    public IActionResult Crear(int? idInmueble)
    {
      Contrato contrato = new Contrato
      {
        IdContrato = 0,
        Inquilino = new Inquilino(),
        Inmueble = new Inmueble(),
      };
      string? contratoJson = TempData["Contrato"] as string;
      if (!string.IsNullOrEmpty(contratoJson))
      {
        contrato = JsonSerializer.Deserialize<Contrato>(contratoJson) ?? contrato;
      }

      string? dni = TempData["Dni"] as string;
      if (!string.IsNullOrEmpty(dni))
      {
        ViewBag.Dni = dni;
        contrato.Inquilino = repositorioInquilino.ObtenerPorDni(dni) ?? new Inquilino();
      }

      if (idInmueble != null)
      {
        contrato.IdInmueble = idInmueble.Value;
        contrato.Inmueble = repositorioInmueble.ObtenerPorID(idInmueble.Value) ?? new Inmueble();
      }

      return View("Gestion", contrato);
    }
    public IActionResult CrearContrato(Contrato contrato, string DniInquilino)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          TempData["Error"] = "Modelo invalido";
          TempData["Contrato"] = JsonSerializer.Serialize(contrato);
          TempData["DniInquilino"] = DniInquilino;
          return RedirectToAction("Crear");
        }

        var errores = new List<string>();
        errores.AddRange(ValidarInquilino(null, DniInquilino));
        errores.AddRange(ValidarInmueble(contrato.IdInmueble));
        errores.AddRange(ValidarFechaContrato(contrato));
        if (errores.Any())
        {
          TempData["Error"] = string.Join(" | ", errores);
          TempData["Contrato"] = JsonSerializer.Serialize(contrato);
          TempData["DniInquilino"] = DniInquilino;
          return RedirectToAction("Crear");
        }

        Pago pago = new Pago
        {
          numeroPago = 1,
          FechaPago = DateTime.Today
        };
        if (contrato.Tipo == 1)
        {
          pago.Concepto = "Deposito de todo el Alquiler del Contrato";
          pago.Monto = contrato.Monto;
        }
        else
        {
          if (((contrato.FechaFinalizacion.Year - contrato.FechaInicio.Year) * 12) + (contrato.FechaFinalizacion.Month - contrato.FechaInicio.Month) < 4)
          {
            pago.Concepto = "Deposito de un mes";
            pago.Monto = contrato.Monto;
          }
          else
          {
            pago.Concepto = "Deposito de dos meses";
            pago.Monto = contrato.Monto * 2;
          }
        }

        var idContrato = repositorio.CrearContratoConPago(contrato, pago);
        if (idContrato <= 0)
        {
          TempData["Error"] = "Error al intentar crear el Contrato";
          TempData["Contrato"] = JsonSerializer.Serialize(contrato);
          TempData["DniInquilino"] = DniInquilino;
          return RedirectToAction("Crear");
        }

        TempData["Success"] = "Se creo correctamente el Contrato";
        return RedirectToAction("Ver", "Contrato", new { id = idContrato, returnUrl = Url.Action("Crear") });
      }
      catch (Exception)
      {
        TempData["Error"] = "Error al intentar crear el Contrato Excepcion";
        TempData["Contrato"] = JsonSerializer.Serialize(contrato);
        TempData["DniInquilino"] = DniInquilino;
        return RedirectToAction("Crear");
      }
    }

    [HttpGet]
    [Route("Contrato/Ver/{idContrato}")]
    public IActionResult Ver(int idContrato, string returnUrl)
    {
      try
      {
        Contrato contrato = repositorio.ObtenerPorID(idContrato);
        ViewBag.ReturnUrl = returnUrl ?? Url.Action("Listar");
        ViewBag.PagarMulta = contrato.IdContrato > 0 && contrato.Estado == 3 && !repositorioPago.MultaPagada(contrato.IdContrato);
        ViewBag.Pagos = repositorioPago.BuscarPorContrato(contrato.IdContrato).Count == 1;
        return View("Gestion", contrato);
      }
      catch (Exception)
      {
        return RedirectToAction("Listar");
      }
    }

    [HttpPost]
    public IActionResult ModificarContrato(Contrato contrato)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          TempData["Error"] = "Modelo invalido";
          return RedirectToAction("Ver", new { id = contrato.IdContrato });
        }
        var errores = new List<string>();

        errores.AddRange(ValidarInquilino(contrato.IdInquilino, null));

        errores.AddRange(ValidarInmueble(contrato.IdInmueble));

        errores.AddRange(ValidarFechaContrato(contrato));

        if (errores.Any())
        {
          TempData["Error"] = string.Join(" | ", errores);
          return RedirectToAction("Ver", new { id = contrato.IdContrato });
        }

        var modificar = repositorio.Modificar(contrato);
        if (modificar <= 0)
        {
          TempData["Error"] = "Error al intentar modificar el Contrato";
          return RedirectToAction("Ver", new { id = contrato.IdContrato });
        }

        TempData["Success"] = "Se modifico correctamente el Contrato";
        return RedirectToAction("Ver", new { id = contrato.IdContrato });
      }
      catch (Exception)
      {
        TempData["Error"] = "Error al intentar modificar el Contrato";
        return RedirectToAction("Ver", new { id = contrato.IdContrato });
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

          if (contrato.FechaCancelacion != null && repositorio.validarContratoCancelar(contrato.IdContrato, contrato.FechaCancelacion) != 1)
          {
            TempData["MensajeError"] = "Contrato no cancelable";
            return View("Gestion", contrato);
          }
          if (repositorio.validarFechaMayorMulta(contrato.IdContrato, contrato.FechaCancelacion) == 1)
          {
            TempData["MensajeError"] = "Multa de un mes";
            contrato.Multa = contrato.Monto;
          }
          else
          {
            TempData["MensajeError"] = "Multa de dos meses";
            contrato.Multa = contrato.Monto * 2;
          }
          repositorio.Cancelado(contrato);
          Contrato nuevo = repositorio.ObtenerPorID(contrato.IdContrato);
          return RedirectToAction("Ver", new { id = nuevo.IdContrato });
        }
        else
          TempData["MensajeError"] = "Modelo invalido";
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
    public IActionResult EliminarContrato(int IdContrato)
    {
      try
      {
        if (repositorio.EliminarContratoConPagoS(IdContrato) > 0)
        {
          TempData["Success"] = "Se eliminó correctamente el contrato";
          return RedirectToAction(nameof(Listar));
        }
        TempData["Error"] = "No se pudo eliminar el contrato";
        return RedirectToAction("Ver", "Contrato", new { IdContrato, returnUrl = Url.Action("Listar") });
      }
      catch (Exception)
      {
        TempData["Error"] = "Error al intentar eliminar el contrato";
        return RedirectToAction("Ver", "Contrato", new { IdContrato, returnUrl = Url.Action("Listar") });
      }
    }

    // GET: Contrato/Listar
    [HttpGet]
    public IActionResult Listar(string? idContrato, string? dniInquilino, string? idInmueble, string? estado, string? Fecha_desde, string? Fecha_hasta, int PaginaActual = 1)
    {
      int registrosPorPagina = 9;
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

    [NonAction]
    public List<string> ValidarFechaContrato(Contrato contrato)
    {
      var errores = new List<string>();
      try
      {
        if (repositorio.ValidarSolapamiento(contrato) != 0)
        {
          errores.Add("Solapamiento de fechas con otro contrato");
        }
      }
      catch (Exception)
      {
        errores.Add("Error al verificar el solapamiento de fechas");
      }
      return errores;
    }

    [NonAction]
    public List<string> ValidarInmueble(int idInmueble)
    {
      var errores = new List<string>();
      try
      {
        Inmueble inmueble = repositorioInmueble.ObtenerPorID(idInmueble);
        if (inmueble.IdInmueble == 0 || inmueble.Estado != 1)
        {
          errores.Add("Inmueble no disponible");
        }
      }
      catch (Exception)
      {
        errores.Add("Error al verificar el Inmueble");
      }
      return errores;
    }
    [NonAction]
    public List<string> ValidarInquilino(int? idInquilino, string? dniInquilino)
    {
      var errores = new List<string>();
      try
      {
        Inquilino? inquilino = null;
        if (idInquilino != null)
        {
          inquilino = repositorioInquilino.ObtenerPorID(idInquilino.Value);
        }
        else if (dniInquilino != null)
        {
          inquilino = repositorioInquilino.ObtenerPorDni(dniInquilino);
        }
        if (inquilino == null || inquilino.IdInquilino == 0 || inquilino.Estado != 1)
        {
          errores.Add("Inquilino no disponible");
        }
      }
      catch (Exception)
      {
        errores.Add("Error al verificar el Inquilino");
      }
      return errores;
    }

    [HttpGet]
    public IActionResult Fechas(int idInmueble, string? idContrato)
    {
      var contratos = repositorio.FechasOcupadas(idInmueble, idContrato);
      var fechas = contratos.Select(c => new
      {
        inicio = c.FechaInicio.ToString("yyyy-MM-dd"),
        fin = c.FechaFinalizacion.ToString("yyyy-MM-dd")
      });
      return Json(fechas);
    }
  }
}
