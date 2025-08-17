using System.Collections.Generic;

namespace Inmobiliaria.Models
{
    public interface IRepositorioPropietario
    {
        int Crear(Propietario propietario);
        int Modificar(Propietario propietario);
        int Alta(string dni);
        int Baja(string dni);
        Propietario ObtenerPorDni(string dni);
        List<Propietario> BuscarPorNombre(string nombre);
        List<Propietario> ObtenerTodos();
    }
}
