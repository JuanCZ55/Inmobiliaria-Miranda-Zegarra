using Inmobiliaria_.Models;

namespace Inmobiliaria.Models
{
  public interface IRepositorioInquilino : IRepositorio<Inquilino>
  {
    int Alta(string dni);
    int Baja(string dni);
    Inquilino ObtenerPorDni(string dni);
    List<Inquilino> BuscarPorNombre(string nombre);
    List<Inquilino> ObtenerTodos();
    List<Inquilino> filtrarDNI(string dni);
    int ContarFiltro(string dni);
    List<Inquilino> Filtro(string dni, int limit, int offset);
  }
}
