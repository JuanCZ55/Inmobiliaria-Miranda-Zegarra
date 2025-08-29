using System.Collections.Generic;

namespace Inmobiliaria.Models
{
    public interface IRepositorioTipoInmueble
    {
        int Crear(TipoInmueble tipoInmueble);
        int Modificar(TipoInmueble tipoInmueble);
        int Eliminar(int idInmueble);
        int ExisteTipoInmueble(string nombre);
        List<TipoInmueble> ObtenerTodos();
    }
}
