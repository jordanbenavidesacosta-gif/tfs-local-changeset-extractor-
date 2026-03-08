using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GitFolderExportWinForms.Core
{
    internal static class GitRunner
    {
        public static int Run(string arguments, out byte[] stdout, out byte[] stderr)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process p = new Process())
            {
                p.StartInfo = psi;
                p.Start();

                using (MemoryStream msOut = new MemoryStream())
                using (MemoryStream msErr = new MemoryStream())
                {
                    p.StandardOutput.BaseStream.CopyTo(msOut);
                    p.StandardError.BaseStream.CopyTo(msErr);
                    p.WaitForExit();

                    stdout = msOut.ToArray();
                    stderr = msErr.ToArray();
                    return p.ExitCode;
                }
            }
        }

        public static string RunText(string arguments, out string errorText)
        {
            byte[] o, e;
            int code = Run(arguments, out o, out e);
            errorText = SafeToString(e);
            if (code != 0) return null;
            return System.Text.Encoding.UTF8.GetString(o);
        }

        public static byte[] RunBytes(string arguments, out string errorText)
        {
            byte[] o, e;
            int code = Run(arguments, out o, out e);
            errorText = SafeToString(e);
            if (code != 0) return null;
            return o;
        }

        private static string SafeToString(byte[] b)
        {
            if (b == null || b.Length == 0) return "";
            try { return System.Text.Encoding.UTF8.GetString(b); }
            catch { return ""; }
        }

        public static List<string> GetAuthors()
        {
            string err;
            string output = RunText("log --format=\"%an\"", out err);

            if (output == null) return new List<string>();

            return output
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }
    }
}
