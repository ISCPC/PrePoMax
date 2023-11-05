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
    internal class CalGapSection : CalculixKeyword
    {
        // Variables                                                                                                                
        private GapSection _gapSection;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalGapSection(GapSection gapSection)
        {
            _gapSection = gapSection;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*Gap, Elset={0}{1}", _gapSection.RegionName, Environment.NewLine);
            return sb.ToString();
        }
        public override string GetDataString()
        {
            double springStiffness;
            if (double.IsNaN(_gapSection.SpringStiffness)) springStiffness = GapSection.InitialSpringStiffness;
            else springStiffness = _gapSection.SpringStiffness;
            //
            double tensileForce;
            if (double.IsNaN(_gapSection.TensileForceAtNegativeInfinity))
                tensileForce = GapSection.InitialTensileForceAtNegativeInfinity;
            else tensileForce = _gapSection.TensileForceAtNegativeInfinity;
            //

            string properties = "";
            if (!double.IsNaN(_gapSection.SpringStiffness) || !double.IsNaN(_gapSection.TensileForceAtNegativeInfinity))
                properties = string.Format(", , {0}, {1}",
                                           springStiffness.ToCalculiX16String(),
                                           tensileForce.ToCalculiX16String());
            
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}, {1}, {2}, {3}{4}{5}",
                            _gapSection.Clearance,
                            _gapSection.Direction[0].ToCalculiX16String(),
                            _gapSection.Direction[1].ToCalculiX16String(),
                            _gapSection.Direction[2].ToCalculiX16String(),
                            properties,
                            Environment.NewLine);
            return sb.ToString();
        }
    }
}
