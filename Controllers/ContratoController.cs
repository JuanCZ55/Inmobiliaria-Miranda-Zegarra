using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
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
      TempData["MensajeError"] = JsonSerializer.Serialize(contrato.Estado);
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

          Inquilino inqui =  (repositorioInquilino.ObtenerPorID(contrato.IdInquilino));
          if (inqui == null)
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

          Contrato nuevo =  repositorio.ObtenerPorID(repositorio.Crear(contrato));
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

    // POST: Contrato/Cancelado/5
    [HttpPost]
    public IActionResult Cancelado(Contrato contrato)
    {
      try
      {
        if (ModelState.IsValid)
        {
          Contrato c = repositorio.ObtenerPorID(contrato.IdContrato);
          if (c == null)
          {
            return RedirectToAction(nameof(Listar));
          }
          if (c.Estado == 1)
          {
            repositorio.Cancelado(contrato);
            return RedirectToAction(nameof(Listar));
          }
          TempData["ContratoNoCancelado"] = "Contrato no cancelado";
          return RedirectToAction(nameof(Listar));
        }
        else
          return View(contrato);
      }
      catch (System.Exception)
      {
        throw;
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
    public IActionResult Listar()
    {
      var lista = repositorio.ObtenerTodos();
      return View(lista);
    }

    [HttpGet]
    public IActionResult Listartodos(string idContrato, string? dniInquilino, string? idInmueble, string? estado, string? Fecha_desde, string? Fecha_hasta)
    {

      var contratos = repositorio.Filtrar(idContrato, dniInquilino, idInmueble, estado, Fecha_desde, Fecha_hasta);
      return Ok(contratos);
    }

  }
}
