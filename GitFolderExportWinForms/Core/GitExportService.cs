using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GitFolderExportWinForms.Core
{
    internal sealed class GitExportService
    {
        public event Action<string> OnLog;

        public void Export(ExportOptions opt)
        {
            Validate(opt);

            string repo = Path.GetFullPath(opt.Repo);
            Directory.SetCurrentDirectory(repo);

            string err;
            string inside = GitRunner.RunText("rev-parse --is-inside-work-tree", out err);
            if (inside == null || !inside.Trim().Equals("true", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("La ruta no parece ser un repositorio Git.\n" + err);

            if (opt.Fetch)
            {
                Log("git fetch --all --prune");
                string _ = GitRunner.RunText("fetch --all --prune", out err);
                if (_ == null) throw new InvalidOperationException("Falló git fetch.\n" + err);
            }

            opt.PathFilter = (opt.PathFilter ?? "").Trim().TrimStart('\\', '/');
            if (string.IsNullOrWhiteSpace(opt.PathFilter))
                throw new InvalidOperationException("Debe indicar una carpeta en 'Path' (ej: EquiSoftIncomercio).");

            if (string.IsNullOrWhiteSpace(opt.OutDir))
            {
                string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
                opt.OutDir = Path.Combine(repo, "_export_" + stamp);
            }

            string outRoot = Path.GetFullPath(opt.OutDir);

            if (Directory.Exists(outRoot))
            {
                Directory.Delete(outRoot, true);
            }

            Directory.CreateDirectory(outRoot);

            Log("Repo:   " + repo);
            Log("Rango:  " + opt.From + " .. " + opt.To);
            Log("Filtro: " + opt.PathFilter);
            Log("Salida: " + outRoot);

            List<DiffEntry> changes = GetChangedFiles(opt);
            if (changes.Count == 0)
            {
                Log("No se encontraron cambios en esa ruta.");
                return;
            }

            string manifestPath = Path.Combine(outRoot, "manifest_changes.txt");
            using (StreamWriter sw = new StreamWriter(manifestPath, false))
            {
                sw.WriteLine("GitFolderExportWinForms");
                sw.WriteLine("Repo:  " + repo);
                sw.WriteLine("From:  " + opt.From);
                sw.WriteLine("To:    " + opt.To);
                sw.WriteLine("Path:  " + opt.PathFilter);
                sw.WriteLine("Fecha: " + DateTime.Now.ToString("s", CultureInfo.InvariantCulture));
                sw.WriteLine();
                sw.WriteLine("STATUS\tPATH");
                foreach (DiffEntry d in changes)
                    sw.WriteLine(d.Status + "\t" + d.Path);
            }

            int exported = 0;
            List<DiffEntry> deleted = new List<DiffEntry>();

            foreach (DiffEntry d in changes)
            {
                if (d.Status == "D")
                {
                    deleted.Add(d);
                    continue;
                }

                if (!opt.IncludeBinTracked && IsBinPath(opt.PathFilter, d.Path))
                    continue;

                byte[] content = GitRunner.RunBytes("show " + EscapeRef(opt.To) + ":" + EscapeGitPath(d.Path), out err);
                if (content == null)
                {
                    deleted.Add(new DiffEntry { Status = "?", Path = d.Path });
                    continue;
                }

                string outFile = Path.Combine(outRoot, d.Path.Replace('/', Path.DirectorySeparatorChar));
                string outFolder = Path.GetDirectoryName(outFile);
                if (!Directory.Exists(outFolder))
                    Directory.CreateDirectory(outFolder);

                File.WriteAllBytes(outFile, content);
                exported++;
            }

            if (opt.CopyWorkingBin)
            {
                string binSrc = Path.Combine(repo, opt.PathFilter.Replace('/', Path.DirectorySeparatorChar), "bin");
                if (Directory.Exists(binSrc))
                {
                    Log("Copiando bin desde working tree: " + binSrc);
                    string binDst = Path.Combine(outRoot, opt.PathFilter.Replace('/', Path.DirectorySeparatorChar), "bin");
                    CopyDirectory(binSrc, binDst);
                }
                else
                {
                    Log("No existe bin en working tree: " + binSrc);
                }
            }

            if (deleted.Count > 0)
            {
                string delPath = Path.Combine(outRoot, "manifest_deleted.txt");
                File.WriteAllLines(delPath, deleted.Select(x => x.Status + "\t" + x.Path).ToArray());
            }

            Log("Archivos exportados: " + exported.ToString(CultureInfo.InvariantCulture));
            Log("Manifest: " + manifestPath);
            Log("Listo.");
        }

        private static void Validate(ExportOptions opt)
        {
            if (opt == null) throw new ArgumentNullException("opt");
            if (string.IsNullOrWhiteSpace(opt.Repo)) throw new InvalidOperationException("Debe indicar Repo.");
            if (!Directory.Exists(opt.Repo)) throw new InvalidOperationException("Repo no existe: " + opt.Repo);
            if (string.IsNullOrWhiteSpace(opt.From)) throw new InvalidOperationException("Debe indicar From.");
            if (string.IsNullOrWhiteSpace(opt.To)) throw new InvalidOperationException("Debe indicar To.");
        }

        private static bool IsBinPath(string root, string fullPath)
        {
            string p = (fullPath ?? "").Replace('\\', '/');
            string r = (root ?? "").Replace('\\', '/').Trim('/');
            if (!p.StartsWith(r + "/", StringComparison.OrdinalIgnoreCase))
                return false;

            return p.IndexOf("/bin/", StringComparison.OrdinalIgnoreCase) >= 0
                || p.EndsWith("/bin", StringComparison.OrdinalIgnoreCase);
        }

        private List<DiffEntry> GetChangedFiles(ExportOptions opt)
        {
            string renameArg = opt.NoRenames ? " --no-renames" : " -M";

            string cmd;

            if (!string.IsNullOrWhiteSpace(opt.Author))
            {
                cmd = "log " + EscapeRef(opt.From) + ".." + EscapeRef(opt.To)
                      + " --author=\"" + opt.Author.Replace("\"", "") + "\""
                      + " --name-status --pretty=format: "
                      + " -- " + EscapeGitPath(opt.PathFilter);
            }
            else
            {
                string range;

                if (opt.From.Equals(opt.To, StringComparison.OrdinalIgnoreCase))
                {
                    range = EscapeRef(opt.From) + "^.." + EscapeRef(opt.To);
                }
                else
                {
                    range = EscapeRef(opt.From) + ".." + EscapeRef(opt.To);
                }

                cmd = "diff --name-status" + renameArg + " "
                      + range
                      + " -- " + EscapeGitPath(opt.PathFilter);
            }

            string err;
            string outText = GitRunner.RunText(cmd, out err);
            if (string.IsNullOrWhiteSpace(outText))
                return new List<DiffEntry>();

            List<DiffEntry> list = new List<DiffEntry>();

            foreach (string rawLine in outText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string line = rawLine.Trim();
                if (line.Length == 0) continue;

                string[] parts = line.Split('\t');
                if (parts.Length < 2) continue;

                string st = parts[0].Trim();

                if (st.StartsWith("R", StringComparison.OrdinalIgnoreCase) && parts.Length >= 3)
                {
                    string oldPath = parts[1].Trim().Replace('\\', '/');
                    string newPath = parts[2].Trim().Replace('\\', '/');

                    list.Add(new DiffEntry { Status = "R", Path = newPath });
                    list.Add(new DiffEntry { Status = "D", Path = oldPath });
                }
                else
                {
                    string path = parts[1].Trim().Replace('\\', '/');
                    string s1 = st.Length > 0 ? st.Substring(0, 1).ToUpperInvariant() : "?";
                    list.Add(new DiffEntry { Status = s1, Path = path });
                }
            }

            return list
                .GroupBy(x => x.Status + "\u0001" + x.Path, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();
        }

        private static string EscapeRef(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            bool ok = s.All(ch =>
                (ch >= 'a' && ch <= 'z') ||
                (ch >= 'A' && ch <= 'Z') ||
                (ch >= '0' && ch <= '9') ||
                ch == '/' || ch == '-' || ch == '_' || ch == '.' || ch == '^' || ch == '~');

            if (ok) return s;
            return "\"" + s.Replace("\"", "") + "\"";
        }

        private static string EscapeGitPath(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Replace('\\', '/');
        }

        private static void CopyDirectory(string sourceDir, string destDir)
        {
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destDir, name);
                File.Copy(file, dest, true);
            }

            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                string name = Path.GetFileName(dir);
                string dest = Path.Combine(destDir, name);
                CopyDirectory(dir, dest);
            }
        }

        private void Log(string msg)
        {
            if (OnLog != null) OnLog(msg);
        }
    }
}
