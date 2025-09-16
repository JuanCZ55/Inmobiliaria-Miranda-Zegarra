using Inmobiliaria_.Models;

namespace Inmobiliaria.Models
{
    public interface IRepositorioPropietario : IRepositorio<Propietario>
    {
        Propietario ObtenerPorDni(string dni);
        List<Propietario> ObtenerTodos();
        int ContarFiltro(string dni);
        List<Propietario> Filtro(string dni, int limit, int offset);
        List<Propietario> ListarPropietarios(string nombre, string dni);
    }
}
