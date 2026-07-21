using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace GitFolderExportWinForms.Core
{
    internal static class HistoryService
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "GitFolderExportWinForms", "history.json");

        private const int MaxEntries = 20;

        public static List<HistoryEntry> Load()
        {
            try
            {
                if (!File.Exists(FilePath)) return new List<HistoryEntry>();
                string json = File.ReadAllText(FilePath);
                var serializer = new JavaScriptSerializer();
                return serializer.Deserialize<List<HistoryEntry>>(json) ?? new List<HistoryEntry>();
            }
            catch
            {
                return new List<HistoryEntry>();
            }
        }

        public static void Add(HistoryEntry entry)
        {
            try
            {
                List<HistoryEntry> list = Load();
                list.Insert(0, entry); 

                if (list.Count > MaxEntries)
                    list = list.Take(MaxEntries).ToList();

                string dir = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var serializer = new JavaScriptSerializer();
                File.WriteAllText(FilePath, serializer.Serialize(list));
            }
            catch
            {

            }
        }
        public static List<HistoryEntry> LoadForRepo(string repo)
        {
            string target = NormalizeRepo(repo);
            return Load()
                .Where(e => NormalizeRepo(e.Repo).Equals(target, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        private static string NormalizeRepo(string repo)
        {
            if (string.IsNullOrWhiteSpace(repo)) return "";
            try { return Path.GetFullPath(repo).TrimEnd('\\', '/'); }
            catch { return repo.Trim().TrimEnd('\\', '/'); }
        }
    }
}
