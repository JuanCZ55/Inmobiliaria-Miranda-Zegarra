using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Inmobiliaria.Models
{
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration)
            : base(configuration) { }

        public int Crear(Propietario propietario)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"INSERT INTO propietario (dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at) VALUES (@dni, @nombre, @apellido, @telefono, @email, @direccion, @estado, @created_at, @updated_at ); SELECT LAST_INSERT_ID();";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dni", propietario.Dni);
                    cmd.Parameters.AddWithValue("@nombre", propietario.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", propietario.Apellido);
                    cmd.Parameters.AddWithValue("@telefono", propietario.Telefono);
                    cmd.Parameters.AddWithValue("@email", propietario.Email);
                    cmd.Parameters.AddWithValue("@direccion", propietario.Direccion);
                    cmd.Parameters.AddWithValue("@estado", propietario.Estado);
                    cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                    cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);
                    conn.Open();
                    res = Convert.ToInt32(cmd.ExecuteScalar());
                    propietario.IdPropietario = res;
                    conn.Close();
                }
            }
            return res;
        }

        public int Modificar(Propietario propietario)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"UPDATE propietario SET dni=@dni, nombre=@nombre, apellido=@apellido, telefono=@telefono, email=@email, direccion=@direccion, estado=@estado, updated_at=NOW() WHERE id_propietario=@id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dni", propietario.Dni);
                    cmd.Parameters.AddWithValue("@nombre", propietario.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", propietario.Apellido);
                    cmd.Parameters.AddWithValue("@telefono", propietario.Telefono);
                    cmd.Parameters.AddWithValue("@email", propietario.Email);
                    cmd.Parameters.AddWithValue("@direccion", propietario.Direccion);
                    cmd.Parameters.AddWithValue("@estado", propietario.Estado);
                    cmd.Parameters.AddWithValue("@id", propietario.IdPropietario);
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
                var sql = @"DELETE FROM propietario WHERE id_propietario=@id";
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

        public Propietario ObtenerPorID(int id)
        {
            Propietario p = new Propietario();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"SELECT id_propietario, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at FROM propietario WHERE id_propietario = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p.IdPropietario = reader.GetInt32("id_propietario");
                            p.Dni = reader.GetString("dni");
                            p.Nombre = reader.GetString("nombre");
                            p.Apellido = reader.GetString("apellido");
                            p.Telefono = reader.GetString("telefono");
                            p.Email = reader.GetString("email");
                            p.Direccion = reader.GetString("direccion");
                            p.Estado = reader.GetInt32("estado");
                            p.CreatedAt = reader.GetDateTime("created_at");
                            p.UpdatedAt = reader.GetDateTime("updated_at");
                        }
                    }
                    conn.Close();
                }
            }
            return p;
        }

        public Propietario ObtenerPorDni(string dni)
        {
            Propietario p = new Propietario();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"SELECT id_propietario, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at FROM propietario WHERE dni = @dni";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dni", dni);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p.IdPropietario = reader.GetInt32("id_propietario");
                            p.Dni = reader.GetString("dni");
                            p.Nombre = reader.GetString("nombre");
                            p.Apellido = reader.GetString("apellido");
                            p.Telefono = reader.GetString("telefono");
                            p.Email = reader.GetString("email");
                            p.Direccion = reader.GetString("direccion");
                            p.Estado = reader.GetInt32("estado");
                            p.CreatedAt = reader.GetDateTime("created_at");
                            p.UpdatedAt = reader.GetDateTime("updated_at");
                        }
                    }
                    conn.Close();
                }
            }
            return p;
        }

        public List<Propietario> ObtenerTodos()
        {
            var lista = new List<Propietario>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"SELECT id_propietario, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at FROM propietario ORDER BY id_propietario DESC;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(
                                new Propietario
                                {
                                    IdPropietario = reader.GetInt32("id_propietario"),
                                    Dni = reader.GetString("dni"),
                                    Nombre = reader.GetString("nombre"),
                                    Apellido = reader.GetString("apellido"),
                                    Telefono = reader.GetString("telefono"),
                                    Email = reader.GetString("email"),
                                    Direccion = reader.GetString("direccion"),
                                    Estado = reader.GetInt32("estado"),
                                    CreatedAt = reader.GetDateTime("created_at"),
                                    UpdatedAt = reader.GetDateTime("updated_at"),
                                }
                            );
                        }
                    }
                    conn.Close();
                }
            }
            return lista;
        }

        public int ContarFiltro(string dni)
        {
            int count = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT COUNT(*) FROM propietario WHERE 1=1 ";
                if (!string.IsNullOrEmpty(dni))
                {
                    sql += "AND dni LIKE @dni ";
                }
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (!string.IsNullOrEmpty(dni))
                    {
                        cmd.Parameters.AddWithValue("@dni", dni + "%");
                    }
                    conn.Open();
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                    conn.Close();
                }
            }
            return count;
        }

        public List<Propietario> Filtro(string dni, int limit, int offset)
        {
            var lista = new List<Propietario>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"SELECT id_propietario, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at FROM propietario WHERE 1=1 ";
                if (!string.IsNullOrEmpty(dni))
                {
                    sql += "AND dni LIKE @dni ORDER BY dni ";
                }
                else
                {
                    sql += "ORDER BY id_propietario DESC ";
                }
                sql += "LIMIT @limit OFFSET @offset;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (!string.IsNullOrEmpty(dni))
                    {
                        cmd.Parameters.AddWithValue("@dni", dni + "%");
                    }

                    cmd.Parameters.AddWithValue("@limit", limit);
                    cmd.Parameters.AddWithValue("@offset", offset);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(
                                new Propietario
                                {
                                    IdPropietario = reader.GetInt32("id_propietario"),
                                    Dni = reader.GetString("dni"),
                                    Nombre = reader.GetString("nombre"),
                                    Apellido = reader.GetString("apellido"),
                                    Telefono = reader.GetString("telefono"),
                                    Email = reader.GetString("email"),
                                    Direccion = reader.GetString("direccion"),
                                    Estado = reader.GetInt32("estado"),
                                    CreatedAt = reader.GetDateTime("created_at"),
                                    UpdatedAt = reader.GetDateTime("updated_at"),
                                }
                            );
                        }
                    }
                    conn.Close();
                }
            }
            return lista;
        }
    }
}
