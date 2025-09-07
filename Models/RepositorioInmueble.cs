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

        public Inmueble ObtenerPorID(int IdInmueble)
        {
            var i = new Inmueble();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"SELECT * FROM inmueble JOIN tipo_inmueble ON inmueble.id_tipo_inmueble = tipo_inmueble.id_tipo_inmueble WHERE id_inmueble=@IdInmueble";
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
                            i.TipoInmueble = new TipoInmueble
                            {
                                IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                                Nombre = reader.GetString("nombre"),
                            };
                        }
                    }
                }
            }
            return i;
        }

        public bool SeEstaUsando(int idInmueble)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"SELECT COUNT(*) FROM inmueble WHERE id_inmueble = @IdInmueble AND estado=2";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdInmueble", idInmueble);
                    conn.Open();
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    conn.Close();
                    return count > 0;
                }
            }
        }

        public int ContarFiltro(
            string? direccion,
            string? dni,
            int? idTipoInmueble,
            int? uso,
            int? cantidadAmbientes,
            decimal? precioMin,
            decimal? precioMax,
            int? estado
        )
        {
            int total = 0;

            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"
                      SELECT COUNT(*) 
                      FROM inmueble i
                      JOIN propietario pr 
                          ON i.id_propietario = pr.id_propietario
                      WHERE 1=1
                  ";

                if (!string.IsNullOrEmpty(direccion))
                {
                    sql += " AND i.direccion LIKE CONCAT('%', @Direccion, '%')";
                }
                if (!string.IsNullOrEmpty(dni))
                {
                    sql += " AND pr.dni LIKE CONCAT('%', @Dni, '%')";
                }
                if (idTipoInmueble > 0)
                {
                    sql += " AND i.id_tipo_inmueble = @IdTipoInmueble";
                }
                if (uso == 1 || uso == 2)
                {
                    sql += " AND i.uso = @Uso";
                }
                if (cantidadAmbientes > 0)
                {
                    sql += " AND i.cantidad_ambientes >= @CantidadAmbientes";
                }
                if (precioMin.HasValue && precioMin.Value > 0)
                {
                    sql += " AND i.precio >= @PrecioMin";
                }
                if (precioMax.HasValue && precioMax.Value > 0)
                {
                    sql += " AND i.precio <= @PrecioMax";
                }
                if (estado == 1 || estado == 2 || estado == 3)
                {
                    sql += " AND i.estado = @Estado";
                }

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (!string.IsNullOrEmpty(direccion))
                    {
                        cmd.Parameters.AddWithValue("@Direccion", direccion);
                    }
                    if (!string.IsNullOrEmpty(dni))
                    {
                        cmd.Parameters.AddWithValue("@Dni", dni);
                    }
                    if (idTipoInmueble > 0)
                    {
                        cmd.Parameters.AddWithValue("@IdTipoInmueble", idTipoInmueble);
                    }
                    if (uso == 1 || uso == 2)
                    {
                        cmd.Parameters.AddWithValue("@Uso", uso);
                    }
                    if (cantidadAmbientes > 0)
                    {
                        cmd.Parameters.AddWithValue("@CantidadAmbientes", cantidadAmbientes);
                    }
                    if (precioMin.HasValue && precioMin.Value > 0)
                    {
                        cmd.Parameters.AddWithValue("@PrecioMin", precioMin.Value);
                    }
                    if (precioMax.HasValue && precioMax.Value > 0)
                    {
                        cmd.Parameters.AddWithValue("@PrecioMax", precioMax.Value);
                    }
                    if (estado == 1 || estado == 2 || estado == 3)
                    {
                        cmd.Parameters.AddWithValue("@Estado", estado);
                    }

                    conn.Open();
                    total = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return total;
        }

        public List<Inmueble> Filtro(
            string? direccion,
            string? dni,
            int? idTipoInmueble,
            int? uso,
            int? cantidadAmbientes,
            decimal? precioMin,
            decimal? precioMax,
            int? estado,
            int? limit,
            int? offset
        )
        {
            var lista = new List<Inmueble>();

            using (var conn = new MySqlConnection(connectionString))
            {
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
                        pr.dni                  AS Propietario_Dni,
                        ti.id_tipo_inmueble     AS TipoInmueble_IdTipoInmueble,
                        ti.nombre               AS TipoInmueble_Nombre

                    FROM inmueble i
                    JOIN propietario pr ON i.id_propietario = pr.id_propietario
                    JOIN tipo_inmueble ti ON i.id_tipo_inmueble = ti.id_tipo_inmueble
                    WHERE 1=1
                ";

                if (!string.IsNullOrEmpty(direccion))
                {
                    sql += " AND i.direccion LIKE CONCAT('%', @Direccion, '%')";
                }
                if (!string.IsNullOrEmpty(dni))
                {
                    sql += " AND pr.dni LIKE CONCAT('%', @Dni, '%')";
                }
                if (idTipoInmueble > 0)
                {
                    sql += " AND i.id_tipo_inmueble = @IdTipoInmueble";
                }
                if (uso == 1 || uso == 2)
                {
                    sql += " AND i.uso = @Uso";
                }
                if (cantidadAmbientes > 0)
                {
                    sql += " AND i.cantidad_ambientes >= @CantidadAmbientes";
                }
                if (precioMin.HasValue && precioMin.Value > 0)
                {
                    sql += " AND i.precio >= @PrecioMin";
                }
                if (precioMax.HasValue && precioMax.Value > 0)
                {
                    sql += " AND i.precio <= @PrecioMax";
                }
                if (estado == 1 || estado == 2 || estado == 3)
                {
                    sql += " AND i.estado = @Estado";
                }

                sql += " LIMIT @Limit OFFSET @Offset";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (!string.IsNullOrEmpty(direccion))
                    {
                        cmd.Parameters.AddWithValue("@Direccion", direccion);
                    }
                    if (!string.IsNullOrEmpty(dni))
                    {
                        cmd.Parameters.AddWithValue("@Dni", dni);
                    }
                    if (idTipoInmueble > 0)
                    {
                        cmd.Parameters.AddWithValue("@IdTipoInmueble", idTipoInmueble);
                    }
                    if (uso == 1 || uso == 2)
                    {
                        cmd.Parameters.AddWithValue("@Uso", uso);
                    }
                    if (cantidadAmbientes > 0)
                    {
                        cmd.Parameters.AddWithValue("@CantidadAmbientes", cantidadAmbientes);
                    }
                    if (precioMin.HasValue && precioMin.Value > 0)
                    {
                        cmd.Parameters.AddWithValue("@PrecioMin", precioMin.Value);
                    }
                    if (precioMax.HasValue && precioMax.Value > 0)
                    {
                        cmd.Parameters.AddWithValue("@PrecioMax", precioMax.Value);
                    }
                    if (estado == 1 || estado == 2 || estado == 3)
                    {
                        cmd.Parameters.AddWithValue("@Estado", estado);
                    }

                    cmd.Parameters.AddWithValue("@Limit", limit);
                    cmd.Parameters.AddWithValue("@Offset", offset);

                    conn.Open();
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
                                TipoInmueble = new TipoInmueble
                                {
                                    IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                                    Nombre = reader.GetString("TipoInmueble_Nombre"),
                                },
                            };

                            lista.Add(inmueble);
                        }
                    }
                }
            }

            return lista;
        }
    }
}
