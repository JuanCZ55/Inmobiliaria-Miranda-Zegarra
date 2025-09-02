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

        public int Alta(string dni)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE propietario SET estado=1, updated_at=NOW() WHERE dni=@dni";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dni", dni);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public int Baja(string dni)
        {
            int res = -1;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE propietario SET estado=0, updated_at=NOW() WHERE dni=@dni";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dni", dni);
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

        public Propietario ObtenerPorDni(string dni)
        {
            Propietario? propietario = null;
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
                            propietario = new Propietario
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
                            };
                        }
                    }
                    conn.Close();
                }
            }
            return propietario;
        }

        public List<Propietario> filtrarDNI(string dni)
        {
            List<Propietario> lista = new List<Propietario>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"SELECT id_propietario, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at FROM propietario WHERE dni LIKE CONCAT(@dni, '%')";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dni", dni);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Propietario propietario = new Propietario
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
                            };
                            lista.Add(propietario);
                        }
                    }
                    conn.Close();
                }
            }
            return lista;
        }

        public List<Propietario> BuscarPorNombre(string nombre)
        {
            var lista = new List<Propietario>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"SELECT id_propietario, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at FROM propietario WHERE nombre LIKE @nombre";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", $"%{nombre}%");
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

        public Propietario ObtenerPorID(int id)
        {
            Propietario propietario = null;
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
                            propietario = new Propietario
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
                            };
                        }
                    }
                    conn.Close();
                }
            }
            return propietario;
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

        public int ContarTodos()
        {
            int total = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql = "SELECT COUNT(*) FROM propietario";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    conn.Open();
                    total = Convert.ToInt32(cmd.ExecuteScalar());
                    conn.Close();
                }
            }
            return total;
        }

        public List<Propietario> ObtenerTodosPaginado(int offset, int limite)
        {
            var lista = new List<Propietario>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"SELECT id_propietario, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at
                    FROM propietario
                    ORDER BY nombre ASC
                    LIMIT @limite OFFSET @offset";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@limite", limite);
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

        public int ContarPorNombre(string nombre)
        {
            int total = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"SELECT COUNT(*) FROM propietario 
                    WHERE (@nombre IS NULL OR nombre LIKE @nombreFiltro)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@nombre",
                        string.IsNullOrEmpty(nombre) ? DBNull.Value : nombre
                    );
                    cmd.Parameters.AddWithValue("@nombreFiltro", $"%{nombre}%");
                    conn.Open();
                    total = Convert.ToInt32(cmd.ExecuteScalar());
                    conn.Close();
                }
            }
            return total;
        }

        public List<Propietario> ObtenerPaginado(string nombre, int offset, int limite)
        {
            var lista = new List<Propietario>();
            using (var conn = new MySqlConnection(connectionString))
            {
                var sql =
                    @"SELECT id_propietario, dni, nombre, apellido, telefono, email, direccion, estado, created_at, updated_at
                    FROM propietario
                    WHERE (@nombre IS NULL OR nombre LIKE @nombreFiltro)
                    ORDER BY nombre ASC
                    LIMIT @limite OFFSET @offset";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@nombre",
                        string.IsNullOrEmpty(nombre) ? DBNull.Value : nombre
                    );
                    cmd.Parameters.AddWithValue("@nombreFiltro", $"%{nombre}%");
                    cmd.Parameters.AddWithValue("@limite", limite);
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
