using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;

class Program
{
    public static string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;TrustServerCertificate=True;";
    static int ID_UsuarioLogueado;

    static void Main(string[] args)
    {
        if (!InicioSesion())
        {
            Console.WriteLine("No se pudo iniciar sesión. Presione cualquier tecla para salir...");
            Console.ReadKey();
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("========== Menú Principal ==========");
            Console.WriteLine("1. Gestionar Usuarios");
            Console.WriteLine("2. Gestionar Proveedores");
            Console.WriteLine("3. Salir");
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    GestionarUsuarios();
                    break;
                case "2":
                    GestionarProveedores();
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static bool InicioSesion()
    {
        Console.Clear();
        Console.WriteLine("===== Inicio de Sesión =====");
        Console.Write("Usuario: ");
        string usuario = Console.ReadLine();
        Console.Write("Contraseña: ");
        string contraseña = LeerContraseña();

        byte[] contraseñaCifrada = CifrarSHA256(contraseña);

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = "SELECT ID_Usuario FROM Usuario WHERE Nombre = @Nombre AND Contraseña = @Contraseña AND Estatus = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = usuario;
                    cmd.Parameters.Add("@Contraseña", SqlDbType.VarBinary).Value = contraseñaCifrada;

                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        ID_UsuarioLogueado = (int)result;
                        Console.WriteLine("Inicio de sesión exitoso. Presione una tecla para continuar...");
                        Console.ReadKey();
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Credenciales incorrectas. Presione una tecla para intentar nuevamente...");
                        Console.ReadKey();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al conectar con la base de datos: " + ex.Message);
                Console.ReadKey();
                return false;
            }
        }
    }

    static string LeerContraseña()
    {
        StringBuilder contraseña = new StringBuilder();
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(intercept: true);
            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                contraseña.Append(key.KeyChar);
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && contraseña.Length > 0)
            {
                contraseña.Remove(contraseña.Length - 1, 1);
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return contraseña.ToString();
    }

    static byte[] CifrarSHA256(string contrasena)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(contrasena);
            return sha256Hash.ComputeHash(bytes);
        }
    }

 

    static void GestionarUsuarios()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== Gestión de Usuarios =====");
            Console.WriteLine("1. Agregar Usuario");
            Console.WriteLine("2. Modificar Usuario");
            Console.WriteLine("3. Eliminar Usuario");
            Console.WriteLine("4. Ver Usuarios");
            Console.WriteLine("5. Volver");
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    AgregarUsuario();
                    break;
                case "2":
                    ModificarUsuario();
                    break;
                case "3":
                    EliminarUsuario();
                    break;
                case "4":
                    VerUsuarios();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void AgregarUsuario()
    {
        Console.Clear();
        Console.WriteLine("===== Agregar Usuario =====");
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Apellido: ");
        string apellido = Console.ReadLine();
        Console.Write("Correo Electrónico: ");
        string correo = Console.ReadLine();
        Console.Write("Contraseña: ");
        string contraseña = LeerContraseña();
        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Dirección: ");
        string direccion = Console.ReadLine();
        Console.Write("Tipo de Usuario: ");
        string tipoUsuario = Console.ReadLine();
        DateTime fechaRegistro = DateTime.Now;

       
        byte[] contraseñaCifrada = CifrarSHA256(contraseña);

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = "INSERT INTO Usuario (Nombre, Apellido, CorreoElectronico, Contraseña, Telefono, Direccion, FechaRegistro, TipoUsuario, Estatus) " +
                               "VALUES (@Nombre, @Apellido, @CorreoElectronico, @Contraseña, @Telefono, @Direccion, @FechaRegistro, @TipoUsuario, 1)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Apellido", apellido);
                    cmd.Parameters.AddWithValue("@CorreoElectronico", correo);
                    cmd.Parameters.AddWithValue("@Contraseña", contraseñaCifrada);
                    cmd.Parameters.AddWithValue("@Telefono", telefono);
                    cmd.Parameters.AddWithValue("@Direccion", direccion);
                    cmd.Parameters.AddWithValue("@FechaRegistro", fechaRegistro);
                    cmd.Parameters.AddWithValue("@TipoUsuario", tipoUsuario);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Usuario agregado exitosamente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar usuario: " + ex.Message);
            }
        }
        Console.ReadKey();
    }

    static void ModificarUsuario()
    {
        Console.Clear();
        Console.WriteLine("===== Modificar Usuario =====");
        Console.Write("ID del Usuario a modificar: ");
        int id = int.Parse(Console.ReadLine());
        Console.Write("Nuevo Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Nuevo Apellido: ");
        string apellido = Console.ReadLine();
        Console.Write("Nuevo Correo Electrónico: ");
        string correo = Console.ReadLine();
        Console.Write("Nueva Contraseña: ");
        string contraseña = LeerContraseña();
        Console.Write("Nuevo Teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Nueva Dirección: ");
        string direccion = Console.ReadLine();
        Console.Write("Nuevo Tipo de Usuario: ");
        string tipoUsuario = Console.ReadLine();

       
        byte[] contraseñaCifrada = CifrarSHA256(contraseña);

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = "UPDATE Usuario SET Nombre = @Nombre, Apellido = @Apellido, CorreoElectronico = @CorreoElectronico, Contraseña = @Contraseña, " +
                               "Telefono = @Telefono, Direccion = @Direccion, TipoUsuario = @TipoUsuario WHERE ID_Usuario = @ID AND Estatus = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Apellido", apellido);
                    cmd.Parameters.AddWithValue("@CorreoElectronico", correo);
                    cmd.Parameters.AddWithValue("@Contraseña", contraseñaCifrada);
                    cmd.Parameters.AddWithValue("@Telefono", telefono);
                    cmd.Parameters.AddWithValue("@Direccion", direccion);
                    cmd.Parameters.AddWithValue("@TipoUsuario", tipoUsuario);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Usuario modificado exitosamente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al modificar usuario: " + ex.Message);
            }
        }
        Console.ReadKey();
    }

    static void EliminarUsuario()
    {
        Console.Clear();
        Console.WriteLine("===== Eliminar Usuario =====");
        Console.Write("ID del Usuario a eliminar: ");
        int id = int.Parse(Console.ReadLine());

        Console.WriteLine("¿Qué desea hacer con este usuario?");
        Console.WriteLine("1. Desactivar (estatus inactivo)");
        Console.WriteLine("2. Eliminar permanentemente");
        Console.Write("Seleccione una opción: ");
        string opcion = Console.ReadLine();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = opcion == "1"
                    ? "UPDATE Usuario SET Estatus = 0 WHERE ID_Usuario = @ID"
                    : "DELETE FROM Usuario WHERE ID_Usuario = @ID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();

                    string accion = opcion == "1" ? "desactivado" : "eliminado permanentemente";
                    Console.WriteLine($"Usuario {accion} exitosamente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar usuario: " + ex.Message);
            }
        }
        Console.ReadKey();
    }

    static void VerUsuarios()
    {
        Console.Clear();
        Console.WriteLine("===== Ver Usuarios =====");
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = "SELECT ID_Usuario, Nombre, Apellido, CorreoElectronico, Telefono, Direccion, FechaRegistro, TipoUsuario, Estatus FROM Usuario";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string estatus = (bool)reader["Estatus"] ? "Activo" : "Inactivo";
                            Console.WriteLine($"ID: {reader["ID_Usuario"]}, Nombre: {reader["Nombre"]} {reader["Apellido"]}, Correo: {reader["CorreoElectronico"]}, " +
                                              $"Teléfono: {reader["Telefono"]}, Dirección: {reader["Direccion"]}, Fecha de Registro: {reader["FechaRegistro"]}, " +
                                              $"Tipo: {reader["TipoUsuario"]}, Estatus: {estatus}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al mostrar usuarios: " + ex.Message);
            }
        }
        Console.ReadKey();
    }


 
    static void GestionarProveedores()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== Gestión de Proveedores =====");
            Console.WriteLine("1. Agregar Proveedor");
            Console.WriteLine("2. Modificar Proveedor");
            Console.WriteLine("3. Eliminar Proveedor");
            Console.WriteLine("4. Ver Proveedores");
            Console.WriteLine("5. Volver");
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    AgregarProveedor();
                    break;
                case "2":
                    ModificarProveedor();
                    break;
                case "3":
                    EliminarProveedor();
                    break;
                case "4":
                    VerProveedores();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void AgregarProveedor()
    {
        Console.Clear();
        Console.WriteLine("===== Agregar Proveedor =====");
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Correo Electrónico: ");
        string correo = Console.ReadLine();
        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Dirección: ");
        string direccion = Console.ReadLine();
        Console.Write("Tipo de Proveedor: ");
        string tipoProveedor = Console.ReadLine();
        DateTime fechaRegistro = DateTime.Now;

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = "INSERT INTO Proveedor (Nombre, CorreoElectronico, Telefono, Direccion, TipoProveedor, FechaRegistro, Estatus, ID_Usuario) " +
                               "VALUES (@Nombre, @CorreoElectronico, @Telefono, @Direccion, @TipoProveedor, @FechaRegistro, 1, @ID_Usuario)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@CorreoElectronico", correo);
                    cmd.Parameters.AddWithValue("@Telefono", telefono);
                    cmd.Parameters.AddWithValue("@Direccion", direccion);
                    cmd.Parameters.AddWithValue("@TipoProveedor", tipoProveedor);
                    cmd.Parameters.AddWithValue("@FechaRegistro", fechaRegistro);
                    cmd.Parameters.AddWithValue("@ID_Usuario", ID_UsuarioLogueado); 
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Proveedor agregado exitosamente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar proveedor: " + ex.Message);
            }
        }
        Console.ReadKey();
    }

    static void ModificarProveedor()
    {
        Console.Clear();
        Console.WriteLine("===== Modificar Proveedor =====");
        Console.Write("ID del Proveedor a modificar: ");
        int id = int.Parse(Console.ReadLine());
        Console.Write("Nuevo Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Nuevo Correo Electrónico: ");
        string correo = Console.ReadLine();
        Console.Write("Nuevo Teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Nueva Dirección: ");
        string direccion = Console.ReadLine();
        Console.Write("Nuevo Tipo de Proveedor: ");
        string tipoProveedor = Console.ReadLine();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = "UPDATE Proveedor SET Nombre = @Nombre, CorreoElectronico = @CorreoElectronico, Telefono = @Telefono, Direccion = @Direccion, " +
                               "TipoProveedor = @TipoProveedor, ID_Usuario = @ID_Usuario WHERE ID_Proveedor = @ID AND Estatus = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@CorreoElectronico", correo);
                    cmd.Parameters.AddWithValue("@Telefono", telefono);
                    cmd.Parameters.AddWithValue("@Direccion", direccion);
                    cmd.Parameters.AddWithValue("@TipoProveedor", tipoProveedor);
                    cmd.Parameters.AddWithValue("@ID_Usuario", ID_UsuarioLogueado);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Proveedor modificado exitosamente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al modificar proveedor: " + ex.Message);
            }
        }
        Console.ReadKey();
    }

    static void EliminarProveedor()
    {
        Console.Clear();
        Console.WriteLine("===== Eliminar Proveedor =====");
        Console.Write("ID del Proveedor a eliminar: ");
        int id = int.Parse(Console.ReadLine());

        Console.WriteLine("¿Qué desea hacer con este proveedor?");
        Console.WriteLine("1. Desactivar (estatus inactivo)");
        Console.WriteLine("2. Eliminar permanentemente");
        Console.Write("Seleccione una opción: ");
        string opcion = Console.ReadLine();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = opcion == "1"
                    ? "UPDATE Proveedor SET Estatus = 0 WHERE ID_Proveedor = @ID"
                    : "DELETE FROM Proveedor WHERE ID_Proveedor = @ID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();

                    string accion = opcion == "1" ? "desactivado" : "eliminado permanentemente";
                    Console.WriteLine($"Proveedor {accion} exitosamente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar proveedor: " + ex.Message);
            }
        }
        Console.ReadKey();
    }

    static void VerProveedores()
    {
        Console.Clear();
        Console.WriteLine("===== Ver Proveedores =====");
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = "SELECT ID_Proveedor, Nombre, CorreoElectronico, Telefono, Direccion, TipoProveedor, FechaRegistro, Estatus, ID_Usuario " +
                               "FROM Proveedor";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string estatus = (bool)reader["Estatus"] ? "Activo" : "Inactivo";
                            Console.WriteLine($"ID: {reader["ID_Proveedor"]}, Nombre: {reader["Nombre"]}, Correo: {reader["CorreoElectronico"]}, Teléfono: {reader["Telefono"]}, " +
                                              $"Dirección: {reader["Direccion"]}, Tipo: {reader["TipoProveedor"]}, Fecha de Registro: {reader["FechaRegistro"]}, Estatus: {estatus}, " +
                                              $"Usuario que registró/modificó: {reader["ID_Usuario"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al mostrar proveedores: " + ex.Message);
            }
        }
        Console.ReadKey();
    }


}
