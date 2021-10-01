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
    class CAddContactPair : Command
    {
        // Variables                                                                                                                
        private ContactPair _contactPair;
        private bool _update;


        // Constructor                                                                                                              
        public CAddContactPair(ContactPair contactPair, bool update)
            : base("Add contact pair")
        {
            _contactPair = contactPair.DeepClone();
            _update = update;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddContactPair(_contactPair.DeepClone(), _update);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _contactPair.ToString();
        }
    }
}
