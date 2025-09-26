using Inmobiliaria_.Models;

namespace Inmobiliaria.Models 
{
    public interface IRepositorioContrato : IRepositorio<Contrato>
    {
        int Cancelado(Contrato contrato);

        public List<Contrato> Filtrar(string? idContrato, string? dniInquilino, string? idInmueble, string? estado, string? Fecha_desde, string? Fecha_hasta, string? tipo, string? MontoMenor, string? MontoMayor,int offset, int limite);

        public int validarContratoCancelar(int idContrato, DateTime? fechaCancelar);
        public int validarFechaMayorMulta(int idContrato, DateTime? fechaCancelar);
        public int CantidadFiltro(string? idContrato, string? dniInquilino, string? idInmueble, string? estado, string? Fecha_desde, string? Fecha_hasta, string? tipo, string? MontoMenor, string? MontoMayor);
        public int ValidarSolapamiento(Contrato contrato);
        public int CrearContratoConPago(Contrato contrato, Pago pago);
        public List<Contrato> FechasOcupadas(int idInmueble, string? idContrato);
        public int EliminarContratoConPagoS(int idContrato);
        int CancelarContratoConPago(Contrato contrato, Pago pago);
    }
}
