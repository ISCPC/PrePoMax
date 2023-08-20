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
        private ComplexLoadTypeEnum _complexLoadType;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalGravityLoad(GravityLoad load, ComplexLoadTypeEnum complexLoadType)
        {
            _load = load;
            _complexLoadType = complexLoadType;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _load.Name);
            string amplitude = "";
            if (_load.AmplitudeName != Load.DefaultAmplitudeName) amplitude = ", Amplitude=" + _load.AmplitudeName;
            //
            string loadCase = GetComplexLoadCase(_complexLoadType);
            //
            sb.AppendFormat("*Dload{0}{1}{2}", amplitude, loadCase, Environment.NewLine);
            //
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            //
            double ratio = GetComplexRatio(_complexLoadType, _load.PhaseDeg.Value);
            //
            Vec3D f = ratio * new Vec3D(_load.F1.Value, _load.F2.Value, _load.F3.Value);
            double len = f.Normalize();
            //
            sb.AppendFormat("{0}, Grav, {1}, {2}, {3}, {4}", _load.RegionName, len.ToCalculiX16String(),
                            f.X.ToCalculiX16String(), f.Y.ToCalculiX16String(), f.Z.ToCalculiX16String());
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
