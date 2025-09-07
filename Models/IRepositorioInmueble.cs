using Inmobiliaria_.Models;

namespace Inmobiliaria.Models
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        int SetEstado(int IdInmueble, int Estado);
        bool SeEstaUsando(int idInmueble);
        int ContarFiltro(
            string? direccion,
            string? dni,
            int? idTipoInmueble,
            int? uso,
            int? cantidadAmbientesMin,
            decimal? precioMin,
            decimal? precioMax,
            int? estado
        );
        List<Inmueble> Filtro(
            string? direccion,
            string? dni,
            int? idTipoInmueble,
            int? uso,
            int? cantidadAmbientes,
            decimal? precioMin,
            decimal? precioMax,
            int? estado,
            int? limit,
            int? offset
        );
    }
}
