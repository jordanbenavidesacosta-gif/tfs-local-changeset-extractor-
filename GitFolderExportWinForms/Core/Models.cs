using System;
using System.Collections.Generic;

namespace GitFolderExportWinForms.Core
{
    internal sealed class ExportOptions
    {
        public string Repo;
        public string From;
        public string To;
        public string PathFilter;
        public string OutDir;
        public bool ExcludeCs { get; set; }
        public bool Fetch;
        public bool IncludeBinTracked;
        public bool CopyWorkingBin;
        public bool NoRenames;
        public List<string> Authors { get; set; } = new List<string>();
    }

    internal sealed class DiffEntry
    {
        public string Status; 
        public string Path;   
    }
    public class HistoryEntry
    {
        public DateTime Timestamp { get; set; }
        public string Repo { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string PathFilter { get; set; }
        public string OutDir { get; set; }
        public string Authors { get; set; } 
        public bool ExcludeCs { get; set; }
        public int Exported { get; set; }

        public override string ToString()
        {
            return Timestamp.ToString("yyyy-MM-dd HH:mm") + " | "
                 + From + ".." + To + " | "
                 + PathFilter + " | "
                 + (string.IsNullOrWhiteSpace(Authors) ? "(sin autor)" : Authors) + " | "
                 + Exported + " archivo(s)";
        }
    }
}
