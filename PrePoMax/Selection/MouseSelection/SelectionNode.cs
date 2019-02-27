using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vtkControl;

namespace PrePoMax
{
    [Serializable]
    public abstract class SelectionNode
    {
        // Variables                                                                                                                
        protected vtkSelectOperation _selectOpreation;


        // Properties                                                                                                               
        public vtkSelectOperation SelectOperation { get { return _selectOpreation; } }


        // Constructors                                                                                                             
        public SelectionNode(vtkSelectOperation selectOpreation)
        {
            _selectOpreation = selectOpreation;
        }

        // Methods                                                                                                                  
    }
}
