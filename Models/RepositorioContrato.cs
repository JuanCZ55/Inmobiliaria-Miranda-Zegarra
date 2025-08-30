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
        var sql = @"INSERT INTO contrato (id_inquilino, id_inmueble, fecha_desde, fecha_hasta, monto_mensual, estado) VALUES (@id_inquilino, @id_inmueble, @fecha_desde, @fecha_hasta, @monto_mensual, @estado); SELECT LAST_INSERT_ID();";

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
        c.fecha_terminacion_anticipada, 
        c.monto_mensual, 
        c.multa, 
        c.estado, 
        c.id_inmueble, 
        c.id_inquilino, 
        c.created_at, 
        c.updated_at,

        inm.id_Propietario AS inm_id_Propietario, 
        inm.direccion AS inm_direccion, 
        inm.cantidad_ambientes AS inm_cantidad_ambientes, 
        inm.descripcion AS inm_descripcion,

        p.dni AS p_dni, 
        p.nombre AS p_nombre, 
        p.apellido AS p_apellido, 
        p.telefono AS p_telefono, 
        p.email AS p_email,

        inq.dni AS inq_dni, 
        inq.nombre AS inq_nombre, 
        inq.apellido AS inq_apellido, 
        inq.telefono AS inq_telefono, 
        inq.email AS inq_email,

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
                FechaFin = reader.GetDateTime("fecha_terminacion_anticipada"),
                MontoMensual = reader.GetDecimal("monto_mensual"),
                Multa = reader.GetDecimal("multa"),
                Estado = reader.GetInt32("estado"),
                IdInmueble = reader.GetInt32("id_inmueble"),
                Inmueble = new Inmueble
                {
                  IdInmueble = reader.GetInt32("id_inmueble"),
                  IdPropietario = reader.GetInt32("inm_id_Propietario"),
                  Propietario = new Propietario
                  {
                    IdPropietario = reader.GetInt32("inm_id_propietario"),
                    Dni = reader.GetString("p_dni"),
                    Nombre = reader.GetString("p_nombre"),
                    Apellido = reader.GetString("p_apellido"),
                    Telefono = reader.GetString("p_telefono"),
                    Email = reader.GetString("p_email")
                  },
                  Direccion = reader.GetString("inm_direccion"),
                  CantidadAmbientes = reader.GetInt32("inm_cantidad_ambientes"),
                  Descripcion = reader.GetString("inm_descripcion")
                },
                IdInquilino = reader.GetInt32("id_inquilino"),
                Inquilino = new Inquilino
                {
                  IdInquilino = reader.GetInt32("id_inquilino"),
                  Dni = reader.GetString("inq_dni"),
                  Nombre = reader.GetString("inq_nombre"),
                  Apellido = reader.GetString("inq_apellido"),
                  Telefono = reader.GetString("inq_telefono"),
                  Email = reader.GetString("inq_email")
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
            c.fecha_terminacion_anticipada, 
            c.monto_mensual, 
            c.multa, 
            c.estado, 
            c.id_inmueble, 
            c.id_inquilino, 
            c.created_at, 
            c.updated_at,

            inm.id_Propietario AS inm_id_propietario, 
            inm.direccion AS inm_direccion, 
            inm.cantidad_ambientes AS inm_cantidad_ambientes, 
            inm.descripcion AS inm_descripcion,

            p.id_propietario AS p_id_propietario,
            p.dni AS p_dni, 
            p.nombre AS p_nombre, 
            p.apellido AS p_apellido, 
            p.telefono AS p_telefono, 
            p.email AS p_email,

            inq.id_inquilino AS inq_id_inquilino,
            inq.dni AS inq_dni, 
            inq.nombre AS inq_nombre, 
            inq.apellido AS inq_apellido, 
            inq.telefono AS inq_telefono, 
            inq.email AS inq_email

        FROM contrato c 
        JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
        JOIN propietario p ON inm.id_propietario = p.id_propietario
        JOIN inquilino inq ON c.id_inquilino = inq.id_inquilino;

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
                FechaFin = reader.GetDateTime("fecha_terminacion_anticipada"),
                MontoMensual = reader.GetDecimal("monto_mensual"),
                Multa = reader.GetDecimal("multa"),
                Estado = reader.GetInt32("estado"),
                IdInmueble = reader.GetInt32("id_inmueble"),
                Inmueble = new Inmueble
                {
                  IdInmueble = reader.GetInt32("id_inmueble"),
                  IdPropietario = reader.GetInt32("inm_id_propietario"),
                  Propietario = new Propietario
                  {
                    IdPropietario = reader.GetInt32("p_id_propietario"),
                    Dni = reader.GetString("p_dni"),
                    Nombre = reader.GetString("p_nombre"),
                    Apellido = reader.GetString("p_apellido"),
                    Telefono = reader.GetString("p_telefono"),
                    Email = reader.GetString("p_email")
                  },
                  Direccion = reader.GetString("inm_direccion"),
                  CantidadAmbientes = reader.GetInt32("inm_cantidad_ambientes"),
                  Descripcion = reader.GetString("inm_descripcion")
                },
                IdInquilino = reader.GetInt32("id_inquilino"),
                Inquilino = new Inquilino
                {
                  IdInquilino = reader.GetInt32("inq_id_inquilino"),
                  Dni = reader.GetString("inq_dni"),
                  Nombre = reader.GetString("inq_nombre"),
                  Apellido = reader.GetString("inq_apellido"),
                  Telefono = reader.GetString("inq_telefono"),
                  Email = reader.GetString("inq_email")
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
            c.fecha_terminacion_anticipada, 
            c.monto_mensual, 
            c.multa, 
            c.estado, 
            c.id_inmueble, 
            c.id_inquilino, 
            c.created_at, 
            c.updated_at,

            inm.id_Propietario AS inm_id_propietario, 
            inm.direccion AS inm_direccion, 
            inm.cantidad_ambientes AS inm_cantidad_ambientes, 
            inm.descripcion AS inm_descripcion,

            p.id_propietario AS p_id_propietario,
            p.dni AS p_dni, 
            p.nombre AS p_nombre, 
            p.apellido AS p_apellido, 
            p.telefono AS p_telefono, 
            p.email AS p_email,

            inq.id_inquilino AS inq_id_inquilino,
            inq.dni AS inq_dni, 
            inq.nombre AS inq_nombre, 
            inq.apellido AS inq_apellido, 
            inq.telefono AS inq_telefono, 
            inq.email AS inq_email

        FROM contrato c 
        JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
        JOIN propietario p ON inm.id_propietario = p.id_propietario
        JOIN inquilino inq ON c.id_inquilino = inq.id_inquilino;
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
                FechaFin = reader.GetDateTime("fecha_terminacion_anticipada"),
                MontoMensual = reader.GetDecimal("monto_mensual"),
                Multa = reader.GetDecimal("multa"),
                Estado = reader.GetInt32("estado"),
                IdInmueble = reader.GetInt32("id_inmueble"),
                Inmueble = new Inmueble
                {
                  IdInmueble = reader.GetInt32("id_inmueble"),
                  IdPropietario = reader.GetInt32("inm_id_propietario"),
                  Propietario = new Propietario
                  {
                    IdPropietario = reader.GetInt32("p_id_propietario"),
                    Dni = reader.GetString("p_dni"),
                    Nombre = reader.GetString("p_nombre"),
                    Apellido = reader.GetString("p_apellido"),
                    Telefono = reader.GetString("p_telefono"),
                    Email = reader.GetString("p_email")
                  },
                  Direccion = reader.GetString("inm_direccion"),
                  CantidadAmbientes = reader.GetInt32("inm_cantidad_ambientes"),
                  Descripcion = reader.GetString("inm_descripcion")
                },
                IdInquilino = reader.GetInt32("id_inquilino"),
                Inquilino = new Inquilino
                {
                  IdInquilino = reader.GetInt32("inq_id_inquilino"),
                  Dni = reader.GetString("inq_dni"),
                  Nombre = reader.GetString("inq_nombre"),
                  Apellido = reader.GetString("inq_apellido"),
                  Telefono = reader.GetString("inq_telefono"),
                  Email = reader.GetString("inq_email")
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
            c.fecha_terminacion_anticipada, 
            c.monto_mensual, 
            c.multa, 
            c.estado, 
            c.id_inmueble, 
            c.id_inquilino, 
            c.created_at, 
            c.updated_at,

            inm.id_Propietario AS inm_id_propietario, 
            inm.direccion AS inm_direccion, 
            inm.cantidad_ambientes AS inm_cantidad_ambientes, 
            inm.descripcion AS inm_descripcion,

            p.id_propietario AS p_id_propietario,
            p.dni AS p_dni, 
            p.nombre AS p_nombre, 
            p.apellido AS p_apellido, 
            p.telefono AS p_telefono, 
            p.email AS p_email,

            inq.id_inquilino AS inq_id_inquilino,
            inq.dni AS inq_dni, 
            inq.nombre AS inq_nombre, 
            inq.apellido AS inq_apellido, 
            inq.telefono AS inq_telefono, 
            inq.email AS inq_email

        FROM contrato c 
        JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
        JOIN propietario p ON inm.id_propietario = p.id_propietario
        JOIN inquilino inq ON c.id_inquilino = inq.id_inquilino
        ";
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
                FechaFin = reader.IsDBNull(reader.GetOrdinal("fecha_terminacion_anticipada")) 
                                  ? (DateTime?)null 
                                  : reader.GetDateTime("fecha_terminacion_anticipada"),
                MontoMensual = reader.GetDecimal("monto_mensual"),
                Multa = reader.IsDBNull(reader.GetOrdinal("multa")) 
                                  ? (decimal?)null 
                                  : reader.GetDecimal(reader.GetOrdinal("multa")),
                Estado = reader.GetInt32("estado"),
                IdInmueble = reader.GetInt32("id_inmueble"),
                Inmueble = new Inmueble
                {
                  IdInmueble = reader.GetInt32("id_inmueble"),
                  IdPropietario = reader.GetInt32("inm_id_propietario"),
                  Propietario = new Propietario
                  {
                    IdPropietario = reader.GetInt32("p_id_propietario"),
                    Dni = reader.GetString("p_dni"),
                    Nombre = reader.GetString("p_nombre"),
                    Apellido = reader.GetString("p_apellido"),
                    Telefono = reader.GetString("p_telefono"),
                    Email = reader.GetString("p_email")
                  },
                  Direccion = reader.GetString("inm_direccion"),
                  CantidadAmbientes = reader.GetInt32("inm_cantidad_ambientes"),
                  Descripcion = reader.GetString("inm_descripcion")
                },
                IdInquilino = reader.GetInt32("id_inquilino"),
                Inquilino = new Inquilino
                {
                  IdInquilino = reader.GetInt32("inq_id_inquilino"),
                  Dni = reader.GetString("inq_dni"),
                  Nombre = reader.GetString("inq_nombre"),
                  Apellido = reader.GetString("inq_apellido"),
                  Telefono = reader.GetString("inq_telefono"),
                  Email = reader.GetString("inq_email")
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


public List<Contrato> Filtrar(string? idContrato,string? dniInquilino, string? idInmueble, string? estado, string? Fecha_desde, string? Fecha_hasta)
{
    var lista = new List<Contrato>();
    using (var conn = new MySqlConnection(connectionString))
    {
        var sql = @"
            SELECT c.id_Contrato, c.fecha_desde, c.fecha_hasta, c.fecha_terminacion_anticipada, 
                  c.monto_mensual, c.multa, c.estado, c.id_inmueble, c.id_inquilino, 
                  c.created_at, c.updated_at,
                  inm.id_Propietario AS inm_id_propietario, inm.direccion AS inm_direccion, 
                  inm.cantidad_ambientes AS inm_cantidad_ambientes, inm.descripcion AS inm_descripcion,
                  p.id_propietario AS p_id_propietario, p.dni AS p_dni, p.nombre AS p_nombre, 
                  p.apellido AS p_apellido, p.telefono AS p_telefono, p.email AS p_email,
                  inq.id_inquilino AS inq_id_inquilino, inq.dni AS inq_dni, inq.nombre AS inq_nombre, 
                  inq.apellido AS inq_apellido, inq.telefono AS inq_telefono, inq.email AS inq_email
            FROM contrato c 
            JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
            JOIN propietario p ON inm.id_propietario = p.id_propietario
            JOIN inquilino inq ON c.id_inquilino = inq.id_inquilino
            WHERE 1=1
        ";

        // Filtros dinámicos con LIKE
        if (!string.IsNullOrEmpty(idContrato))
            sql += " AND c.id_contrato LIKE @idContrato";

        if (!string.IsNullOrEmpty(dniInquilino))
            sql += " AND inq.dni LIKE @dniInquilino";

        if (!string.IsNullOrEmpty(idInmueble))
            sql += " AND inm.id_inmueble LIKE @idInmueble";

        if (!string.IsNullOrEmpty(estado))
            sql += " AND c.estado = @estado";

        if (!string.IsNullOrEmpty(Fecha_desde))
            sql += " AND DATE(c.fecha_desde) >= @fechaDesde";

        if (!string.IsNullOrEmpty(Fecha_hasta))
            sql += " AND c.fecha_hasta <= @fechaHasta";

        using (var cmd = new MySqlCommand(sql, conn))
        {
            if (!string.IsNullOrEmpty(idContrato))
                cmd.Parameters.AddWithValue("@idContrato", "%" + idContrato + "%");

            if (!string.IsNullOrEmpty(dniInquilino))
                cmd.Parameters.AddWithValue("@dniInquilino", "%" + dniInquilino + "%");

            if (!string.IsNullOrEmpty(idInmueble))
                cmd.Parameters.AddWithValue("@idInmueble", "%" + idInmueble + "%");

            if (!string.IsNullOrEmpty(estado))
                cmd.Parameters.AddWithValue("@estado", estado);

            if (!string.IsNullOrEmpty(Fecha_desde))
                cmd.Parameters.AddWithValue("@fechaDesde", Fecha_desde);

            if (!string.IsNullOrEmpty(Fecha_hasta))
                cmd.Parameters.AddWithValue("@fechaHasta", Fecha_hasta);


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
                        FechaFin = reader.IsDBNull("fecha_terminacion_anticipada")
                                          ? (DateTime?)null 
                                          : reader.GetDateTime("fecha_terminacion_anticipada"),
                        MontoMensual = reader.GetDecimal("monto_mensual"),
                        Multa = reader.IsDBNull(reader.GetOrdinal("multa")) 
                                          ? (decimal?)null 
                                          : reader.GetDecimal(reader.GetOrdinal("multa")),
                        Estado = reader.GetInt32("estado"),
                        IdInmueble = reader.GetInt32("id_inmueble"),
                        Inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32("id_inmueble"),
                            IdPropietario = reader.GetInt32("inm_id_propietario"),
                            Propietario = new Propietario
                            {
                                IdPropietario = reader.GetInt32("p_id_propietario"),
                                Dni = reader.GetString("p_dni"),
                                Nombre = reader.GetString("p_nombre"),
                                Apellido = reader.GetString("p_apellido"),
                                Telefono = reader.GetString("p_telefono"),
                                Email = reader.GetString("p_email")
                            },
                            Direccion = reader.GetString("inm_direccion"),
                            CantidadAmbientes = reader.GetInt32("inm_cantidad_ambientes"),
                            Descripcion = reader.GetString("inm_descripcion")
                        },
                        IdInquilino = reader.GetInt32("id_inquilino"),
                        Inquilino = new Inquilino
                        {
                            IdInquilino = reader.GetInt32("inq_id_inquilino"),
                            Dni = reader.GetString("inq_dni"),
                            Nombre = reader.GetString("inq_nombre"),
                            Apellido = reader.GetString("inq_apellido"),
                            Telefono = reader.GetString("inq_telefono"),
                            Email = reader.GetString("inq_email")
                        },
                        CreatedAt = reader.GetDateTime("created_at"),
                        UpdatedAt = reader.GetDateTime("updated_at")
                    });
                }
            }
        }
    }
    return lista;
}

  }
}