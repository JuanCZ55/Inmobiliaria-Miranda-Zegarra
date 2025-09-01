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

        public TipoInmueble BuscarPorId(int id)
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

        public int ContarPorTodos()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = "SELECT COUNT(*) FROM tipo_inmueble";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    conn.Open();
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    conn.Close();
                    return count;
                }
            }
        }

        public List<TipoInmueble> ObtenerTodos(int limit, int offset)
        {
            var lista = new List<TipoInmueble>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = "SELECT * FROM tipo_inmueble LIMIT @Limit OFFSET @Offset";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Limit", limit);
                    cmd.Parameters.AddWithValue("@Offset", offset);
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

        public List<TipoInmueble> ListarPorNombre(string nombre, int limite, int offset)
        {
            var lista = new List<TipoInmueble>();
            using var conn = new MySqlConnection(connectionString);
            var sql =
                @"SELECT * FROM tipo_inmueble WHERE Nombre LIKE @Nombre LIMIT @Limite OFFSET @Offset";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Nombre", nombre + "%");
            cmd.Parameters.AddWithValue("@Limite", limite);
            cmd.Parameters.AddWithValue("@Offset", offset);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(
                    new TipoInmueble
                    {
                        IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                        Nombre = reader.GetString("Nombre"),
                    }
                );
            }

            return lista;
        }

        public int ContarPorNombre(string nombre)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = "SELECT count(*) FROM tipo_inmueble WHERE Nombre LIKE @Nombre";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre + "%");
                    conn.Open();
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count;
                }
            }
        }
    }
}
