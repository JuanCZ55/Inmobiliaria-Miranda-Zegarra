using Inmobiliaria_.Models;

namespace Inmobiliaria.Models
{
  public interface IRepositorioInquilino : IRepositorio<Inquilino>
  {
    Inquilino ObtenerPorDni(string dni);
    int ContarFiltro(string dni);
    List<Inquilino> Filtro(string dni, int limit, int offset);
  }
}
