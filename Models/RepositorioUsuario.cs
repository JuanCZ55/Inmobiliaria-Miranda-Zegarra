namespace Inmobiliaria.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using MySql.Data.MySqlClient;

    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
    {
        public RepositorioUsuario(IConfiguration configuration)
            : base(configuration) { }

        public int Registrar(Usuario usuario)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"
                    INSERT INTO usuario (nombre, apellido, genero, email, contrasena, rol, url, estado, created_at, updated_at)
                    VALUES (@nombre, @apellido, @genero, @email, @contrasena, @rol, @url, @estado, NOW(), NOW());
                    SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", usuario.Apellido);
                    cmd.Parameters.AddWithValue("@genero", usuario.Genero);
                    cmd.Parameters.AddWithValue("@email", usuario.Email);
                    cmd.Parameters.AddWithValue("@contrasena", usuario.Password);
                    cmd.Parameters.AddWithValue("@rol", usuario.Rol);

                    cmd.Parameters.AddWithValue("@url", usuario.AvatarURL ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@estado", usuario.Estado);

                    conn.Open();
                    res = Convert.ToInt32(cmd.ExecuteScalar());
                    usuario.IdUsuario = res;
                }
            }
            return res;
        }

        public Usuario ObtenerPorId(int idUsuario)
        {
            Usuario usuario = new Usuario();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"
                    SELECT id_usuario, nombre, apellido, genero, email, contrasena, rol,
                          url, estado, created_at, updated_at
                    FROM usuario
                    WHERE id_usuario = @Id;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", idUsuario);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = MapUsuario(reader);
                        }
                    }
                    conn.Close();
                }
            }
            return usuario;
        }

        public Usuario ObtenerPorEmail(string email)
        {
            Usuario usuario = new Usuario();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"SELECT id_usuario, nombre, apellido, genero, email, contrasena, rol, url, estado, created_at, updated_at
                    FROM usuario 
                    WHERE email = @Email;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = MapUsuario(reader);
                        }
                    }
                }
            }
            return usuario;
        }

        public int Modificar(Usuario usuario)
        {
            int res = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sqlBuilder = new System.Text.StringBuilder(
                    @"UPDATE usuario
            SET 
                nombre = @nombre,
                apellido = @apellido,
                email = @email,
                genero = @genero,
                rol = @rol,
                url = @url,
                estado = @estado,
                updated_at = NOW()"
                );

                if (!string.IsNullOrEmpty(usuario.Password))
                {
                    sqlBuilder.Append(", contrasena = @contrasena ");
                }

                sqlBuilder.Append(" WHERE id_usuario = @id;");

                using (var cmd = new MySqlCommand(sqlBuilder.ToString(), conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", usuario.Apellido);
                    cmd.Parameters.AddWithValue("@email", usuario.Email);
                    cmd.Parameters.AddWithValue("@genero", usuario.Genero);
                    cmd.Parameters.AddWithValue("@rol", usuario.Rol);

                    if (!string.IsNullOrEmpty(usuario.Password))
                    {
                        cmd.Parameters.AddWithValue("@contrasena", usuario.Password);
                    }

                    cmd.Parameters.AddWithValue(
                        "@url",
                        !string.IsNullOrEmpty(usuario.AvatarURL)
                            ? (object)usuario.AvatarURL
                            : DBNull.Value
                    );
                    cmd.Parameters.AddWithValue("@estado", usuario.Estado);
                    cmd.Parameters.AddWithValue("@id", usuario.IdUsuario);

                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        private Usuario MapUsuario(MySqlDataReader reader)
        {
            return new Usuario
            {
                IdUsuario = reader.GetInt32("id_usuario"),
                Nombre = reader.GetString("nombre"),
                Apellido = reader.GetString("apellido"),
                Genero = reader.GetInt32("genero"),
                Email = reader.GetString("email"),
                Password = reader.GetString("contrasena"),
                Rol = reader.GetInt32("rol"),
                AvatarURL = reader.IsDBNull(reader.GetOrdinal("url"))
                    ? ""
                    : reader.GetString("url"),
                Estado = reader.GetInt32("estado"),
                CreatedAt = reader.GetDateTime("created_at"),
                UpdatedAt = reader.GetDateTime("updated_at"),
            };
        }

        public Usuario? Autenticar(string email)
        {
            throw new NotImplementedException();
        }

        public int CantidadFiltro(
            string? idUsuario,
            string? nombre,
            string? apellido,
            string? email,
            string? rol,
            string? estado
        )
        {
            int total = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = new System.Text.StringBuilder("SELECT COUNT(*) FROM usuario WHERE 1=1 ");
                var parameters = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(idUsuario) && int.TryParse(idUsuario, out int id))
                {
                    sql.Append("AND id_usuario = @idUsuario ");
                    parameters.Add("@idUsuario", id);
                }
                if (!string.IsNullOrEmpty(nombre))
                {
                    sql.Append("AND nombre LIKE @nombre ");
                    parameters.Add("@nombre", $"%{nombre}%");
                }
                if (!string.IsNullOrEmpty(apellido))
                {
                    sql.Append("AND apellido LIKE @apellido ");
                    parameters.Add("@apellido", $"%{apellido}%");
                }
                if (!string.IsNullOrEmpty(email))
                {
                    sql.Append("AND email LIKE @email ");
                    parameters.Add("@email", $"%{email}%");
                }
                if (!string.IsNullOrEmpty(rol))
                {
                    sql.Append("AND rol = @rol ");
                    parameters.Add("@rol", rol == "Administrador" ? 1 : 2);
                }
                if (!string.IsNullOrEmpty(estado) && int.TryParse(estado, out int est))
                {
                    sql.Append("AND estado = @estado ");
                    parameters.Add("@estado", est);
                }

                using (var cmd = new MySqlCommand(sql.ToString(), conn))
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                    conn.Open();
                    total = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return total;
        }

        public List<Usuario> Filtrar(
            string? idUsuario,
            string? nombre,
            string? apellido,
            string? email,
            string? rol,
            string? estado,
            int? limit,
            int? offset
        )
        {
            var lista = new List<Usuario>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = new System.Text.StringBuilder(
                    @"
                    SELECT id_usuario, nombre, apellido, genero, email, contrasena, rol,
                           url, estado, created_at, updated_at
                    FROM usuario
                    WHERE 1=1 "
                );

                var parameters = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(idUsuario) && int.TryParse(idUsuario, out int id))
                {
                    sql.Append("AND id_usuario = @idUsuario ");
                    parameters.Add("@idUsuario", id);
                }
                if (!string.IsNullOrEmpty(nombre))
                {
                    sql.Append("AND nombre LIKE @nombre ");
                    parameters.Add("@nombre", $"%{nombre}%");
                }
                if (!string.IsNullOrEmpty(apellido))
                {
                    sql.Append("AND apellido LIKE @apellido ");
                    parameters.Add("@apellido", $"%{apellido}%");
                }
                if (!string.IsNullOrEmpty(email))
                {
                    sql.Append("AND email LIKE @email ");
                    parameters.Add("@email", $"%{email}%");
                }
                if (!string.IsNullOrEmpty(rol))
                {
                    sql.Append("AND rol = @rol ");
                    parameters.Add("@rol", rol == "Administrador" ? 1 : 2);
                }
                if (!string.IsNullOrEmpty(estado) && int.TryParse(estado, out int est))
                {
                    sql.Append("AND estado = @estado ");
                    parameters.Add("@estado", est);
                }

                sql.Append("ORDER BY id_usuario DESC ");

                if (limit.HasValue)
                {
                    sql.Append("LIMIT @limit ");
                    parameters.Add("@limit", limit.Value);
                }
                if (offset.HasValue)
                {
                    sql.Append("OFFSET @offset ");
                    parameters.Add("@offset", offset.Value);
                }

                using (var cmd = new MySqlCommand(sql.ToString(), conn))
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapUsuario(reader));
                        }
                    }
                    conn.Close();
                }
            }
            return lista;
        }
    }
}
