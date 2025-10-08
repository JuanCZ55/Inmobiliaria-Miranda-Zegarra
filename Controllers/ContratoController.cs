using System.Security.Claims;
using System.Text.Json;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class ContratoController : Controller
    {
        private readonly IRepositorioContrato repositorio;
        private readonly IRepositorioPago repositorioPago;
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioInquilino repositorioInquilino;
        private readonly IRepositorioUsuario repositorioUsuario;

        private readonly IConfiguration config;

        // GET: Contrato
        public ContratoController(
            IRepositorioContrato repositorio,
            IRepositorioPago repositorioPago,
            IRepositorioInmueble repositorioInmueble,
            IRepositorioInquilino repositorioInquilino,
            IRepositorioUsuario repositorioUsuario,
            IConfiguration config
        )
        {
            this.repositorio = repositorio;
            this.repositorioPago = repositorioPago;
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioInquilino = repositorioInquilino;
            this.repositorioUsuario = repositorioUsuario;
            this.config = config;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Listar");
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Crear(int? idInmueble, DateTime? fecha)
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
                contrato.Inmueble =
                    repositorioInmueble.ObtenerPorID(idInmueble.Value) ?? new Inmueble();

                var contratos = repositorio.FechasOcupadas(idInmueble.Value, null);
                var fechas = contratos
                    .Select(c => new
                    {
                        inicio = c.FechaInicio.ToString("yyyy-MM-dd"),
                        fin = c.FechaFinalizacion.ToString("yyyy-MM-dd"),
                    })
                    .ToList();

                ViewBag.ListFechas = System.Text.Json.JsonSerializer.Serialize(fechas);
            }
            if (fecha != null)
            {
                contrato.FechaInicio = fecha.Value;
            }
            return View("Gestion", contrato);
        }

        [Authorize(Roles = "Administrador,Empleado")]
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

                var idUsuarioClaim = User.FindFirstValue("IdUsuario");
                if (int.TryParse(idUsuarioClaim, out int idUsuario))
                {
                    contrato.IdUsuarioCreador = idUsuario;
                }
                else
                {
                    TempData["Error"] =
                        "No se pudo identificar al usuario creador. Sesión inválida.";
                    return RedirectToAction("Crear");
                }

                Inquilino? inquilino = repositorioInquilino.ObtenerPorDni(DniInquilino);
                if (inquilino == null || inquilino.IdInquilino == 0)
                {
                    TempData["Error"] = "El DNI del inquilino no fue encontrado o no es válido.";
                    TempData["Contrato"] = JsonSerializer.Serialize(contrato);
                    TempData["DniInquilino"] = DniInquilino;
                    return RedirectToAction("Crear");
                }
                contrato.IdInquilino = inquilino.IdInquilino;
                var errores = new List<string>();
                errores.AddRange(ValidarInquilino(contrato.IdInquilino, null));
                errores.AddRange(ValidarInmueble(contrato.IdInmueble));
                errores.AddRange(ValidarFechaContrato(contrato));
                if (errores.Any())
                {
                    TempData["Error"] = string.Join(" | ", errores);
                    TempData["Contrato"] = JsonSerializer.Serialize(contrato);
                    TempData["DniInquilino"] = DniInquilino;
                    return RedirectToAction("Crear");
                }

                Pago pago = new Pago { numeroPago = 1, FechaPago = DateTime.Today };
                if (contrato.Tipo == 1)
                {
                    pago.Concepto = "Deposito de todo el Alquiler del Contrato";
                    pago.Monto = contrato.Monto;
                }
                else
                {
                    if (
                        ((contrato.FechaFinalizacion.Year - contrato.FechaInicio.Year) * 12)
                            + (contrato.FechaFinalizacion.Month - contrato.FechaInicio.Month)
                        < 4
                    )
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
                return RedirectToAction(
                    "Ver",
                    "Contrato",
                    new { id = idContrato, returnUrl = Url.Action("Crear") }
                );
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
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Ver(int idContrato, string returnUrl)
        {
            try
            {
                Contrato contrato = repositorio.ObtenerPorID(idContrato);
                if (contrato.FechaCancelacion != null)
                {
                    if (!repositorioPago.MultaPagada(contrato.IdContrato))
                    {
                        contrato.Estado = "Cancelado con Multa Pendiente";
                    }
                    else
                    {
                        contrato.Estado = "Cancelado con Multa Saldada";
                    }
                }
                else if (contrato.FechaFinalizacion < DateTime.Now)
                {
                    contrato.Estado = "Finalizado";
                }
                else
                {
                    ViewBag.Pagos =
                        repositorioPago.BuscarPorContrato(contrato.IdContrato).Count == 1;
                    contrato.Estado = "Vigente";
                    contrato.Multa = CalcularMulta(contrato);
                }
                if (User.IsInRole("Administrador"))
                {
                    if (contrato.IdUsuarioCreador > 0)
                    {
                        var usuarioCreador = repositorioUsuario.ObtenerPorId(
                            contrato.IdUsuarioCreador
                        );
                        ViewBag.UsuarioCreador = usuarioCreador;
                    }
                    if (
                        contrato.IdUsuarioFinalizador.HasValue
                        && contrato.IdUsuarioFinalizador.Value > 0
                    )
                    {
                        var usuarioFinalizador = repositorioUsuario.ObtenerPorId(
                            contrato.IdUsuarioFinalizador.Value
                        );
                        ViewBag.UsuarioFinalizador = usuarioFinalizador;
                    }
                }
                ViewBag.ReturnUrl = returnUrl ?? Url.Action("Listar");
                return View("Gestion", contrato);
            }
            catch (Exception)
            {
                TempData["Error"] = "Error al intentar obtener el Contrato";
                return RedirectToAction("Listar");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Empleado")]
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

        [HttpPost]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult RenovarContrato(Contrato contrato, int IdContratoOriginal)
        {
            var id = IdContratoOriginal;
            try
            {
                Contrato contratoViejo = repositorio.ObtenerPorID(IdContratoOriginal);
                if (
                    contratoViejo.FechaCancelacion != null
                    || contratoViejo.FechaFinalizacion >= DateTime.Today
                )
                {
                    TempData["Error"] = "Contrato no renobable";
                    return RedirectToAction("Ver", "Contrato", new { id });
                }
                var errores = new List<string>();
                errores.AddRange(ValidarInquilino(contrato.IdInquilino, null));
                errores.AddRange(ValidarInmueble(contrato.IdInmueble));
                errores.AddRange(ValidarFechaContrato(contrato));
                if (errores.Any())
                {
                    TempData["Error"] = string.Join(" | ", errores);
                    return RedirectToAction("Ver", "Contrato", new { id });
                }
                Pago pago = new Pago { numeroPago = 1, FechaPago = DateTime.Today };
                if (contrato.Tipo == 1)
                {
                    pago.Concepto = "Deposito de todo el Alquiler del Contrato";
                    pago.Monto = contrato.Monto;
                }
                else
                {
                    if (
                        ((contrato.FechaFinalizacion.Year - contrato.FechaInicio.Year) * 12)
                            + (contrato.FechaFinalizacion.Month - contrato.FechaInicio.Month)
                        < 4
                    )
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

                var idUsuarioClaim = User.FindFirstValue("IdUsuario");
                if (int.TryParse(idUsuarioClaim, out int idUsuario))
                {
                    contrato.IdUsuarioCreador = idUsuario;
                }
                else
                {
                    TempData["Error"] =
                        "No se pudo identificar al usuario creador. Sesión inválida.";
                    TempData["Contrato"] = JsonSerializer.Serialize(contrato);
                    return RedirectToAction("Ver", "Contrato", new { id });
                }
                var idContrato = repositorio.CrearContratoConPago(contrato, pago);
                if (idContrato <= 0)
                {
                    TempData["Error"] = "Error al intentar renovar el Contrato";
                    TempData["Contrato"] = JsonSerializer.Serialize(contrato);
                    return RedirectToAction("Ver", "Contrato", new { id });
                }

                TempData["Success"] = "Se renovo correctamente el Contrato";
                return RedirectToAction(
                    "Ver",
                    "Contrato",
                    new { id = idContrato, returnUrl = Url.Action("Ver", "Contrato", new { id }) }
                );
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error inesperado al renovar el contrato";
                return RedirectToAction("Ver", "Contrato", new { id });
            }
        }

        // POST: Contrato/Eliminar/5
        [HttpPost]
        [Authorize(Roles = "Administrador")]
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
                return RedirectToAction(
                    "Ver",
                    "Contrato",
                    new { IdContrato, returnUrl = Url.Action("Listar") }
                );
            }
            catch (Exception)
            {
                TempData["Error"] = "Error al intentar eliminar el contrato";
                return RedirectToAction(
                    "Ver",
                    "Contrato",
                    new { IdContrato, returnUrl = Url.Action("Listar") }
                );
            }
        }

        // GET: Contrato/Listar
        [HttpGet]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Listar(
            string? idContrato,
            string? dniInquilino,
            string? idInmueble,
            string? estado,
            string? Fecha_desde,
            string? Fecha_hasta,
            string? tipo,
            string? MontoMenor,
            string? MontoMayor,
            int PaginaActual = 1
        )
        {
            int registrosPorPagina = 9;
            int total = 0;
            int offset = (PaginaActual - 1) * registrosPorPagina;
            int limite = registrosPorPagina;
            List<Contrato> lista;
            try
            {
                total = repositorio.CantidadFiltro(
                    idContrato,
                    dniInquilino,
                    idInmueble,
                    estado,
                    Fecha_desde,
                    Fecha_hasta,
                    tipo,
                    MontoMenor,
                    MontoMayor
                );
                limite = Math.Min(registrosPorPagina, total - offset);
                lista = repositorio.Filtrar(
                    idContrato,
                    dniInquilino,
                    idInmueble,
                    estado,
                    Fecha_desde,
                    Fecha_hasta,
                    tipo,
                    MontoMenor,
                    MontoMayor,
                    offset,
                    limite
                );

                int totalPaginas = (int)Math.Ceiling((double)total / registrosPorPagina);

                ViewBag.PaginaActual = PaginaActual;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.IdContrato = idContrato;
                ViewBag.DniInquilino = dniInquilino;
                ViewBag.IdInmueble = idInmueble;
                ViewBag.Estado = estado;
                ViewBag.FechaDesde = Fecha_desde;
                ViewBag.FechaHasta = Fecha_hasta;
                ViewBag.Tipo = tipo;
                ViewBag.MontoMenor = MontoMenor;
                ViewBag.MontoMayor = MontoMayor;

                return View(lista);
            }
            catch (System.Exception)
            {
                return View(new List<Contrato>());
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Empleado")]
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
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Fechas(int idInmueble, string? idContrato)
        {
            var contratos = repositorio.FechasOcupadas(idInmueble, idContrato);
            var fechas = contratos.Select(c => new
            {
                inicio = c.FechaInicio.ToString("yyyy-MM-dd"),
                fin = c.FechaFinalizacion.ToString("yyyy-MM-dd"),
            });
            return Json(fechas);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult CancelarContrato(int idContrato, decimal Multa = 0)
        {
            try
            {
                Contrato contrato = repositorio.ObtenerPorID(idContrato);
                if (contrato.IdContrato == 0)
                {
                    TempData["Error"] = "Contrato no encontrado, al cancelar contrato";
                    return RedirectToAction("Ver", "Contrato", new { id = idContrato });
                }
                if (contrato.Estado != "Vigente")
                {
                    TempData["Error"] = "Contrato no cancelable";
                    return RedirectToAction("Ver", "Contrato", new { id = idContrato });
                }

                var idUsuarioClaim = User.FindFirstValue("IdUsuario");
                if (int.TryParse(idUsuarioClaim, out int idUsuario))
                {
                    contrato.IdUsuarioFinalizador = idUsuario;
                }
                else
                {
                    TempData["Error"] =
                        "No se pudo identificar al usuario para finalizar el contrato. Sesión inválida.";
                    return RedirectToAction("Ver", "Contrato", new { id = idContrato });
                }

                contrato.Multa = Multa;
                if (Multa == 0)
                {
                    Pago pago = new Pago
                    {
                        IdContrato = idContrato,
                        Monto = Multa,
                        numeroPago = repositorioPago.CantidadPago(idContrato) + 1,
                        Concepto = "Multa de Cancelacion",
                        FechaPago = DateTime.Today,
                    };
                    repositorio.CancelarContratoConPago(contrato, pago);
                    TempData["Success"] = "Contrato cancelado";
                    return RedirectToAction("Ver", "Contrato", new { id = idContrato });
                }
                var resultado = repositorio.Cancelado(contrato);
                if (resultado > 0)
                {
                    TempData["Success"] = "Contrato cancelado";
                }
                else
                {
                    TempData["Error"] = "No se pudo cancelar el contrato.";
                }
                return RedirectToAction("Ver", "Contrato", new { id = idContrato });
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error inesperado al cancelar el contrato.";
                return RedirectToAction("Ver", "Contrato", new { id = idContrato });
            }
        }

        [NonAction]
        public decimal CalcularMulta(Contrato c)
        {
            try
            {
                decimal multa = -1;
                switch (c.Tipo)
                {
                    case 1:
                        // Pierde todo
                        multa = 0;
                        //multa de 0, por que se le cobro por adelantado todo el contrato
                        break;

                    case 2: // mensual
                        DateTime mitad = c.FechaInicio.AddDays(
                            (c.FechaFinalizacion - c.FechaInicio).TotalDays / 2
                        );
                        //si fuera 5 meses, si cancela en el mes 1 o 2 paga multa de 2 meses, si cancela en el 3,4 o 5 paga multa de 1 mes
                        //si fuera 6 meses, si cancela en el mes 1,2 o 3 paga multa de 2 meses, si cancela en el 4,5 o 6 paga multa de 1 mes
                        if ((c.FechaCancelacion ?? DateTime.Today) <= mitad)
                        {
                            multa = c.Monto * 2;
                        }
                        else
                        {
                            multa = c.Monto;
                        }
                        return multa;
                }
                return multa;
            }
            catch
            {
                return -1;
            }
        }

        [HttpGet]
        // GET: Contrato/Calendario/{year}/{idInmueble}
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Calendario(int year, int idInmueble)
        {
            try
            {
                if (year <= 0 || idInmueble <= 0)
                {
                    return BadRequest(
                        "Error en calendario controller: Datos incorrectos Y: "
                            + year
                            + " Id: "
                            + idInmueble
                    );
                }
                var contratos = repositorio.Calendario(year, idInmueble);
                return Ok(contratos);
            }
            catch (Exception e)
            {
                return BadRequest("Error en calendario controller: " + e.Message);
            }
        }
    }
}
