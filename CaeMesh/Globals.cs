using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CaeMesh
{
    public class Globals
    {
        public static string InternalName = "Internal";
        public static string InternalSelectionName = "Internal_Selection";
        public static string SlaveNameSuffix = "_Slave";
        public static string MasterNameSuffix = "_Master";
        public static string MasterSlaveSeparator = "_to_";
        //
        public static Color[] ColorTable = new Color[]
        {
            Color.Beige,
            Color.SeaGreen,
            Color.LightGray,
            Color.SteelBlue,
            Color.BurlyWood,
            Color.LightSteelBlue,
            Color.Gray,
            Color.DarkSalmon,
            Color.DarkKhaki,
            Color.NavajoWhite
        };
        public static double EdgeAngle = 30;
    }
}
