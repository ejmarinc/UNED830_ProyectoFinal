using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using SistemaTransportes.Entidades;

namespace SistemaTransportes.Datos
{
    //Clase requerida para escribir y leer datos hacia y desde la base de datos
    public class DatosTransportes
    {

        private string cadenaConexion;
        public DatosTransportes()
        {
            //Se obtiene la cadena de conexión del app config del proyecto de interfaz
            cadenaConexion = ConfigurationManager.ConnectionStrings["conexionTransportes"].ConnectionString;
        }

        //Agrega un viaje nuevo

        public void AgregarViaje(Viaje pViaje)
        {

            SqlConnection conexion;
            SqlCommand comando = new SqlCommand();
            string sentencia;

            conexion = new SqlConnection(cadenaConexion);
            sentencia = " Insert	Into	ConductorViajes (Identificacion,FechaCreacion,LugarInicio,LugarFinalizacion,Carga,TiempoEstimado," +
                        "Estado, UbicacionActual)" +
                        " Values (@Identificacion,@FechaCreacion,@LugarInicio, @LugarFinalizacion,@Carga,@TiempoEstimado,@Estado,@UbicacionActual)";

            comando.CommandType = CommandType.Text;
            comando.CommandText = sentencia;
            comando.Connection = conexion;
            comando.Parameters.AddWithValue("@Identificacion", pViaje.Identificacion);
            comando.Parameters.AddWithValue("@FechaCreacion", pViaje.FechaCreacion);
            comando.Parameters.AddWithValue("@LugarInicio", pViaje.LugarInicio);
            comando.Parameters.AddWithValue("@LugarFinalizacion", pViaje.LugarFinalizacion);
            comando.Parameters.AddWithValue("@Carga", pViaje.Carga);
            comando.Parameters.AddWithValue("@TiempoEstimado", pViaje.TiempoEstimado);
            comando.Parameters.AddWithValue("@Estado", "en_curso");
            comando.Parameters.AddWithValue("@UbicacionActual", pViaje.LugarInicio);

            conexion.Open();

            comando.ExecuteNonQuery();

            conexion.Close();
        }


        /// Agrega un nuevo conductor en la base de datos
        public void AgregarConductor(Conductor pConductor)
        {

            SqlConnection conexion;
            SqlCommand comando = new SqlCommand();
            string sentencia;

            conexion = new SqlConnection(cadenaConexion);
            sentencia = " Insert	Into	Conductor (Identificacion, Nombre, PrimerApellido,	SegundoApellido, Usuario, Contrasena, Validado)" +
                        " Values (@Identificacion,	@Nombre, @PrimerApellido,	@SegundoApellido,		@Usuario,   @Contrasena,    @Validado)";

            comando.CommandType = CommandType.Text;
            comando.CommandText = sentencia;
            comando.Connection = conexion;
            comando.Parameters.AddWithValue("@Identificacion", pConductor.Identificacion);
            comando.Parameters.AddWithValue("@Nombre", pConductor.Nombre);
            comando.Parameters.AddWithValue("@PrimerApellido", pConductor.PrimerApellido);
            comando.Parameters.AddWithValue("@SegundoApellido", pConductor.SegundoApellido);
            comando.Parameters.AddWithValue("@Usuario", pConductor.Usuario);
            comando.Parameters.AddWithValue("@Contrasena", pConductor.Contrasena);
            //Por defecto se crea como esperando para que sea aprobado por el administrador
            comando.Parameters.AddWithValue("@Validado", "validado");

            conexion.Open();

            comando.ExecuteNonQuery();

            conexion.Close();
        }
        //Consulta los datos de todos los conductores en la tabla Conductores de la base de datos
        public List<Conductor> ObtenerConductores()
        {
            List<Conductor> listaConductor = new List<Conductor>();
            SqlConnection conexion;
            SqlCommand comando = new SqlCommand();
            string sentencia;
            SqlDataReader reader;

            conexion = new SqlConnection(cadenaConexion);

            sentencia = " Select	Identificacion,    Nombre,	    PrimerApeliido,     SegundoApellido,  Usuario,  Contrasena, Validado" +
                        " From	    Conductor";

            comando.CommandType = CommandType.Text;
            comando.CommandText = sentencia;
            comando.Connection = conexion;

            conexion.Open();

            reader = comando.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    listaConductor.Add(new Conductor
                    {
                        Identificacion = reader.GetString(0),
                        Nombre = reader.GetString(1),
                        PrimerApellido = reader.GetString(2),
                        SegundoApellido = reader.GetString(3),
                        Usuario = reader.GetString(4),
                        Contrasena = reader.GetString(5),
                        Validado = reader.GetString(6)
                    });
                }
            }

            conexion.Close();

            return listaConductor;
        }

        //Devueleve los datos del conduuctor de acuerdo a usuario y contraseña suministrados
        public Conductor ValidarUsuario(string pUsuario, string pContrasena)
        {
            Conductor conductorValidado = new Conductor();
            SqlConnection conexion;
            SqlCommand comando = new SqlCommand();
            string sentencia;
            SqlDataReader reader;

            conexion = new SqlConnection(cadenaConexion);

            sentencia = " Select	Identificacion,    Nombre,	    PrimerApellido,     SegundoApellido,  Usuario,  Contrasena, Validado" +
                        " From	    Conductor" +
                        " Where Usuario = @Usuario AND Contrasena = @Contrasena";
                         comando.Parameters.AddWithValue("@Usuario", pUsuario);
                         comando.Parameters.AddWithValue("@Contrasena", pContrasena);
            ;

            comando.CommandType = CommandType.Text;
            comando.CommandText = sentencia;
            comando.Connection = conexion;

            conexion.Open();

            reader = comando.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    conductorValidado.Identificacion = reader.GetString(0);
                    conductorValidado.Nombre = reader.GetString(1);
                    conductorValidado.PrimerApellido = reader.GetString(2);
                    conductorValidado.SegundoApellido = reader.GetString(3);
                    conductorValidado.Usuario = reader.GetString(4);
                    conductorValidado.Contrasena = reader.GetString(5);
                    conductorValidado.Validado = reader.GetString(6);
                }
            }

            conexion.Close();

            return conductorValidado;
        }



        //Agrega un nuevo camion en la tabla de camiones 
        public void AgregarCamion(Camion pCamion)
        {

            SqlConnection conexion;
            SqlCommand comando = new SqlCommand();
            string sentencia;

            conexion = new SqlConnection(cadenaConexion);
            sentencia = " Insert	Into	Camion (Placa, Modelo, Marca)" +
                        " Values (@Placa,	@Modelo, @Marca)";

            comando.CommandType = CommandType.Text;
            comando.CommandText = sentencia;
            comando.Connection = conexion;
            comando.Parameters.AddWithValue("@Placa", pCamion.Placa);
            comando.Parameters.AddWithValue("@Modelo", pCamion.Modelo);
            comando.Parameters.AddWithValue("@Marca", pCamion.Marca);

            conexion.Open();

            comando.ExecuteNonQuery();

            conexion.Close();
        }
        //Agrega el camión ingresado con el método anterior al respectivo conductor
        public void AgregarConductorCamion(string Identificacion, string Placa)
        {

            SqlConnection conexion;
            SqlCommand comando = new SqlCommand();
            string sentencia;

            conexion = new SqlConnection(cadenaConexion);
            sentencia = " Insert	Into	ConductorCamion (Identificacion, Placa)" +
                        " Values (@Identificacion,	@Placa)";

            comando.CommandType = CommandType.Text;
            comando.CommandText = sentencia;
            comando.Connection = conexion;
            comando.Parameters.AddWithValue("@Identificacion", Identificacion);
            comando.Parameters.AddWithValue("@Placa", Placa);

            conexion.Open();

            comando.ExecuteNonQuery();

            conexion.Close();
        }

        //Obtiene la lista de camiones si no se especifica la placa el valor defecto es nulo y retorna todos
        //los camiones
        public List<Camion> ObtenerCamionesPorPlaca(string P_Placa = null)
        {
            List<Camion> listaCamiones = new List<Camion>();

            SqlConnection conexion;
            SqlCommand comando = new SqlCommand();
            string sentencia;
            SqlDataReader reader;

            conexion = new SqlConnection(cadenaConexion);

            //El siguiente if si no se especifica un valor genera la sentencia SQL para todos los camiones,
            //de lo contrario utiliza la placa

            if (string.IsNullOrEmpty(P_Placa))
            {
                sentencia = " Select	Placa,    AnnoModelo,	    Marca" +
                            " From	    Camion";
            }
            else
            {
                sentencia = " Select	Placa,	Modelo,	Marca" +
                            " From	    Camion" +
                            " Where     Placa =   @P_Placa";
                comando.Parameters.AddWithValue("@Placa", P_Placa);
            }

            comando.CommandType = CommandType.Text;
            comando.CommandText = sentencia;
            comando.Connection = conexion;

            conexion.Open();

            reader = comando.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    listaCamiones.Add(new Camion
                    {
                        Placa = reader.GetString(0),
                        Modelo = reader.GetString(1),
                        Marca = reader.GetString(2),
 
                    });
                }
            }
            conexion.Close();
            return listaCamiones;
        }

        //Obtiene los camiones por conductor, regresa todos los datos de la tabla camiones
        //realizando una consulta avanzada utilizando además la tabla conductorcamión y la identificación
        //seleccionada por el usuario
        public List<Camion> ObtenerCamionesPorCondutor(string P_Identificacion)
        {
            List<Camion> listaCamiones = new List<Camion>();

            SqlConnection conexion;
            SqlCommand comando = new SqlCommand();
            string sentencia;
            SqlDataReader reader;

            conexion = new SqlConnection(cadenaConexion);

            //El siguiente if si no se especifica un valor genera la sentencia SQL para todos los camiones, de lo contrario utiliza la placa

            sentencia = " Select Camion.*" +
                            " From	    Camion INNER JOIN ConductorCamion ON Camion.Placa = ConductorCamion.Placa" +
                            " Where     ConductorCamion.Identificacion =   @P_Identificacion";
            comando.Parameters.AddWithValue("@P_Identificacion", P_Identificacion);


            comando.CommandType = CommandType.Text;
            comando.CommandText = sentencia;
            comando.Connection = conexion;

            conexion.Open();

            reader = comando.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    listaCamiones.Add(new Camion
                    {
                        Placa = reader.GetString(0),
                        Modelo = reader.GetString(1),
                        Marca = reader.GetString(2),

                    });

                }
            }

            conexion.Close();
            return listaCamiones;

        }

        //Obtiene los viajes activos del conductor
        public Viaje ObtenerViajesActivos(string P_Identificacion)
        {
            Viaje viajeActivo = new Viaje();

            SqlConnection conexion;
            SqlCommand comando = new SqlCommand();
            string sentencia;
            SqlDataReader reader;

            conexion = new SqlConnection(cadenaConexion);

            //El siguiente if si no se especifica un valor genera la sentencia SQL para todos los camiones, de lo contrario utiliza la placa

            sentencia = " Select ConductorViajes.*" +
                            " From	    ConductorViajes" +
                            " Where     ConductorViajes.Identificacion =   @P_Identificacion AND ConductorViajes.Estado = 'en_curso'";
            comando.Parameters.AddWithValue("@P_Identificacion", P_Identificacion);
            comando.CommandType = CommandType.Text;
            comando.CommandText = sentencia;
            comando.Connection = conexion;

            conexion.Open();

            reader = comando.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    viajeActivo.ViajeID = reader.GetInt32(0);
                    viajeActivo.Identificacion = reader.GetString(1);
                    viajeActivo.LugarInicio = reader.GetString(2);
                    viajeActivo.LugarFinalizacion = reader.GetString(3);
                    viajeActivo.Carga = reader.GetString(4);
                    viajeActivo.TiempoEstimado = reader.GetString(5);
                    viajeActivo.FechaCreacion = reader.GetString(6);
                    viajeActivo.UbicacionActual = reader.GetString(7);
                    viajeActivo.Estado = reader.GetString(8);


                }
            }

            conexion.Close();
            return viajeActivo;

        }
    }
}
