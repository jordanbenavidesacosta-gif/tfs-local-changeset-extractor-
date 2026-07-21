using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GitFolderExportWinForms.Core
{
    internal sealed class GitExportService
    {
        #region Eventos y campos estáticos

        public event Action<string> OnLog;

        private static readonly Regex ReAppSetting = new Regex(
            "<add\\s+key\\s*=\\s*\"(?<key>[^\"]*)\"\\s+value\\s*=\\s*\"(?<value>[^\"]*)\"[^>]*/?>",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex ReConnStr = new Regex(
            "<add\\s+name\\s*=\\s*\"(?<key>[^\"]*)\"\\s+connectionString\\s*=\\s*\"(?<value>[^\"]*)\"[^>]*/?>",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex RePackage = new Regex(
            "<package\\s+[^>]*?id\\s*=\\s*\"(?<id>[^\"]*)\"[^>]*?version\\s*=\\s*\"(?<version>[^\"]*)\"[^>]*?/?>",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex ReAssemblyName = new Regex(
            "<AssemblyName>\\s*(?<name>[^<]+?)\\s*</AssemblyName>",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        #region Método público (punto de entrada)

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

            List<DiffEntry> excluidosCs = new List<DiffEntry>();
            if (opt.ExcludeCs)
            {
                excluidosCs = changes.Where(x => x.Status != "D" && IsCsFile(x.Path)).ToList();
                if (excluidosCs.Count > 0)
                    Log("Excluyendo " + excluidosCs.Count.ToString(CultureInfo.InvariantCulture) + " archivo(s) .cs");
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
                var relevantes = changes.Where(x => !IsProjectFile(x.Path)).ToList();

                var vaEnDll = relevantes.Where(x => IsCodeFile(x.Path)).ToList();
                var despliegaAparte = relevantes.Where(x => !IsCodeFile(x.Path)).ToList();

                sw.WriteLine("========================================");
                sw.WriteLine("SE DESPLIEGA APARTE (" + despliegaAparte.Count + ") - archivos sueltos");
                sw.WriteLine("========================================");
                EscribirPorEstado(sw, despliegaAparte);

                sw.WriteLine("========================================");
                sw.WriteLine("VA EN LA DLL (" + vaEnDll.Count + ") - código, no se sube suelto");
                sw.WriteLine("========================================");
                EscribirPorEstado(sw, vaEnDll);

                if (excluidosCs.Count > 0)
                {
                    sw.WriteLine();
                    sw.WriteLine("EXCLUIDOS (.cs):");
                    foreach (DiffEntry d in excluidosCs)
                        sw.WriteLine(d.Status + "\t" + d.Path);
                }
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

                if (opt.ExcludeCs && IsCsFile(d.Path))
                    continue;

                if (IsPackagesConfig(d.Path))
                {
                    string baseRef = opt.From.Equals(opt.To, StringComparison.OrdinalIgnoreCase)
                        ? opt.From + "^"
                        : opt.From;

                    string oldText = d.Status == "A" ? "" : TextoDeGit(baseRef, d.Path);
                    string newText = TextoDeGit(opt.To, d.Path);

                    string reporte = BuildPackagesReport(oldText, newText, MapStatus(d.Status), d.Path);
                    string reportFile = Path.Combine(outRoot,
                        d.Path.Replace('/', Path.DirectorySeparatorChar) + ".paquetes.txt");
                    string reportFolder = Path.GetDirectoryName(reportFile);
                    if (!Directory.Exists(reportFolder))
                        Directory.CreateDirectory(reportFolder);

                    File.WriteAllText(reportFile, reporte, Encoding.UTF8);
                    Log("packages.config -> reporte de paquetes: " + d.Path);
                    exported++;
                    continue;
                }

                if (IsConfigCandidate(d.Path))
                {
                    string baseRef = opt.From.Equals(opt.To, StringComparison.OrdinalIgnoreCase)
                        ? opt.From + "^"
                        : opt.From;

                    string oldText = d.Status == "A" ? "" : TextoDeGit(baseRef, d.Path);
                    string newText = TextoDeGit(opt.To, d.Path);

                    bool tieneKeys = ParseConfigEntries(oldText).Count > 0
                                  || ParseConfigEntries(newText).Count > 0;

                    if (tieneKeys)
                    {
                        string reporte = BuildConfigReport(oldText, newText, MapStatus(d.Status), d.Path);
                        string reportFile = Path.Combine(outRoot,
                            d.Path.Replace('/', Path.DirectorySeparatorChar) + ".cambios.txt");
                        string reportFolder = Path.GetDirectoryName(reportFile);
                        if (!Directory.Exists(reportFolder))
                            Directory.CreateDirectory(reportFolder);

                        File.WriteAllText(reportFile, reporte, Encoding.UTF8);
                        Log("Config con keys -> reporte de cambios: " + d.Path);
                        exported++;
                        continue;
                    }
                }

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

            BuildDllReport(repo, changes, outRoot);
            Log("Reporte de DLLs: dlls_a_desplegar.txt");

            Log("Archivos exportados: " + exported.ToString(CultureInfo.InvariantCulture));
            Log("Manifest: " + manifestPath);
            Log("Listo.");
        }

        #endregion

        #region Validación

        private static void Validate(ExportOptions opt)
        {
            if (opt == null) throw new ArgumentNullException("opt");
            if (string.IsNullOrWhiteSpace(opt.Repo)) throw new InvalidOperationException("Debe indicar Repo.");

            string repoFull = Path.GetFullPath(opt.Repo);
            if (!Directory.Exists(repoFull))
                throw new InvalidOperationException("Repo no existe: " + repoFull + " (valor original recibido: '" + opt.Repo + "')");

            if (string.IsNullOrWhiteSpace(opt.From)) throw new InvalidOperationException("Debe indicar From.");
            if (string.IsNullOrWhiteSpace(opt.To)) throw new InvalidOperationException("Debe indicar To.");
        }

        #endregion

        #region Detección de cambios (git)

        private List<DiffEntry> GetChangedFiles(ExportOptions opt)
        {
            string renameArg = opt.NoRenames ? " --no-renames" : " -M";
            string cmd;
            string pathPart = string.IsNullOrWhiteSpace(opt.PathFilter)
                ? ""
                : " -- " + EscapeGitPath(opt.PathFilter);
            bool tieneAutores = opt.Authors != null && opt.Authors.Count > 0;

            if (tieneAutores)
            {
                string authorArgs = string.Join(" ", opt.Authors.Select(a => "--author=\"" + BuildAuthorPattern(a) + "\""));

                cmd = "-c core.quotepath=false log " + EscapeRef(opt.From) + ".." + EscapeRef(opt.To)
                    + " " + authorArgs
                    + " --name-status --pretty=format: "
                    + pathPart;
            }
            else
            {
                string range;
                if (opt.From.Equals(opt.To, StringComparison.OrdinalIgnoreCase))
                    range = EscapeRef(opt.From) + "^.." + EscapeRef(opt.To);
                else
                    range = EscapeRef(opt.From) + ".." + EscapeRef(opt.To);

                cmd = "-c core.quotepath=false diff --name-status" + renameArg + " " + range + pathPart;
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
                    string oldPath = parts[1].Trim().Trim('"').Replace('\\', '/');
                    string newPath = parts[2].Trim().Trim('"').Replace('\\', '/');

                    list.Add(new DiffEntry { Status = "R", Path = newPath });
                    list.Add(new DiffEntry { Status = "D", Path = oldPath });
                }
                else
                {
                    string path = parts[1].Trim().Trim('"').Replace('\\', '/');
                    string s1 = st.Length > 0 ? st.Substring(0, 1).ToUpperInvariant() : "?";
                    list.Add(new DiffEntry { Status = s1, Path = path });
                }
            }

            return list
                .GroupBy(x => x.Status + "\u0001" + x.Path, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();
        }

        private static string BuildAuthorPattern(string rawAuthor)
        {
            string clean = (rawAuthor ?? "").Replace("\"", "").Trim();
            return string.Join(".*", clean.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
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
            s = s.Replace('\\', '/');
            if (s.IndexOf(' ') >= 0)
                return "\"" + s.Replace("\"", "") + "\"";
            return s;
        }

        #endregion

        #region Escritura del manifest

        private static void EscribirPorEstado(StreamWriter sw, List<DiffEntry> grupo)
        {
            var añadidos = grupo.Where(x => x.Status == "A").OrderBy(x => x.Path, StringComparer.OrdinalIgnoreCase).ToList();
            var modificados = grupo.Where(x => x.Status == "M").OrderBy(x => x.Path, StringComparer.OrdinalIgnoreCase).ToList();
            var renombrados = grupo.Where(x => x.Status == "R").OrderBy(x => x.Path, StringComparer.OrdinalIgnoreCase).ToList();
            var eliminados = grupo.Where(x => x.Status == "D").OrderBy(x => x.Path, StringComparer.OrdinalIgnoreCase).ToList();
            var otros = grupo.Where(x => x.Status != "A" && x.Status != "M" && x.Status != "R" && x.Status != "D")
                                   .OrderBy(x => x.Path, StringComparer.OrdinalIgnoreCase).ToList();

            if (añadidos.Count > 0)
            {
                sw.WriteLine("  AÑADIDOS (" + añadidos.Count + "):");
                foreach (DiffEntry d in añadidos) sw.WriteLine("    " + d.Path);
            }
            if (modificados.Count > 0)
            {
                sw.WriteLine("  MODIFICADOS (" + modificados.Count + "):");
                foreach (DiffEntry d in modificados) sw.WriteLine("    " + d.Path);
            }
            if (renombrados.Count > 0)
            {
                sw.WriteLine("  RENOMBRADOS (" + renombrados.Count + "):");
                foreach (DiffEntry d in renombrados) sw.WriteLine("    " + d.Path);
            }
            if (eliminados.Count > 0)
            {
                sw.WriteLine("  ELIMINADOS (" + eliminados.Count + "):");
                foreach (DiffEntry d in eliminados) sw.WriteLine("    " + d.Path);
            }
            if (otros.Count > 0)
            {
                sw.WriteLine("  OTROS (" + otros.Count + "):");
                foreach (DiffEntry d in otros) sw.WriteLine("    " + d.Status + "\t" + d.Path);
            }
            sw.WriteLine();
        }

        #endregion

        #region Reportes de configuración (web.config / app.config / packages.config)

        private static Dictionary<string, KeyValuePair<string, string>> ParseConfigEntries(string text)
        {
            var entries = new Dictionary<string, KeyValuePair<string, string>>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(text)) return entries;

            foreach (Match m in ReAppSetting.Matches(text))
                entries["appSettings|" + m.Groups["key"].Value] =
                    new KeyValuePair<string, string>(m.Groups["value"].Value, m.Value.Trim());
            foreach (Match m in ReConnStr.Matches(text))
                entries["connectionStrings|" + m.Groups["key"].Value] =
                    new KeyValuePair<string, string>(m.Groups["value"].Value, m.Value.Trim());

            return entries;
        }

        private static string BuildConfigReport(string oldText, string newText, string statusArchivo, string ruta)
        {
            var oldE = ParseConfigEntries(oldText);
            var newE = ParseConfigEntries(newText);

            var added = new List<string>();
            var modified = new List<string>();
            var deleted = new List<string>();

            foreach (var kv in newE)
            {
                string sec = kv.Key.Split('|')[0];
                string key = kv.Key.Substring(sec.Length + 1);
                string newVal = kv.Value.Key;
                string newLine = kv.Value.Value;

                if (!oldE.ContainsKey(kv.Key))
                {
                    added.Add("  [A] (" + sec + ") " + key);
                    added.Add("        " + newLine);
                }
                else if (!string.Equals(oldE[kv.Key].Key, newVal, StringComparison.Ordinal))
                {
                    modified.Add("  [M] (" + sec + ") " + key);
                    modified.Add("        antes: " + oldE[kv.Key].Value);
                    modified.Add("        ahora: " + newLine);
                }
            }
            foreach (var kv in oldE)
            {
                if (!newE.ContainsKey(kv.Key))
                {
                    string sec = kv.Key.Split('|')[0];
                    string key = kv.Key.Substring(sec.Length + 1);
                    deleted.Add("  [D] (" + sec + ") " + key);
                    deleted.Add("        " + kv.Value.Value);
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine(ruta);
            sb.AppendLine("Estado del archivo: " + statusArchivo);
            sb.AppendLine();

            if (added.Count == 0 && modified.Count == 0 && deleted.Count == 0)
            {
                sb.AppendLine("  (sin cambios detectables en appSettings/connectionStrings)");
                return sb.ToString();
            }

            if (added.Count > 0) { sb.AppendLine("AÑADIDAS:"); added.ForEach(l => sb.AppendLine(l)); sb.AppendLine(); }
            if (modified.Count > 0) { sb.AppendLine("MODIFICADAS:"); modified.ForEach(l => sb.AppendLine(l)); sb.AppendLine(); }
            if (deleted.Count > 0) { sb.AppendLine("ELIMINADAS:"); deleted.ForEach(l => sb.AppendLine(l)); sb.AppendLine(); }

            return sb.ToString().TrimEnd();
        }

        private static Dictionary<string, KeyValuePair<string, string>> ParsePackages(string text)
        {
            var result = new Dictionary<string, KeyValuePair<string, string>>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(text) || RePackage == null) return result;

            foreach (Match m in RePackage.Matches(text))
            {
                var idGroup = m.Groups["id"];
                var verGroup = m.Groups["version"];
                if (!idGroup.Success || !verGroup.Success) continue;
                result[idGroup.Value] = new KeyValuePair<string, string>(verGroup.Value, m.Value.Trim());
            }
            return result;
        }

        private static string BuildPackagesReport(string oldText, string newText, string statusArchivo, string ruta)
        {
            var oldP = ParsePackages(oldText);
            var newP = ParsePackages(newText);

            var added = new List<string>();
            var updated = new List<string>();
            var removed = new List<string>();

            foreach (var kv in newP)
            {
                string id = kv.Key;
                string ver = kv.Value.Key;
                string line = kv.Value.Value;

                if (!oldP.ContainsKey(id))
                {
                    added.Add("  [A] " + id);
                    added.Add("        " + line);
                }
                else if (!string.Equals(oldP[id].Key, ver, StringComparison.Ordinal))
                {
                    updated.Add("  [U] " + id);
                    updated.Add("        antes: versión " + oldP[id].Key);
                    updated.Add("        ahora: versión " + ver);
                }
            }
            foreach (var kv in oldP)
            {
                if (!newP.ContainsKey(kv.Key))
                    removed.Add("  [D] " + kv.Key + " (versión " + kv.Value.Key + ")");
            }

            var sb = new StringBuilder();
            sb.AppendLine(ruta);
            sb.AppendLine("Estado del archivo: " + statusArchivo);
            sb.AppendLine();

            if (added.Count == 0 && updated.Count == 0 && removed.Count == 0)
            {
                sb.AppendLine("  (sin cambios detectables en paquetes NuGet)");
                return sb.ToString();
            }

            if (added.Count > 0) { sb.AppendLine("PAQUETES AÑADIDOS:"); added.ForEach(l => sb.AppendLine(l)); sb.AppendLine(); }
            if (updated.Count > 0) { sb.AppendLine("PAQUETES ACTUALIZADOS:"); updated.ForEach(l => sb.AppendLine(l)); sb.AppendLine(); }
            if (removed.Count > 0) { sb.AppendLine("PAQUETES QUITADOS:"); removed.ForEach(l => sb.AppendLine(l)); sb.AppendLine(); }

            return sb.ToString().TrimEnd();
        }

        #endregion

        #region Reporte de DLLs a desplegar

        private static string FindCsprojFor(string repoRoot, string fileRel)
        {
            string abs = Path.GetFullPath(Path.Combine(repoRoot, fileRel.Replace('/', Path.DirectorySeparatorChar)));
            string dir = Path.GetDirectoryName(abs);
            string repoAbs = Path.GetFullPath(repoRoot).TrimEnd('\\', '/');

            while (!string.IsNullOrEmpty(dir))
            {
                string dirAbs = Path.GetFullPath(dir).TrimEnd('\\', '/');
                try
                {
                    string[] found = Directory.GetFiles(dirAbs, "*.csproj", SearchOption.TopDirectoryOnly);
                    if (found.Length > 0) return found[0];
                }
                catch { }

                if (dirAbs.Length <= repoAbs.Length) break;
                string parent = Path.GetDirectoryName(dirAbs);
                if (parent == null || parent == dirAbs) break;
                dir = parent;
            }
            return null;
        }

        private static string AssemblyNameOf(string csprojPath)
        {
            try
            {
                string text = File.ReadAllText(csprojPath);
                Match m = ReAssemblyName.Match(text);
                if (m.Success) return m.Groups["name"].Value.Trim();
            }
            catch { }
            return Path.GetFileNameWithoutExtension(csprojPath);
        }

        private void BuildDllReport(string repoRoot, List<DiffEntry> changes, string outRoot)
        {
            var dllCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var contenido = new List<string>();
            var sinProyecto = new List<string>();

            foreach (DiffEntry d in changes)
            {
                if (d.Status == "D") continue;

                if (!IsCodeFile(d.Path))
                {
                    contenido.Add(d.Path);
                    continue;
                }

                string csproj = FindCsprojFor(repoRoot, d.Path);
                if (csproj == null) { sinProyecto.Add(d.Path); continue; }

                string dll = AssemblyNameOf(csproj) + ".dll";
                dllCounts[dll] = (dllCounts.ContainsKey(dll) ? dllCounts[dll] : 0) + 1;
            }

            var sb = new StringBuilder();
            sb.AppendLine("DLLs A DESPLEGAR (según archivos de código modificados)");
            sb.AppendLine("=======================================================");
            sb.AppendLine();
            if (dllCounts.Count > 0)
            {
                foreach (string dll in dllCounts.Keys.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                    sb.AppendLine("  " + dll + "   (" + dllCounts[dll].ToString(CultureInfo.InvariantCulture) + " archivo(s) de código modificado(s))");
            }
            else
            {
                sb.AppendLine("  (ningún archivo de código modificado -> ninguna DLL que subir)");
            }
            sb.AppendLine();
            sb.AppendLine("Recuerda: compila/publica ANTES de tomar las DLLs de tu carpeta de publicación.");
            sb.AppendLine();

            if (contenido.Count > 0)
            {
                sb.AppendLine("ARCHIVOS DE CONTENIDO (se despliegan tal cual, NO generan DLL):");
                foreach (string f in contenido.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                    sb.AppendLine("  " + f);
                sb.AppendLine();
            }
            if (sinProyecto.Count > 0)
            {
                sb.AppendLine("ADVERTENCIA - código sin .csproj contenedor (revisar manualmente):");
                foreach (string f in sinProyecto.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                    sb.AppendLine("  " + f);
                sb.AppendLine();
            }

            File.WriteAllText(Path.Combine(outRoot, "dlls_a_desplegar.txt"), sb.ToString().TrimEnd(), Encoding.UTF8);
        }

        #endregion

        #region Clasificadores de archivos

        private static bool IsCsFile(string path)
        {
            return (path ?? "").EndsWith(".cs", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsCodeFile(string path)
        {
            string p = (path ?? "");
            return p.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
                || p.EndsWith(".vb", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsConfigCandidate(string path)
        {
            string leaf = Path.GetFileName((path ?? "").Replace('/', Path.DirectorySeparatorChar));
            if (string.IsNullOrEmpty(leaf)) return false;

            bool esWeb = leaf.Equals("web.config", StringComparison.OrdinalIgnoreCase)
                || (leaf.StartsWith("web.", StringComparison.OrdinalIgnoreCase)
                    && leaf.EndsWith(".config", StringComparison.OrdinalIgnoreCase));

            bool esApp = leaf.Equals("app.config", StringComparison.OrdinalIgnoreCase)
                || (leaf.StartsWith("app.", StringComparison.OrdinalIgnoreCase)
                    && leaf.EndsWith(".config", StringComparison.OrdinalIgnoreCase));

            return esWeb || esApp;
        }

        private static bool IsPackagesConfig(string path)
        {
            string leaf = Path.GetFileName((path ?? "").Replace('/', Path.DirectorySeparatorChar));
            return leaf.Equals("packages.config", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsProjectFile(string path)
        {
            string p = (path ?? "");
            return p.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)
                || p.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsBinPath(string root, string fullPath)
        {
            string p = (fullPath ?? "").Replace('\\', '/');
            string r = (root ?? "").Replace('\\', '/').Trim('/');

            if (string.IsNullOrEmpty(r))
            {
                return p.IndexOf("/bin/", StringComparison.OrdinalIgnoreCase) >= 0
                    || p.EndsWith("/bin", StringComparison.OrdinalIgnoreCase)
                    || p.StartsWith("bin/", StringComparison.OrdinalIgnoreCase);
            }

            if (!p.StartsWith(r + "/", StringComparison.OrdinalIgnoreCase))
                return false;

            return p.IndexOf("/bin/", StringComparison.OrdinalIgnoreCase) >= 0
                || p.EndsWith("/bin", StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Utilidades

        private static string TextoDeGit(string reference, string path)
        {
            string err;
            byte[] bytes = GitRunner.RunBytes("show " + EscapeRef(reference) + ":" + EscapeGitPath(path), out err);
            return bytes == null ? "" : Encoding.UTF8.GetString(bytes);
        }

        private static string MapStatus(string status)
        {
            switch ((status ?? "").ToUpperInvariant())
            {
                case "A": return "Añadido (A)";
                case "M": return "Modificado (M)";
                case "D": return "Eliminado (D)";
                case "R": return "Renombrado (R)";
                default: return status;
            }
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

        #endregion
    }
}