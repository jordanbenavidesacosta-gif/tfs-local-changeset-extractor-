using GitFolderExportWinForms.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GitFolderExportWinForms
{
    public partial class FormMain : Form
    {
        private readonly BackgroundWorker _worker;
        private string _lastOutDir;
        private List<string> _commitCache = new List<string>();
        private List<string> _authorCache = new List<string>();
        public FormMain()
        {
            InitializeComponent();

            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = false;
            _worker.WorkerSupportsCancellation = false;
            _worker.DoWork += Worker_DoWork;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

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
            opt.Author = txtAuthor.Text?.Trim();
            btnRun.Enabled = false;
            btnOpenOut.Enabled = false;

            _worker.RunWorkerAsync(opt);

            SaveSettings();
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

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ExportOptions opt = (ExportOptions)e.Argument;

            GitExportService svc = new GitExportService();
            svc.OnLog += msg => AppendLog(msg);

            try
            {
                svc.Export(opt);

                string outDir = opt.OutDir;
                if (string.IsNullOrWhiteSpace(outDir))
                {
                    outDir = Path.Combine(Path.GetFullPath(opt.Repo), "_export_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
                }

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

        private void FormMain_Load(object sender, EventArgs e)
        {
            txtRepo.Text = Properties.Settings.Default.RepoPath;
            txtOut.Text = Properties.Settings.Default.OutPath;
            txtPath.Text = Properties.Settings.Default.FilterPath;

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
                    .Select(x => x.Split(' ')[0]) // solo hash
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
        private void SaveSettings()
        {
            Properties.Settings.Default.RepoPath = txtRepo.Text;
            Properties.Settings.Default.OutPath = txtOut.Text;
            Properties.Settings.Default.FilterPath = txtPath.Text;

            Properties.Settings.Default.Save();
        }

        private void lblPath_Click(object sender, EventArgs e)
        {

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
    }
}
