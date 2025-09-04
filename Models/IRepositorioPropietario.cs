using Inmobiliaria_.Models;

namespace Inmobiliaria.Models
{
    public interface IRepositorioPropietario : IRepositorio<Propietario>
    {
        int Alta(string dni);
        int Baja(string dni);
        Propietario ObtenerPorDni(string dni);
        List<Propietario> BuscarPorNombre(string nombre);
        List<Propietario> ObtenerTodos();
        List<Propietario> filtrarDNI(string dni);
        int ContarTodos();
        List<Propietario> ObtenerTodosPaginado(int offset, int limite);
        int ContarPorNombre(string nombre);
        List<Propietario> ObtenerPaginado(string nombre, int offset, int limite);
    }
}
