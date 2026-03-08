using System;

namespace GitFolderExportWinForms.Core
{
    internal sealed class ExportOptions
    {
        public string Repo;
        public string From;
        public string To;
        public string PathFilter;
        public string OutDir;

        public bool Fetch;
        public bool IncludeBinTracked;
        public bool CopyWorkingBin;
        public bool NoRenames;

        public string Author;
    }

    internal sealed class DiffEntry
    {
        public string Status; 
        public string Path;   
    }
}
