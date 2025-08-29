namespace Inmobiliaria.Models
{
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.Extensions.Configuration;
    using MySql.Data.MySqlClient;

    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration)
            : base(configuration) { }

        public int Crear(Inmueble Inmueble)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"INSERT INTO inmueble (id_propietario, id_tipo_inmueble, direccion, uso, cantidad_ambientes, longitud, latitud, precio, estado, descripcion, created_at, updated_at) VALUES (@IdPropietario, @IdTipoInmueble, @Direccion, @Uso, @CantidadAmbientes, @Longitud, @Latitud, @Precio, '1', @Descripcion, current_timestamp(), current_timestamp())";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdPropietario", Inmueble.IdPropietario);
                    cmd.Parameters.AddWithValue("@IdTipoInmueble", Inmueble.IdTipoInmueble);
                    cmd.Parameters.AddWithValue("@Direccion", Inmueble.Direccion);
                    cmd.Parameters.AddWithValue("@Uso", Inmueble.Uso);
                    cmd.Parameters.AddWithValue("@CantidadAmbientes", Inmueble.CantidadAmbientes);
                    cmd.Parameters.AddWithValue("@Longitud", Inmueble.Longitud);
                    cmd.Parameters.AddWithValue("@Latitud", Inmueble.Latitud);
                    cmd.Parameters.AddWithValue("@Precio", Inmueble.Precio);
                    cmd.Parameters.AddWithValue("@Descripcion", Inmueble.Descripcion);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }

            return res;
        }

        public int Modificar(Inmueble Inmueble)
        {
            int res = -1;
            var onk = new Inmueble();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"UPDATE inmueble SET id_propietario=@IdPropietario, id_tipo_inmueble=@IdTipoInmueble, direccion=@Direccion, uso=@Uso, cantidad_ambientes=@CantidadAmbientes, longitud=@Longitud, latitud=@Latitud, precio=@Precio, descripcion=@Descripcion, updated_at=current_timestamp() WHERE id_inmueble=@IdInmueble";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdInmueble", Inmueble.IdInmueble);
                    cmd.Parameters.AddWithValue("@IdPropietario", Inmueble.IdPropietario);
                    cmd.Parameters.AddWithValue("@IdTipoInmueble", Inmueble.IdTipoInmueble);
                    cmd.Parameters.AddWithValue("@Direccion", Inmueble.Direccion);
                    cmd.Parameters.AddWithValue("@Uso", Inmueble.Uso);
                    cmd.Parameters.AddWithValue("@CantidadAmbientes", Inmueble.CantidadAmbientes);
                    cmd.Parameters.AddWithValue("@Longitud", Inmueble.Longitud);
                    cmd.Parameters.AddWithValue("@Latitud", Inmueble.Latitud);
                    cmd.Parameters.AddWithValue("@Precio", Inmueble.Precio);
                    cmd.Parameters.AddWithValue("@Descripcion", Inmueble.Descripcion);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public int Eliminar(int IdInmueble)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = @"DELETE FROM inmueble WHERE id_inmueble=@IdInmueble";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdInmueble", IdInmueble);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public Inmueble ObtenerPorID(int IdInmueble)
        {
            Inmueble inmueble = null;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT * FROM inmueble WHERE id_inmueble=@IdInmueble";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdInmueble", IdInmueble);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                IdPropietario = reader.GetInt32("id_propietario"),
                                IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                Uso = reader.GetInt32("uso"),
                                CantidadAmbientes = reader.GetInt32("cantidad_ambientes"),
                                Longitud = reader.GetString("longitud"),
                                Latitud = reader.GetString("latitud"),
                                Precio = reader.GetDecimal("precio"),
                                Descripcion = reader.GetString("descripcion"),
                                Estado = reader.GetInt32("estado"),
                                CreatedAt = reader.GetDateTime("created_at"),
                                UpdatedAt = reader.GetDateTime("updated_at"),
                            };
                        }
                    }
                }
            }
            return inmueble;
        }

        public List<Inmueble> ObtenerTodos()
        {
            var lista = new List<Inmueble>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT * FROM inmueble";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                IdPropietario = reader.GetInt32("id_propietario"),
                                IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                Uso = reader.GetInt32("uso"),
                                CantidadAmbientes = reader.GetInt32("cantidad_ambientes"),
                                Longitud = reader.GetString("longitud"),
                                Latitud = reader.GetString("latitud"),
                                Precio = reader.GetDecimal("precio"),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion"))
                                    ? null
                                    : reader.GetString("descripcion"),
                                Estado = reader.GetInt32("estado"),
                                CreatedAt = reader.GetDateTime("created_at"),
                                UpdatedAt = reader.GetDateTime("updated_at"),
                            };
                            lista.Add(inmueble);
                        }
                    }
                }
            }
            return lista;
        }

        public List<Inmueble> ObtenerPorPropietario(int IdPropietario)
        {
            var lista = new List<Inmueble>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT * FROM inmueble WHERE id_propietario = @IdPropietario";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdPropietario", IdPropietario);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                IdPropietario = reader.GetInt32("id_propietario"),
                                IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                Uso = reader.GetInt32("uso"),
                                CantidadAmbientes = reader.GetInt32("cantidad_ambientes"),
                                Longitud = reader.GetString("longitud"),
                                Latitud = reader.GetString("latitud"),
                                Precio = reader.GetDecimal("precio"),
                                Descripcion = reader.GetString("descripcion"),
                                Estado = reader.GetInt32("estado"),
                                CreatedAt = reader.GetDateTime("created_at"),
                                UpdatedAt = reader.GetDateTime("updated_at"),
                            };
                            lista.Add(inmueble);
                        }
                    }
                    conn.Close();
                }
                return lista;
            }
        }

        public List<Inmueble> ObtenerPorTipo(int IdTipoInmueble)
        {
            var lista = new List<Inmueble>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT * FROM inmueble WHERE id_tipo_inmueble = @IdTipoInmueble";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdTipoInmueble", IdTipoInmueble);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                IdPropietario = reader.GetInt32("id_propietario"),
                                IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                Uso = reader.GetInt32("uso"),
                                CantidadAmbientes = reader.GetInt32("cantidad_ambientes"),
                                Longitud = reader.GetString("longitud"),
                                Latitud = reader.GetString("latitud"),
                                Precio = reader.GetDecimal("precio"),
                                Descripcion = reader.GetString("descripcion"),
                                Estado = reader.GetInt32("estado"),
                                CreatedAt = reader.GetDateTime("created_at"),
                                UpdatedAt = reader.GetDateTime("updated_at"),
                            };
                            lista.Add(inmueble);
                        }
                    }
                    conn.Close();
                }
            }
            return lista;
        }

        public List<Inmueble> ObtenerPorUso(int Uso)
        {
            var lista = new List<Inmueble>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT * FROM inmueble WHERE uso = @Uso";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Uso", Uso);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                IdPropietario = reader.GetInt32("id_propietario"),
                                IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                Uso = reader.GetInt32("uso"),
                                CantidadAmbientes = reader.GetInt32("cantidad_ambientes"),
                                Longitud = reader.GetString("longitud"),
                                Latitud = reader.GetString("latitud"),
                                Precio = reader.GetDecimal("precio"),
                                Descripcion = reader.GetString("descripcion"),
                                Estado = reader.GetInt32("estado"),
                                CreatedAt = reader.GetDateTime("created_at"),
                                UpdatedAt = reader.GetDateTime("updated_at"),
                            };
                            lista.Add(inmueble);
                        }
                    }
                    conn.Close();
                }
            }
            return lista;
        }

        public List<Inmueble> ObtenerPorCantidadAmbientes(int CantidadDeAmbientes)
        {
            var lista = new List<Inmueble>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"SELECT * FROM inmueble WHERE cantidad_ambientes >= @CantidadDeAmbientes";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CantidadDeAmbientes", CantidadDeAmbientes);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                IdPropietario = reader.GetInt32("id_propietario"),
                                IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                Uso = reader.GetInt32("uso"),
                                CantidadAmbientes = reader.GetInt32("cantidad_ambientes"),
                                Longitud = reader.GetString("longitud"),
                                Latitud = reader.GetString("latitud"),
                                Precio = reader.GetDecimal("precio"),
                                Descripcion = reader.GetString("descripcion"),
                                Estado = reader.GetInt32("estado"),
                                CreatedAt = reader.GetDateTime("created_at"),
                                UpdatedAt = reader.GetDateTime("updated_at"),
                            };
                            lista.Add(inmueble);
                        }
                    }
                    conn.Close();
                }
            }
            return lista;
        }

        public int SetEstado(int IdInmueble, int Estado)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE inmueble SET estado = @Estado WHERE id_inmueble = @IdInmueble";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdInmueble", IdInmueble);
                    cmd.Parameters.AddWithValue("@Estado", Estado);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }
    }
}
