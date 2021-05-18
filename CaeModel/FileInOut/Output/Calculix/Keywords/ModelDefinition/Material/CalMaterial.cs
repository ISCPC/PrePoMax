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
        public object GetBase { get { return _material; } }


        // Constructor                                                                                                              
        public CalMaterial(Material material)
        {
            _material = material;
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
