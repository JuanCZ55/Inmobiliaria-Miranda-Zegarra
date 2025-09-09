namespace Inmobiliaria.Models
{
  using System.Collections.Generic;
  using System.Data;
  using MySql.Data.MySqlClient;
  using Microsoft.Extensions.Configuration;
  using System.ComponentModel;

  public class RepositorioPago : RepositorioBase, IRepositorioPago
  {
    public RepositorioPago(IConfiguration configuration) : base(configuration)
    {
    }

    public int Crear(Pago pago)
    {
      int res = -1;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"
            INSERT INTO pago (id_contrato, numero_pago, fecha_pago, concepto, monto, estado) 
            VALUES (@id_contrato, @numero_pago, @fecha_pago, @concepto, @monto, @estado);
            SELECT LAST_INSERT_ID();";

        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@id_contrato", pago.IdContrato);
          cmd.Parameters.AddWithValue("@numero_pago", pago.numeroPago);
          cmd.Parameters.AddWithValue("@fecha_pago", pago.FechaPago);
          cmd.Parameters.AddWithValue("@concepto", pago.Concepto);
          cmd.Parameters.AddWithValue("@monto", pago.Monto);
          cmd.Parameters.AddWithValue("@estado", pago.Estado);

          conn.Open();
          res = Convert.ToInt32(cmd.ExecuteScalar());
          pago.IdPago = res;
        }
      }
      return res;
    }

    public int Modificar(Pago pago)
    {
      int res = -1;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"
        UPDATE pago SET concepto=@concepto, estado=@estado, updated_at=NOW()
        WHERE id_pago=@id";

        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@concepto", pago.Concepto);
          cmd.Parameters.AddWithValue("@estado", pago.Estado);
          cmd.Parameters.AddWithValue("@estado", pago.IdPago);
          conn.Open();
          res = System.Convert.ToInt32(cmd.ExecuteScalar());
          pago.IdContrato = res;
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
        var sql = @"DELETE FROM pago WHERE id_pago=@id";
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
    public int EliminarPorContrato(int id)
    {
      int res = -1;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"DELETE FROM pago WHERE id_contrato=@id";
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

    public Pago ObtenerPorID(int id)
    {
      Pago pago = new Pago();
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"
        SELECT id_Pago, id_Contrato, numero_pago, fecha_pago, concepto, monto, estado, created_at, updated_at
        FROM pago WHERE id_Pago = @id;
        ";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@id", id);
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            if (reader.Read())
            {
              pago = new Pago
              {
                IdPago = reader.GetInt32("id_pago"),
                IdContrato = reader.GetInt32("id_Contrato"),
                numeroPago = reader.GetInt32("numero_pago"),
                FechaPago = reader.GetDateTime("fecha_pago"),
                Concepto = reader.GetString("concepto"),
                Monto = reader.GetDecimal("monto"),
                Estado = reader.GetInt32("estado"),
                CreatedAt = reader.GetDateTime("created_at"),
                UpdatedAt = reader.GetDateTime("updated_at")
              };
            }
          }
          conn.Close();
        }
      }
      return pago;
    }

    public List<Pago> BuscarPorContrato(int id)
    {
      var lista = new List<Pago>();
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"
        SELECT id_Pago, numero_pago, fecha_pago, concepto, monto, estado, id_Contrato 
        FROM pago WHERE id_contrato = @id;";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@id", id);
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
              lista.Add(new Pago
              {
                IdPago = reader.GetInt32("id_Pago"),
                numeroPago = reader.GetInt32("numero_pago"),
                FechaPago = reader.GetDateTime("fecha_pago"),
                Concepto = reader.GetString("concepto"),
                Monto = reader.GetDecimal("monto"),
                Estado = reader.GetInt32("estado"),
                IdContrato = reader.GetInt32("id_Contrato")
              });
            }
            conn.Close();
          }
        }
        return lista;
      }
    }

    public List<Pago> ObtenerTodos()
    {
      var lista = new List<Pago>();
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"
        SELECT id_Pago, numero_pago, fecha_pago, concepto, monto, estado, id_Contrato 
        FROM pago ORDER BY fecha_pago DESC;";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
              lista.Add(new Pago
              {
                IdPago = reader.GetInt32("id_Pago"),
                numeroPago = reader.GetInt32("numero_pago"),
                FechaPago = reader.GetDateTime("fecha_pago"),
                Concepto = reader.GetString("concepto"),
                Monto = reader.GetDecimal("monto"),
                Estado = reader.GetInt32("estado"),
                IdContrato = reader.GetInt32("id_Contrato")
              });
            }
            conn.Close();
          }
        }
        return lista;
      }
    }

    public List<Pago> Filtrar(string? idPago, string? idContrato, string? dniInquilino, string? MontoMenor, string? MontoMayor, string? estado, string? Fecha_desde, string? Fecha_hasta, int offset, int limite)
    {
      var lista = new List<Pago>();
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"
          SELECT 
            p.id_pago, p.numero_pago, p.fecha_pago, p.concepto, p.monto, p.estado AS pago_estado, p.id_contrato,
            c.fecha_desde, c.fecha_hasta, c.fecha_terminacion_anticipada, c.monto_mensual, c.multa, c.estado AS contrato_estado, c.created_at AS contrato_created_at, c.updated_at AS contrato_updated_at,
            inm.id_inmueble, inm.direccion AS inm_direccion, inm.cantidad_ambientes AS inm_cantidad_ambientes, inm.descripcion AS inm_descripcion, inm.precio AS inm_precio, inm.id_propietario AS inm_id_propietario,
            prop.id_propietario AS prop_id_propietario, prop.dni AS prop_dni, prop.nombre AS prop_nombre, prop.apellido AS prop_apellido, prop.telefono AS prop_telefono, prop.email AS prop_email,
            ti.id_tipo_inmueble, ti.nombre AS ti_nombre,
            inq.id_inquilino, inq.dni AS inq_dni, inq.nombre AS inq_nombre, inq.apellido AS inq_apellido, inq.telefono AS inq_telefono, inq.email AS inq_email
          FROM pago p
          JOIN contrato c ON p.id_contrato = c.id_contrato
          JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
          JOIN tipo_inmueble ti ON inm.id_tipo_inmueble = ti.id_tipo_inmueble
          JOIN propietario prop ON inm.id_propietario = prop.id_propietario
          JOIN inquilino inq ON c.id_inquilino = inq.id_inquilino
          WHERE 1=1
          ";

        if (!string.IsNullOrEmpty(idPago))
          sql += " AND id_pago LIKE @idPago";

        if (!string.IsNullOrEmpty(idContrato))
            sql += " AND p.id_contrato LIKE @idContrato";


        if (!string.IsNullOrEmpty(dniInquilino))
          sql += " AND inq.dni LIKE @dniInquilino";

        if (!string.IsNullOrEmpty(MontoMenor))
          sql += " AND monto >= @MontoMenor";

        if (!string.IsNullOrEmpty(MontoMayor))
          sql += " AND monto <= @MontoMayor";

        if (!string.IsNullOrEmpty(Fecha_desde))
          sql += " AND DATE(fecha_pago) >= @fechaDesde";

        if (!string.IsNullOrEmpty(Fecha_hasta))
          sql += " AND DATE(fecha_pago) <= @fechaHasta";

        if (!string.IsNullOrEmpty(estado))
          sql += " AND estado = @estado";

        sql += " LIMIT @limite OFFSET @offset";

        using (var cmd = new MySqlCommand(sql, conn))
        {

          if (!string.IsNullOrEmpty(idPago))
            cmd.Parameters.AddWithValue("@idPago", "%" + idPago + "%");

          if (!string.IsNullOrEmpty(idContrato))
            cmd.Parameters.AddWithValue("@idContrato", "%" + idContrato + "%");

          if (!string.IsNullOrEmpty(dniInquilino))
            cmd.Parameters.AddWithValue("@dniInquilino", "%" + dniInquilino + "%");

          if (!string.IsNullOrEmpty(MontoMenor))
            cmd.Parameters.AddWithValue("@MontoMenor", MontoMenor);

          if (!string.IsNullOrEmpty(MontoMayor))
            cmd.Parameters.AddWithValue("@MontoMayor", MontoMayor);

          if (!string.IsNullOrEmpty(Fecha_desde))
            cmd.Parameters.AddWithValue("@fechaDesde", Fecha_desde);

          if (!string.IsNullOrEmpty(Fecha_hasta))
            cmd.Parameters.AddWithValue("@fechaHasta", Fecha_hasta);

          if (!string.IsNullOrEmpty(estado))
            cmd.Parameters.AddWithValue("@estado", estado);

          cmd.Parameters.AddWithValue("@limite", limite);
          cmd.Parameters.AddWithValue("@offset", offset);
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
              lista.Add(new Pago
              {
                IdPago = reader.GetInt32("id_pago"),
                numeroPago = reader.GetInt32("numero_pago"),
                FechaPago = reader.GetDateTime("fecha_pago"),
                Concepto = reader.GetString("concepto"),
                Monto = reader.GetDecimal("monto"),
                Estado = reader.GetInt32("pago_estado"),
                IdContrato = reader.GetInt32("id_contrato"),
                contrato = new Contrato
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
                  Estado = reader.GetInt32("contrato_estado"),
                  IdInmueble = reader.GetInt32("id_inmueble"),
                  Inmueble = new Inmueble
                  {
                    IdInmueble = reader.GetInt32("id_inmueble"),
                    IdPropietario = reader.GetInt32("inm_id_propietario"),
                    Propietario = new Propietario
                    {
                      IdPropietario = reader.GetInt32("prop_id_propietario"),
                      Dni = reader.GetString("prop_dni"),
                      Nombre = reader.GetString("prop_nombre"),
                      Apellido = reader.GetString("prop_apellido"),
                      Telefono = reader.GetString("prop_telefono"),
                      Email = reader.GetString("prop_email")
                    },
                    TipoInmueble = new TipoInmueble
                    {
                      IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                      Nombre = reader.GetString("ti_nombre"),
                    },
                    Direccion = reader.GetString("inm_direccion"),
                    CantidadAmbientes = reader.GetInt32("inm_cantidad_ambientes"),
                    Descripcion = reader.GetString("inm_descripcion"),
                    Precio = reader.GetDecimal("inm_precio"),
                    IdTipoInmueble = reader.GetInt32("id_tipo_inmueble")
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
                  CreatedAt = reader.GetDateTime("contrato_created_at"),
                  UpdatedAt = reader.GetDateTime("contrato_updated_at")
                }
              });
            }
          }
        }
      }
      return lista;
    }

    public int CantidadFiltro(string? idPago, string? idContrato, string? dniInquilino, string? MontoMenor, string? MontoMayor, string? estado, string? Fecha_desde, string? Fecha_hasta)
    {
      int total = 0;
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"
            SELECT COUNT(*)
            FROM pago p
            JOIN contrato c ON p.id_contrato = c.id_contrato
            JOIN inquilino inq ON c.id_inquilino = inq.id_inquilino
            WHERE 1=1
          ";

        if (!string.IsNullOrEmpty(idPago))
          sql += " AND p.id_pago LIKE @idPago";

        if (!string.IsNullOrEmpty(idContrato))
          sql += " AND p.id_contrato LIKE @idContrato";

        if (!string.IsNullOrEmpty(dniInquilino))
          sql += " AND inq.dni LIKE @dniInquilino";

        if (!string.IsNullOrEmpty(MontoMenor))
          sql += " AND p.monto >= @MontoMenor";

        if (!string.IsNullOrEmpty(MontoMayor))
          sql += " AND p.monto <= @MontoMayor";

        if (!string.IsNullOrEmpty(Fecha_desde))
          sql += " AND DATE(p.fecha_pago) >= @fechaDesde";

        if (!string.IsNullOrEmpty(Fecha_hasta))
          sql += " AND DATE(p.fecha_pago) <= @fechaHasta";

        if (!string.IsNullOrEmpty(estado))
          sql += " AND p.estado = @estado";
        using (var cmd = new MySqlCommand(sql, conn))
        {

          if (!string.IsNullOrEmpty(idPago))
            cmd.Parameters.AddWithValue("@idPago", "%" + idPago + "%");

          if (!string.IsNullOrEmpty(idContrato))
            cmd.Parameters.AddWithValue("@idContrato", "%" + idContrato + "%");

          if (!string.IsNullOrEmpty(dniInquilino))
            cmd.Parameters.AddWithValue("@dniInquilino", "%" + dniInquilino + "%");

          if (!string.IsNullOrEmpty(MontoMenor))
            cmd.Parameters.AddWithValue("@MontoMenor", MontoMenor);

          if (!string.IsNullOrEmpty(MontoMayor))
            cmd.Parameters.AddWithValue("@MontoMayor", MontoMayor);

          if (!string.IsNullOrEmpty(Fecha_desde))
            cmd.Parameters.AddWithValue("@fechaDesde", Fecha_desde);

          if (!string.IsNullOrEmpty(Fecha_hasta))
            cmd.Parameters.AddWithValue("@fechaHasta", Fecha_hasta);

          if (!string.IsNullOrEmpty(estado))
            cmd.Parameters.AddWithValue("@estado", estado);
          conn.Open();
          total = Convert.ToInt32(cmd.ExecuteScalar());
          conn.Close();
        }
      }
      return total;
    }

  }
}