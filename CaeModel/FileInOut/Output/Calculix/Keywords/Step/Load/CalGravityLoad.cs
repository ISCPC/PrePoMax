using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;
using CaeGlobals;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalGravityLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private GravityLoad _load;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalGravityLoad(GravityLoad load)
        {
            _load = load;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _load.Name);
            string amplitude = "";
            if (_load.AmplitudeName != Load.DefaultAmplitudeName) amplitude = ", Amplitude=" + _load.AmplitudeName;
            //
            sb.AppendFormat("*Dload{0}{1}", amplitude, Environment.NewLine);
            //
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            //
            Vec3D f = new Vec3D(_load.F1, _load.F2, _load.F3);
            double len = f.Normalize();
            //
            sb.AppendFormat("{0}, Grav, {1}, {2}, {3}, {4}", _load.RegionName, len.ToCalculiX16String(),
                            f.X.ToCalculiX16String(), f.Y.ToCalculiX16String(), f.Z.ToCalculiX16String());
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
