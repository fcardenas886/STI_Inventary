using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace STI_Inventary
{
    public static class BaseDatos
    {
        static string cadenaConexion = @"data source=45.236.164.148,4994;initial catalog=LOCAL_MUNDIAL_SOS;user id=fcardenas;password=fcardenas; Connect Timeout=60";

        public static Producto ObtenerProd(String b)
        {
            List<Producto> listaEmpleados = new List<Producto>();
            string sql = "SELECT id_mprod,id_mcodbarra,descprod FROM vista_mproducto_mcodbarra where id_mcodbarra=@barra";

            using (SqlConnection con = new SqlConnection(cadenaConexion))
            {
                con.Open();

                using (SqlCommand comando = new SqlCommand(sql, con))
                {
                    comando.Parameters.AddWithValue("@barra",b);
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Producto prod = new Producto()
                            {
                                Id = reader.GetInt32(0),
                                Barra = reader.GetString(1),
                                Nombre = reader.GetString(2)
                            };

                            return prod;
                        }
                    }
                }

                con.Close();
                Producto p = new Producto()
                {

                    Id = 0,
                    Barra="0",
                    Nombre="No existe"
                    
                };
                   
                return p;
                
            }
        }

        public static Inventario ObtenerInvProd(int id, string u)
        {
            string sql = "SELECT TOP(1) id_prod, id_mcodbarra, descripcion, ubicacion, cantidad FROM inventario WHERE id_prod = @id and ubicacion=@u";
            Console.WriteLine($"Cadena de conexión: {cadenaConexion}");
            Console.WriteLine($"Valor del parámetro id: {id}");

            try
            {
                using (SqlConnection con = new SqlConnection(cadenaConexion))
                {
                    con.Open();
                    Console.WriteLine("Conexión abierta.");

                    using (SqlCommand comando = new SqlCommand(sql, con))
                    {
                        comando.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
                        comando.Parameters.Add(new SqlParameter("@u", SqlDbType.VarChar) { Value = u });
                        Console.WriteLine($"Ejecutando consulta: {sql} con parámetro id = {id}");

                        using (SqlDataReader reader = comando.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine("Registro encontrado.");

                                // Imprimir tipos de datos reales
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    
                                    Console.WriteLine($"Columna {i}: {reader.GetName(i)}, Tipo: {reader.GetFieldType(i)}");
                                }

                                // Leer y convertir los datos de forma segura
                                int id_prod = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                string id_mcodbarra = reader.IsDBNull(1) ? "0" : reader.GetString(1);
                                string descripcion = reader.IsDBNull(2) ? "No existe2" : reader.GetString(2);
                                string ubicacion = reader.IsDBNull(3) ? "0" : reader.GetString(3);
                                double cantidad = reader.IsDBNull(4) ? 0.0f : reader.GetDouble(4);

                                Console.WriteLine($"id_prod: {id_prod}, id_mcodbarra: {id_mcodbarra}, descripcion: {descripcion}, ubicacion: {ubicacion}, cantidad: {cantidad}");

                                return new Inventario
                                {
                                    id = id_prod,
                                    barra = id_mcodbarra,
                                    nombre = descripcion,
                                    ubicacion = ubicacion,
                                    cantidad = cantidad
                                };
                            }
                            else
                            {
                                Console.WriteLine("No se encontró ningún registro con el id proporcionado.");
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Manejar errores específicos de SQL
                Console.WriteLine($"Error de SQL: {sqlEx.Message}");
                Console.WriteLine($"Error número: {sqlEx.Number}");
                Console.WriteLine($"Error fuente: {sqlEx.Source}");
                return new Inventario
                {
                    id = 0,
                    barra = "0",
                    nombre = sqlEx.Message,
                    ubicacion = "0",
                    cantidad = 0.0f
                };
            }
            catch (Exception ex)
            {
                // Manejar otros errores
                Console.WriteLine($"Error: {ex.Message}");
                return new Inventario
                {
                    id = 0,
                    barra = "0",
                    nombre = ex.Message,
                    ubicacion = "0",
                    cantidad = 0.0f
                };
            }

            // Si no se encuentra el producto o hay un error, se retorna el inventario por defecto
            Console.WriteLine("Retornando inventario por defecto.");
            return new Inventario
            {
                id = 0,
                barra = "0",
                nombre = "No existe",
                ubicacion = "0",
                cantidad = 0.0f
            };
        }





        public static string AgregarInventario(Inventario inv)
        {
            string sql = @"
        IF EXISTS (SELECT 1 FROM inventario WHERE id_prod = @id AND ubicacion = @u)
        BEGIN
            UPDATE inventario 
            SET cantidad = @c 
            WHERE id_prod = @id AND ubicacion = @u;
            SELECT 'ACT' AS Resultado;
        END
        ELSE
        BEGIN
            INSERT INTO inventario (id_prod, id_mcodbarra, descripcion, ubicacion, cantidad) 
            VALUES (@id, @b, @d, @u, @c);
            SELECT 'INS' AS Resultado;
        END";

            using (SqlConnection con = new SqlConnection(cadenaConexion))
            {
                try
                {
                    con.Open();
                    using (SqlCommand comando = new SqlCommand(sql, con))
                    {
                        comando.Parameters.Add("@id", SqlDbType.Int).Value = inv.id;
                        comando.Parameters.Add("@b", SqlDbType.VarChar, 100).Value = inv.barra;
                        comando.Parameters.Add("@d", SqlDbType.VarChar, 100).Value = inv.nombre;
                        comando.Parameters.Add("@u", SqlDbType.VarChar, 100).Value = inv.ubicacion;
                        comando.Parameters.Add("@c", SqlDbType.Float).Value = inv.cantidad;

                        using (SqlDataReader reader = comando.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader["Resultado"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al agregar o actualizar inventario: {ex.Message}");
                    return "Error"+ex.Message;
                }
            }
            return "N";
        }

        //public static void ModificarEmpleado(Empleados empleado)
        //{
        //    string sql = "UPDATE Empleados set Nombre = @nombre, Salario = @salario WHERE ID = @id";

        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(cadenaConexion))
        //        {
        //            con.Open();

        //            using (SqlCommand comando = new SqlCommand(sql, con))
        //            {
        //                comando.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = empleado.Nombre;
        //                comando.Parameters.Add("@salario", SqlDbType.Decimal).Value = empleado.Salario;
        //                comando.Parameters.Add("@id", SqlDbType.Int).Value = empleado.ID;
        //                comando.CommandType = CommandType.Text;
        //                comando.ExecuteNonQuery();
        //            }

        //            con.Close();
        //        }
        //    }
        //    catch(Exception ex)
        //    {

        //    }
        //}

        //public static void EliminarEmpleado(Empleados empleado)
        //{
        //    string sql = "DELETE FROM Empleados WHERE ID = @id";

        //    using (SqlConnection con = new SqlConnection(cadenaConexion))
        //    {
        //        con.Open();

        //        using (SqlCommand comando = new SqlCommand(sql, con))
        //        {
        //            comando.Parameters.Add("@id", SqlDbType.Int).Value = empleado.ID;
        //            comando.CommandType = CommandType.Text;
        //            comando.ExecuteNonQuery();
        //        }

        //        con.Close();
        //    }
        //}
    }
}