using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace GitFolderExportWinForms.Core
{
    public sealed class ExportProfile
    {
        public string Name { get; set; }
        public string Repo { get; set; }       
        public string PathFilter { get; set; } 
        public string OutDir { get; set; }     
    }

    internal static class ProfileService
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "GitFolderExportWinForms", "profiles.json");

        public static List<ExportProfile> Load()
        {
            try
            {
                if (!File.Exists(FilePath)) return new List<ExportProfile>();
                string json = File.ReadAllText(FilePath);
                return new JavaScriptSerializer().Deserialize<List<ExportProfile>>(json)
                       ?? new List<ExportProfile>();
            }
            catch { return new List<ExportProfile>(); }
        }

        public static void Save(ExportProfile profile)
        {
            if (profile == null || string.IsNullOrWhiteSpace(profile.Name)) return;
            try
            {
                List<ExportProfile> list = Load();
                list.RemoveAll(p => string.Equals(p.Name, profile.Name, StringComparison.OrdinalIgnoreCase));
                list.Add(profile);
                list = list.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase).ToList();

                string dir = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(FilePath, new JavaScriptSerializer().Serialize(list));
            }
            catch { }
        }

        public static void Delete(string name)
        {
            try
            {
                List<ExportProfile> list = Load();
                list.RemoveAll(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
                File.WriteAllText(FilePath, new JavaScriptSerializer().Serialize(list));
            }
            catch { }
        }
    }
}