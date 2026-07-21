using GitFolderExportWinForms.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.ApplicationServices;
using System.Windows.Forms;

namespace GitFolderExportWinForms
{
    public partial class FormMain : Form
    {
        #region Campos

        private readonly BackgroundWorker _worker;
        private string _lastOutDir;
        private List<string> _commitCache = new List<string>();
        private List<string> _authorCache = new List<string>();
        private List<string> _selectedAuthors = new List<string>();

        #endregion

        #region Constructor y carga inicial

        public FormMain()
        {
            InitializeComponent();

            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = false;
            _worker.WorkerSupportsCancellation = false;
            _worker.DoWork += Worker_DoWork;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            txtRepo.Text = Properties.Settings.Default.RepoPath;
            txtOut.Text = Properties.Settings.Default.OutPath;
            txtPath.Text = Properties.Settings.Default.FilterPath;
            RefreshProfiles();
        }

        #endregion

        #region Exportación (BackgroundWorker)

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (_worker.IsBusy) return;

            txtLog.Clear();

            ExportOptions opt = new ExportOptions();
            opt.Repo = txtRepo.Text.Trim();
            opt.From = txtFrom.Text.Trim();
            opt.To = txtTo.Text.Trim();
            opt.PathFilter = txtPath.Text.Trim();
            opt.OutDir = txtOut.Text.Trim();
            btnRun.Enabled = false;
            btnOpenOut.Enabled = false;
            opt.PathFilter = txtPath.Text.Trim();
            opt.OutDir = txtOut.Text.Trim();
            opt.Authors = new List<string>(_selectedAuthors);
            opt.ExcludeCs = chkExcludeCs.Checked;
            _worker.RunWorkerAsync(opt);

            SaveSettings();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ExportOptions opt = (ExportOptions)e.Argument;

            GitExportService svc = new GitExportService();
            int exportedCount = 0;
            svc.OnLog += msg =>
            {
                AppendLog(msg);
                if (msg.StartsWith("Archivos exportados:", StringComparison.OrdinalIgnoreCase))
                {
                    string numPart = msg.Substring("Archivos exportados:".Length).Trim();
                    int.TryParse(numPart, out exportedCount);
                }
            };

            try
            {
                svc.Export(opt);

                HistoryService.Add(new HistoryEntry
                {
                    Timestamp = DateTime.Now,
                    Repo = opt.Repo,
                    From = opt.From,
                    To = opt.To,
                    PathFilter = opt.PathFilter,
                    OutDir = opt.OutDir,
                    Authors = opt.Authors != null && opt.Authors.Count > 0 ? string.Join(", ", opt.Authors) : "",
                    ExcludeCs = opt.ExcludeCs,
                    Exported = exportedCount
                });

                if (!string.IsNullOrWhiteSpace(opt.OutDir))
                    _lastOutDir = Path.GetFullPath(opt.OutDir);

                e.Result = null;
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnRun.Enabled = true;
            btnOpenOut.Enabled = !string.IsNullOrWhiteSpace(_lastOutDir) && Directory.Exists(_lastOutDir);

            Exception ex = e.Result as Exception;
            if (ex != null)
            {
                AppendLog("ERROR: " + ex.Message);
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(this, "Export finalizado. Revisa el log y los manifests en la carpeta de salida.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AppendLog(string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(AppendLog), msg);
                return;
            }

            txtLog.AppendText(msg + Environment.NewLine);

            if (msg.StartsWith("Salida:", StringComparison.OrdinalIgnoreCase))
            {
                string p = msg.Substring("Salida:".Length).Trim();
                if (!string.IsNullOrWhiteSpace(p))
                    _lastOutDir = p;
            }
        }

        #endregion

        #region Selección de carpetas (Repo / Path / Salida)

        private void txtRepo_TextChanged(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (string.IsNullOrEmpty(txtRepo.Text))
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        txtRepo.Text = dlg.SelectedPath;
                    }
                if (ValidateRepo())
                {
                    LoadCommits();
                    LoadAuthors();
                    SetupCommitAutocomplete();
                    SetupAuthorAutocomplete();
                }
            }
        }

        private void btnBrowseRepo_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Selecciona la carpeta del proyecto principal";
                dlg.SelectedPath = string.IsNullOrWhiteSpace(txtRepo.Text) ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) : txtRepo.Text;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtRepo.Text = dlg.SelectedPath;
            }
        }

        private void btnBrowsePath_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRepo.Text) || !Directory.Exists(txtRepo.Text))
            {
                MessageBox.Show(this, "Primero indica una 'Carpeta local proyecto' válida.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Selecciona la carpeta a recuperar (dentro del repositorio)";
                dlg.SelectedPath = txtRepo.Text;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    string relative = GetRelativePath(txtRepo.Text, dlg.SelectedPath);

                    if (relative == null)
                    {
                        MessageBox.Show(this, "La carpeta seleccionada debe estar dentro de 'Carpeta local proyecto'.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    txtPath.Text = relative;
                }
            }
        }

        private void btnBrowseOut_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Selecciona la carpeta de salida (donde quedará el export)";
                dlg.SelectedPath = string.IsNullOrWhiteSpace(txtOut.Text) ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) : txtOut.Text;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtOut.Text = dlg.SelectedPath;
            }
        }

        private void btnOpenOut_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_lastOutDir)) return;
            if (!Directory.Exists(_lastOutDir)) return;

            try
            {
                Process.Start(_lastOutDir);
            }
            catch { }
        }

        private static string GetRelativePath(string basePath, string fullPath)
        {
            string b = Path.GetFullPath(basePath).TrimEnd('\\', '/');
            string f = Path.GetFullPath(fullPath).TrimEnd('\\', '/');

            if (!f.StartsWith(b, StringComparison.OrdinalIgnoreCase))
                return null;

            if (f.Equals(b, StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            string rel = f.Substring(b.Length).TrimStart('\\', '/');
            return rel.Replace('\\', '/');
        }

        #endregion

        #region Git (validación, commits y autores)

        private bool ValidateRepo()
        {
            try
            {
                string err;

                Directory.SetCurrentDirectory(txtRepo.Text);

                string inside = GitRunner.RunText("rev-parse --is-inside-work-tree", out err);

                return inside != null &&
                       inside.Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private void LoadCommits()
        {
            try
            {
                string err;

                string txt = GitRunner.RunText("log --oneline --abbrev=8 -1000", out err);

                if (string.IsNullOrWhiteSpace(txt))
                    return;

                _commitCache = txt
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 7)
                    .Select(x => x.Split(' ')[0])
                    .ToList();
            }
            catch
            {
                _commitCache.Clear();
            }
        }

        private void SetupCommitAutocomplete()
        {
            if (_commitCache == null || _commitCache.Count == 0)
                return;

            var src = new AutoCompleteStringCollection();

            foreach (var c in _commitCache)
                src.Add(c);

            txtFrom.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtFrom.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtFrom.AutoCompleteCustomSource = src;

            txtTo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtTo.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtTo.AutoCompleteCustomSource = src;
        }

        private void LoadAuthors()
        {
            try
            {
                string err;

                string txt = GitRunner.RunText("log --format=\"%an\" -1000", out err);

                if (string.IsNullOrWhiteSpace(txt))
                    return;

                _authorCache = txt
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x)
                    .ToList();
            }
            catch
            {
                _authorCache.Clear();
            }
        }

        private void SetupAuthorAutocomplete()
        {
            if (_authorCache == null || _authorCache.Count == 0)
                return;

            var src = new AutoCompleteStringCollection();

            foreach (var a in _authorCache)
                src.Add(a);

            txtAuthor.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtAuthor.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtAuthor.AutoCompleteCustomSource = src;
        }

        private void btnSelectAuthors_Click(object sender, EventArgs e)
        {
            if (_authorCache == null || _authorCache.Count == 0)
            {
                MessageBox.Show(this, "No hay autores cargados. Verifica que 'Carpeta local proyecto' sea un repo válido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (Form dlg = new Form())
            {
                dlg.Text = "Seleccionar autor(es)";
                dlg.Width = 400;
                dlg.Height = 450;
                dlg.StartPosition = FormStartPosition.CenterParent;

                CheckedListBox clb = new CheckedListBox();
                clb.Dock = DockStyle.Fill;
                clb.CheckOnClick = true;
                foreach (string a in _authorCache)
                {
                    int idx = clb.Items.Add(a);
                    if (_selectedAuthors.Contains(a))
                        clb.SetItemChecked(idx, true);
                }

                Panel panelBotones = new Panel();
                panelBotones.Dock = DockStyle.Bottom;
                panelBotones.Height = 40;

                Button btnOk = new Button { Text = "Aceptar", DialogResult = DialogResult.OK, Left = 210, Top = 5, Width = 80 };
                Button btnCancel = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Left = 300, Top = 5, Width = 80 };
                panelBotones.Controls.Add(btnOk);
                panelBotones.Controls.Add(btnCancel);

                dlg.Controls.Add(clb);
                dlg.Controls.Add(panelBotones);
                dlg.AcceptButton = btnOk;
                dlg.CancelButton = btnCancel;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _selectedAuthors = clb.CheckedItems.Cast<string>().ToList();
                    txtAuthor.Text = _selectedAuthors.Count == 0
                        ? ""
                        : (_selectedAuthors.Count == 1 ? _selectedAuthors[0] : _selectedAuthors.Count + " autores seleccionados");
                }
            }
        }

        #endregion

        #region Perfiles

        private void RefreshProfiles()
        {
            cmbProfiles.DataSource = null;
            cmbProfiles.DataSource = GitFolderExportWinForms.Core.ProfileService.Load();
            cmbProfiles.DisplayMember = "Name";
            cmbProfiles.SelectedIndex = -1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var p = cmbProfiles.SelectedItem as ExportProfile;
            if (p == null) return;
            txtRepo.Text = p.Repo;
            txtPath.Text = p.PathFilter;
            txtOut.Text = p.OutDir;
        }

        private void btnSaveProfile_Click(object sender, EventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox(
            "Nombre del perfil:", "Guardar perfil", "");
            if (string.IsNullOrWhiteSpace(name)) return;

            GitFolderExportWinForms.Core.ProfileService.Save(new ExportProfile
            {
                Name = name.Trim(),
                Repo = txtRepo.Text.Trim(),
                PathFilter = txtPath.Text.Trim(),
                OutDir = txtOut.Text.Trim()
            });
            RefreshProfiles();
            cmbProfiles.SelectedIndex = cmbProfiles.FindStringExact(name.Trim());
        }

        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            var p = cmbProfiles.SelectedItem as ExportProfile;
            if (p == null) return;
            if (MessageBox.Show("¿Eliminar el perfil '" + p.Name + "'?", "Confirmar",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                GitFolderExportWinForms.Core.ProfileService.Delete(p.Name);
                RefreshProfiles();
            }
        }

        #endregion

        #region Historial

        private void btnHistory_Click(object sender, EventArgs e)
        {
            List<HistoryEntry> history = HistoryService.Load();

            if (history.Count == 0)
            {
                MessageBox.Show(this, "Aún no hay historial de exportaciones.", "Historial", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Form dlg = new Form();
            dlg.Text = "Historial de exportaciones (últimas " + history.Count + ")";
            dlg.Width = 700;
            dlg.Height = 400;
            dlg.StartPosition = FormStartPosition.CenterParent;

            ListBox lb = new ListBox();
            lb.Dock = DockStyle.Fill;
            lb.Font = new Font("Consolas", 9);
            foreach (HistoryEntry h in history)
                lb.Items.Add(h.ToString());

            dlg.Controls.Add(lb);

            dlg.FormClosed += (s, args) => dlg.Dispose();

            dlg.Show(this);
        }

        #endregion

        #region Ayuda

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string mensaje =
                "GUÍA DE USO — GitFolderExport\r\n" +
                "────────────────────────────────────\r\n\r\n" +
                "Carpeta local proyecto:\r\n" +
                "  Ruta local del repositorio Git (donde está la carpeta .git, esta es una carpeta oculta habilitar visibilidad en caso de no encontrarla).\r\n" +
                "Desde changeset / Hasta:\r\n" +
                "  Hash del commit inicial (exclusivo) y final (inclusivo).\r\n" +
                "  Se autocompletan al escribir, tomados del historial del repo.\r\n" +
                "  Ej: Desde = 13659416   Hasta = eaed80b9\r\n\r\n" +
                "Autor (opcional):\r\n" +
                "  Filtra solo commits de ese autor. Puedes escribir nombre parcial\r\n" +
                "  o completo (se busca como coincidencia flexible).\r\n" +
                "Carpeta a recuperar:\r\n" +
                "  Subcarpeta DENTRO del repositorio que quieres exportar,\r\n" +
                "  relativa a 'Carpeta local proyecto'. Usa el botón 'Buscar...'\r\n" +
                "  para seleccionarla sin errores de ruta.\r\n" +
                "Carpeta local salida:\r\n" +
                "  Dónde se guardarán los archivos exportados. Si ya existe,\r\n" +
                "  su contenido se borrara y regenerara.\r\n\r\n" +
                "Excluir archivos .cs:\r\n" +
                "  Si se marca, no se exportan archivos .cs (útil si solo\r\n" +
                "  necesitas los binarios ya compilados en /bin).\r\n\r\n" +
                "Ver últimos cambios obtenidos:\r\n" +
                "  Muestra el historial de tus últimas exportaciones (rango,\r\n" +
                "  carpeta, autor y cantidad de archivos), sin volver a exportar.\r\n\r\n" +
                "EJEMPLO COMPLETO:\r\n" +
                "  Carpeta local proyecto:  C:\\Tuproyecto\r\n" +
                "  Desde changeset:         13659416\r\n" +
                "  Hasta:                   eaed80b9\r\n" +
                "  Carpeta a recuperar:     Carpetapertenecientealproyecto\r\n" +
                "  Carpeta local salida:    C:\\Users\\...\\Downloads\\Export1";

            MessageBox.Show(this, mensaje, "Ayuda — Cómo usar esta herramienta",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Handlers vacíos (generados por el diseñador)

        private void lblPath_Click(object sender, EventArgs e)
        {

        }

        private void lblOut_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void SaveSettings()
        {
            Properties.Settings.Default.RepoPath = txtRepo.Text;
            Properties.Settings.Default.OutPath = txtOut.Text;
            Properties.Settings.Default.FilterPath = txtPath.Text;

            Properties.Settings.Default.Save();
        }
        #endregion
    }
}