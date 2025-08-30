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
                    cmd.Parameters.AddWithValue("@Descripcion", Inmueble.Descripcion ?? " - ");
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
            var i = new Inmueble();
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
                            i.IdInmueble = reader.GetInt32("id_inmueble");
                            i.IdPropietario = reader.GetInt32("id_propietario");
                            i.IdTipoInmueble = reader.GetInt32("id_tipo_inmueble");
                            i.Direccion = reader.GetString("direccion");
                            i.Uso = reader.GetInt32("uso");
                            i.CantidadAmbientes = reader.GetInt32("cantidad_ambientes");
                            i.Longitud = reader.GetString("longitud");
                            i.Latitud = reader.GetString("latitud");
                            i.Precio = reader.GetDecimal("precio");
                            i.Descripcion = reader.GetString("descripcion");
                            i.Estado = reader.GetInt32("estado");
                            i.CreatedAt = reader.GetDateTime("created_at");
                            i.UpdatedAt = reader.GetDateTime("updated_at");
                        }
                    }
                }
            }
            return i;
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
                                Descripcion = reader.GetString("descripcion"),
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

        public int SeEstaUsando(int IdInmueble)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"SELECT COUNT(*) FROM contrato WHERE id_inmueble = @IdInmueble AND estado = 1";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdInmueble", IdInmueble);
                    conn.Open();
                    res = Convert.ToInt32(cmd.ExecuteScalar());
                    conn.Close();
                }
            }
            return res;
        }

        public List<Inmueble> SupaFiltro(
            string? direccion,
            string? dni,
            int? idTipoInmueble,
            int? uso,
            int? cantidadAmbientesMin,
            decimal? precioMin,
            decimal? precioMax,
            int? estado
        )
        {
            var lista = new List<Inmueble>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                var sql =
                    @"
            SELECT
                i.id_inmueble           AS IdInmueble,
                i.id_propietario        AS Inmueble_IdPropietario,
                i.direccion             AS Inmueble_Direccion,
                i.id_tipo_inmueble      AS IdTipoInmueble,
                i.uso                   AS Uso,
                i.cantidad_ambientes    AS CantidadAmbientes,
                i.longitud              AS Inmueble_Longitud,
                i.latitud               AS Inmueble_Latitud,
                i.precio                AS Precio,
                i.descripcion           AS Descripcion,
                i.estado                AS Inmueble_Estado,
                i.created_at            AS Inmueble_CreatedAt,
                i.updated_at            AS Inmueble_UpdatedAt,
                pr.id_propietario       AS Propietario_IdPropietario,
                pr.nombre               AS Propietario_Nombre,
                pr.apellido             AS Propietario_Apellido,
                pr.dni                  AS Propietario_Dni
            FROM inmueble i
            JOIN propietario pr 
                ON i.id_propietario = pr.id_propietario
            WHERE 1=1
        ";

                // filtros dinámicos
                if (!string.IsNullOrEmpty(direccion))
                    sql += " AND i.direccion LIKE CONCAT(@Direccion, '%')";
                if (!string.IsNullOrEmpty(dni))
                    sql += " AND pr.dni LIKE CONCAT(@Dni, '%')";
                if (idTipoInmueble.HasValue && idTipoInmueble.Value != 0)
                    sql += " AND i.id_tipo_inmueble = @IdTipoInmueble";
                if (uso.HasValue && uso.Value != 0)
                    sql += " AND i.uso = @Uso";
                if (cantidadAmbientesMin.HasValue && cantidadAmbientesMin.Value > 0)
                    sql += " AND i.cantidad_ambientes >= @CantidadAmbientesMin";
                if (precioMin.HasValue && precioMin.Value > 0)
                    sql += " AND i.precio >= @PrecioMin";
                if (precioMax.HasValue && precioMax.Value > 0)
                    sql += " AND i.precio <= @PrecioMax";
                if (estado.HasValue && estado.Value != 0)
                    sql += " AND i.estado = @Estado";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    // parámetros dinámicos
                    if (!string.IsNullOrEmpty(direccion))
                        cmd.Parameters.AddWithValue("@Direccion", direccion);
                    if (!string.IsNullOrEmpty(dni))
                        cmd.Parameters.AddWithValue("@Dni", dni);
                    if (idTipoInmueble.HasValue && idTipoInmueble.Value != 0)
                        cmd.Parameters.AddWithValue("@IdTipoInmueble", idTipoInmueble.Value);
                    if (uso.HasValue && uso.Value != 0)
                        cmd.Parameters.AddWithValue("@Uso", uso.Value);
                    if (cantidadAmbientesMin.HasValue && cantidadAmbientesMin.Value > 0)
                        cmd.Parameters.AddWithValue(
                            "@CantidadAmbientesMin",
                            cantidadAmbientesMin.Value
                        );
                    if (precioMin.HasValue && precioMin.Value > 0)
                        cmd.Parameters.AddWithValue("@PrecioMin", precioMin.Value);
                    if (precioMax.HasValue && precioMax.Value > 0)
                        cmd.Parameters.AddWithValue("@PrecioMax", precioMax.Value);
                    if (estado.HasValue && estado.Value != 0)
                        cmd.Parameters.AddWithValue("@Estado", estado.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                IdPropietario = reader.GetInt32("Inmueble_IdPropietario"),
                                Direccion = reader.GetString("Inmueble_Direccion"),
                                IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                                Uso = reader.GetInt32("Uso"),
                                CantidadAmbientes = reader.GetInt32("CantidadAmbientes"),
                                Longitud = reader.GetString("Inmueble_Longitud"),
                                Latitud = reader.GetString("Inmueble_Latitud"),
                                Precio = reader.GetDecimal("Precio"),
                                Descripcion = reader.GetString("Descripcion"),
                                Estado = reader.GetInt32("Inmueble_Estado"),
                                CreatedAt = reader.GetDateTime("Inmueble_CreatedAt"),
                                UpdatedAt = reader.GetDateTime("Inmueble_UpdatedAt"),

                                Propietario = new Propietario
                                {
                                    IdPropietario = reader.GetInt32("Propietario_IdPropietario"),
                                    Nombre = reader.GetString("Propietario_Nombre"),
                                    Apellido = reader.GetString("Propietario_Apellido"),
                                    Dni = reader.GetString("Propietario_Dni"),
                                },
                            };

                            lista.Add(inmueble);
                        }
                    }
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
