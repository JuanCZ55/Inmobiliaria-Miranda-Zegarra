using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
namespace Inmobiliaria.Controllers
{

  public class ContratoController : Controller
  {
    // Sin inyección de dependencias (crear dentro del ctor)
    private readonly RepositorioContraro repositorio;
    private readonly RepositorioPago repositorioPago;
    // GET: Contrato
    public ContratoController(IConfiguration config)
    {
      // Sin inyección de dependencias y sin usar el config (quitar el parámetro repo del ctor)
      this.repositorio = new RepositorioContraro(config);
      this.repositorioPago = new RepositorioPago(config);
    }
    public IActionResult Index()
    {
      return RedirectToAction("Index", "Home");
    }

        // GET: Contrato/Gestion
        [HttpGet]
        public IActionResult Crear()
        {
          return View("Gestion");
        }


        //POST: Contrato/Crear
        [HttpPost]
        public IActionResult Crear(Contrato contrato)
        {
          try
          {
            if (ModelState.IsValid)
            {
              repositorio.Crear(contrato);
              TempData["ContratoCreado"] = "Se agrego correctamente el contrato";
              return RedirectToAction(nameof(Crear));
            }
            else
              return View(contrato);
          }
          catch (System.Exception)
          {
            throw;
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
