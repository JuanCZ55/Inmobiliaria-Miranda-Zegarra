using Inmobiliaria_.Models;

namespace Inmobiliaria.Models
{
    public interface IRepositorioTipoInmueble : IRepositorio<TipoInmueble>
    {
        int ExisteTipoInmueble(string nombre);
        int SeEstaUsando(int IdTipoInmueble);
        int ContarPorTodos();
        List<TipoInmueble> ObtenerTodos(int limit, int offset);
        List<TipoInmueble> TenerTodos();
        List<TipoInmueble> ListarPorNombre(string nombre, int limite, int offset);
        int ContarPorNombre(string nombre);
    }
}
