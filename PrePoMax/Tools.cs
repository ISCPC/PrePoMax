using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    internal static class Tools
    {
        public static string GetLocalPath(string path)
        {
            string startUpPath = System.Windows.Forms.Application.StartupPath;
            if (path.StartsWith(startUpPath))
            {
                return "#" + path.Substring(startUpPath.Length);
            }
            return path;
        }
        //
        public static string GetGlobalPath(string path)
        {
            if (path != null && path.StartsWith("#"))
            {
                string startUpPath = System.Windows.Forms.Application.StartupPath;
                return System.IO.Path.Combine(startUpPath, path.Substring(1).TrimStart('\\'));
            }
            return path;
        }
        //
        public static string GetNonExistentRandomFileName(string path, string extension = "")
        {
            string hash;
            bool repeate;
            string[] allFiles = System.IO.Directory.GetFiles(path);
            //
            do
            {
                hash = CaeGlobals.Tools.GetRandomString(8);
                //
                repeate = false;
                foreach (var fileName in allFiles)
                {
                    if (fileName.StartsWith(hash))
                    {
                        repeate = true;
                        break;
                    }
                }
            }
            while (repeate);
            //
            return System.IO.Path.Combine(path, System.IO.Path.ChangeExtension(hash, extension));
        }
    }
}
