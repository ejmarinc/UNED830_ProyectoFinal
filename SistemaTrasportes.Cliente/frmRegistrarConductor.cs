using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SistemaTransportes.Entidades;
using SistemaTransportes.Datos;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;



namespace SistemaTrasportes.Cliente
{
    public partial class frmRegistrarConductor : Form
    {
       // Conductor NuevoConductor = new Conductor();
        //Camion nuevoCamion = new Camion();

        DatosTransportes datos = new DatosTransportes();
        TcpClient clienteRed = new TcpClient(); //Cliente TCP para conectar con el servidor|
        ASCIIEncoding encoder; // Codificador para transformar de bytes a string y viceversa.
        NetworkStream clientStream; // Cadena de escritura para enviar datos al socket.

        //Formulario para registrar Conductores en la base de datos recibe TCP Client desde el formulario principal del Cliente
        public frmRegistrarConductor(TcpClient cliente)
        {
            InitializeComponent();
            clienteRed = cliente;
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {

            //Comprueba que todos los campos requeridos sean completados por el usuario
            if (!(txtIndentificacion.Text.Equals(string.Empty) || txtNombre.Text.Equals(string.Empty)
    || txtPrimerApellido.Text.Equals(string.Empty) || txtSegundoApellido.Text.Equals(string.Empty) || txtNuevoUsuario.Text.Equals(string.Empty)
    || txtNuevaContrasena.Text.Equals(string.Empty) || txtPlaca.Text.Equals(string.Empty) || txtMarca.Text.Equals(string.Empty)
     || txtModelo.Text.Equals(string.Empty)))
            {
                if (clienteRed.Connected)
                {
                    try
                    {
                        //Lee los campos del formulario y los solicita al servidor el registro de Nuevo Usuario
                        //Envia solicitud de validación al Servidor
                        String mensaje = "RegistrarUsuario/" + txtIndentificacion.Text + "/" +
                            txtNombre.Text + "/" + txtPrimerApellido.Text + "/" + txtSegundoApellido.Text +
                            "/" + txtNuevoUsuario.Text + "/" + txtNuevaContrasena.Text + "/" +
                            txtPlaca.Text + "/" + txtMarca.Text + "/" + txtModelo.Text;
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
                            case "registrado":
                                MessageBox.Show("Conductor agregado esperando aprobacion", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            //Si el conductor está validado recibe desde servidor los datos del usuario para usarlos en la aplicación
                            case "error":
                                MessageBox.Show("Servidor reporta error durante el registro \n" +
                                    "Consulte con el Administrador", "Información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;

                        }


                        this.Close();


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ha ocurrido un error al crear el Conductor " + ex.Message, "A", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {

                        MessageBox.Show("No existe conexion al servidor" , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                //En caso de que los datos no estén completos se muestra un mensaje de advertencia
                //Se debe tener cuidado que no parezca un mensaje de error, dado que no es un error
                MessageBox.Show("Debe completar todos los campos!", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
    }
}
