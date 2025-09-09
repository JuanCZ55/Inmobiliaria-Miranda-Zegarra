using Inmobiliaria_.Models;

namespace Inmobiliaria.Models 
{
    public interface IRepositorioContrato : IRepositorio<Contrato>
    {
        int Finalizar(Contrato contrato);
        int Cancelado(Contrato contrato);

        public List<Contrato> Filtrar(string? idContrato, string? dniInquilino, string? idInmueble, string? estado, string? Fecha_desde, string? Fecha_hasta, int offset, int limite);

        public int ExisteSolapamiento(int idInmueble, DateTime fechaDesde, DateTime fechaHasta);

        public int validarContratoCancelar(int idContrato, DateTime? fechaCancelar);
        public int validarFechaMayorMulta(int idContrato, DateTime? fechaCancelar);
        public int CantidadFiltro(string? idContrato, string? dniInquilino, string? idInmueble, string? estado, string? Fecha_desde, string? Fecha_hasta);
    }
}
