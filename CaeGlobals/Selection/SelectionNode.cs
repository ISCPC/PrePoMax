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
        protected double _hash;


        // Properties                                                                                                               
        public vtkSelectOperation SelectOperation { get { return _selectOpreation; } }
        public double Hash { get { return _hash; } set { _hash = value; } }


        // Constructors                                                                                                             
        public SelectionNode(vtkSelectOperation selectOpreation)
        {
            _selectOpreation = selectOpreation;
            _hash = -1;
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
                    case "_hash":
                        _hash = (double)entry.Value;
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
            // Using typeof() works also for null fields
            info.AddValue("_selectOpreation", _selectOpreation, typeof(vtkSelectOperation));
            info.AddValue("_hash", _hash, typeof(double));
        }
    }
}
