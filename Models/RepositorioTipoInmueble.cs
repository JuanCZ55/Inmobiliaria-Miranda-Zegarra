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

        public List<TipoInmueble> ObtenerTodos()
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
    }
}
