using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CaeGlobals
{
    [Serializable]
    public abstract class SelectionNode : ISerializable
    {
        // Variables                                                                                                                
        protected vtkSelectOperation _selectOpreation;      //ISerializable


        // Properties                                                                                                               
        public vtkSelectOperation SelectOperation { get { return _selectOpreation; } }


        // Constructors                                                                                                             
        public SelectionNode(vtkSelectOperation selectOpreation)
        {
            _selectOpreation = selectOpreation;
        }
        public SelectionNode(SerializationInfo info, StreamingContext context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_selectOpreation":
                        _selectOpreation = (vtkSelectOperation)entry.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        // Methods                                                                                                                  

        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // using typeof() works also for null fields
            info.AddValue("_selectOpreation", _selectOpreation, typeof(vtkSelectOperation));
        }
    }
}
