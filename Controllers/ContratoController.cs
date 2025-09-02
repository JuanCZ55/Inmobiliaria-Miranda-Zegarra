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
      Contrato contrato = repositorio.ObtenerPorID(id);
      return View("Gestion", contrato);

    }



    //POST: Contrato/Crear
    [HttpPost]
    public IActionResult Crear(Contrato contrato)
    {
      try
      {

        TempData["MensajeError"] = "Modelo invalido";
        if (ModelState.IsValid)
        {
          if (repositorioInmueble.ObtenerPorID(contrato.IdInmueble).Estado != 1)
          {
            TempData["MensajeError"] = "Inmueble inactivo";
            return View("Gestion", contrato);
          }

          Inquilino inqui = repositorioInquilino.ObtenerPorID(contrato.IdInquilino);
          if (inqui.IdInquilino == 0)
          {
            TempData["MensajeError"] = "Inquilino inactivo";
            return View("Gestion", contrato);
          }

          if (contrato.FechaDesde < DateTime.Now || contrato.FechaHasta < contrato.FechaDesde)
          {
            TempData["MensajeError"] = "Fachas Invalidas";
            return View("Gestion", contrato);
          }

          int cont = repositorio.ExisteSolapamiento(contrato.IdInmueble, contrato.FechaDesde, contrato.FechaHasta);
          if (cont != 0)
          {
            TempData["MensajeError"] = "Se solapa con otro contrato";
            return View("Gestion", contrato);
          }

          Contrato nuevo = repositorio.ObtenerPorID(repositorio.Crear(contrato));
          TempData["MensajeError"] = "Contrato Creado";
          return View("Gestion", nuevo);
        }

        return View("Gestion", contrato);
      }
      catch (System.Exception)
      {
        return View("Gestion", contrato);
      }
    }

    /*
        // POST: Contrato/Finalizar/5
        [HttpPost]
        public IActionResult Finalizar(int id)
        {
          try
          {
            Contrato c = repositorio.ObtenerPorID(id);
            if (c == null)
            {
              return RedirectToAction(nameof(Listar));
            }
            if (c.Estado == 1)
            {
              repositorio.Finalizar(c);
              return View(c);
            }
            TempData["ContratoNoVigente"] = "Contrato no finalizado";
            return View(c);
          }
          catch (System.Exception)
          {
            throw;
          }
        }
    */

    // POST: Contrato/Cancelado
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
          return View("Gestion", contrato);
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
  }
}
