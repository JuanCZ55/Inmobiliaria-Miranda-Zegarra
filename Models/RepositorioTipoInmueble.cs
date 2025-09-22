namespace Inmobiliaria.Models
{
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.Extensions.Configuration;
    using MySql.Data.MySqlClient;

    public class RepositorioTipoInmueble : RepositorioBase, IRepositorioTipoInmueble
    {
        public RepositorioTipoInmueble(IConfiguration configuration)
            : base(configuration) { }

        public int Crear(TipoInmueble tipoInmueble)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = "INSERT INTO tipo_inmueble (Nombre) VALUES (@Nombre)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", tipoInmueble.Nombre);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public int Modificar(TipoInmueble tipoInmueble)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    "UPDATE tipo_inmueble SET Nombre = @Nombre WHERE id_tipo_inmueble = @IdTipoInmueble";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", tipoInmueble.Nombre);
                    cmd.Parameters.AddWithValue("@IdTipoInmueble", tipoInmueble.IdTipoInmueble);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public int Eliminar(int IdTipoInmueble)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = "DELETE FROM tipo_inmueble WHERE id_tipo_inmueble = @IdTipoInmueble;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdTipoInmueble", IdTipoInmueble);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public int SeEstaUsando(int IdTipoInmueble)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = "SELECT COUNT(*) FROM inmueble WHERE id_tipo_inmueble = @IdTipoInmueble;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdTipoInmueble", IdTipoInmueble);
                    conn.Open();
                    res = Convert.ToInt32(cmd.ExecuteScalar());
                    conn.Close();
                }
            }
            return res;
        }

        public int ExisteTipoInmueble(string nombre)
        {
            int res = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = "SELECT COUNT(*) FROM tipo_inmueble WHERE Nombre = @Nombre";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    conn.Open();
                    res = Convert.ToInt32(cmd.ExecuteScalar());
                    conn.Close();
                }
            }
            return res;
        }

        public TipoInmueble ObtenerPorID(int id)
        {
            TipoInmueble tipoInmueble = new TipoInmueble(); // objeto por defecto
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = "SELECT * FROM tipo_inmueble WHERE id_tipo_inmueble = @IdTipoInmueble";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdTipoInmueble", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tipoInmueble.IdTipoInmueble = reader.GetInt32("id_tipo_inmueble");
                            tipoInmueble.Nombre = reader.GetString("Nombre");
                        }
                    }
                }
            }
            return tipoInmueble;
        }
        public List<TipoInmueble> TenerTodos()
        {
            var lista = new List<TipoInmueble>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = "SELECT * FROM tipo_inmueble";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tipoInmueble = new TipoInmueble
                            {
                                IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                                Nombre = reader.GetString("Nombre"),
                            };

                            lista.Add(tipoInmueble);
                        }
                    }

                    conn.Close();
                }
            }
            return lista;
        }
        public int ContarFiltro(string? nombre)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = "SELECT COUNT(*) FROM tipo_inmueble WHERE 1=1 ";

                if (!string.IsNullOrEmpty(nombre))
                {
                    sql += "AND Nombre LIKE @nombre ";
                }
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", "%" + nombre + "%");
                    conn.Open();
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count;
                }
            }
        }
        public List<TipoInmueble> Filtro(string? nombre, int limit, int offset)
        {
            var lista = new List<TipoInmueble>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT id_tipo_inmueble, Nombre FROM tipo_inmueble WHERE 1=1 ";

                if (!string.IsNullOrEmpty(nombre))
                {
                    sql += "AND Nombre LIKE @nombre ";
                }

                sql += "ORDER BY Nombre ASC LIMIT @limit OFFSET @offset;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (!string.IsNullOrEmpty(nombre))
                    {
                        cmd.Parameters.AddWithValue("@nombre", "%" + nombre + "%");
                    }

                    cmd.Parameters.AddWithValue("@limit", limit);
                    cmd.Parameters.AddWithValue("@offset", offset);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new TipoInmueble
                            {
                                IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                                Nombre = reader.GetString("Nombre"),
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
