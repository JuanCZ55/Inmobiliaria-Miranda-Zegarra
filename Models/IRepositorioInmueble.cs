using Inmobiliaria_.Models;

namespace Inmobiliaria.Models
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        List<Inmueble> ObtenerTodos();
        List<Inmueble> ObtenerPorPropietario(int IdPropietario);
        List<Inmueble> ObtenerPorTipo(int IdTipoInmueble);
        List<Inmueble> ObtenerPorUso(int Uso);
        List<Inmueble> ObtenerPorCantidadAmbientes(int CantidadDeAmbientes);
        int SeEstaUsando(int IdInmueble);
        int SetEstado(int IdInmueble, int Estado);
        List<Inmueble> SupaFiltro(string? direccion, string? dni, int? idTipoInmueble, int? uso, int? cantidadAmbientesMin, decimal? precioMin, decimal? precioMax, int? estado);
    }
}
