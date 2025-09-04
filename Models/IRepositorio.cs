namespace Inmobiliaria_.Models
{
	public interface IRepositorio<T>
	{
		int Crear(T p);
		int Modificar(T p);
		int Eliminar(int id);
		T ObtenerPorID(int id);
	}
}
