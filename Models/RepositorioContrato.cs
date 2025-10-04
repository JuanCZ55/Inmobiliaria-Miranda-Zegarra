namespace Inmobiliaria.Models
{
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.Extensions.Configuration;
    using MySql.Data.MySqlClient;

    public class RepositorioContraro : RepositorioBase, IRepositorioContrato
    {
        public RepositorioContraro(IConfiguration configuration)
            : base(configuration) { }

        public int Crear(Contrato contrato)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"
        INSERT INTO contrato (id_inquilino, id_inmueble, fecha_desde, fecha_hasta, monto_mensual, tipo, id_usuario_creador, created_at, updated_at) 
        VALUES (@id_inquilino, @id_inmueble, @fecha_desde, @fecha_hasta, @monto_mensual, @tipo, @id_usuario_creador,current_timestamp(), current_timestamp()); 
        SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id_inquilino", contrato.IdInquilino);
                    cmd.Parameters.AddWithValue("@id_inmueble", contrato.IdInmueble);
                    cmd.Parameters.AddWithValue("@fecha_desde", contrato.FechaInicio);
                    cmd.Parameters.AddWithValue("@fecha_hasta", contrato.FechaFinalizacion);
                    cmd.Parameters.AddWithValue("@monto_mensual", contrato.Monto);
                    cmd.Parameters.AddWithValue("@tipo", contrato.Tipo);
                    cmd.Parameters.AddWithValue("@id_usuario_creador", contrato.IdUsuarioCreador);
                    conn.Open();
                    res = System.Convert.ToInt32(cmd.ExecuteScalar());
                    contrato.IdContrato = res;
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
                var sql =
                    @"UPDATE contrato SET fecha_terminacion_anticipada=@fecha_fin, multa=@multa, id_usuario_finalizador=@id_usuario_finalizador, updated_at=NOW() WHERE id_contrato=@id_contrato";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@fecha_fin", DateTime.Now);
                    cmd.Parameters.AddWithValue("@multa", contrato.Multa ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@id_usuario_finalizador", contrato.IdUsuarioFinalizador);
                    cmd.Parameters.AddWithValue("@id_contrato", contrato.IdContrato);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public int Modificar(Contrato contrato)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"UPDATE contrato 
                    SET fecha_desde = @fecha_desde,
                        fecha_hasta = @fecha_hasta,
                        monto_mensual = @monto_mensual,
                        tipo = @tipo,
                        updated_at = NOW()
                    WHERE id_contrato = @id_contrato;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@fecha_desde", contrato.FechaInicio);
                    cmd.Parameters.AddWithValue("@fecha_hasta", contrato.FechaFinalizacion);
                    cmd.Parameters.AddWithValue("@monto_mensual", contrato.Monto);
                    cmd.Parameters.AddWithValue("@tipo", contrato.Tipo);
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
            Contrato Contrato = new Contrato();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"
                SELECT 
            c.id_Contrato, 
            c.fecha_desde, 
            c.fecha_hasta, 
            c.fecha_terminacion_anticipada, 
            c.monto_mensual, 
            c.multa, 
            c.tipo,
            c.id_inmueble, 
            c.id_inquilino, 
            c.created_at, 
            c.updated_at,
            CASE
                WHEN c.fecha_terminacion_anticipada IS NULL 
                    AND c.fecha_hasta >= CURDATE()
                    THEN 'Vigente'

                WHEN c.fecha_terminacion_anticipada IS NULL 
                    AND c.fecha_hasta < CURDATE()
                    THEN 'Finalizado'

                WHEN c.fecha_terminacion_anticipada IS NOT NULL 
                    AND EXISTS (
                          SELECT 1 FROM pago pa
                          WHERE pa.id_contrato = c.id_contrato
                          AND pa.concepto = 'Multa de Cancelacion'
                          AND pa.estado = 1
                    )
                    THEN 'Cancelado con Multa Saldada'

                WHEN c.fecha_terminacion_anticipada IS NOT NULL 
                    AND NOT EXISTS (
                          SELECT 1 FROM pago pa
                          WHERE pa.id_contrato = c.id_contrato
                          AND pa.concepto = 'Multa de Cancelacion'
                          AND pa.estado = 1
                    )
                    THEN 'Cancelado con Multa Pendiente'
            END AS estado_contrato,

            inm.id_Propietario AS inm_id_propietario, 
            inm.direccion AS inm_direccion, 
            inm.cantidad_ambientes AS inm_cantidad_ambientes, 
            inm.descripcion AS inm_descripcion,
            inm.precio AS inm_precio,
            inm.id_tipo_inmueble AS id_tipo_inmueble,

            ti.id_tipo_inmueble AS ti_id_tipo_inmueble,
            ti.nombre AS ti_nombre,

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
        JOIN tipo_inmueble ti ON inm.id_tipo_inmueble = ti.id_tipo_inmueble
        JOIN propietario p ON inm.id_propietario = p.id_propietario
        JOIN inquilino inq ON c.id_inquilino = inq.id_inquilino
        WHERE c.id_contrato = @id
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
                                FechaInicio = reader.GetDateTime("fecha_desde"),
                                FechaFinalizacion = reader.GetDateTime("fecha_hasta"),
                                FechaCancelacion = reader.IsDBNull(
                                    reader.GetOrdinal("fecha_terminacion_anticipada")
                                )
                                    ? (DateTime?)null
                                    : reader.GetDateTime("fecha_terminacion_anticipada"),
                                Monto = reader.GetDecimal("monto_mensual"),
                                Multa = !reader.IsDBNull(reader.GetOrdinal("multa"))
                                    ? Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("multa")))
                                    : (decimal?)null,

                                Estado = reader.GetString("estado_contrato"),
                                Tipo = reader.GetInt32("tipo"),
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
                                        Email = reader.GetString("p_email"),
                                    },
                                    TipoInmueble = new TipoInmueble
                                    {
                                        IdTipoInmueble = reader.GetInt32("ti_id_tipo_inmueble"),
                                        Nombre = reader.GetString("ti_nombre"),
                                    },
                                    Direccion = reader.GetString("inm_direccion"),
                                    CantidadAmbientes = reader.GetInt32("inm_cantidad_ambientes"),
                                    Descripcion = reader.GetString("inm_descripcion"),
                                    Precio = reader.GetDecimal("inm_precio"),
                                    IdTipoInmueble = reader.GetInt32("id_tipo_inmueble"),
                                },
                                IdInquilino = reader.GetInt32("id_inquilino"),
                                Inquilino = new Inquilino
                                {
                                    IdInquilino = reader.GetInt32("inq_id_inquilino"),
                                    Dni = reader.GetString("inq_dni"),
                                    Nombre = reader.GetString("inq_nombre"),
                                    Apellido = reader.GetString("inq_apellido"),
                                    Telefono = reader.GetString("inq_telefono"),
                                    Email = reader.GetString("inq_email"),
                                },
                                CreatedAt = reader.GetDateTime("created_at"),
                                UpdatedAt = reader.GetDateTime("updated_at"),
                            };
                        }
                    }
                    conn.Close();
                }
            }
            return Contrato;
        }

        public List<Contrato> Filtrar(
            string? idContrato,
            string? dniInquilino,
            string? idInmueble,
            string? estado,
            string? Fecha_desde,
            string? Fecha_hasta,
            string? tipo,
            string? MontoMenor,
            string? MontoMayor,
            int offset,
            int limite
        )
        {
            var lista = new List<Contrato>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"
      SELECT 
        c.id_Contrato, c.fecha_desde, c.fecha_hasta, c.fecha_terminacion_anticipada, c.tipo, 
        c.monto_mensual, c.multa, c.id_inmueble, c.id_inquilino, c.created_at, c.updated_at,
        CASE
          WHEN c.fecha_terminacion_anticipada IS NULL AND c.fecha_hasta >= CURDATE() THEN 'Vigente'
          WHEN c.fecha_terminacion_anticipada IS NULL AND c.fecha_hasta < CURDATE() THEN 'Finalizado'
          WHEN c.fecha_terminacion_anticipada IS NOT NULL 
              AND EXISTS (SELECT 1 FROM pago pa WHERE pa.id_contrato = c.id_contrato AND pa.concepto = 'Multa de Cancelacion' AND pa.estado = 1)
            THEN 'Cancelado con Multa Saldada'
          WHEN c.fecha_terminacion_anticipada IS NOT NULL 
              AND NOT EXISTS (SELECT 1 FROM pago pa WHERE pa.id_contrato = c.id_contrato AND pa.concepto = 'Multa de Cancelacion' AND pa.estado = 1)
            THEN 'Cancelado con Multa Pendiente'
        END AS estado_contrato,
        inm.id_Propietario AS inm_id_propietario, inm.direccion AS inm_direccion, inm.cantidad_ambientes AS inm_cantidad_ambientes, inm.descripcion AS inm_descripcion, inm.precio AS inm_precio,
        p.id_propietario AS p_id_propietario, p.dni AS p_dni, p.nombre AS p_nombre, p.apellido AS p_apellido, p.telefono AS p_telefono, p.email AS p_email,
        ti.id_tipo_inmueble AS ti_id_tipo_inmueble, ti.nombre AS ti_nombre,
        inq.id_inquilino AS inq_id_inquilino, inq.dni AS inq_dni, inq.nombre AS inq_nombre, inq.apellido AS inq_apellido, inq.telefono AS inq_telefono, inq.email AS inq_email, inq.direccion AS inq_direccion
      FROM contrato c 
      JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
      JOIN tipo_inmueble ti ON inm.id_tipo_inmueble = ti.id_tipo_inmueble
      JOIN propietario p ON inm.id_propietario = p.id_propietario
      JOIN inquilino inq ON c.id_inquilino = inq.id_inquilino
      WHERE 1=1
    ";

                if (!string.IsNullOrEmpty(idContrato))
                    sql += " AND c.id_contrato LIKE @idContrato";
                if (!string.IsNullOrEmpty(dniInquilino))
                    sql += " AND inq.dni LIKE @dniInquilino";
                if (!string.IsNullOrEmpty(idInmueble))
                    sql += " AND inm.id_inmueble LIKE @idInmueble";
                if (!string.IsNullOrEmpty(Fecha_desde))
                    sql += " AND DATE(c.fecha_desde) >= @fechaDesde";
                if (!string.IsNullOrEmpty(Fecha_hasta))
                    sql += " AND c.fecha_hasta <= @fechaHasta";
                if (!string.IsNullOrEmpty(tipo))
                    sql += " AND c.tipo = @tipo";
                if (!string.IsNullOrEmpty(MontoMenor))
                    sql += " AND c.monto_mensual >= @MontoMenor";
                if (!string.IsNullOrEmpty(MontoMayor))
                    sql += " AND c.monto_mensual <= @MontoMayor";
                if (!string.IsNullOrEmpty(estado))
                {
                    switch (estado.Trim())
                    {
                        case "1": // Vigente
                            sql +=
                                " AND c.fecha_terminacion_anticipada IS NULL AND c.fecha_hasta >= CURDATE()";
                            break;
                        case "2": // Finalizado
                            sql +=
                                " AND c.fecha_terminacion_anticipada IS NULL AND c.fecha_hasta < CURDATE()";
                            break;
                        case "3": // Cancelado (todos los cancelados)
                            sql += " AND c.fecha_terminacion_anticipada IS NOT NULL";
                            break;
                        case "4": // Cancelado con Multa Saldada
                            sql +=
                                " AND c.fecha_terminacion_anticipada IS NOT NULL AND EXISTS(SELECT 1 FROM pago pa WHERE pa.id_contrato = c.id_contrato AND pa.concepto = 'Multa de Cancelacion' AND pa.estado = 1)";
                            break;
                        case "5": // Cancelado con Multa Pendiente
                            sql +=
                                " AND c.fecha_terminacion_anticipada IS NOT NULL AND NOT EXISTS(SELECT 1 FROM pago pa WHERE pa.id_contrato = c.id_contrato AND pa.concepto = 'Multa de Cancelacion' AND pa.estado = 1)";
                            break;
                    }
                }

                sql += " ORDER BY c.id_contrato DESC LIMIT @limite OFFSET @offset";

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
                    else
                        cmd.Parameters.AddWithValue("@estado", DBNull.Value);

                    if (!string.IsNullOrEmpty(Fecha_desde))
                        cmd.Parameters.AddWithValue("@fechaDesde", Fecha_desde);

                    if (!string.IsNullOrEmpty(Fecha_hasta))
                        cmd.Parameters.AddWithValue("@fechaHasta", Fecha_hasta);

                    if (!string.IsNullOrEmpty(tipo))
                        cmd.Parameters.AddWithValue("@tipo", tipo);

                    if (!string.IsNullOrEmpty(MontoMenor))
                        cmd.Parameters.AddWithValue("@MontoMenor", MontoMenor);

                    if (!string.IsNullOrEmpty(MontoMayor))
                        cmd.Parameters.AddWithValue("@MontoMayor", MontoMayor);

                    cmd.Parameters.AddWithValue("@fechaActual", DateTime.Today);
                    cmd.Parameters.AddWithValue("@limite", limite);
                    cmd.Parameters.AddWithValue("@offset", offset);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(
                                new Contrato
                                {
                                    IdContrato = reader.GetInt32("id_Contrato"),
                                    FechaInicio = reader.GetDateTime("fecha_desde"),
                                    FechaFinalizacion = reader.GetDateTime("fecha_hasta"),
                                    FechaCancelacion = reader.IsDBNull(
                                        "fecha_terminacion_anticipada"
                                    )
                                        ? (DateTime?)null
                                        : reader.GetDateTime("fecha_terminacion_anticipada"),
                                    Monto = reader.GetDecimal("monto_mensual"),
                                    Multa = reader.IsDBNull(reader.GetOrdinal("multa"))
                                        ? (decimal?)null
                                        : reader.GetDecimal(reader.GetOrdinal("multa")),
                                    Estado = reader.GetString("estado_contrato"),
                                    Tipo = reader.GetInt32("tipo"),
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
                                            Email = reader.GetString("p_email"),
                                        },
                                        Direccion = reader.GetString("inm_direccion"),
                                        CantidadAmbientes = reader.GetInt32(
                                            "inm_cantidad_ambientes"
                                        ),
                                        Precio = reader.GetDecimal("inm_precio"),
                                        Descripcion = reader.GetString("inm_descripcion"),
                                        IdTipoInmueble = reader.GetInt32("ti_id_tipo_inmueble"),
                                        TipoInmueble = new TipoInmueble
                                        {
                                            Nombre = reader.GetString("ti_nombre"),
                                        },
                                    },
                                    IdInquilino = reader.GetInt32("id_inquilino"),
                                    Inquilino = new Inquilino
                                    {
                                        IdInquilino = reader.GetInt32("inq_id_inquilino"),
                                        Dni = reader.GetString("inq_dni"),
                                        Nombre = reader.GetString("inq_nombre"),
                                        Apellido = reader.GetString("inq_apellido"),
                                        Telefono = reader.GetString("inq_telefono"),
                                        Email = reader.GetString("inq_email"),
                                        Direccion = reader.GetString("inq_direccion"),
                                    },
                                    CreatedAt = reader.GetDateTime("created_at"),
                                    UpdatedAt = reader.GetDateTime("updated_at"),
                                }
                            );
                        }
                    }
                }
            }
            return lista;
        }

        public int validarContratoCancelar(int idContrato, DateTime? fechaCancelar)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"
                  SELECT COUNT(1) AS total
                  FROM contrato
                  WHERE id_contrato = @idContrato
                  AND fecha_hasta > @fechaCancelar;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@idContrato", idContrato);
                    cmd.Parameters.AddWithValue(
                        "@fechaCancelar",
                        fechaCancelar ?? (object)DBNull.Value
                    );
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public int validarFechaMayorMulta(int idContrato, DateTime? fechaCancelar)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"
                  SELECT
                    COUNT(1)
                  FROM contrato
                  WHERE id_contrato = @idContrato
                  AND DATE_ADD(
                    fecha_desde,
                    INTERVAL TIMESTAMPDIFF(SECOND, fecha_desde, fecha_hasta) / 2 SECOND
                  ) < @fechaCancelar;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@idContrato", idContrato);
                    cmd.Parameters.AddWithValue("@fechaCancelar", fechaCancelar);
                    conn.Open();

                    int cont = Convert.ToInt32(cmd.ExecuteScalar());

                    return cont;
                }
            }
        }

        public int CantidadFiltro(
            string? idContrato,
            string? dniInquilino,
            string? idInmueble,
            string? estado,
            string? Fecha_desde,
            string? Fecha_hasta,
            string? tipo,
            string? MontoMenor,
            string? MontoMayor
        )
        {
            int total = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"
      SELECT COUNT(*)
      FROM contrato c
      JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
      JOIN tipo_inmueble ti ON inm.id_tipo_inmueble = ti.id_tipo_inmueble
      JOIN propietario p ON inm.id_propietario = p.id_propietario
      JOIN inquilino inq ON c.id_inquilino = inq.id_inquilino
      WHERE 1=1
    ";
                if (!string.IsNullOrEmpty(idContrato))
                    sql += " AND c.id_contrato LIKE @idContrato";

                if (!string.IsNullOrEmpty(dniInquilino))
                    sql += " AND inq.dni LIKE @dniInquilino";

                if (!string.IsNullOrEmpty(idInmueble))
                    sql += " AND inm.id_inmueble LIKE @idInmueble";

                if (!string.IsNullOrEmpty(Fecha_desde))
                    sql += " AND DATE(c.fecha_desde) >= @fechaDesde";

                if (!string.IsNullOrEmpty(Fecha_hasta))
                    sql += " AND c.fecha_hasta <= @fechaHasta";

                if (!string.IsNullOrEmpty(tipo))
                    sql += " AND c.tipo = @tipo";

                if (!string.IsNullOrEmpty(MontoMenor))
                    sql += " AND c.monto_mensual >= @MontoMenor";

                if (!string.IsNullOrEmpty(MontoMayor))
                    sql += " AND c.monto_mensual <= @MontoMayor";

                if (!string.IsNullOrEmpty(estado))
                {
                    switch (estado.Trim())
                    {
                        case "1": // Vigente
                            sql +=
                                " AND c.fecha_terminacion_anticipada IS NULL AND c.fecha_hasta >= CURDATE()";
                            break;
                        case "2": // Finalizado
                            sql +=
                                " AND c.fecha_terminacion_anticipada IS NULL AND c.fecha_hasta < CURDATE()";
                            break;
                        case "3": // Cancelado (todos los cancelados)
                            sql += " AND c.fecha_terminacion_anticipada IS NOT NULL";
                            break;
                        case "4": // Cancelado con Multa Saldada
                            sql +=
                                " AND c.fecha_terminacion_anticipada IS NOT NULL AND EXISTS (SELECT 1 FROM pago pa WHERE pa.id_contrato = c.id_contrato AND pa.concepto = 'Multa de Cancelacion' AND pa.estado = 1)";
                            break;
                        case "5": // Cancelado con Multa Pendiente
                            sql +=
                                " AND c.fecha_terminacion_anticipada IS NOT NULL AND NOT EXISTS (SELECT 1 FROM pago pa WHERE pa.id_contrato = c.id_contrato AND pa.concepto = 'Multa de Cancelacion' AND pa.estado = 1)";
                            break;
                    }
                }

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (!string.IsNullOrEmpty(idContrato))
                        cmd.Parameters.AddWithValue("@idContrato", "%" + idContrato + "%");

                    if (!string.IsNullOrEmpty(dniInquilino))
                        cmd.Parameters.AddWithValue("@dniInquilino", "%" + dniInquilino + "%");

                    if (!string.IsNullOrEmpty(idInmueble))
                        cmd.Parameters.AddWithValue("@idInmueble", "%" + idInmueble + "%");

                    if (!string.IsNullOrEmpty(Fecha_desde))
                        cmd.Parameters.AddWithValue("@fechaDesde", Fecha_desde);

                    if (!string.IsNullOrEmpty(Fecha_hasta))
                        cmd.Parameters.AddWithValue("@fechaHasta", Fecha_hasta);

                    if (!string.IsNullOrEmpty(tipo))
                        cmd.Parameters.AddWithValue("@tipo", tipo);

                    if (!string.IsNullOrEmpty(MontoMenor))
                        cmd.Parameters.AddWithValue("@MontoMenor", MontoMenor);

                    if (!string.IsNullOrEmpty(MontoMayor))
                        cmd.Parameters.AddWithValue("@MontoMayor", MontoMayor);

                    conn.Open();
                    total = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
                    conn.Close();
                }
            }
            return total;
        }

        public int ValidarSolapamiento(Contrato contrato)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"
            SELECT COUNT(1)
            FROM contrato c
            JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
            WHERE c.id_inmueble = @idInmueble
              AND inm.estado = 1
              AND @fechaInicio <= COALESCE(c.fecha_terminacion_anticipada, c.fecha_hasta)
              AND @fechaFin >= c.fecha_desde ";
                if (contrato.IdContrato != 0)
                    sql += " AND c.id_contrato <> @idContrato";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@idInmueble", contrato.IdInmueble);
                    cmd.Parameters.AddWithValue("@fechaInicio", contrato.FechaInicio);
                    cmd.Parameters.AddWithValue("@fechaFin", contrato.FechaFinalizacion);
                    if (contrato.IdContrato != 0)
                        cmd.Parameters.AddWithValue("@idContrato", contrato.IdContrato);
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public int CrearContratoConPago(Contrato contrato, Pago pago)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var sqlContrato =
                            @"
                    INSERT INTO contrato (id_inquilino, id_inmueble, fecha_desde, fecha_hasta, monto_mensual, tipo, id_usuario_creador,created_at, updated_at) 
                    VALUES (@id_inquilino, @id_inmueble, @fecha_desde, @fecha_hasta, @monto_mensual, @tipo, @id_usuario_creador,current_timestamp(), current_timestamp());
                    SELECT LAST_INSERT_ID();";
                        using (var cmdContrato = new MySqlCommand(sqlContrato, conn, transaction))
                        {
                            cmdContrato.Parameters.AddWithValue(
                                "@id_inquilino",
                                contrato.IdInquilino
                            );
                            cmdContrato.Parameters.AddWithValue(
                                "@id_inmueble",
                                contrato.IdInmueble
                            );
                            cmdContrato.Parameters.AddWithValue(
                                "@fecha_desde",
                                contrato.FechaInicio
                            );
                            cmdContrato.Parameters.AddWithValue(
                                "@fecha_hasta",
                                contrato.FechaFinalizacion
                            );
                            cmdContrato.Parameters.AddWithValue("@monto_mensual", contrato.Monto);
                            cmdContrato.Parameters.AddWithValue("@tipo", contrato.Tipo);
                            cmdContrato.Parameters.AddWithValue("@id_usuario_creador", contrato.IdUsuarioCreador);
                            contrato.IdContrato = Convert.ToInt32(cmdContrato.ExecuteScalar());
                        }

                        pago.IdContrato = contrato.IdContrato;
                        var sqlPago =
                            @"
                    INSERT INTO pago (id_contrato, numero_pago, fecha_pago, concepto, monto, estado, id_usuario,created_at, updated_at) 
                    VALUES (@id_contrato, @numero_pago, @fecha_pago, @concepto, @monto, @estado, @id_usuario, current_timestamp(), current_timestamp());
                    SELECT LAST_INSERT_ID();";
                        using (var cmdPago = new MySqlCommand(sqlPago, conn, transaction))
                        {
                            cmdPago.Parameters.AddWithValue("@id_contrato", pago.IdContrato);
                            cmdPago.Parameters.AddWithValue("@numero_pago", pago.numeroPago);
                            cmdPago.Parameters.AddWithValue("@fecha_pago", pago.FechaPago);
                            cmdPago.Parameters.AddWithValue("@concepto", pago.Concepto);
                            cmdPago.Parameters.AddWithValue("@monto", pago.Monto);
                            cmdPago.Parameters.AddWithValue("@id_usuario", contrato.IdUsuarioCreador);
                            cmdPago.Parameters.AddWithValue("@estado", 1);
                            pago.IdPago = Convert.ToInt32(cmdPago.ExecuteScalar());
                        }
                        transaction.Commit();
                        return contrato.IdContrato;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
            }
        }

        public List<Contrato> FechasOcupadas(int idInmueble, string? idContrato)
        {
            var contratos = new List<Contrato>();

            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"
            SELECT 
                c.fecha_desde,
                CASE 
                    WHEN c.fecha_terminacion_anticipada IS NOT NULL 
                        THEN c.fecha_terminacion_anticipada
                    ELSE c.fecha_hasta
                END AS fecha_hasta
            FROM contrato c
            JOIN inmueble inm ON c.id_inmueble = inm.id_inmueble
            WHERE c.id_inmueble = @idInmueble
              AND c.fecha_hasta > @fecha
              AND inm.estado = 1 ";

                if (!string.IsNullOrEmpty(idContrato))
                    sql += " AND c.id_contrato <> @idContrato";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@idInmueble", idInmueble);
                    if (!string.IsNullOrEmpty(idContrato))
                        cmd.Parameters.AddWithValue("@idContrato", idContrato);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Today.AddMonths(-1));

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contratos.Add(
                                new Contrato
                                {
                                    FechaInicio = reader.GetDateTime("fecha_desde"),
                                    FechaFinalizacion = reader.GetDateTime("fecha_hasta"),
                                }
                            );
                        }
                    }
                }
            }
            return contratos;
        }

        public int EliminarContratoConPagoS(int idContrato)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    int res = -1;
                    try
                    {
                        var sqlPago = @"DELETE FROM pago WHERE id_contrato = @id_contrato;";
                        using (var cmdPago = new MySqlCommand(sqlPago, conn, transaction))
                        {
                            cmdPago.Parameters.AddWithValue("@id_contrato", idContrato);
                            cmdPago.ExecuteNonQuery();
                        }

                        var sqlContrato = @"DELETE FROM contrato WHERE id_contrato = @id_contrato;";
                        using (var cmdContrato = new MySqlCommand(sqlContrato, conn, transaction))
                        {
                            cmdContrato.Parameters.AddWithValue("@id_contrato", idContrato);
                            res = cmdContrato.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        return res;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return -1;
                    }
                }
            }
        }

        public int CancelarContratoConPago(Contrato contrato, Pago pago)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var sqlContrato =
                            @"UPDATE contrato SET fecha_terminacion_anticipada=@fecha_fin, multa=@multa, id_usuario_finalizador=@id_usuario_finalizador, updated_at=NOW() WHERE id_contrato=@id_contrato";
                        using (var cmdContrato = new MySqlCommand(sqlContrato, conn, transaction))
                        {
                            cmdContrato.Parameters.AddWithValue("@fecha_fin", DateTime.Today);
                            cmdContrato.Parameters.AddWithValue("@multa", contrato.Multa);
                            cmdContrato.Parameters.AddWithValue("@id_usuario_finalizador", contrato.IdUsuarioFinalizador);
                            cmdContrato.Parameters.AddWithValue("@id_contrato",contrato.IdContrato);
                            cmdContrato.ExecuteNonQuery();
                        }

                        pago.IdContrato = contrato.IdContrato;
                        var sqlPago =
                            @"
                    INSERT INTO pago (id_contrato, numero_pago, fecha_pago, concepto, monto, id_usuario, estado, created_at, updated_at) 
                    VALUES (@id_contrato, @numero_pago, @fecha_pago, @concepto, @monto, @id_usuario, @estado, current_timestamp(), current_timestamp());
                    SELECT LAST_INSERT_ID();";
                        using (var cmdPago = new MySqlCommand(sqlPago, conn, transaction))
                        {
                            cmdPago.Parameters.AddWithValue("@id_contrato", pago.IdContrato);
                            cmdPago.Parameters.AddWithValue("@numero_pago", pago.numeroPago);
                            cmdPago.Parameters.AddWithValue("@fecha_pago", pago.FechaPago);
                            cmdPago.Parameters.AddWithValue("@concepto", pago.Concepto);
                            cmdPago.Parameters.AddWithValue("@monto", pago.Monto);
                            cmdPago.Parameters.AddWithValue("@id_usuario", contrato.IdUsuarioFinalizador);
                            cmdPago.Parameters.AddWithValue("@estado", 1);
                            pago.IdPago = Convert.ToInt32(cmdPago.ExecuteScalar());
                        }
                        transaction.Commit();
                        return contrato.IdContrato;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
            }
        }

        public List<Contrato> Calendario(int year, int idInmueble)
        {
            var contratos = new List<Contrato>();

            using (var conn = new MySqlConnection(connectionString))
            {
                string sql =
                    @"SELECT *
                      FROM contrato
                      WHERE id_inmueble = @id_inmueble
                      AND fecha_desde <= CONCAT(@year, '-12-31')
                      AND (fecha_hasta >= CONCAT(@year, '-01-01')
                      OR fecha_hasta IS NULL);";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id_inmueble", idInmueble);
                    cmd.Parameters.AddWithValue("@year", year);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contratos.Add(
                                new Contrato
                                {
                                    IdContrato = Convert.ToInt32(reader["id_contrato"]),
                                    FechaInicio = Convert.ToDateTime(reader["fecha_desde"]),
                                    FechaFinalizacion = Convert.ToDateTime(reader["fecha_hasta"]),
                                    FechaCancelacion =
                                        reader["fecha_terminacion_anticipada"] != DBNull.Value
                                            ? (DateTime?)reader["fecha_terminacion_anticipada"]
                                            : null,
                                }
                            );
                        }
                    }
                    conn.Close();
                }
            }

            return contratos;
        }
    }
}
