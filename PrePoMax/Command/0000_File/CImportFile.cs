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
    class CImportFile : Command, ICommandWithDialog
    {
        // Variables                                                                                                                
        private string _fileName;
        private bool _resetCamera;


        // Constructor                                                                                                              
        public CImportFile(string fileName)
            :base("Import file")
        {
            _fileName = Tools.GetLocalPath(fileName);
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ImportFile(Tools.GetGlobalPath(_fileName));
            return true;
        }

        public void ExecuteWithDialogs(Controller receiver)
        {
            string fileName = receiver.GetFileNameToImport();
            if (fileName != null) _fileName = Tools.GetLocalPath(fileName);
            Execute(receiver);
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _fileName;
        }
    }
}
