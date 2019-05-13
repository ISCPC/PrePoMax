using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalNodePrint : CalculixKeyword
    {
        // Variables                                                                                                                
        private string regionName;
        private readonly NodalHistoryOutput _nodalHistoryOutput;


        // Properties                                                                                                               
        public override object BaseItem { get { return _nodalHistoryOutput; } }


        // Events                                                                                                                   


        // Constructor                                                                                                              
        public CalNodePrint(FeModel model, NodalHistoryOutput nodalHistoryOutput)
        {
            _nodalHistoryOutput = nodalHistoryOutput;
            _active = nodalHistoryOutput.Active;

            regionName = ", Nset=";
            if (_nodalHistoryOutput.RegionType == CaeGlobals.RegionTypeEnum.NodeSetName)
                regionName += _nodalHistoryOutput.RegionName;
            else if (_nodalHistoryOutput.RegionType == CaeGlobals.RegionTypeEnum.SurfaceName)
                regionName += model.Mesh.Surfaces[_nodalHistoryOutput.RegionName].NodeSetName;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string totals = "";
            if (_nodalHistoryOutput.TotalsType == TotalsTypeEnum.Yes) totals = ", Totals=Yes";
            else if (_nodalHistoryOutput.TotalsType == TotalsTypeEnum.Only) totals = ", Totals=Only";
            return string.Format("*Node print{0}{1}{2}", regionName, totals, Environment.NewLine);
        }

        public override string GetDataString()
        {
            return string.Format("{0}{1}", _nodalHistoryOutput.Variables.ToString(), Environment.NewLine);
        }
    }
}
