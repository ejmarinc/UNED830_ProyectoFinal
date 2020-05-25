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
using System.Threading;
using System.Net;
using SistemaTransportes.Datos;
using SistemaTransportes.Entidades;

namespace SistemaTransportes.Servidor
{
    public partial class frmPrincipalServidor : Form
    {
        private TcpListener tcpListener;
        private Thread listenThread;
        private String lastMessage;
        uint clientesConectados;
        DatosTransportes datos = new DatosTransportes();
        public frmPrincipalServidor()
        {
            InitializeComponent();
        }
        private void frmPrincipalServidor_Load(object sender, EventArgs e)
        {
            clientesConectados = 0;
            txtConectados.Text = clientesConectados.ToString();
            CheckForIllegalCrossThreadCalls = false;
            txtMensajesRecibidos.Text = "Servidor Iniciado. Esperando por clientes...\n";
            this.tcpListener = new TcpListener(IPAddress.Any, 30000);
            this.listenThread = new Thread(new
            ThreadStart(ListenForClients));
            this.listenThread.Start();
        }


        private void ListenForClients()
        {
            this.tcpListener.Start();

            while (true)
            {
                //blocks until a client has connected to the server
                TcpClient client = this.tcpListener.AcceptTcpClient();

                //create a thread to handle communication
                //with connected client
                lock (this)
                {
                    clientesConectados++;
                    txtConectados.Text = clientesConectados.ToString();
                }
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }
        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes("Connected");
            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    lock (this)
                    {
                        clientesConectados--;
                        txtConectados.Text = clientesConectados.ToString();
                    }
                    break;
                }

                //message has successfully been received
                encoder = new ASCIIEncoding();
                System.Diagnostics.Debug.WriteLine(encoder.GetString(message, 0, bytesRead));
                lastMessage = encoder.GetString(message, 0, bytesRead);
                String[] solicitud = lastMessage.Split('/');
                // MessageBox.Show(solicitud[0], "Avión TCP", MessageBoxButtons.OK);
                string mensaje="default";

                switch (solicitud[0])
                {
                    //Caso en el que cliente solicita validar usuario y clave.
                    case "ValidarUsuario":
                        Conductor conductorValidado = new Conductor();
                        Viaje viajeEnCurso = new Viaje();
                        conductorValidado = datos.ValidarUsuario(solicitud[1], solicitud[2]);
                        if (conductorValidado.Validado != null)
                        {
                            viajeEnCurso = datos.ObtenerViajesActivos(conductorValidado.Identificacion);
                        }
                        //MessageBox.Show("Resultado de Validacion:"+nuevoConductor.Validado, "Informacion", MessageBoxButtons.OK);
                        switch (conductorValidado.Validado)
                        {
                            case null:
                                mensaje = "noexiste";
                                break;
                            //Si se encuentra validado envia la informacion del usuario al cliente
                            case "validado":
                                if (viajeEnCurso.Estado != "en_curso")
                                {
                                    mensaje = "validado" + "/" + conductorValidado.Identificacion + "/" + conductorValidado.Nombre + "/" + conductorValidado.PrimerApellido +
                                    "/" + conductorValidado.SegundoApellido;
                                }
                                else
                                {
                                    mensaje = "validadoyencurso" + "/" + conductorValidado.Identificacion + "/" + conductorValidado.Nombre + "/" + conductorValidado.PrimerApellido +
                                    "/" + conductorValidado.SegundoApellido + "/" + viajeEnCurso.ViajeID + "/" + viajeEnCurso.LugarInicio + "/" + viajeEnCurso.LugarFinalizacion
                                    + "/" + viajeEnCurso.Carga + "/" + viajeEnCurso.TiempoEstimado + "/" + viajeEnCurso.UbicacionActual + "/" + viajeEnCurso.FechaCreacion;

                                }
                                break;
                            case "esperando":
                                mensaje = "esperando";
                                break;
                        }
                        buffer = encoder.GetBytes(mensaje);
                        clientStream.Write(buffer, 0, buffer.Length);
                        clientStream.Flush();
                        break;

                    case "RegistrarUsuario":
                        //crea un objeto conductor para proceder al registro
                        Conductor nuevoConductor = new Conductor();
                        nuevoConductor.Identificacion = solicitud[1];
                        nuevoConductor.Nombre = solicitud[2];
                        nuevoConductor.PrimerApellido = solicitud[3];
                        nuevoConductor.SegundoApellido = solicitud[4];
                        nuevoConductor.Usuario = solicitud[5];
                        nuevoConductor.Contrasena = solicitud[6];
                        //crea un objeto camion para proceder al registro
                        Camion nuevoCamion = new Camion();
                        nuevoCamion.Placa = solicitud[7];
                        nuevoCamion.Marca = solicitud[8];
                        nuevoCamion.Modelo = solicitud[9];

                        try
                        {
                            datos.AgregarConductor(nuevoConductor);
                            datos.AgregarCamion(nuevoCamion);
                            datos.AgregarConductorCamion(nuevoConductor.Identificacion, nuevoCamion.Placa);
                            //MessageBox.Show("Resultado de Validacion:"+nuevoConductor.Validado, "Informacion", MessageBoxButtons.OK);
                            mensaje = "registrado/Fin";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al Intentar Registro en la base de datos" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            mensaje = "error/Fin";
                        }
                        buffer = encoder.GetBytes(mensaje);
                        clientStream.Write(buffer, 0, buffer.Length);
                        clientStream.Flush();
                        break;

                    case "CrearViaje":
                        //crea un objeto conductor para proceder al registro
                        Viaje nuevoViaje = new Viaje();
                        nuevoViaje.Identificacion = solicitud[1];
                        nuevoViaje.LugarInicio = solicitud[2];
                        nuevoViaje.LugarFinalizacion = solicitud[3];
                        nuevoViaje.TiempoEstimado = solicitud[5];
                        nuevoViaje.Carga = solicitud[4];
                        nuevoViaje.FechaCreacion = solicitud[6];
                        nuevoViaje.UbicacionActual = solicitud[2]; //En nuevo viaje ubicacion actuar es el inicio

                        try
                        {
                            datos.AgregarViaje(nuevoViaje);
                            nuevoViaje = datos.ObtenerViajesActivos(solicitud[1]);
                            //MessageBox.Show("Resultado de Validacion:"+nuevoConductor.Validado, "Informacion", MessageBoxButtons.OK);
                            mensaje = nuevoViaje.ViajeID.ToString() + "/Fin" ;//Retorna el ID creado para el viaje
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al Intentar Registro en la base de datos" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            mensaje = "error/Fin";
                        }
                        buffer = encoder.GetBytes(mensaje);
                        clientStream.Write(buffer, 0, buffer.Length);
                        clientStream.Flush();
                        break;

                        //Caso en el que cliente solicita consultar las notas.
                        //    case "Volar":
                        //Llamado a Metodo YYY
                        /*buffer = encoder.GetBytes(mensaje);
                        clientStream.Write(buffer, 0, buffer.Length);
                        clientStream.Flush();
                        MessageBox.Show("BOTON VOLAR--SERVIDRO", "Avión TCP", MessageBoxButtons.OK);
                        break;
                    case "ZZZ":
                        try
                        {
                            //Llamado a Metodo ZZZ
                            buffer = encoder.GetBytes(mensaje);
                            clientStream.Write(buffer, 0, buffer.Length);
                            clientStream.Flush();
                            break;
                        }
                        catch
                        {
                            MessageBox.Show("Ocurrio error al registra Alumn...", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }*/
                }
                lock (this)
                {
                    txtMensajesRecibidos.Text += Environment.NewLine + lastMessage;
                    txtMensajesRecibidos.SelectionStart = txtMensajesRecibidos.Text.Length;
                    txtMensajesRecibidos.ScrollToCaret();
                    txtMensajesRecibidos.Refresh();
                }
            }
            tcpClient.Close();
            lock (this)
            {
                //clientesConectados--;
                txtConectados.Text = clientesConectados.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Opcion en Desarrollo:\n Temporalmente todos los usuarios se crean como validado", "ATENCION", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
