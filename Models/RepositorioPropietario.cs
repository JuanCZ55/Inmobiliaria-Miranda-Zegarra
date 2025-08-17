using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Models
{
  public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
  {
    public RepositorioPropietario(IConfiguration configuration) : base(configuration) { }

    public int Crear(Propietario propietario)
    {
      int res = -1;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"INSERT INTO propietario (dni, nombre, apellido, telefono, email, direccion, estado) VALUES (@dni, @nombre, @apellido, @telefono, @email, @direccion, @estado); SELECT LAST_INSERT_ID();";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@dni", propietario.Dni);
          cmd.Parameters.AddWithValue("@nombre", propietario.Nombre);
          cmd.Parameters.AddWithValue("@apellido", propietario.Apellido);
          cmd.Parameters.AddWithValue("@telefono", propietario.Telefono);
          cmd.Parameters.AddWithValue("@email", propietario.Email);
          cmd.Parameters.AddWithValue("@direccion", propietario.Direccion);
          cmd.Parameters.AddWithValue("@estado", propietario.Estado);
          conn.Open();
          res = Convert.ToInt32(cmd.ExecuteScalar());
          propietario.IdPropietario = res;
          conn.Close();
        }
      }
      return res;
    }

    public int Modificar(Propietario propietario)
    {
      int res = -1;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"UPDATE propietario SET dni=@dni, nombre=@nombre, apellido=@apellido, telefono=@telefono, email=@email, direccion=@direccion, estado=@estado, updated_at=NOW() WHERE id_propietario=@id";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@dni", propietario.Dni);
          cmd.Parameters.AddWithValue("@nombre", propietario.Nombre);
          cmd.Parameters.AddWithValue("@apellido", propietario.Apellido);
          cmd.Parameters.AddWithValue("@telefono", propietario.Telefono);
          cmd.Parameters.AddWithValue("@email", propietario.Email);
          cmd.Parameters.AddWithValue("@direccion", propietario.Direccion);
          cmd.Parameters.AddWithValue("@estado", propietario.Estado);
          cmd.Parameters.AddWithValue("@id", propietario.IdPropietario);
          conn.Open();
          res = cmd.ExecuteNonQuery();
          conn.Close();

        }
      }
      return res;
    }

    public int Alta(string dni)
    {
      int res = -1;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"UPDATE propietario SET estado=1, updated_at=NOW() WHERE dni=@dni";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@dni", dni);
          conn.Open();
          res = cmd.ExecuteNonQuery();
          conn.Close();

        }
      }
      return res;
    }

    public int Baja(string dni)
    {
      int res = -1;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"UPDATE propietario SET estado=0, updated_at=NOW() WHERE dni=@dni";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@dni", dni);
          conn.Open();
          res = cmd.ExecuteNonQuery();
          conn.Close();

        }
      }
      return res;
    }

    public Propietario ObtenerPorDni(string dni)
    {
      Propietario? propietario = null;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"SELECT id_propietario, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at FROM propietario WHERE dni = @dni";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@dni", dni);
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            if (reader.Read())
            {
              propietario = new Propietario
              {
                IdPropietario = reader.GetInt32("id_propietario"),
                Dni = reader.GetString("dni"),
                Nombre = reader.GetString("nombre"),
                Apellido = reader.GetString("apellido"),
                Telefono = reader.GetString("telefono"),
                Email = reader.GetString("email"),
                Direccion = reader.GetString("direccion"),
                Estado = reader.GetInt32("estado"),
                CreatedAt = reader.GetDateTime("created_at"),
                UpdatedAt = reader.GetDateTime("updated_at")
              };
            }
          }
          conn.Close();

        }
      }
      return propietario;
    }

    public List<Propietario> BuscarPorNombre(string nombre)
    {
      var lista = new List<Propietario>();
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"SELECT id_propietario, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at FROM propietario WHERE nombre LIKE @nombre";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@nombre", $"%{nombre}%");
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
              lista.Add(new Propietario
              {
                IdPropietario = reader.GetInt32("id_propietario"),
                Dni = reader.GetString("dni"),
                Nombre = reader.GetString("nombre"),
                Apellido = reader.GetString("apellido"),
                Telefono = reader.GetString("telefono"),
                Email = reader.GetString("email"),
                Direccion = reader.GetString("direccion"),
                Estado = reader.GetInt32("estado"),
                CreatedAt = reader.GetDateTime("created_at"),
                UpdatedAt = reader.GetDateTime("updated_at")
              });
            }
          }
          conn.Close();

        }
      }
      return lista;
    }

    public List<Propietario> ObtenerTodos()
    {
      var lista = new List<Propietario>();
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"SELECT id_propietario, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at FROM propietario";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
              lista.Add(new Propietario
              {
                IdPropietario = reader.GetInt32("id_propietario"),
                Dni = reader.GetString("dni"),
                Nombre = reader.GetString("nombre"),
                Apellido = reader.GetString("apellido"),
                Telefono = reader.GetString("telefono"),
                Email = reader.GetString("email"),
                Direccion = reader.GetString("direccion"),
                Estado = reader.GetInt32("estado"),
                CreatedAt = reader.GetDateTime("created_at"),
                UpdatedAt = reader.GetDateTime("updated_at")
              });
            }
          }
          conn.Close();

        }
      }
      return lista;
    }
  }
}
