using System.Collections.Generic;

namespace Inmobiliaria.Models
{
    public interface IRepositorioContrato
    {
        int Crear(Contrato contrato);
        int Finalizar(Contrato contrato);
        int Cancelado(Contrato contrato);
        int Eliminar(int IdContrato);
        Contrato ObtenerPorID(int IdContrato);
        List<Contrato> BuscarPorInmueble(int IdInmueble);
        List<Contrato> BuscarPorInquilino(int IdInquilino);
        List<Contrato> ObtenerTodos();
    }
}
