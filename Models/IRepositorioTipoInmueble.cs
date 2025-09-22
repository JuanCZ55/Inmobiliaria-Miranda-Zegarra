using Inmobiliaria_.Models;

namespace Inmobiliaria.Models
{
    public interface IRepositorioTipoInmueble : IRepositorio<TipoInmueble>
    {
        int ExisteTipoInmueble(string nombre);
        int SeEstaUsando(int IdTipoInmueble);
        List<TipoInmueble> TenerTodos();
        List<TipoInmueble> Filtro(string? nombre, int limit, int offset);
        int ContarFiltro(string? nombre);
    }
}
