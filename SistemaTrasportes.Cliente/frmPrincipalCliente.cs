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
using System.Net.Sockets;
using System.Net;

namespace SistemaTrasportes.Cliente
{
    public partial class frmPrincipalCliente : Form
    {
        DatosTransportes datos = new DatosTransportes();
        Conductor conductorActual = new Conductor();
        Viaje viajeEnCurso = new Viaje();
        TcpClient clienteRed = new TcpClient(); //Cliente TCP para conectar con el servidor|
        ASCIIEncoding encoder; // Codificador para transformar de bytes a string y viceversa.
        NetworkStream clientStream; // Cadena de escritura para enviar datos al socket.

        public frmPrincipalCliente(Conductor conductor,  TcpClient cliente, Viaje viaje)
        {
            InitializeComponent();
            conductorActual = conductor;
            txtConductorID.Text = conductorActual.Identificacion;
            viajeEnCurso = viaje;
            clienteRed = cliente;
            if (viajeEnCurso.Estado == "en_curso")
            {
                formatoViajeEnCurso(viajeEnCurso);
            }
            else
            {
                formatoNuevoViaje();
            }

        }

        public void formatoNuevoViaje()
        {
            txtViajeID.Clear();
            txtFechaInicio.Clear();
            txtLugarInicio.Clear();
            txtLugarInicio.Enabled = true;
            txtLugarFinalizacion.Clear();
            txtLugarFinalizacion.Enabled = true;
            txtTiempoEstimado.Clear();
            txtTiempoEstimado.Enabled = true;
            txtCarga.Clear();
            txtCarga.Enabled = true;
            txtTiempoEstimado.Clear();
            txtTiempoEstimado.Enabled = true;
            txtUbicacionActual.Clear();
            txtUbicacionActual.Enabled = false;
            btnActualizar.Enabled = false;
            btnFinalizar.Enabled = false;
            
        }

        private void formatoViajeEnCurso(Viaje viajeactual)
        {
            txtLugarInicio.Enabled = false;
            txtLugarFinalizacion.Enabled = false;
            txtTiempoEstimado.Enabled = false;
            txtCarga.Enabled = false;
            txtTiempoEstimado.Enabled = false;
            txtUbicacionActual.Enabled = true;
            btnAgregarNuevo.Enabled = false;
            btnActualizar.Enabled = true;
            btnFinalizar.Enabled = true;
            txtViajeID.Text = viajeactual.ViajeID.ToString();
            txtFechaInicio.Text = viajeactual.FechaCreacion;
            txtLugarInicio.Text = viajeactual.LugarInicio;
            txtLugarFinalizacion.Text = viajeactual.LugarFinalizacion;
            txtCarga.Text = viajeactual.Carga;
            txtTiempoEstimado.Text = viajeactual.TiempoEstimado;
            txtUbicacionActual.Text = viajeactual.UbicacionActual;



        }

        private void btnAgregarNuevo_Click(object sender, EventArgs e)
        {
            Viaje nuevoViaje = new Viaje();




            if (!(txtLugarInicio.Text.Equals(string.Empty) || txtLugarFinalizacion.Text.Equals(string.Empty)
            || txtCarga.Text.Equals(string.Empty) || txtTiempoEstimado.Text.Equals(string.Empty) ))
            {
                if (clienteRed.Connected)
                {
                    try
                    {
                        //Lee los campos del formulario y los solicita al servidor el registro de Nuevo Usuario
                        //Envia solicitud de validación al Servidor
                        //Obtiene fecha de creacion
                        DateTime currentDateTime = DateTime.Now;
                        string FechaCreacion = currentDateTime.ToString();

                        String mensaje = "CrearViaje/" + conductorActual.Identificacion +"/"+ txtLugarInicio.Text + "/" +
                            txtLugarFinalizacion.Text + "/" + txtCarga.Text + "/" + txtTiempoEstimado.Text +
                            "/" + FechaCreacion ;
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
                            case "error":
                                MessageBox.Show("Servidor reporta error durante el registro \n" +
                                    "Consulte con el Administrador", "Información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            default:
                                nuevoViaje.ViajeID = Convert.ToInt32(respuesta[0]);
                                nuevoViaje.Identificacion = conductorActual.Identificacion;
                                nuevoViaje.LugarInicio = txtLugarInicio.Text;
                                nuevoViaje.UbicacionActual = txtLugarInicio.Text;
                                nuevoViaje.LugarFinalizacion = txtLugarFinalizacion.Text;
                                nuevoViaje.TiempoEstimado = txtTiempoEstimado.Text;
                                nuevoViaje.Carga = txtCarga.Text;
                                nuevoViaje.FechaCreacion = FechaCreacion;
                                //datos.AgregarViaje(nuevoViaje);
                                //nuevoViaje = datos.ObtenerViajesActivosPorCondutor(conductorActual.Identificacion);
                                formatoViajeEnCurso(nuevoViaje);
                                break;

                        }
 

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al intentar escribir a base de datos" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("No existe conexion al servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
