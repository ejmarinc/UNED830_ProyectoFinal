using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using SistemaTransportes.Datos;
using SistemaTransportes.Entidades;



namespace SistemaTrasportes.Cliente
{
    public partial class frmAutenticacionCliente : Form
    {
        DatosTransportes datos = new DatosTransportes();
        TcpClient clienteRed = new TcpClient(); //Cliente TCP para conectar con el servidor|
        Conductor NuevoConductor = new Conductor();
        IPEndPoint serverEndPoint; // Punto de conexion con el servidor.

        ASCIIEncoding encoder; // Codificador para transformar de bytes a string y viceversa.
        NetworkStream clientStream; // Cadena de escritura para enviar datos al socket.


        public frmAutenticacionCliente()
        {
            InitializeComponent();
            clienteRed = new TcpClient();
        }


        private void frmPrincipalCliente_Load(object sender, EventArgs e)
        {
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            if (IPServidor.Text == "")
            {
                MessageBox.Show("Por favor ingrese una dirección IP para el servidor", "Error al conectar", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            }
            else
            {
                if (!clienteRed.Connected)
                {
                    try
                    {
                       serverEndPoint = new IPEndPoint(IPAddress.Parse(IPServidor.Text), 30000);
                        clienteRed = new TcpClient();
                        clienteRed.Connect(serverEndPoint);

                        clientStream = clienteRed.GetStream();

                        encoder = new ASCIIEncoding();
                        byte[] buffer = encoder.GetBytes("Hello Server");

                        clientStream.Write(buffer, 0, buffer.Length);
                        clientStream.Flush();
                        lblConectado.ForeColor = Color.Green;
                        
                        //////
                        clientStream = clienteRed.GetStream();
                        byte[] message = new byte[4096];
                        int bytesRead;
                        bytesRead = clientStream.Read(message, 0, 4096);
                        //message has successfully been received
                        encoder = new ASCIIEncoding();
                        System.Diagnostics.Debug.WriteLine(encoder.GetString(message, 0, bytesRead));
                        String lastMessage = encoder.GetString(message, 0, bytesRead);
                        MessageBox.Show("Mensaje recibido en cliente: " + lastMessage);
                        
                        /////
                        lblConectado.Text = "Conectado";
                        btnConectar.Text = "Desconectar";
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.ToString());
                    }
                }
                // Desconectar
                else {
                    try
                    {
                        if (clienteRed.Connected) 
                        {
                            clientStream = clienteRed.GetStream();
                            encoder = new ASCIIEncoding();
                            byte[] buffer = encoder.GetBytes("Desconexion");
                            clientStream.Write(buffer, 0, buffer.Length);
                            clientStream.Flush();
                            clienteRed.Close();
                            lblConectado.ForeColor = Color.Red;
                            lblConectado.Text = "Desconectado";
                            btnConectar.Text = "Conectar";
                        }                        
                    }
                    catch (Exception Exc) 
                    {
                        MessageBox.Show(Exc.ToString());
                    }
                }
            }
        }



        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (clienteRed.Connected)
            {
                try
                {
                    //NuevoConductor = datos.ValidarUsuario(txtUsuario.Text, txtContrasena.Text);
                    //Envia solicitud de validación al Servidor
                    String mensaje = "ValidarUsuario/" + txtUsuario.Text + "/" + txtContrasena.Text ;
                    clientStream = clienteRed.GetStream();
                    encoder = new ASCIIEncoding();
                    byte[] buffer = encoder.GetBytes(mensaje);
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();
                    //Recibe Respuesta desde el Servidor
                    clientStream = clienteRed.GetStream();
                    byte[] message = new byte[4096];
                    int bytesRead;
                    bytesRead = clientStream.Read(message, 0, 4096);
                    //message has successfully been received
                    encoder = new ASCIIEncoding();
                    System.Diagnostics.Debug.WriteLine(encoder.GetString(message, 0, bytesRead));
                    String lastMessage = encoder.GetString(message, 0, bytesRead);
                    //MessageBox.Show("Mensaje recibido en cliente: " + lastMessage);
                    String[] respuesta = lastMessage.Split('/');
                    switch (respuesta[0])
                    {
                        case "noexiste":
                            MessageBox.Show("Usuario o contraseña Incorrectos", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        //Si el conductor está validado recibe desde servidor los datos del usuario para usarlos en la aplicación
                        case "validado":
                            Viaje nuevoViaje = new Viaje();
                            NuevoConductor.Identificacion = respuesta[1];
                            NuevoConductor.Nombre = respuesta[2];
                            NuevoConductor.PrimerApellido = respuesta[3];
                            NuevoConductor.SegundoApellido = respuesta[4];
                            NuevoConductor.Usuario = txtUsuario.Text;
                            NuevoConductor.Contrasena = txtContrasena.Text;
                            MessageBox.Show("Usuario correcto puede ingresar" +
                                "\nIdentificación:" + NuevoConductor.Identificacion +
                                "\nNombre:" + NuevoConductor.Nombre +
                                "\nPrimer Apellido:" + NuevoConductor.PrimerApellido +
                                "\nSegundo Apellido:" +NuevoConductor.SegundoApellido, "Información",MessageBoxButtons.OK, MessageBoxIcon.Information);
                            frmPrincipalCliente formularioPrincipal = new frmPrincipalCliente(NuevoConductor,clienteRed, nuevoViaje);
                            formularioPrincipal.Show();
                            break;
                        case "validadoyencurso":

/*                            mensaje = "validadoyencurso" + "/" + conductorValidado.Identificacion + "/" + conductorValidado.Nombre + "/" + conductorValidado.PrimerApellido +
                            "/" + conductorValidado.SegundoApellido + "/" + viajeEnCurso.ViajeID + "/" + viajeEnCurso.LugarInicio + "/" + viajeEnCurso.LugarFinalizacion
                            + "/" + viajeEnCurso.Carga + "/" + viajeEnCurso.TiempoEstimado + "/" + viajeEnCurso.UbicacionActual;
                            */
                            Viaje viajeEnCurso = new Viaje();
                            NuevoConductor.Identificacion = respuesta[1];
                            NuevoConductor.Nombre = respuesta[2];
                            NuevoConductor.PrimerApellido = respuesta[3];
                            NuevoConductor.SegundoApellido = respuesta[4];
                            NuevoConductor.Usuario = txtUsuario.Text;
                            NuevoConductor.Contrasena = txtContrasena.Text;
                            viajeEnCurso.ViajeID = Convert.ToInt32(respuesta[5]);
                            viajeEnCurso.LugarInicio = respuesta[6];
                            viajeEnCurso.LugarFinalizacion = respuesta[7];
                            viajeEnCurso.Carga = respuesta[8];
                            viajeEnCurso.TiempoEstimado = respuesta[9];
                            viajeEnCurso.UbicacionActual = respuesta[10];
                            viajeEnCurso.FechaCreacion = respuesta[11];
                            viajeEnCurso.Estado = "en_curso";

                            MessageBox.Show("Usuario correcto puede ingresar" +
                                "\nIdentificación:" + NuevoConductor.Identificacion +
                                "\nNombre:" + NuevoConductor.Nombre +
                                "\nPrimer Apellido:" + NuevoConductor.PrimerApellido +
                                "\nSegundo Apellido:" + NuevoConductor.SegundoApellido, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            frmPrincipalCliente formularioPrincipalEnCurso = new frmPrincipalCliente(NuevoConductor, clienteRed, viajeEnCurso);
                            formularioPrincipalEnCurso.Show();
                            break;
                        case "esperando":
                            MessageBox.Show("Usuario creado pero no autorizado", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case "default":
                            MessageBox.Show("Se ha generado un error en la validación", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;


                    }

                }

                catch (Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error al crear al Ingresar " + ex.Message, "A", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
            else
            {

                        MessageBox.Show("No existe Conexión al Servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //Limpia los campos del formulario
            txtUsuario.Clear();
            txtContrasena.Clear();

        }

        private void txtRegistrarUsuario_Click(object sender, EventArgs e)
        {
            frmRegistrarConductor NuevoRegistro = new frmRegistrarConductor(clienteRed);
            NuevoRegistro.Show();

        }


        }
}
