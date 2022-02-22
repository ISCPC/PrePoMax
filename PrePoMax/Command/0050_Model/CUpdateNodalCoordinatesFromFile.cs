using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;
using CaeMesh;
using CaeGlobals;


namespace PrePoMax.Commands
{
    [Serializable]
    class CUpdateNodalCoordinatesFromFile : Command
    {
        // Variables                                                                                                                
        private string _fileName;


        // Constructor                                                                                                              
        public CUpdateNodalCoordinatesFromFile(string fileName)
            : base("Update nodal coordinates from file")
        {
            _fileName = fileName;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.UpdateNodalCoordinatesFromFile(_fileName);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _fileName;
        }
    }
}
