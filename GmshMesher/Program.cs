using CaeGlobals;
using CaeMesh;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GmshMesherExe
{
    internal class Program
    {
        static int Main(string[] args)
        {
            string error = null;
            if (args.Length == 1)
            {
                string gmshDataFileName = args[0];
                if (File.Exists(gmshDataFileName))
                {
                    GmshData gmshData = Tools.LoadDumpFromFile<GmshData>(gmshDataFileName);
                    //
                    GmshBase gmsh = new GmshBase(gmshData, Console.WriteLine);
                    error = gmsh.CreateMesh();
                }
            }
            //Console.WriteLine("Press any key to stop...");
            //Console.ReadKey();
            //
            Console.Error.WriteLine(error);
            //
            if (error != null) return 1;
            else return 0;
        }
    }
}
