using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CaeGlobals
{
    [Serializable]
    public class SelectionNodeMouse : SelectionNode, ISerializable
    {
        // Variables                                                                                                                
        private double[] _pickedPoint;                                  //ISerializable
        private double[][] _planeParameters; // Ox,Oy,Oz,Nx,Ny,Nz       //ISerializable
        private vtkSelectBy _selectBy;                                  //ISerializable
        private double _angle;                                          //ISerializable        
        private int _partId;                                            //ISerializable


        // Properties                                                                                                               
        public double[] PickedPoint { get { return _pickedPoint; } set { _pickedPoint = value; } }
        public double[][] PlaneParameters { get { return _planeParameters; } set { _planeParameters = value; } }
        public vtkSelectBy SelectBy { get { return _selectBy; } set { _selectBy = value; } }
        public double Angle { get { return _angle; } set { _angle = value; } }        
        public int PartId { get { return _partId; } set { _partId = value; } }
        public bool IsGeometryBased
        {
            get
            {
                return (_selectBy.IsGeometryBased());
            }
        }


        // Constructors                                                                                                             
        public SelectionNodeMouse(double[] pickedPoint, double[][] planeParameters, vtkSelectOperation selectOpreation, vtkSelectBy selectBy, double angle)
            : base(selectOpreation)
        {
            _pickedPoint = pickedPoint;
            _planeParameters = planeParameters;
            _selectBy = selectBy;
            _angle = angle;
            _partId = -1;
        }
        public SelectionNodeMouse(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _partId = -1;   // Compatibility for version v0.5.2
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_pickedPoint":
                        _pickedPoint = (double[])entry.Value; break;
                    case "_planeParameters":
                        _planeParameters = (double[][])entry.Value; break;
                    case "_selectBy":
                        _selectBy = (vtkSelectBy)entry.Value; break;
                    case "_angle":
                        _angle = (double)entry.Value; break;
                    case "_partId":
                        _partId = (int)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // using typeof() works also for null fields
            info.AddValue("_pickedPoint", _pickedPoint, typeof(double[]));
            info.AddValue("_planeParameters", _planeParameters, typeof(double[][]));
            info.AddValue("_selectBy", _selectBy, typeof(vtkSelectBy));
            info.AddValue("_angle", _angle, typeof(double));
            info.AddValue("_partId", _partId, typeof(int));
        }

    }
}
