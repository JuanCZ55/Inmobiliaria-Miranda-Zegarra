using System.Collections.Generic;

namespace Inmobiliaria.Models
{
    public interface IRepositorioPago
    {
        int Crear(Pago Pago);
        int Modificar(Pago Pago);
        int Eliminar(int IdPago);
        int EliminarPorContrato(int IdContrato);
        Pago ObtenerPorID(int IdPago);
        List<Pago> BuscarPorContrato(int IdContrato);
        List<Pago> ObtenerTodos();
    }
}
