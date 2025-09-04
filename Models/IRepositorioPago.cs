using Inmobiliaria_.Models;

namespace Inmobiliaria.Models
{
    public interface IRepositorioPago : IRepositorio<Pago>
    {
        int EliminarPorContrato(int IdContrato);
        List<Pago> BuscarPorContrato(int IdContrato);
        List<Pago> ObtenerTodos();
        List<Pago> Filtrar(string? idPago, string? MontoMenor, string? MontoMayor, string? estado, string? Fecha_desde, string? Fecha_hasta, int offset, int limite);
        int CantidadFiltro(string? idPago, string? MontoMenor, string? MontoMayor, string? estado, string? Fecha_desde, string? Fecha_hasta);
    }
}
