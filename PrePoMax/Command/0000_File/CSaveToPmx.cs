using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;
using CaeMesh;
using CaeGlobals;
using System.IO;

namespace PrePoMax.Commands
{
    [Serializable]
    class CSaveToPmx : Command
    {
        // Variables                                                                                                                
        private string _fileName;
        private byte[] _hash;


        // Properties                                                                                                               
        public byte[] Hash { get { return _hash; } set { _hash = value; } }


        // Constructor                                                                                                              
        public CSaveToPmx(string fileName)
            :base("Save to file")
        {
            _fileName = Tools.GetLocalPath(fileName);
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.SaveToPmx(Tools.GetGlobalPath(_fileName));
            //
            using (FileStream fop = File.OpenRead(Tools.GetGlobalPath(_fileName)))
            {
                _hash = System.Security.Cryptography.SHA1.Create().ComputeHash(fop); // string hash = BitConverter.ToString(_hash);
            }
            //
            return true;
        }
        public override string GetCommandString()
        {
            return _dateCreated.ToString("MM/dd/yy HH:mm:ss") + "   " + "Model saved to file: " + _fileName;
        }
    }
}
