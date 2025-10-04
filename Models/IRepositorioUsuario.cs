namespace Inmobiliaria.Models
{
  public interface IRepositorioUsuario
  {
    int Registrar(Usuario usuario);
    int Modificar(Usuario usuario);
    Usuario? Autenticar(string email);
    Usuario ObtenerPorId(int idUsuario);
    Usuario ObtenerPorEmail(string email);
    int Eliminar(int idUsuario);
    int CantidadFiltro(string? idUsuario, string? nombre, string? apellido, string? email, string? rol, string? estado);
    List<Usuario> Filtrar(string? idUsuario, string? nombre, string? apellido, string? email, string? rol, string? estado, int? limit, int? offset);

  }
}
