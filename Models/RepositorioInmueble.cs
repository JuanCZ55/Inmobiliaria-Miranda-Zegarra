using System;
using System.Collections.Generic;
using System.Data;
using Inmobiliaria.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Inmobiliaria.Models
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration)
            : base(configuration) { }

        public int Crear(Inmueble inmueble)
        {
            int id = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        var sql =
                            @"
                            INSERT INTO inmueble (
                                id_propietario, id_tipo_inmueble, direccion, uso,
                                cantidad_ambientes, longitud, latitud, precio,
                                estado, descripcion, created_at, updated_at
                            ) VALUES (
                                @IdPropietario, @IdTipoInmueble, @Direccion, @Uso,
                                @CantidadAmbientes, @Longitud, @Latitud, @Precio,
                                '1', @Descripcion, current_timestamp(), current_timestamp()
                            );
                            SELECT LAST_INSERT_ID();";

                        using (var cmd = new MySqlCommand(sql, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
                            cmd.Parameters.AddWithValue("@IdTipoInmueble", inmueble.IdTipoInmueble);
                            cmd.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                            cmd.Parameters.AddWithValue("@Uso", inmueble.Uso);
                            cmd.Parameters.AddWithValue(
                                "@CantidadAmbientes",
                                inmueble.CantidadAmbientes
                            );
                            cmd.Parameters.AddWithValue("@Longitud", inmueble.Longitud);
                            cmd.Parameters.AddWithValue("@Latitud", inmueble.Latitud);
                            cmd.Parameters.AddWithValue("@Precio", inmueble.Precio);
                            cmd.Parameters.AddWithValue(
                                "@Descripcion",
                                inmueble.Descripcion ?? "-"
                            );
                            id = Convert.ToInt32(cmd.ExecuteScalar());
                            inmueble.IdInmueble = id;
                        }

                        var sqlImagen =
                            @"
                            INSERT INTO imagen (id_inmueble, url, tipo)
                            VALUES (@IdInmueble, @Url, @Tipo);";

                        if (inmueble.listImagenes != null)
                        {
                            foreach (var img in inmueble.listImagenes)
                            {
                                using (var cmd = new MySqlCommand(sqlImagen, conn, tran))
                                {
                                    cmd.Parameters.AddWithValue("@IdInmueble", inmueble.IdInmueble);
                                    cmd.Parameters.AddWithValue("@Url", img.Url);
                                    cmd.Parameters.AddWithValue("@Tipo", img.Tipo);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
            }
            return id;
        }

        // lo mismo pero asincrono
        public async Task<int> CrearAsync(Inmueble inmueble)
        {
            int id = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                // Usamos los métodos ...Async() y la palabra clave 'await'
                await conn.OpenAsync();
                await using (var tran = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        var sql =
                            @"
                    INSERT INTO inmueble (
                        id_propietario, id_tipo_inmueble, direccion, uso,
                        cantidad_ambientes, longitud, latitud, precio,
                        estado, descripcion, created_at, updated_at
                    ) VALUES (
                        @IdPropietario, @IdTipoInmueble, @Direccion, @Uso,
                        @CantidadAmbientes, @Longitud, @Latitud, @Precio,
                        '1', @Descripcion, current_timestamp(), current_timestamp()
                    );
                    SELECT LAST_INSERT_ID();";

                        using (var cmd = new MySqlCommand(sql, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
                            cmd.Parameters.AddWithValue("@IdTipoInmueble", inmueble.IdTipoInmueble);
                            cmd.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                            cmd.Parameters.AddWithValue("@Uso", inmueble.Uso);
                            cmd.Parameters.AddWithValue(
                                "@CantidadAmbientes",
                                inmueble.CantidadAmbientes
                            );
                            cmd.Parameters.AddWithValue("@Longitud", inmueble.Longitud);
                            cmd.Parameters.AddWithValue("@Latitud", inmueble.Latitud);
                            cmd.Parameters.AddWithValue("@Precio", inmueble.Precio);
                            cmd.Parameters.AddWithValue(
                                "@Descripcion",
                                inmueble.Descripcion ?? "-"
                            );

                            // Ejecutamos la consulta de forma asíncrona
                            var result = await cmd.ExecuteScalarAsync();
                            id = Convert.ToInt32(result);
                            inmueble.IdInmueble = id;
                        }

                        var sqlImagen =
                            @"
                    INSERT INTO imagen (id_inmueble, url, tipo)
                    VALUES (@IdInmueble, @Url, @Tipo);";

                        if (inmueble.listImagenes != null)
                        {
                            foreach (var img in inmueble.listImagenes)
                            {
                                using (var cmd = new MySqlCommand(sqlImagen, conn, tran))
                                {
                                    cmd.Parameters.AddWithValue("@IdInmueble", inmueble.IdInmueble);
                                    cmd.Parameters.AddWithValue("@Url", img.Url);
                                    cmd.Parameters.AddWithValue("@Tipo", img.Tipo);
                                    // Ejecutamos la inserción de forma asíncrona
                                    await cmd.ExecuteNonQueryAsync();
                                }
                            }
                        }

                        // Confirmamos la transacción de forma asíncrona
                        await tran.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        // Revertimos la transacción de forma asíncrona
                        await tran.RollbackAsync();
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
            }
            return id;
        }

        public int Modificar(Inmueble inmueble)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        var sqlInmueble =
                            @"
                            UPDATE inmueble SET 
                                id_propietario = @IdPropietario, 
                                id_tipo_inmueble = @IdTipoInmueble, 
                                direccion = @Direccion, 
                                uso = @Uso, 
                                cantidad_ambientes = @CantidadAmbientes, 
                                longitud = @Longitud, 
                                latitud = @Latitud, 
                                precio = @Precio, 
                                descripcion = @Descripcion,
                                updated_at = current_timestamp()
                            WHERE id_inmueble = @IdInmueble;";

                        using (var cmd = new MySqlCommand(sqlInmueble, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
                            cmd.Parameters.AddWithValue("@IdTipoInmueble", inmueble.IdTipoInmueble);
                            cmd.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                            cmd.Parameters.AddWithValue("@Uso", inmueble.Uso);
                            cmd.Parameters.AddWithValue(
                                "@CantidadAmbientes",
                                inmueble.CantidadAmbientes
                            );
                            cmd.Parameters.AddWithValue("@Longitud", inmueble.Longitud);
                            cmd.Parameters.AddWithValue("@Latitud", inmueble.Latitud);
                            cmd.Parameters.AddWithValue("@Precio", inmueble.Precio);
                            cmd.Parameters.AddWithValue(
                                "@Descripcion",
                                inmueble.Descripcion ?? "-"
                            );
                            cmd.Parameters.AddWithValue("@IdInmueble", inmueble.IdInmueble);
                            res = cmd.ExecuteNonQuery();
                        }

                        var sqlImagen =
                            @"
                            INSERT INTO imagen (id_inmueble, url, tipo) 
                            VALUES (@IdInmueble, @Url, @Tipo);";

                        if (inmueble.listImagenes != null)
                        {
                            foreach (var img in inmueble.listImagenes)
                            {
                                if (img.IdImagen == 0)
                                {
                                    using (var cmd = new MySqlCommand(sqlImagen, conn, tran))
                                    {
                                        cmd.Parameters.AddWithValue(
                                            "@IdInmueble",
                                            inmueble.IdInmueble
                                        );
                                        cmd.Parameters.AddWithValue("@Url", img.Url);
                                        cmd.Parameters.AddWithValue("@Tipo", img.Tipo);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
            }
            return res;
        }

        // Añadir este método a la clase RepositorioInmueble.cs

        public async Task<int> ModificarAsync(Inmueble inmueble)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                await using (var tran = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        var sqlInmueble =
                            @"
                    UPDATE inmueble SET 
                        id_propietario = @IdPropietario, 
                        id_tipo_inmueble = @IdTipoInmueble, 
                        direccion = @Direccion, 
                        uso = @Uso, 
                        cantidad_ambientes = @CantidadAmbientes, 
                        longitud = @Longitud, 
                        latitud = @Latitud, 
                        precio = @Precio, 
                        descripcion = @Descripcion,
                        estado = @Estado,
                        updated_at = current_timestamp()
                    WHERE id_inmueble = @IdInmueble;";

                        using (var cmd = new MySqlCommand(sqlInmueble, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
                            cmd.Parameters.AddWithValue("@IdTipoInmueble", inmueble.IdTipoInmueble);
                            cmd.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                            cmd.Parameters.AddWithValue("@Uso", inmueble.Uso);
                            cmd.Parameters.AddWithValue(
                                "@CantidadAmbientes",
                                inmueble.CantidadAmbientes
                            );
                            cmd.Parameters.AddWithValue("@Longitud", inmueble.Longitud);
                            cmd.Parameters.AddWithValue("@Latitud", inmueble.Latitud);
                            cmd.Parameters.AddWithValue("@Precio", inmueble.Precio);
                            cmd.Parameters.AddWithValue(
                                "@Descripcion",
                                inmueble.Descripcion ?? "-"
                            );
                            cmd.Parameters.AddWithValue("@Estado", inmueble.Estado);
                            cmd.Parameters.AddWithValue("@IdInmueble", inmueble.IdInmueble);
                            res = await cmd.ExecuteNonQueryAsync();
                        }

                        var sqlImagen =
                            @"
                    INSERT INTO imagen (id_inmueble, url, tipo) 
                    VALUES (@IdInmueble, @Url, @Tipo);";

                        if (inmueble.listImagenes != null)
                        {
                            foreach (var img in inmueble.listImagenes)
                            {
                                if (img.IdImagen == 0) // Solo inserta las imágenes nuevas
                                {
                                    using (var cmd = new MySqlCommand(sqlImagen, conn, tran))
                                    {
                                        cmd.Parameters.AddWithValue(
                                            "@IdInmueble",
                                            inmueble.IdInmueble
                                        );
                                        cmd.Parameters.AddWithValue("@Url", img.Url);
                                        cmd.Parameters.AddWithValue("@Tipo", img.Tipo);
                                        await cmd.ExecuteNonQueryAsync();
                                    }
                                }
                            }
                        }

                        await tran.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await tran.RollbackAsync();
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
            }
            return res;
        }

        public int Eliminar(int id)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM inmueble WHERE id_inmueble = @Id;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    res = cmd.ExecuteNonQuery(); // Devuelve la cantidad de filas afectadas.
                }
            }
            return res;
        }

        public List<Inmueble> ObtenerTodos()
        {
            var lista = new List<Inmueble>();
            using (var conn = new MySqlConnection(connectionString))
            {
                string sql = "SELECT * FROM inmueble;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(
                                new Inmueble
                                {
                                    IdInmueble = reader.GetInt32("id_inmueble"),
                                    Direccion = reader.GetString("direccion"),
                                    Uso = reader.GetInt32("uso"),
                                    CantidadAmbientes = reader.GetInt32("cantidad_ambientes"),
                                    Precio = reader.GetDecimal("precio"),
                                    Estado = reader.GetInt32("estado"),
                                }
                            );
                        }
                    }
                }
            }
            return lista;
        }

        public Inmueble ObtenerPorID(int id)
        {
            Inmueble? inmueble = null;
            using (var conn = new MySqlConnection(connectionString))
            {
                string sql =
                    "SELECT i.*, p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido, t.Nombre AS TipoInmuebleNombre FROM inmueble i INNER JOIN propietario p ON i.id_propietario = p.id_propietario INNER JOIN tipo_inmueble t ON i.id_tipo_inmueble = t.id_tipo_inmueble WHERE i.id_inmueble = @IdInmueble;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdInmueble", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                CantidadAmbientes = reader.GetInt32("cantidad_ambientes"),
                                Precio = reader.GetDecimal("precio"),
                                Descripcion = reader.GetString("descripcion"),
                                Estado = reader.GetInt32("estado"),
                                IdPropietario = reader.GetInt32("id_propietario"),
                                IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                                Uso = reader.GetInt32("uso"),
                                Propietario = new Propietario
                                {
                                    IdPropietario = reader.GetInt32("id_propietario"),
                                    Nombre = reader.GetString("PropietarioNombre"),
                                    Apellido = reader.GetString("PropietarioApellido"),
                                },
                                TipoInmueble = new TipoInmueble
                                {
                                    IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                                    Nombre = reader.GetString("TipoInmuebleNombre"),
                                },
                            };
                        }
                    }
                }

                if (inmueble != null)
                {
                    inmueble.listImagenes = ObtenerImagenesPorInmueble(inmueble.IdInmueble);
                }
            }
            return inmueble ?? new Inmueble();
        }

        public List<Imagen> ObtenerImagenesPorInmueble(int idInmueble)
        {
            var lista = new List<Imagen>();
            using (var conn = new MySqlConnection(connectionString))
            {
                string sql =
                    "SELECT id_imagen, url, tipo FROM imagen WHERE id_inmueble = @IdInmueble ORDER BY tipo, id_imagen;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdInmueble", idInmueble);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(
                                new Imagen
                                {
                                    IdImagen = reader.GetInt32("id_imagen"),
                                    Url = reader.GetString("url"),
                                    Tipo = reader.GetInt32("tipo"),
                                }
                            );
                        }
                    }
                }
            }
            return lista;
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
                    LEFT JOIN contrato co ON i.id_inmueble=co.id_inmueble 
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

                // Si todos los filtros están vacíos o nulos, aplicar ORDER BY IdInmueble DESC
                if (
                    string.IsNullOrEmpty(direccion)
                    && string.IsNullOrEmpty(dni)
                    && (!idTipoInmueble.HasValue || idTipoInmueble.Value <= 0)
                    && (!uso.HasValue || (uso.Value != 1 && uso.Value != 2))
                    && (!cantidadAmbientes.HasValue || cantidadAmbientes.Value <= 0)
                    && (!precioMin.HasValue || precioMin.Value <= 0)
                    && (!precioMax.HasValue || precioMax.Value <= 0)
                    && (
                        !estado.HasValue
                        || (estado.Value != 1 && estado.Value != 2 && estado.Value != 3)
                    )
                )
                {
                    sql += " ORDER BY IdInmueble DESC";
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

        public Imagen? ObtenerImagenPorId(int idImagen)
        {
            Imagen? img = null;
            using (var conn = new MySqlConnection(connectionString))
            {
                // Traemos todos los campos por si los necesitas en el futuro
                string sql =
                    "SELECT id_imagen, id_inmueble, url, tipo FROM imagen WHERE id_imagen = @IdImagen;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdImagen", idImagen);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            img = new Imagen
                            {
                                IdImagen = reader.GetInt32("id_imagen"),
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                Url = reader.GetString("url"),
                                Tipo = reader.GetInt32("tipo"),
                            };
                        }
                    }
                }
            }
            return img;
        }

        public int EliminarImagen(int idImagen)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM imagen WHERE id_imagen = @IdImagen;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdImagen", idImagen);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                }
            }
            return res;
        }
    }
}
