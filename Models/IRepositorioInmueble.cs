using Inmobiliaria_.Models;

namespace Inmobiliaria.Models
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        bool SeEstaUsando(int idInmueble);
        int ContarFiltro(
            string? direccion,
            string? dni,
            int? idTipoInmueble,
            int? uso,
            int? cantidadAmbientesMin,
            decimal? precioMin,
            decimal? precioMax,
            int? estado,
            DateTime? fechaInicio,
            DateTime? fechaFin
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
            int? offset,
            DateTime? fechaInicio,
            DateTime? fechaFin
        );
        Task<int> CrearAsync(Inmueble inmueble);
        Task<int> ModificarAsync(Inmueble inmueble);

        List<Imagen> ObtenerImagenesPorInmueble(int idInmueble); // <-- Agregado
        Imagen? ObtenerImagenPorId(int idImagen);
        int EliminarImagen(int idImagen);
    }
}
