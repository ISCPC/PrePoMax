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
    internal class CalPhysicalConstants : CalculixKeyword
    {
        // Variables                                                                                                                
        private ModelProperties _modelProperties;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalPhysicalConstants(ModelProperties modelProperties)
        {
            _modelProperties = modelProperties;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string absoluteZero = "";
            string stefanBoltzman = "";
            string newtonGravity = "";
            //
            if (_modelProperties.AbsoluteZero != double.PositiveInfinity)
                absoluteZero = ", Absolute zero=" + _modelProperties.AbsoluteZero.ToCalculiX16String();
            if (_modelProperties.StefanBoltzmann != double.PositiveInfinity)
                stefanBoltzman = ", Stefan Boltzmann=" + _modelProperties.StefanBoltzmann.ToCalculiX16String();
            if (_modelProperties.NewtonGravity != double.PositiveInfinity)
                newtonGravity = ", Newton gravity=" + _modelProperties.NewtonGravity.ToCalculiX16String();
            //
            if (absoluteZero != "" || stefanBoltzman != "" || newtonGravity != "")
            {
                return string.Format("*Physical constants{0}{1}{2}{3}", absoluteZero, stefanBoltzman, newtonGravity,
                                     Environment.NewLine);
            }
            else return "";
        }
        public override string GetDataString()
        {
            return "";
        }
    }
}
