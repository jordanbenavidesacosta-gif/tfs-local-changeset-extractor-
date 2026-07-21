namespace GitFolderExportWinForms
{
    partial class FormMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblRepo = new System.Windows.Forms.Label();
            this.txtRepo = new System.Windows.Forms.TextBox();
            this.btnBrowseRepo = new System.Windows.Forms.Button();
            this.lblFrom = new System.Windows.Forms.Label();
            this.txtFrom = new System.Windows.Forms.TextBox();
            this.lblTo = new System.Windows.Forms.Label();
            this.txtTo = new System.Windows.Forms.TextBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lblOut = new System.Windows.Forms.Label();
            this.txtOut = new System.Windows.Forms.TextBox();
            this.btnBrowseOut = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnOpenOut = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblLog = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.chkExcludeCs = new System.Windows.Forms.CheckBox();
            this.btnBrowsePath = new System.Windows.Forms.Button();
            this.btnHistory = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnSelectAuthors = new System.Windows.Forms.Button();
            this.txtAuthor = new System.Windows.Forms.TextBox();
            this.cmbProfiles = new System.Windows.Forms.ComboBox();
            this.Perfil = new System.Windows.Forms.Label();
            this.btnDeleteProfile = new System.Windows.Forms.Button();
            this.btnSaveProfile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblRepo
            // 
            this.lblRepo.AutoSize = true;
            this.lblRepo.Location = new System.Drawing.Point(4, 42);
            this.lblRepo.Name = "lblRepo";
            this.lblRepo.Size = new System.Drawing.Size(113, 13);
            this.lblRepo.TabIndex = 0;
            this.lblRepo.Text = "Carpeta local proyecto";
            // 
            // txtRepo
            // 
            this.txtRepo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRepo.Location = new System.Drawing.Point(123, 40);
            this.txtRepo.Name = "txtRepo";
            this.txtRepo.Size = new System.Drawing.Size(559, 20);
            this.txtRepo.TabIndex = 1;
            this.txtRepo.TextChanged += new System.EventHandler(this.txtRepo_TextChanged);
            // 
            // btnBrowseRepo
            // 
            this.btnBrowseRepo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseRepo.Location = new System.Drawing.Point(696, 37);
            this.btnBrowseRepo.Name = "btnBrowseRepo";
            this.btnBrowseRepo.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseRepo.TabIndex = 2;
            this.btnBrowseRepo.Text = "Buscar...";
            this.btnBrowseRepo.UseVisualStyleBackColor = true;
            this.btnBrowseRepo.Click += new System.EventHandler(this.btnBrowseRepo_Click);
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(4, 74);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(91, 13);
            this.lblFrom.TabIndex = 3;
            this.lblFrom.Text = "Desde changeset";
            // 
            // txtFrom
            // 
            this.txtFrom.Location = new System.Drawing.Point(108, 71);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(164, 20);
            this.txtFrom.TabIndex = 4;
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(278, 74);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(35, 13);
            this.lblTo.TabIndex = 5;
            this.lblTo.Text = "Hasta";
            // 
            // txtTo
            // 
            this.txtTo.Location = new System.Drawing.Point(319, 71);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(194, 20);
            this.txtTo.TabIndex = 6;
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(4, 105);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(101, 13);
            this.lblPath.TabIndex = 7;
            this.lblPath.Text = "Carpeta a recuperar";
            this.lblPath.Click += new System.EventHandler(this.lblPath_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(108, 102);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(582, 20);
            this.txtPath.TabIndex = 8;
            // 
            // lblOut
            // 
            this.lblOut.AutoSize = true;
            this.lblOut.Location = new System.Drawing.Point(4, 131);
            this.lblOut.Name = "lblOut";
            this.lblOut.Size = new System.Drawing.Size(99, 13);
            this.lblOut.TabIndex = 9;
            this.lblOut.Text = "Carpeta local salida";
            this.lblOut.Click += new System.EventHandler(this.lblOut_Click);
            // 
            // txtOut
            // 
            this.txtOut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOut.Location = new System.Drawing.Point(108, 128);
            this.txtOut.Name = "txtOut";
            this.txtOut.Size = new System.Drawing.Size(582, 20);
            this.txtOut.TabIndex = 10;
            // 
            // btnBrowseOut
            // 
            this.btnBrowseOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseOut.Location = new System.Drawing.Point(696, 126);
            this.btnBrowseOut.Name = "btnBrowseOut";
            this.btnBrowseOut.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseOut.TabIndex = 11;
            this.btnBrowseOut.Text = "Buscar...";
            this.btnBrowseOut.UseVisualStyleBackColor = true;
            this.btnBrowseOut.Click += new System.EventHandler(this.btnBrowseOut_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(615, 169);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 16;
            this.btnRun.Text = "Exportar";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnOpenOut
            // 
            this.btnOpenOut.Location = new System.Drawing.Point(694, 169);
            this.btnOpenOut.Name = "btnOpenOut";
            this.btnOpenOut.Size = new System.Drawing.Size(75, 23);
            this.btnOpenOut.TabIndex = 17;
            this.btnOpenOut.Text = "Abrir Out";
            this.btnOpenOut.UseVisualStyleBackColor = true;
            this.btnOpenOut.Click += new System.EventHandler(this.btnOpenOut_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(15, 207);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(756, 300);
            this.txtLog.TabIndex = 19;
            this.txtLog.WordWrap = false;
            // 
            // lblLog
            // 
            this.lblLog.AutoSize = true;
            this.lblLog.Location = new System.Drawing.Point(12, 191);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(25, 13);
            this.lblLog.TabIndex = 18;
            this.lblLog.Text = "Log";
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Location = new System.Drawing.Point(519, 74);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(32, 13);
            this.lblAuthor.TabIndex = 21;
            this.lblAuthor.Text = "Autor";
            // 
            // chkExcludeCs
            // 
            this.chkExcludeCs.AutoSize = true;
            this.chkExcludeCs.Location = new System.Drawing.Point(293, 173);
            this.chkExcludeCs.Name = "chkExcludeCs";
            this.chkExcludeCs.Size = new System.Drawing.Size(114, 17);
            this.chkExcludeCs.TabIndex = 22;
            this.chkExcludeCs.Text = "Excluir archivos cs";
            this.chkExcludeCs.UseVisualStyleBackColor = true;
            this.chkExcludeCs.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // btnBrowsePath
            // 
            this.btnBrowsePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowsePath.Location = new System.Drawing.Point(696, 99);
            this.btnBrowsePath.Name = "btnBrowsePath";
            this.btnBrowsePath.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnBrowsePath.Size = new System.Drawing.Size(75, 23);
            this.btnBrowsePath.TabIndex = 23;
            this.btnBrowsePath.Text = "Buscar...";
            this.btnBrowsePath.UseVisualStyleBackColor = true;
            this.btnBrowsePath.Click += new System.EventHandler(this.btnBrowsePath_Click);
            // 
            // btnHistory
            // 
            this.btnHistory.Location = new System.Drawing.Point(413, 169);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(196, 23);
            this.btnHistory.TabIndex = 24;
            this.btnHistory.Text = "Ver registro ultimos cambios obtenidos";
            this.btnHistory.UseVisualStyleBackColor = true;
            this.btnHistory.Click += new System.EventHandler(this.btnHistory_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.Location = new System.Drawing.Point(7, 12);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 25;
            this.btnHelp.Text = "Guia de uso";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnSelectAuthors
            // 
            this.btnSelectAuthors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectAuthors.Location = new System.Drawing.Point(661, 69);
            this.btnSelectAuthors.Name = "btnSelectAuthors";
            this.btnSelectAuthors.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnSelectAuthors.Size = new System.Drawing.Size(110, 23);
            this.btnSelectAuthors.TabIndex = 26;
            this.btnSelectAuthors.Text = "Seleccionar autores";
            this.btnSelectAuthors.UseVisualStyleBackColor = true;
            this.btnSelectAuthors.Click += new System.EventHandler(this.btnSelectAuthors_Click);
            // 
            // txtAuthor
            // 
            this.txtAuthor.Location = new System.Drawing.Point(557, 71);
            this.txtAuthor.Name = "txtAuthor";
            this.txtAuthor.Size = new System.Drawing.Size(98, 20);
            this.txtAuthor.TabIndex = 27;
            // 
            // cmbProfiles
            // 
            this.cmbProfiles.FormattingEnabled = true;
            this.cmbProfiles.Location = new System.Drawing.Point(192, 14);
            this.cmbProfiles.Name = "cmbProfiles";
            this.cmbProfiles.Size = new System.Drawing.Size(121, 21);
            this.cmbProfiles.TabIndex = 28;
            this.cmbProfiles.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // Perfil
            // 
            this.Perfil.AutoSize = true;
            this.Perfil.Location = new System.Drawing.Point(120, 17);
            this.Perfil.Name = "Perfil";
            this.Perfil.Size = new System.Drawing.Size(63, 13);
            this.Perfil.TabIndex = 29;
            this.Perfil.Text = "Perfil Actual";
            this.Perfil.Click += new System.EventHandler(this.label1_Click);
            // 
            // btnDeleteProfile
            // 
            this.btnDeleteProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteProfile.Location = new System.Drawing.Point(332, 12);
            this.btnDeleteProfile.Name = "btnDeleteProfile";
            this.btnDeleteProfile.Size = new System.Drawing.Size(89, 23);
            this.btnDeleteProfile.TabIndex = 30;
            this.btnDeleteProfile.Text = "Eliminar Perfil";
            this.btnDeleteProfile.UseVisualStyleBackColor = true;
            this.btnDeleteProfile.Click += new System.EventHandler(this.btnDeleteProfile_Click);
            // 
            // btnSaveProfile
            // 
            this.btnSaveProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveProfile.Location = new System.Drawing.Point(427, 12);
            this.btnSaveProfile.Name = "btnSaveProfile";
            this.btnSaveProfile.Size = new System.Drawing.Size(89, 23);
            this.btnSaveProfile.TabIndex = 31;
            this.btnSaveProfile.Text = "Guardar Perfil";
            this.btnSaveProfile.UseVisualStyleBackColor = true;
            this.btnSaveProfile.Click += new System.EventHandler(this.btnSaveProfile_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 519);
            this.Controls.Add(this.btnSaveProfile);
            this.Controls.Add(this.btnDeleteProfile);
            this.Controls.Add(this.Perfil);
            this.Controls.Add(this.cmbProfiles);
            this.Controls.Add(this.txtAuthor);
            this.Controls.Add(this.btnSelectAuthors);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnHistory);
            this.Controls.Add(this.btnBrowsePath);
            this.Controls.Add(this.chkExcludeCs);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.btnOpenOut);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnBrowseOut);
            this.Controls.Add(this.txtOut);
            this.Controls.Add(this.lblOut);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.txtTo);
            this.Controls.Add(this.lblTo);
            this.Controls.Add(this.txtFrom);
            this.Controls.Add(this.lblFrom);
            this.Controls.Add(this.btnBrowseRepo);
            this.Controls.Add(this.txtRepo);
            this.Controls.Add(this.lblRepo);
            this.MinimumSize = new System.Drawing.Size(800, 450);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GitFolderExport - WinForms (.NET 4.8)";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblRepo;
        private System.Windows.Forms.TextBox txtRepo;
        private System.Windows.Forms.Button btnBrowseRepo;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.TextBox txtFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.TextBox txtTo;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label lblOut;
        private System.Windows.Forms.TextBox txtOut;
        private System.Windows.Forms.Button btnBrowseOut;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnOpenOut;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.CheckBox chkExcludeCs;
        private System.Windows.Forms.Button btnBrowsePath;
        private System.Windows.Forms.Button btnHistory;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnSelectAuthors;
        private System.Windows.Forms.TextBox txtAuthor;
        private System.Windows.Forms.ComboBox cmbProfiles;
        private System.Windows.Forms.Label Perfil;
        private System.Windows.Forms.Button btnDeleteProfile;
        private System.Windows.Forms.Button btnSaveProfile;
    }
}
