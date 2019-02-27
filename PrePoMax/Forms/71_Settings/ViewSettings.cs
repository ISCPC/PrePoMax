using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;
using CaeGlobals;

namespace PrePoMax.Settings
{
    [Serializable]
    public abstract class ViewSettings
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        


        // Constructors                                                                                                             
       

        // Methods                                                                                                                  
        public abstract ISettings GetBase();
    }
}
