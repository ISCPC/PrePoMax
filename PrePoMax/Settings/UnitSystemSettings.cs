using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using System.IO;
using DynamicTypeDescriptor;
using System.Drawing;

namespace PrePoMax
{
    [Serializable]
    public class UnitSystemSettings : ISettings
    {
        // Variables                                                                                                                
        private UnitSystemType _modelUnitSystemType;
        private UnitSystemType _resultsUnitSystemType;


        // Properties                                                                                                               
        public UnitSystemType ModelUnitSystemType
        {
            get { return _modelUnitSystemType; }
            set { _modelUnitSystemType = value; }
        }
        public UnitSystemType ResultsUnitSystemType
        {
            get { return _resultsUnitSystemType; }
            set { _resultsUnitSystemType = value; }
        }


        // Constructors                                                                                                             
        public UnitSystemSettings()
        {
            Reset();
            //
            _modelUnitSystemType = UnitSystemType.Undefined;
            _resultsUnitSystemType = UnitSystemType.Undefined;
        }


        // Methods                                                                                                                  
        public void CheckValues()
        {
        }
        public void Reset()
        {
        }

    }
}
