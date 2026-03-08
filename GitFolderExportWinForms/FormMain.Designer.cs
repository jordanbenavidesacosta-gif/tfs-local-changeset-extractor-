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
            this.txtAuthor = new System.Windows.Forms.TextBox();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblRepo
            // 
            this.lblRepo.AutoSize = true;
            this.lblRepo.Location = new System.Drawing.Point(12, 43);
            this.lblRepo.Name = "lblRepo";
            this.lblRepo.Size = new System.Drawing.Size(113, 13);
            this.lblRepo.TabIndex = 0;
            this.lblRepo.Text = "Carpeta local proyecto";
            // 
            // txtRepo
            // 
            this.txtRepo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRepo.Location = new System.Drawing.Point(131, 40);
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
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(12, 69);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(91, 13);
            this.lblFrom.TabIndex = 3;
            this.lblFrom.Text = "Desde changeset";
            // 
            // txtFrom
            // 
            this.txtFrom.Location = new System.Drawing.Point(131, 66);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(180, 20);
            this.txtFrom.TabIndex = 4;
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(317, 69);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(35, 13);
            this.lblTo.TabIndex = 5;
            this.lblTo.Text = "Hasta";
            // 
            // txtTo
            // 
            this.txtTo.Location = new System.Drawing.Point(358, 66);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(210, 20);
            this.txtTo.TabIndex = 6;
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(4, 105);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(145, 13);
            this.lblPath.TabIndex = 7;
            this.lblPath.Text = "Ruta (ej: EquiSoftIncomercio)";
            this.lblPath.Click += new System.EventHandler(this.lblPath_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(155, 102);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(535, 20);
            this.txtPath.TabIndex = 8;
            // 
            // lblOut
            // 
            this.lblOut.AutoSize = true;
            this.lblOut.Location = new System.Drawing.Point(4, 131);
            this.lblOut.Name = "lblOut";
            this.lblOut.Size = new System.Drawing.Size(74, 13);
            this.lblOut.TabIndex = 9;
            this.lblOut.Text = "Carpeta salida";
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
            this.btnBrowseOut.Location = new System.Drawing.Point(696, 97);
            this.btnBrowseOut.Name = "btnBrowseOut";
            this.btnBrowseOut.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseOut.TabIndex = 11;
            this.btnBrowseOut.Text = "Buscar...";
            this.btnBrowseOut.UseVisualStyleBackColor = true;
            this.btnBrowseOut.Click += new System.EventHandler(this.btnBrowseOut_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(696, 125);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 16;
            this.btnRun.Text = "Exportar";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnOpenOut
            // 
            this.btnOpenOut.Location = new System.Drawing.Point(696, 154);
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
            // txtAuthor
            // 
            this.txtAuthor.Location = new System.Drawing.Point(627, 66);
            this.txtAuthor.Name = "txtAuthor";
            this.txtAuthor.Size = new System.Drawing.Size(142, 20);
            this.txtAuthor.TabIndex = 20;
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Location = new System.Drawing.Point(589, 69);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(32, 13);
            this.lblAuthor.TabIndex = 21;
            this.lblAuthor.Text = "Autor";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 519);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.txtAuthor);
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
        private System.Windows.Forms.TextBox txtAuthor;
        private System.Windows.Forms.Label lblAuthor;
    }
}
