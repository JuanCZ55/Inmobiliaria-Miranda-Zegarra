namespace Inmobiliaria.Models
{
  using System.Collections.Generic;
  using System.Data;
  using MySql.Data.MySqlClient;
  using Microsoft.Extensions.Configuration;

  public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
  {
    public RepositorioInquilino(IConfiguration configuration) : base(configuration)
    {
    }

    public int Crear(Inquilino inquilino)
    {
      int res = -1;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"INSERT INTO inquilino (dni, nombre, apellido, telefono, email, direccion, estado) VALUES (@dni, @nombre, @apellido, @telefono, @email, @direccion, @estado); SELECT LAST_INSERT_ID();";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@dni", inquilino.Dni);
          cmd.Parameters.AddWithValue("@nombre", inquilino.Nombre);
          cmd.Parameters.AddWithValue("@apellido", inquilino.Apellido);
          cmd.Parameters.AddWithValue("@telefono", inquilino.Telefono);
          cmd.Parameters.AddWithValue("@email", inquilino.Email);
          cmd.Parameters.AddWithValue("@direccion", inquilino.Direccion);
          cmd.Parameters.AddWithValue("@estado", inquilino.Estado);
          conn.Open();
          res = System.Convert.ToInt32(cmd.ExecuteScalar());
          inquilino.IdInquilino = res;
          conn.Close();

        }
      }
      return res;
    }

    public int Modificar(Inquilino inquilino)
    {
      int res = -1;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"UPDATE inquilino SET dni=@dni, nombre=@nombre, apellido=@apellido, telefono=@telefono, email=@email, direccion=@direccion, estado=@estado, updated_at=NOW() WHERE id_inquilino=@id";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@dni", inquilino.Dni);
          cmd.Parameters.AddWithValue("@nombre", inquilino.Nombre);
          cmd.Parameters.AddWithValue("@apellido", inquilino.Apellido);
          cmd.Parameters.AddWithValue("@telefono", inquilino.Telefono);
          cmd.Parameters.AddWithValue("@email", inquilino.Email);
          cmd.Parameters.AddWithValue("@direccion", inquilino.Direccion);
          cmd.Parameters.AddWithValue("@estado", inquilino.Estado);
          cmd.Parameters.AddWithValue("@id", inquilino.IdInquilino);
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
        var sql = @"UPDATE inquilino SET estado=1, updated_at=NOW() WHERE dni=@dni";
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
        var sql = @"UPDATE inquilino SET estado=0, updated_at=NOW() WHERE dni=@dni";
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

    public int Eliminar(int id)
    {
      int res = -1;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"DELETE FROM inquilino WHERE id_inquilino=@id";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@id", id);
          conn.Open();
          res = cmd.ExecuteNonQuery();
          conn.Close();
        }
      }
      return res;
    }
    public Inquilino ObtenerPorID(int id)
    {
      Inquilino inquilino = null;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"SELECT id_inquilino, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at FROM inquilino WHERE id_inquilino = @id";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@id", id);
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            if (reader.Read())
            {
              inquilino = new Inquilino
              {
                IdInquilino = reader.GetInt32("id_inquilino"),
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
      return inquilino;
    }
    public Inquilino ObtenerPorDni(string dni)
    {
      Inquilino? inquilino = null;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"SELECT id_inquilino, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at FROM inquilino WHERE dni = @dni";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@dni", dni);
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            if (reader.Read())
            {
              inquilino = new Inquilino
              {
                IdInquilino = reader.GetInt32("id_inquilino"),
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
      return inquilino;
    }

    public List<Inquilino> BuscarPorNombre(string nombre)
    {
      var lista = new List<Inquilino>();
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"SELECT id_inquilino, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at FROM inquilino WHERE nombre LIKE @nombre";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@nombre", $"%{nombre}%");
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
              lista.Add(new Inquilino
              {
                IdInquilino = reader.GetInt32("id_inquilino"),
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

    public List<Inquilino> filtrarDNI(string dni)
    {
      List<Inquilino> lista = new List<Inquilino>();
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"SELECT id_inquilino, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at FROM inquilino WHERE dni LIKE CONCAT(@dni, '%')";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@dni", dni);
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
              Inquilino inquilino = new Inquilino
              {
                IdInquilino = reader.GetInt32("id_inquilino"),
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
              lista.Add(inquilino);
            }
          }
          conn.Close();
        }
      }
      return lista;
    }
    public List<Inquilino> ObtenerTodos()
    {
      var lista = new List<Inquilino>();
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"SELECT id_inquilino, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at FROM inquilino ORDER BY id_inquilino DESC;";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
              lista.Add(new Inquilino
              {
                IdInquilino = reader.GetInt32("id_inquilino"),
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
