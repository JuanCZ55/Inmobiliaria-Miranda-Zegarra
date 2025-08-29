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
        VALUES (@id_contrato, @numero_pago, @fecha_pago, @concepto, @monto, @monto_mensual, @estado); SELECT LAST_INSERT_ID();";

        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@id_contrato", pago.IdContrato);
          cmd.Parameters.AddWithValue("@numero_pago", pago.numeroPago);
          cmd.Parameters.AddWithValue("@fecha_pago", DateTime.Now);
          cmd.Parameters.AddWithValue("@concepto", pago.Concepto);
          cmd.Parameters.AddWithValue("@monto", pago.Monto);
          cmd.Parameters.AddWithValue("@estado", pago.Estado);
          conn.Open();
          res = System.Convert.ToInt32(cmd.ExecuteScalar());
          pago.IdContrato = res;
          conn.Close();
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
      Pago Contrato = new Pago();
      using (var conn = new MySqlConnection(connectionString))
      {
        var sql = @"
        SELECT id_Pago, numero_pago, fecha_pago, concepto, monto, estado, id_Contrato
        FROM pago WHERE id_contrato = @id;
        ";
        using (var cmd = new MySqlCommand(sql, conn))
        {
          cmd.Parameters.AddWithValue("@id", id);
          conn.Open();
          using (var reader = cmd.ExecuteReader())
          {
            if (reader.Read())
            {
              Contrato = new Pago
              {
                IdPago = reader.GetInt32("id_pago"),
                IdContrato = reader.GetInt32("id_Contrato"),
                numeroPago = reader.GetInt32("numero_pago"),
                FechaPago = reader.GetDateTime("fecha_pago"),
                Concepto = reader.GetString("concepto"),
                Estado = reader.GetInt32("estado"),
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

  }
}