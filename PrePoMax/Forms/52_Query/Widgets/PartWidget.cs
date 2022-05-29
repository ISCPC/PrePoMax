using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    public class PartWidget : WidgetBase
    {
        // Variables                                                                                                                
        private string _partName;


        // Properties                                                                                                               
        public string PartName { get { return _partName; } set { _partName = value; } }


        // Constructors                                                                                                             
        public PartWidget(string name, string partName)
            : base(name)
        {
            _partName = partName;
        }

    }
}
