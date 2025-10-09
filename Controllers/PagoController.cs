using System.Text.Json;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inmobiliaria.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class PagoController : Controller
    {
        private readonly IRepositorioPago repositorio;
        private readonly IRepositorioPago repositorioPago;
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioInquilino repositorioInquilino;
        private readonly IRepositorioContrato repositorioContraro;
        private readonly IRepositorioUsuario repositorioUsuario;
        private readonly IConfiguration config;

        // GET: Contrato
        public PagoController(
            IRepositorioPago repositorio,
            IRepositorioPago repositorioPago,
            IRepositorioInmueble repositorioInmueble,
            IRepositorioInquilino repositorioInquilino,
            IRepositorioContrato repositorioContraro,
            IRepositorioUsuario repositorioUsuario,
            IConfiguration config
        )
        {
            this.repositorio = repositorio;
            this.repositorioPago = repositorioPago;
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioInquilino = repositorioInquilino;
            this.repositorioContraro = repositorioContraro;
            this.repositorioUsuario = repositorioUsuario;
            this.config = config;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Listar");
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
                        contrato = repositorioContraro.ObtenerPorID(idContrato.Value),
                        FechaPago = DateTime.Today,
                    };
                    if (pago.contrato.Estado == "Finalizado")
                    {
                        ViewBag.MultaPagada = "Contrato Finalizado";
                        return View("Gestion", pago);
                    }
                    if (pago.contrato.Estado == "Cancelado con Multa Saldada")
                    {
                        if (!repositorio.MultaPagada(idContrato))
                        {
                            pago.Concepto = "Multa";
                            pago.Monto = pago.contrato?.Multa ?? 0;
                            pago.numeroPago = repositorio.CantidadPago(idContrato) + 1;
                        }
                        else
                        {
                            ViewBag.MultaPagada = "Multa Pagada";
                        }
                    }
                    else if (pago.contrato.Estado == "Cancelado con Multa Pendiente")
                    {
                        ViewBag.MultaPagada = "Sin Multa";
                        pago.Monto = pago.contrato?.Monto ?? 0;
                        pago.numeroPago = repositorio.CantidadPago(idContrato) + 1;
                    }
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
                if (User.IsInRole("Administrador"))
                {
                    pago.user = repositorioUsuario.ObtenerPorId(pago.IdUsuario);
                }
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
                return RedirectToAction("Crear", new { idContrato=pago.IdContrato });
            }
            try
            {
                Contrato contrato = repositorioContraro.ObtenerPorID(pago.IdContrato);
                if (pago.FechaPago != DateTime.Today)
                {
                    TempData["MensajeError"] = "Fachas Invalidas";
                    TempData["Pago"] = JsonSerializer.Serialize(pago);
                    return RedirectToAction("Crear", new { idContrato=pago.IdContrato });
                }
                pago.numeroPago = repositorio.CantidadPago(pago.IdContrato) + 1;

                var idUsuarioClaim = User.FindFirstValue("IdUsuario");
                if (int.TryParse(idUsuarioClaim, out int idUsuario))
                {
                    pago.IdUsuario = idUsuario;
                }
                else
                {
                    TempData["Error"] =
                        "No se pudo identificar al usuario creador. Sesión inválida.";
                    return RedirectToAction("Crear", new { idContrato=pago.IdContrato });
                }


                if (pago.Concepto == "Multa de Cancelacion")
                {
                    if (contrato.Estado != "Cancelado con Multa Pendiente" || contrato.Estado != "Cancelado con Multa Saldada")
                    {
                        TempData["Error"] =
                            "Ese concepto de pago es unicamente para los contratos Cancelados";
                        return RedirectToAction("Crear", new { idContrato=pago.IdContrato });
                    }   
                    if ( pago.Monto < contrato.Multa)
                    {
                        TempData["Error"] =
                            "La multa se debe pagar en un unico pago";
                        return RedirectToAction("Crear", new { idContrato=pago.IdContrato });
                    }   
                }

                var idPago = repositorio.Crear(pago);
                if (idPago <= 0)
                {
                    TempData["MensajeError"] = "Error al crear el pago";
                    TempData["Pago"] = JsonSerializer.Serialize(pago);
                    return RedirectToAction("Crear", new { idContrato=pago.IdContrato });
                }
                TempData["Success"] = "Pago Creado";
                return RedirectToAction("Ver", new { id = idPago });
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Modelo invalido";
                TempData["Pago"] = JsonSerializer.Serialize(pago);
                return RedirectToAction("Crear", new { idContrato=pago.IdContrato });
            }
        }

        [HttpPost]
        public IActionResult ModificarPago(Pago pago)
        {
            try
            {
                if (pago.IdPago <= 0)
                {
                    TempData["Error"] = "ID de pago inválido.";
                    return RedirectToAction("Listar");
                }

                Pago pagoExistente = repositorio.ObtenerPorID(pago.IdPago);
                if (pagoExistente == null)
                {
                    TempData["Error"] = "El pago que intenta modificar no existe.";
                    return RedirectToAction("Listar");
                }

                Contrato contrato = repositorioContraro.ObtenerPorID(pagoExistente.IdContrato);
                if (contrato.Estado == "Cancelado con Multa Pendiente" || contrato.Estado == "Cancelado con Multa Saldada")
                {
                    TempData["Error"] = "No se pueden modificar pagos de un contrato que ya ha finalizado o ha sido cancelado.";
                    return RedirectToAction("Ver", new { id = pago.IdPago });
                }

                pagoExistente.Concepto = pago.Concepto;

                int resultado = repositorio.Modificar(pagoExistente);

                if (resultado > 0)
                {
                    TempData["Success"] = "El pago se modificó correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se pudo modificar el pago.";
                }
                return RedirectToAction("Ver", new { id = pago.IdPago });
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error inesperado al modificar el pago.";
                return RedirectToAction("Ver", new { id = pago.IdPago });
            }
        }

        // GET: Pago/Listar
        [HttpGet]
        public IActionResult Listar(
            string? idPago,
            string? idContrato,
            string? dniInquilino,
            string? MontoMenor,
            string? MontoMayor,
            string? estado,
            string? Fecha_desde,
            string? Fecha_hasta,
            int PaginaActual = 1
        )
        {
            int registrosPorPagina = 7;
            int total = 0;
            int offset = (PaginaActual - 1) * registrosPorPagina;
            int limite = registrosPorPagina;
            List<Pago> lista;
            try
            {
                total = repositorio.CantidadFiltro(
                    idPago,
                    idContrato,
                    dniInquilino,
                    MontoMenor,
                    MontoMayor,
                    estado,
                    Fecha_desde,
                    Fecha_hasta
                );
                limite = Math.Min(registrosPorPagina, total - offset);
                lista = repositorio.Filtrar(
                    idPago,
                    idContrato,
                    dniInquilino,
                    MontoMenor,
                    MontoMayor,
                    estado,
                    Fecha_desde,
                    Fecha_hasta,
                    offset,
                    limite
                );

                int totalPaginas = (int)Math.Ceiling((double)total / registrosPorPagina);

                ViewBag.PaginaActual = PaginaActual;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.IdPago = idPago;
                ViewBag.IdContrato = idContrato;
                ViewBag.DniInquilino = dniInquilino;
                ViewBag.MontoMenor = MontoMenor;
                ViewBag.MontoMayor = MontoMayor;
                ViewBag.estado = estado;
                ViewBag.FechaDesde = Fecha_desde;
                ViewBag.FechaHasta = Fecha_hasta;

                return View(lista);
            }
            catch (System.Exception)
            {
                return View(new List<Pago>());
            }
        }

        [HttpPost]
        public IActionResult PagarMulta(int IdContrato)
        {
            try
            {
                Contrato contrato = repositorioContraro.ObtenerPorID(IdContrato);
                if (contrato.Estado != "Cancelado con Multa Pendiente" || contrato.Multa == null)
                {
                    TempData["Error"] = "No se puede pagar la multa";
                    return RedirectToAction("Ver", "Contrato", new { id = IdContrato });
                }
                Pago pago = new Pago
                {
                    IdContrato = IdContrato,
                    Monto = (decimal)contrato.Multa,
                    FechaPago = DateTime.Today,
                    numeroPago = repositorio.CantidadPago(IdContrato) + 1,
                    Concepto = "Multa de Cancelacion",
                };
                var idUsuarioClaim = User.FindFirstValue("IdUsuario");
                if (int.TryParse(idUsuarioClaim, out int idUsuario))
                {
                    pago.IdUsuario = idUsuario;
                }
                else
                {
                    TempData["Error"] =
                        "No se pudo identificar al usuario creador. Sesión inválida.";
                    return RedirectToAction("Ver", "Contrato", new { id = IdContrato });
                }
                int id = repositorio.Crear(pago);
                if (id <= 0)
                {
                    TempData["Error"] = "Error al intentar pagar la multa";
                    return RedirectToAction("Ver", "Contrato", new { id = IdContrato });
                }
                TempData["Success"] = "Se pago correctamente la multa";
                return RedirectToAction("Ver", "Contrato", new { id = IdContrato });
            }
            catch (Exception)
            {
                TempData["Error"] = "Error al intentar pagar la multa";
                return RedirectToAction("Ver", "Contrato", new { id = IdContrato });
            }
        }

        [HttpPost]
        public IActionResult CancelarPago(int idPago)
        {
            try
            {
                Pago pago = repositorio.ObtenerPorID(idPago);
                if (pago.Estado != 1)
                {
                    TempData["Error"] = "Pago no cancelable";
                    return RedirectToAction("Ver", "Pago", new { id = idPago });
                }
                repositorio.SetEstado(idPago, 2);
                TempData["Success"] = "Pago cancelado";
                return RedirectToAction("Ver", "Pago", new { id = idPago });
            }
            catch (System.Exception)
            {
                TempData["Error"] = "Error al intentar cancelar el pago";
                return RedirectToAction("Ver", "Pago", new { id = idPago });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult ActivarPago(int idPago)
        {
            try
            {
                Pago pago = repositorio.ObtenerPorID(idPago);
                pago.contrato = repositorioContraro.ObtenerPorID(pago.IdContrato);
                if (pago.Estado != 2)
                {
                    TempData["Error"] = "Pago no activable";
                    return RedirectToAction("Ver", "Pago", new { id = idPago });
                }
                if (pago.contrato.Estado == "Finalizado")
                {
                    TempData["Error"] = "No se pueden modificar pagos de contratos finalizado";
                    return RedirectToAction("Ver", "Pago", new { id = idPago });
                }
                else if (
                    pago.contrato.Estado
                    == "No se pueden modificar pagos de contratos cancelados con multa saldada"
                )
                {
                    TempData["Error"] = "Contrato Cancelado con Multa Saldada";
                    return RedirectToAction("Ver", "Pago", new { id = idPago });
                }
                repositorio.SetEstado(idPago, 1);
                TempData["Success"] = "Pago Restaurado";
                return RedirectToAction("Ver", "Pago", new { id = idPago });
            }
            catch (System.Exception)
            {
                TempData["Error"] = "Error al intentar cancelar el pago";
                return RedirectToAction("Ver", "Pago", new { id = idPago });
            }
        }
    }
}
