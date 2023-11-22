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
        private GapSectionData _gapSectionData;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalGapSection(GapSectionData gapSectionData)
        {
            _gapSectionData = gapSectionData;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*Gap, Elset={0}{1}", _gapSectionData.RegionName, Environment.NewLine);
            return sb.ToString();
        }
        public override string GetDataString()
        {
            double springStiffness;
            if (double.IsNaN(_gapSectionData.SpringStiffness)) springStiffness = GapSectionData.InitialSpringStiffness;
            else springStiffness = _gapSectionData.SpringStiffness;
            //
            double tensileForce;
            if (double.IsNaN(_gapSectionData.TensileForceAtNegativeInfinity))
                tensileForce = GapSectionData.InitialTensileForceAtNegativeInfinity;
            else tensileForce = _gapSectionData.TensileForceAtNegativeInfinity;
            //
            string properties = "";
            if (!double.IsNaN(_gapSectionData.SpringStiffness) || !double.IsNaN(_gapSectionData.TensileForceAtNegativeInfinity))
                properties = string.Format(", , {0}, {1}",
                                           springStiffness.ToCalculiX16String(),
                                           tensileForce.ToCalculiX16String());
            
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}, {1}, {2}, {3}{4}{5}",
                            _gapSectionData.Clearance,
                            _gapSectionData.Direction[0].ToCalculiX16String(),
                            _gapSectionData.Direction[1].ToCalculiX16String(),
                            _gapSectionData.Direction[2].ToCalculiX16String(),
                            properties,
                            Environment.NewLine);
            return sb.ToString();
        }
    }
}
