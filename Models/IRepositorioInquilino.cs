using System.Collections.Generic;

namespace Inmobiliaria.Models
{
  public interface IRepositorioInquilino
  {
    int Crear(Inquilino inquilino);
    int Modificar(Inquilino inquilino);
    int Alta(string dni);
    int Baja(string dni);
    Inquilino ObtenerPorDni(string dni);
    List<Inquilino> BuscarPorNombre(string nombre);
    List<Inquilino> ObtenerTodos();
  }
}
