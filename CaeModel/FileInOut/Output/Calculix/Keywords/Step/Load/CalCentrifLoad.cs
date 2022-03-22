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
    internal class CalCentrifLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private CentrifLoad _load;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalCentrifLoad(CentrifLoad load)
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
            Vec3D n = new Vec3D(_load.N1, _load.N2, _load.N3);
            n.Normalize();
            //
            sb.AppendFormat("{0}, CENTRIF, {1}, {2}, {3}, {4}, {5}, {6}, {7}", _load.RegionName,
                            _load.RotationalSpeed2.ToCalculiX16String(), 
                            _load.X.ToCalculiX16String(), _load.Y.ToCalculiX16String(), _load.Z.ToCalculiX16String(),
                            n.X.ToCalculiX16String(), n.Y.ToCalculiX16String(), n.Z.ToCalculiX16String());
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
