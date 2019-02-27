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
    internal class CalMaterial : CalculixKeyword
    {
        // Variables                                                                                                                
        private Material _material;


        // Properties                                                                                                               
        public override object BaseItem { get { return _material; } }


        // Events                                                                                                                   


        // Constructor                                                                                                              
        public CalMaterial(Material material)
        {
            _material = material;
            _active = material.Active;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Material, Name={0}{1}", _material.Name, Environment.NewLine);
        }

        public override string GetDataString()
        {
            return "";
        }
    }
}
