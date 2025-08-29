using System.Collections.Generic;

namespace Inmobiliaria.Models
{
    public interface IRepositorioInmueble
    {
        int Crear(Inmueble Inmueble);
        int Modificar(Inmueble Inmueble);
        int Eliminar(int IdInmueble);
        List<Inmueble> ObtenerTodos();
        List<Inmueble> ObtenerPorPropietario(int IdPropietario);
        List<Inmueble> ObtenerPorTipo(int IdTipoInmueble);
        List<Inmueble> ObtenerPorUso(int Uso);
        List<Inmueble> ObtenerPorCantidadAmbientes(int CantidadDeAmbientes);
        int SetEstado(int IdInmueble, int Estado);
    }
}
