namespace MedinetGeneradorDB
{
    partial class frmGenerador
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGenerador));
            this.cmbAno = new System.Windows.Forms.ComboBox();
            this.cmbCiclo = new System.Windows.Forms.ComboBox();
            this.cmbVisitador = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkCicloAbierto = new System.Windows.Forms.CheckBox();
            this.chkLimpia = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.cmbEmpresa = new System.Windows.Forms.ComboBox();
            this.btnEnviarData = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.pb = new System.Windows.Forms.ProgressBar();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnAbrir = new System.Windows.Forms.Button();
            this.txtInstrucciones = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbAno
            // 
            this.cmbAno.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAno.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAno.FormattingEnabled = true;
            this.cmbAno.Location = new System.Drawing.Point(112, 37);
            this.cmbAno.Margin = new System.Windows.Forms.Padding(6);
            this.cmbAno.Name = "cmbAno";
            this.cmbAno.Size = new System.Drawing.Size(120, 33);
            this.cmbAno.TabIndex = 0;
            this.cmbAno.SelectedValueChanged += new System.EventHandler(this.cmbAno_SelectedValueChanged);
            // 
            // cmbCiclo
            // 
            this.cmbCiclo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCiclo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCiclo.FormattingEnabled = true;
            this.cmbCiclo.Location = new System.Drawing.Point(418, 37);
            this.cmbCiclo.Margin = new System.Windows.Forms.Padding(6);
            this.cmbCiclo.Name = "cmbCiclo";
            this.cmbCiclo.Size = new System.Drawing.Size(120, 33);
            this.cmbCiclo.TabIndex = 1;
            this.cmbCiclo.SelectedIndexChanged += new System.EventHandler(this.cmbCiclo_SelectedIndexChanged);
            this.cmbCiclo.SelectedValueChanged += new System.EventHandler(this.cmbCiclo_SelectedValueChanged);
            // 
            // cmbVisitador
            // 
            this.cmbVisitador.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVisitador.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbVisitador.FormattingEnabled = true;
            this.cmbVisitador.Location = new System.Drawing.Point(142, 37);
            this.cmbVisitador.Margin = new System.Windows.Forms.Padding(6);
            this.cmbVisitador.Name = "cmbVisitador";
            this.cmbVisitador.Size = new System.Drawing.Size(470, 33);
            this.cmbVisitador.TabIndex = 2;
            this.cmbVisitador.SelectedIndexChanged += new System.EventHandler(this.cmbVisitador_SelectedIndexChanged);
            this.cmbVisitador.SelectedValueChanged += new System.EventHandler(this.cmbVisitador_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 42);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Año:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(340, 42);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Ciclo:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 42);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 25);
            this.label3.TabIndex = 5;
            this.label3.Text = "Visitador";
            // 
            // btnGenerar
            // 
            this.btnGenerar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerar.Location = new System.Drawing.Point(926, 31);
            this.btnGenerar.Margin = new System.Windows.Forms.Padding(6);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(150, 44);
            this.btnGenerar.TabIndex = 6;
            this.btnGenerar.Text = "Generar";
            this.btnGenerar.UseVisualStyleBackColor = true;
            this.btnGenerar.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbAno);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbCiclo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(24, 140);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox1.Size = new System.Drawing.Size(1106, 113);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Seleccione el Ciclo";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbVisitador);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(24, 265);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox2.Size = new System.Drawing.Size(1106, 119);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Seleccione el Visitador";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkCicloAbierto);
            this.groupBox3.Controls.Add(this.chkLimpia);
            this.groupBox3.Controls.Add(this.btnGenerar);
            this.groupBox3.Location = new System.Drawing.Point(24, 398);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox3.Size = new System.Drawing.Size(1106, 83);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Opciones";
            // 
            // chkCicloAbierto
            // 
            this.chkCicloAbierto.AutoSize = true;
            this.chkCicloAbierto.Location = new System.Drawing.Point(224, 37);
            this.chkCicloAbierto.Margin = new System.Windows.Forms.Padding(6);
            this.chkCicloAbierto.Name = "chkCicloAbierto";
            this.chkCicloAbierto.Size = new System.Drawing.Size(166, 29);
            this.chkCicloAbierto.TabIndex = 1;
            this.chkCicloAbierto.Text = "Ciclo Abierto";
            this.chkCicloAbierto.UseVisualStyleBackColor = true;
            // 
            // chkLimpia
            // 
            this.chkLimpia.AutoSize = true;
            this.chkLimpia.Location = new System.Drawing.Point(32, 38);
            this.chkLimpia.Margin = new System.Windows.Forms.Padding(6);
            this.chkLimpia.Name = "chkLimpia";
            this.chkLimpia.Size = new System.Drawing.Size(149, 29);
            this.chkLimpia.TabIndex = 0;
            this.chkLimpia.Text = "Limpiar BD";
            this.chkLimpia.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblVersion);
            this.groupBox4.Controls.Add(this.btnEliminar);
            this.groupBox4.Controls.Add(this.btnAgregar);
            this.groupBox4.Controls.Add(this.cmbEmpresa);
            this.groupBox4.Location = new System.Drawing.Point(24, 25);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox4.Size = new System.Drawing.Size(1106, 104);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Seleccione la Empresa";
            this.groupBox4.Enter += new System.EventHandler(this.groupBox4_Enter);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(921, 43);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(85, 25);
            this.lblVersion.TabIndex = 3;
            this.lblVersion.Text = "Versión";
            // 
            // btnEliminar
            // 
            this.btnEliminar.Enabled = false;
            this.btnEliminar.Location = new System.Drawing.Point(620, 33);
            this.btnEliminar.Margin = new System.Windows.Forms.Padding(6);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(140, 44);
            this.btnEliminar.TabIndex = 2;
            this.btnEliminar.Text = "Cancelar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(476, 33);
            this.btnAgregar.Margin = new System.Windows.Forms.Padding(6);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(140, 44);
            this.btnAgregar.TabIndex = 1;
            this.btnAgregar.Text = "Agregar";
            this.btnAgregar.UseVisualStyleBackColor = true;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // cmbEmpresa
            // 
            this.cmbEmpresa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmpresa.FormattingEnabled = true;
            this.cmbEmpresa.Location = new System.Drawing.Point(34, 33);
            this.cmbEmpresa.Margin = new System.Windows.Forms.Padding(6);
            this.cmbEmpresa.Name = "cmbEmpresa";
            this.cmbEmpresa.Size = new System.Drawing.Size(426, 33);
            this.cmbEmpresa.TabIndex = 0;
            this.cmbEmpresa.SelectedValueChanged += new System.EventHandler(this.cmbEmpresa_SelectedValueChanged);
            // 
            // btnEnviarData
            // 
            this.btnEnviarData.BackColor = System.Drawing.SystemColors.Control;
            this.btnEnviarData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnviarData.Location = new System.Drawing.Point(924, 421);
            this.btnEnviarData.Margin = new System.Windows.Forms.Padding(6);
            this.btnEnviarData.Name = "btnEnviarData";
            this.btnEnviarData.Size = new System.Drawing.Size(150, 44);
            this.btnEnviarData.TabIndex = 11;
            this.btnEnviarData.Text = "Enviar";
            this.btnEnviarData.UseVisualStyleBackColor = false;
            this.btnEnviarData.Visible = false;
            this.btnEnviarData.Click += new System.EventHandler(this.btnEnviarData_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.pb);
            this.groupBox5.Controls.Add(this.btnLimpiar);
            this.groupBox5.Controls.Add(this.btnAbrir);
            this.groupBox5.Controls.Add(this.txtInstrucciones);
            this.groupBox5.Controls.Add(this.btnEnviarData);
            this.groupBox5.Location = new System.Drawing.Point(26, 492);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox5.Size = new System.Drawing.Size(1104, 477);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Log de Errores (Si existen)";
            // 
            // pb
            // 
            this.pb.Location = new System.Drawing.Point(358, 421);
            this.pb.Margin = new System.Windows.Forms.Padding(6);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(554, 44);
            this.pb.TabIndex = 14;
            this.pb.Visible = false;
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(196, 421);
            this.btnLimpiar.Margin = new System.Windows.Forms.Padding(6);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(150, 44);
            this.btnLimpiar.TabIndex = 13;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Visible = false;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // btnAbrir
            // 
            this.btnAbrir.Location = new System.Drawing.Point(12, 421);
            this.btnAbrir.Margin = new System.Windows.Forms.Padding(6);
            this.btnAbrir.Name = "btnAbrir";
            this.btnAbrir.Size = new System.Drawing.Size(172, 44);
            this.btnAbrir.TabIndex = 12;
            this.btnAbrir.Text = "Abrir archivo...";
            this.btnAbrir.UseVisualStyleBackColor = true;
            this.btnAbrir.Visible = false;
            this.btnAbrir.Click += new System.EventHandler(this.btnAbrir_Click);
            // 
            // txtInstrucciones
            // 
            this.txtInstrucciones.Location = new System.Drawing.Point(12, 37);
            this.txtInstrucciones.Margin = new System.Windows.Forms.Padding(6);
            this.txtInstrucciones.MaxLength = 500000;
            this.txtInstrucciones.Multiline = true;
            this.txtInstrucciones.Name = "txtInstrucciones";
            this.txtInstrucciones.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInstrucciones.Size = new System.Drawing.Size(1058, 369);
            this.txtInstrucciones.TabIndex = 0;
            // 
            // frmGenerador
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1154, 994);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmGenerador";
            this.Text = "Generador de Base de Datos";
            this.Load += new System.EventHandler(this.frmGenerador_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbAno;
        private System.Windows.Forms.ComboBox cmbCiclo;
        private System.Windows.Forms.ComboBox cmbVisitador;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkLimpia;
        private System.Windows.Forms.CheckBox chkCicloAbierto;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.ComboBox cmbEmpresa;
        private System.Windows.Forms.Button btnEnviarData;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtInstrucciones;
        private System.Windows.Forms.Button btnAbrir;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.ProgressBar pb;
        private System.Windows.Forms.Label lblVersion;
    }
}

