using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using localhost = MedinetGeneradorDB.ServiceReference1;


namespace MedinetGeneradorDB
{
    public partial class frmGenerador : Form
    {
        Data miData;
        Generador miGenerador;
        List<Data.Visitador> listaVisitadores;
        DirectoryInfo carpeta { get; set; }
        Data.Visitador miVisitador;
        string rutaBDVisitador;
        string rutaFileVisitador;
        string rutaBase = @"C:\MEDINET BASES DE DATOS\";
        private StreamWriter archivo;
        private string instruccionesAEnviar = "";
        bool desdeArchivo = false;
        public frmGenerador()
        {
            InitializeComponent();
            cargarComboBD();
            mostrarVersion();
        }

        private void mostrarVersion()
        {
            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            lblVersion.Text = $"Versión: {assemblyVersion}";
        }

        private void btnEnviarData_Click(object sender, EventArgs e)
        {

            //string fe = DateTime.Now.ToLongTimeString();  //.Replace("-", "").Replace("/", "").Replace(":", "").Replace("p.m.", "").Replace("a.m.", "").Replace(" ", "");
            //string fe2 = DateTime.Now.//.Replace("-", "").Replace("/", "").Replace(":", "").Replace("p.m.", "").Replace("a.m.", "").Replace(" ", "");

            localhost.WSCallCenterSoapClient myservice;
            if (miVisitador != null)
            {


                string registroGoogleID = miData.buscarGoogleRegistrationID(miVisitador.id);
                if (!registroGoogleID.Equals(""))
                {
                    myservice = new localhost.WSCallCenterSoapClient();
                    try
                    {
                        //if (!desdeArchivo)
                        //{
                        //    instruccionesAEnviar = txtInstrucciones.Text.ToString().Replace('\n', ' ').Replace('\r', ' ');
                        //}
                        //bool respuesta = myservice.setInstruccionesSql(instruccionesAEnviar, registroGoogleID);
                        //if (respuesta)
                        //{
                        //    MessageBox.Show("Mensaje enviado Satisfactoriamente", "Mensaje Enviado", MessageBoxButtons.OK);
                        //}
                        //else
                        //{
                        //    MessageBox.Show("El mensaje no pudo ser enviado", "Mensaje no enviado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}

                        if (!desdeArchivo)
                        {
                            instruccionesAEnviar = txtInstrucciones.Text.ToString().Replace('\n', ' ').Replace('\r', ' ');
                        }
                        bool respuesta = false;
                        int bytes = Encoding.UTF8.GetByteCount(instruccionesAEnviar);
                        int linea = 1;
                        if (bytes < 4096)
                        {
                            respuesta = myservice.setInstruccionesSql(instruccionesAEnviar, registroGoogleID);
                        }
                        else
                        {
                            string[] arrayInstrucciones = instruccionesAEnviar.Split(';');

                            foreach (string instruccion in arrayInstrucciones)
                            {
                                respuesta = myservice.setInstruccionesSql(instruccion.Trim(), registroGoogleID);


                                if (!respuesta) break;
                                pb.Value = linea * 100 / arrayInstrucciones.Length;

                                linea++;
                            }
                        }
                        if (respuesta)
                        {
                            MessageBox.Show("Mensaje enviado Satisfactoriamente", "Mensaje Enviado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("El mensaje no pudo ser enviado." + (desdeArchivo ? " - Específicamente en la Linea: " + linea : ""), "Mensaje no enviado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                else
                {
                    MessageBox.Show("No hay ID para enviar mensaje", "Sin ID", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
            {
                MessageBox.Show("No hay visitador seleccionado para enviar mensaje", "Sin Visitador", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void crearRuta()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            DateTime fechaActual = DateTime.Now;
            carpeta = Directory.CreateDirectory(rutaBase + fechaActual.Year.ToString() + @"\" + fechaActual.Month.ToString() + @"\" + fechaActual.ToShortDateString().Replace("/", "-") + @"\" + miVisitador.nombre);

            if (File.Exists("Cartera"))
            {
                File.Delete(carpeta.FullName.ToString() + @"\Cartera");
                File.Copy("Cartera", carpeta.FullName.ToString() + @"\Cartera");
                rutaBDVisitador = carpeta.FullName.ToString() + @"\Cartera";
            }
            if (File.Exists("Cartera.txt"))
            {
                File.Delete(carpeta.FullName.ToString() + @"\Cartera.txt");
                File.Copy("Cartera.txt", carpeta.FullName.ToString() + @"\Cartera.txt");
                rutaFileVisitador = carpeta.FullName.ToString() + @"\Cartera.txt";
            }
        }
        private void llenarAnnios()
        {
            cmbAno.Items.Clear();
            cmbAno.Items.AddRange(miData.buscarAnnios());
        }
        private void llenarCiclos()
        {
            cmbCiclo.Items.Clear();
            cmbCiclo.Items.AddRange(miData.buscarCiclos());
        }
        private void verificarCombos()
        {
            if ((cmbAno.SelectedItem != null && !cmbAno.SelectedItem.ToString().Equals("")) && (cmbCiclo.SelectedItem != null && !cmbCiclo.SelectedItem.ToString().Equals("")))
            {
                llenarVisitadores();
            }
        }
        private void llenarVisitadores()
        {
            cmbVisitador.Items.Clear();
            cmbVisitador.Enabled = false;
            listaVisitadores = miData.buscarVisitadores(int.Parse(cmbAno.SelectedItem.ToString()), int.Parse(cmbCiclo.SelectedItem.ToString()));
            foreach (Data.Visitador visitador in listaVisitadores)
            {
                cmbVisitador.Items.Add(visitador.id + "         -   " + visitador.nombre);
            }
            if (listaVisitadores.Count > 0)
            {
                cmbVisitador.Enabled = true;
            }

        }

        private void cmbAno_SelectedValueChanged(object sender, EventArgs e)
        {
            verificarCombos();
        }

        private void cmbCiclo_SelectedValueChanged(object sender, EventArgs e)
        {
            verificarCombos();
        }

        private void cmbVisitador_SelectedValueChanged(object sender, EventArgs e)
        {

            miVisitador = new Data.Visitador();
            miVisitador.id = int.Parse(cmbVisitador.SelectedItem.ToString().Substring(0, cmbVisitador.SelectedItem.ToString().IndexOf(" ", 0)));
            foreach (Data.Visitador visitador in listaVisitadores)
            {
                if (visitador.id == miVisitador.id)
                {
                    miVisitador.nombre = visitador.nombre;
                    miVisitador.region = visitador.region;
                    miVisitador.linea = visitador.linea;
                    break;
                }
            }
            //MessageBox.Show(miVisitador.nombre);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void cmbCiclo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (btnAgregar.Text.Equals("Agregar"))
            {
                btnAgregar.Text = "Aceptar";
                btnEliminar.Enabled = true;
                cmbEmpresa.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                if (!cmbEmpresa.Text.Equals(""))
                {
                    Directory.CreateDirectory(rutaBase);
                    archivo = new StreamWriter(rutaBase + "\\Config.txt", true);
                    archivo.WriteLine(cmbEmpresa.Text);
                    archivo.Flush();
                    archivo.Close();
                    btnEliminar.Enabled = false;
                    btnAgregar.Text = "Agregar";
                    cmbEmpresa.DropDownStyle = ComboBoxStyle.DropDownList;
                    cargarComboBD();

                }
                else
                {
                    MessageBox.Show("¡Escribe algo por favor!");
                }
            }
        }

        private void frmGenerador_Load(object sender, EventArgs e)
        {

        }

        private void cargarComboBD()
        {
            if (File.Exists(rutaBase + "\\Config.txt"))
            {

                StreamReader archivoRead = new StreamReader(rutaBase + "\\Config.txt");
                string linea = "--";
                cmbEmpresa.Items.Clear();
                cmbEmpresa.Items.Add(linea);
                while (!archivoRead.EndOfStream)
                {
                    cmbEmpresa.Items.Add(archivoRead.ReadLine());
                }
                archivoRead.Close();
            }
            else
            {
                MessageBox.Show("Debe asociar al menos una base de datos");
            }
        }

        private void cmbEmpresa_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!cmbEmpresa.Text.Equals("--"))
            {
                miData = new Data(cmbEmpresa.Text);
                llenarAnnios();
                llenarCiclos();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            btnAgregar.Text = "Agregar";
            cmbEmpresa.DropDownStyle = ComboBoxStyle.DropDownList;
            cargarComboBD();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            crearRuta();
            miData.terminarConexion();
            try
            {
                txtInstrucciones.Clear();
                miGenerador = new Generador(rutaBDVisitador, rutaFileVisitador, chkLimpia.Checked, miVisitador, int.Parse(cmbAno.SelectedItem.ToString()), int.Parse(cmbCiclo.SelectedItem.ToString()), chkCicloAbierto.Checked, cmbEmpresa.Text);
                if (miGenerador.errores != null && miGenerador.errores.Any())
                {
                    miGenerador.errores.ForEach(ee => txtInstrucciones.Text += $"{ee} \n");
                    MessageBox.Show("Error en el proceso. Por favor vea el Log", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Base de Datos Creada", "Fin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process.Start(rutaFileVisitador.Replace("Cartera.txt", ""));
                }
                miGenerador = null;
            }
            catch (Exception ex)
            {
                txtInstrucciones.Text = ex.Message;
            }

        }

        private void cmbVisitador_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = (openFileDialog1.ShowDialog() == DialogResult.OK);

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                // Open the selected file to read.
                System.IO.Stream fileStream = openFileDialog1.OpenFile();

                using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
                {
                    // Read the first line from the file and write it the textbox.
                    instruccionesAEnviar = reader.ReadToEnd();
                    txtInstrucciones.Text = instruccionesAEnviar;
                    instruccionesAEnviar = instruccionesAEnviar.Replace('\n', ' ').Replace('\r', ' ');
                    txtInstrucciones.Enabled = false;
                    desdeArchivo = true;
                }
                fileStream.Close();
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            desdeArchivo = false;
            txtInstrucciones.Clear();
            instruccionesAEnviar = "";
            txtInstrucciones.Enabled = true;
            pb.Value = 0;
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }
    }
}
