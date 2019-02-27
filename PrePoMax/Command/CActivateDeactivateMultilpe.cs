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
    class CActivateDeactivateMultilpe : Command
    {
        // Variables                                                                                                                
        private NamedClass[] _items;
        private bool _activate;
        private string[] _stepNames;


        // Constructor                                                                                                              
        public CActivateDeactivateMultilpe(NamedClass[] items, bool activate, string[] stepNames)
            : base("Activate/deactivate multiple")
        {
            _items = items.DeepClone();
            _activate = activate;
            _stepNames = stepNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ActivateDeactivateMultiple(_items.DeepClone(), _activate, _stepNames);
            return true;
        }

        public override string GetCommandString()
        {
            string[] names = new string[_items.Length];
            for (int i = 0; i < names.Length; i++) names[i] = _items[i].Name;
            return base.GetCommandString() + GetArrayAsString(names);
        }
    }
}
