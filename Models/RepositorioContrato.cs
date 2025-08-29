namespace Inmobiliaria.Models
{
  using System.Collections.Generic;
  using System.Data;
  using MySql.Data.MySqlClient;
  using Microsoft.Extensions.Configuration;

  public class RepositorioContraro : RepositorioBase, IRepositorioContrato
  {
    public RepositorioContraro(IConfiguration configuration) : base(configuration)
    {
    }

    public int Crear(Contrato contrato)
    {
      int res = -1;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"INSERT INTO contrato (id_inquilino, id_inmueble, fecha_desde, fecha_hasta, monto_mensual, estado) VALUES (@id_inquilino, @id_inmueble, @id_usuario_creador, @fecha_desde, @fecha_hasta, @monto_mensual, @estado); SELECT LAST_INSERT_ID();";

        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@id_inquilino", contrato.IdInquilino);
          cmd.Parameters.AddWithValue("@id_inmueble", contrato.IdInmueble);
          cmd.Parameters.AddWithValue("@fecha_desde", contrato.FechaDesde);
          cmd.Parameters.AddWithValue("@fecha_hasta", contrato.FechaHasta);
          cmd.Parameters.AddWithValue("@monto_mensual", contrato.MontoMensual);
          cmd.Parameters.AddWithValue("@estado", contrato.Estado);
          conn.Open();
          res = System.Convert.ToInt32(cmd.ExecuteScalar());
          contrato.IdContrato = res;
          conn.Close();
        }
      }
      return res;
    }

    public int Finalizar(Contrato contrato)
    {
      int res = -1;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"UPDATE contrato SET fecha_fin=@fecha_fin, estado=2, updated_at=NOW() WHERE id_contrato=@id_contrato";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@fecha_fin", DateTime.Now);
          cmd.Parameters.AddWithValue("@id_contrato", contrato.IdContrato);
          conn.Open();
          res = cmd.ExecuteNonQuery();
          conn.Close();

        }
      }
      return res;
    }

    public int Cancelado(Contrato contrato)
    {
      int res = -1;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"UPDATE contrato SET fecha_fin=@fecha_fin, multa=@multa, estado=3, updated_at=NOW() WHERE id_contrato=@id_contrato";
        using (var cmd = new MySqlCommand(sql, conn))
        {

          cmd.Parameters.AddWithValue("@fecha_fin", DateTime.Now);
          cmd.Parameters.AddWithValue("@multa", contrato.Multa);
          cmd.Parameters.AddWithValue("@id_contrato", contrato.IdContrato);
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
        var sql = @"DELETE FROM contrato WHERE id_contrato=@id";
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

    public Contrato ObtenerPorID(int id)
    {
      Contrato Contrato = null;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"
        SELECT 
        c.id_Contrato, 
        c.fecha_desde, 
        c.fecha_hasta, 
        c.fecha_fin, 
        c.monto_mensual, 
        c.multa, 
        c.estado, 
        c.id_inmueble, 
        c.id_inquilino, 
        c.created_at, 
        c.updated_at,

        inm.id_Propietario AS inm.id_Propietario, 
        inm.direccion AS inm.direccion, 
        inm.cantidad_ambientes AS inm.cantidad_ambientes, 
        inm.descripcion AS inm.descripcion,

        p.dni AS p.dni, 
        p.nombre AS p.nombre, 
        p.apellido AS p.apellido, 
        p.telefono AS p.telefono, 
        p.email AS p.email,

        inq.dni AS inq.dni, 
        inq.nombre AS inq.nombre, 
        inq.apellido AS inq.apellido, 
        inq.telefono AS inq.telefono, 
        inq.email AS inq.email,

        FROM contrato c JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
        JOIN propietario p ON inm.id_propietario = p.id_propietario
        JOIN inquilino inq ON c.id_inquilino = inq.id_inquilino
        WHERE c.id_contrato = @id;
        ";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@id", id);
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            if (reader.Read())
            {
              Contrato = new Contrato
              {
                IdContrato = reader.GetInt32("id_Contrato"),
                FechaDesde = reader.GetDateTime("fecha_desde"),
                FechaHasta = reader.GetDateTime("fecha_hasta"),
                FechaFin = reader.GetDateTime("fecha_fin"),
                MontoMensual = reader.GetDecimal("monto_mensual"),
                Multa = reader.GetDecimal("multa"),
                Estado = reader.GetInt32("estado"),
                IdInmueble = reader.GetInt32("id_inmueble"),
                Inmueble = new Inmueble
                {
                  IdInmueble = reader.GetInt32("id_inmueble"),
                  IdPropietario = reader.GetInt32("inm.id_Propietario"),
                  Propieteario = new Propietario
                  {
                    IdPropietario = reader.GetInt32("inm.id_propietario"),
                    Dni = reader.GetString("p.dni"),
                    Nombre = reader.GetString("p.nombre"),
                    Apellido = reader.GetString("p.apellido"),
                    Telefono = reader.GetString("p.in.telefono"),
                    Email = reader.GetString("p..email")
                  },
                  Direccion = reader.GetString("inm.direccion"),
                  CantidadAmbientes = reader.GetInt32("inm.cantidad_ambientes"),
                  Descripcion = reader.GetString("inm.descripcion")
                },
                IdInquilino = reader.GetInt32("id_inquilino"),
                Inquilino = new Inquilino
                {
                  IdInquilino = reader.GetInt32("id_inquilino"),
                  Dni = reader.GetString("inq.dni"),
                  Nombre = reader.GetString("inq.nombre"),
                  Apellido = reader.GetString("inq.apellido"),
                  Telefono = reader.GetString("inq.telefono"),
                  Email = reader.GetString("inq.email")
                },
                CreatedAt = reader.GetDateTime("created_at"),
                UpdatedAt = reader.GetDateTime("updated_at")
              };
            }
          }
          conn.Close();
        }
      }
      return Contrato;
    }

    public List<Contrato> BuscarPorInmueble(int id)
    {
      var lista = new List<Contrato>();
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"
        SELECT 
        c.id_Contrato, 
        c.fecha_desde, 
        c.fecha_hasta, 
        c.fecha_fin, 
        c.monto_mensual, 
        c.multa, 
        c.estado, 
        c.id_inmueble, 
        c.id_inquilino, 
        c.created_at, 
        c.updated_at,

        inm.id_Propietario AS inm.id_Propietario, 
        inm.direccion AS inm.direccion, 
        inm.cantidad_ambientes AS inm.cantidad_ambientes, 
        inm.descripcion AS inm.descripcion,

        p.dni AS p.dni, 
        p.nombre AS p.nombre, 
        p.apellido AS p.apellido, 
        p.telefono AS p.telefono, 
        p.email AS p.email,

        inq.dni AS inq.dni, 
        inq.nombre AS inq.nombre, 
        inq.apellido AS inq.apellido, 
        inq.telefono AS inq.telefono, 
        inq.email AS inq.email,

        FROM contrato c JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
        JOIN propietario p ON inm.id_propietario = p.id_propietario
        JOIN inquilino inq ON c.id_inquilino = inq.id_inquilino

        WHERE c.id_inmueble = @id";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@id", $"{id}");
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
              lista.Add(new Contrato
              {
                IdContrato = reader.GetInt32("id_Contrato"),
                FechaDesde = reader.GetDateTime("fecha_desde"),
                FechaHasta = reader.GetDateTime("fecha_hasta"),
                FechaFin = reader.GetDateTime("fecha_fin"),
                MontoMensual = reader.GetDecimal("monto_mensual"),
                Multa = reader.GetDecimal("multa"),
                Estado = reader.GetInt32("estado"),
                IdInmueble = reader.GetInt32("id_inmueble"),
                Inmueble = new Inmueble
                {
                  IdInmueble = reader.GetInt32("id_inmueble"),
                  IdPropietario = reader.GetInt32("inm.id_Propietario"),
                  Propieteario = new Propietario
                  {
                    IdPropietario = reader.GetInt32("inm.id_propietario"),
                    Dni = reader.GetString("p.dni"),
                    Nombre = reader.GetString("p.nombre"),
                    Apellido = reader.GetString("p.apellido"),
                    Telefono = reader.GetString("p.in.telefono"),
                    Email = reader.GetString("p..email")
                  },
                  Direccion = reader.GetString("inm.direccion"),
                  CantidadAmbientes = reader.GetInt32("inm.cantidad_ambientes"),
                  Descripcion = reader.GetString("inm.descripcion")
                },
                IdInquilino = reader.GetInt32("id_inquilino"),
                Inquilino = new Inquilino
                {
                  IdInquilino = reader.GetInt32("id_inquilino"),
                  Dni = reader.GetString("inq.dni"),
                  Nombre = reader.GetString("inq.nombre"),
                  Apellido = reader.GetString("inq.apellido"),
                  Telefono = reader.GetString("inq.telefono"),
                  Email = reader.GetString("inq.email")
                },
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

    public List<Contrato> BuscarPorInquilino(int id)
    {
      var lista = new List<Contrato>();
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"
        SELECT 
        c.id_Contrato, 
        c.fecha_desde, 
        c.fecha_hasta, 
        c.fecha_fin, 
        c.monto_mensual, 
        c.multa, 
        c.estado, 
        c.id_inmueble, 
        c.id_inquilino, 
        c.created_at, 
        c.updated_at,

        inm.id_Propietario AS inm.id_Propietario, 
        inm.direccion AS inm.direccion, 
        inm.cantidad_ambientes AS inm.cantidad_ambientes, 
        inm.descripcion AS inm.descripcion,

        p.dni AS p.dni, 
        p.nombre AS p.nombre, 
        p.apellido AS p.apellido, 
        p.telefono AS p.telefono, 
        p.email AS p.email,

        inq.dni AS inq.dni, 
        inq.nombre AS inq.nombre, 
        inq.apellido AS inq.apellido, 
        inq.telefono AS inq.telefono, 
        inq.email AS inq.email,

        FROM contrato c JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
        JOIN propietario p ON inm.id_propietario = p.id_propietario
        JOIN inquilino inq ON c.id_inquilino = inq.id_inquilino

        WHERE c.id_inquilino = @id";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@id", $"{id}");
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
              lista.Add(new Contrato
              {
                IdContrato = reader.GetInt32("id_Contrato"),
                FechaDesde = reader.GetDateTime("fecha_desde"),
                FechaHasta = reader.GetDateTime("fecha_hasta"),
                FechaFin = reader.GetDateTime("fecha_fin"),
                MontoMensual = reader.GetDecimal("monto_mensual"),
                Multa = reader.GetDecimal("multa"),
                Estado = reader.GetInt32("estado"),
                IdInmueble = reader.GetInt32("id_inmueble"),
                Inmueble = new Inmueble
                {
                  IdInmueble = reader.GetInt32("id_inmueble"),
                  IdPropietario = reader.GetInt32("inm.id_Propietario"),
                  Propieteario = new Propietario
                  {
                    IdPropietario = reader.GetInt32("inm.id_propietario"),
                    Dni = reader.GetString("p.dni"),
                    Nombre = reader.GetString("p.nombre"),
                    Apellido = reader.GetString("p.apellido"),
                    Telefono = reader.GetString("p.in.telefono"),
                    Email = reader.GetString("p..email")
                  },
                  Direccion = reader.GetString("inm.direccion"),
                  CantidadAmbientes = reader.GetInt32("inm.cantidad_ambientes"),
                  Descripcion = reader.GetString("inm.descripcion")
                },
                IdInquilino = reader.GetInt32("id_inquilino"),
                Inquilino = new Inquilino
                {
                  IdInquilino = reader.GetInt32("id_inquilino"),
                  Dni = reader.GetString("inq.dni"),
                  Nombre = reader.GetString("inq.nombre"),
                  Apellido = reader.GetString("inq.apellido"),
                  Telefono = reader.GetString("inq.telefono"),
                  Email = reader.GetString("inq.email")
                },
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

    public List<Contrato> ObtenerTodos()
    {
      var lista = new List<Contrato>();
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"
        SELECT 
        c.id_Contrato, 
        c.fecha_desde, 
        c.fecha_hasta, 
        c.fecha_fin, 
        c.monto_mensual, 
        c.multa, 
        c.estado, 
        c.id_inmueble, 
        c.id_inquilino, 
        c.created_at, 
        c.updated_at,

        inm.id_Propietario AS inm.id_Propietario, 
        inm.direccion AS inm.direccion, 
        inm.cantidad_ambientes AS inm.cantidad_ambientes, 
        inm.descripcion AS inm.descripcion,

        p.dni AS p.dni, 
        p.nombre AS p.nombre, 
        p.apellido AS p.apellido, 
        p.telefono AS p.telefono, 
        p.email AS p.email,

        inq.dni AS inq.dni, 
        inq.nombre AS inq.nombre, 
        inq.apellido AS inq.apellido, 
        inq.telefono AS inq.telefono, 
        inq.email AS inq.email,

        FROM contrato c JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
        JOIN propietario p ON inm.id_propietario = p.id_propietario
        JOIN inquilino inq ON c.id_inquilino = inq.id_inquilino"
        ;
        using (var cmd = new MySqlCommand(sql, conn))
        {
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
              lista.Add(new Contrato
              {
                IdContrato = reader.GetInt32("id_Contrato"),
                FechaDesde = reader.GetDateTime("fecha_desde"),
                FechaHasta = reader.GetDateTime("fecha_hasta"),
                FechaFin = reader.GetDateTime("fecha_fin"),
                MontoMensual = reader.GetDecimal("monto_mensual"),
                Multa = reader.GetDecimal("multa"),
                Estado = reader.GetInt32("estado"),
                IdInmueble = reader.GetInt32("id_inmueble"),
                Inmueble = new Inmueble
                {
                  IdInmueble = reader.GetInt32("id_inmueble"),
                  IdPropietario = reader.GetInt32("inm.id_Propietario"),
                  Propieteario = new Propietario
                  {
                    IdPropietario = reader.GetInt32("inm.id_propietario"),
                    Dni = reader.GetString("p.dni"),
                    Nombre = reader.GetString("p.nombre"),
                    Apellido = reader.GetString("p.apellido"),
                    Telefono = reader.GetString("p.in.telefono"),
                    Email = reader.GetString("p..email")
                  },
                  Direccion = reader.GetString("inm.direccion"),
                  CantidadAmbientes = reader.GetInt32("inm.cantidad_ambientes"),
                  Descripcion = reader.GetString("inm.descripcion")
                },
                IdInquilino = reader.GetInt32("id_inquilino"),
                Inquilino = new Inquilino
                {
                  IdInquilino = reader.GetInt32("id_inquilino"),
                  Dni = reader.GetString("inq.dni"),
                  Nombre = reader.GetString("inq.nombre"),
                  Apellido = reader.GetString("inq.apellido"),
                  Telefono = reader.GetString("inq.telefono"),
                  Email = reader.GetString("inq.email")
                },
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