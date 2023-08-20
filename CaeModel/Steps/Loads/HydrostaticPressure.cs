using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using CaeResults;
using System.Runtime.Serialization;

namespace CaeModel
{
    [Serializable]
    public class HydrostaticPressure : VariablePressure, IPreviewable, ISerializable
    {
        // Variables                                                                                                                
        private EquationContainer _x1;                         //ISerializable
        private EquationContainer _y1;                         //ISerializable
        private EquationContainer _z1;                         //ISerializable
        private EquationContainer _x2;                         //ISerializable
        private EquationContainer _y2;                         //ISerializable
        private EquationContainer _z2;                         //ISerializable
        private EquationContainer _n1;                         //ISerializable
        private EquationContainer _n2;                         //ISerializable
        private EquationContainer _n3;                         //ISerializable
        private EquationContainer _firstPointPressure;         //ISerializable
        private EquationContainer _secondPointPressure;        //ISerializable


        // Properties                                                                                                               
        public EquationContainer X1 { get { return _x1; } set { _x1 = value; } }
        public EquationContainer Y1 { get { return _y1; } set { _y1 = value; } }
        public EquationContainer Z1
        {
            get { return _z1; }
            set
            {
                _z1 = value;
                _z1.CheckValue = Check2D;
                //
                _z1.CheckEquation();
            }
        }
        public EquationContainer X2 { get { return _x2; } set { _x2 = value; } }
        public EquationContainer Y2 { get { return _y2; } set { _y2 = value; } }
        public EquationContainer Z2
        {
            get { return _z2; }
            set
            {
                _z2 = value;
                _z2.CheckValue = Check2D;
                //
                _z2.CheckEquation();
            }
        }
        public EquationContainer N1 { get { return _n1; } set { _n1 = value; } }
        public EquationContainer N2 { get { return _n2; } set { _n2 = value; } }
        public EquationContainer N3
        {
            get { return _n3; }
            set
            {
                _n3 = value;
                _n3.CheckValue = Check2D;
                //
                _n3.CheckEquation();
            }
        }
        public EquationContainer FirstPointPressure { get { return _firstPointPressure; } set { _firstPointPressure = value; } }
        public EquationContainer SecondPointPressure { get { return _secondPointPressure; } set { _secondPointPressure = value; } }


        // Constructors                                                                                                             
        public HydrostaticPressure(string name, string regionName, RegionTypeEnum regionType, bool twoD,
                                   bool complex, double phaseDeg)
            : this(name, regionName, regionType, new double[] { 0, 0, 0 }, new double[] { 0, 0, 0 }, 0, 0, 0, 0, 0, twoD,
                   complex, phaseDeg)
        {
        }
        public HydrostaticPressure(string name, string regionName, RegionTypeEnum regionType, double[] firstPointCoor,
                                   double[] secondPointCoor, double nx, double ny, double nz, double firstPointPressure,
                                   double secondPointPressure, bool twoD, bool complex, double phaseDeg)
            : base(name, regionName, regionType, twoD, complex, phaseDeg)
        {
            X1 = new EquationContainer(typeof(StringLengthConverter), firstPointCoor[0]);
            Y1 = new EquationContainer(typeof(StringLengthConverter), firstPointCoor[1]);
            Z1 = new EquationContainer(typeof(StringLengthConverter), firstPointCoor[2]);
            X2 = new EquationContainer(typeof(StringLengthConverter), secondPointCoor[0]);
            Y2 = new EquationContainer(typeof(StringLengthConverter), secondPointCoor[1]);
            Z2 = new EquationContainer(typeof(StringLengthConverter), secondPointCoor[2]);
            N1 = new EquationContainer(typeof(StringLengthConverter), nx);
            N2 = new EquationContainer(typeof(StringLengthConverter), ny);
            N3 = new EquationContainer(typeof(StringLengthConverter), nz);
            //
            FirstPointPressure = new EquationContainer(typeof(StringPressureConverter), firstPointPressure);
            SecondPointPressure = new EquationContainer(typeof(StringPressureConverter), secondPointPressure);
        }
        public HydrostaticPressure(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Compatibility for version v1.4.0
            if (_regionType == RegionTypeEnum.PartName) _regionType = RegionTypeEnum.Selection;
            //
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    // Compatibility for version v1.4.0
                    case "_firstPointCoor":
                        double[] coor1 = (double[])entry.Value;
                        X1 = new EquationContainer(typeof(StringLengthConverter), coor1[0]);
                        Y1 = new EquationContainer(typeof(StringLengthConverter), coor1[1]);
                        Z1 = new EquationContainer(typeof(StringLengthConverter), coor1[2]);
                        break;
                    // Compatibility for version v1.4.0
                    case "_secondPointCoor":
                        double[] coor2 = (double[])entry.Value;
                        X2 = new EquationContainer(typeof(StringLengthConverter), coor2[0]);
                        Y2 = new EquationContainer(typeof(StringLengthConverter), coor2[1]);
                        Z2 = new EquationContainer(typeof(StringLengthConverter), coor2[2]);
                        break;
                    case "_x1":
                        X1 = (EquationContainer)entry.Value; break;
                    case "_y1":
                        Y1 = (EquationContainer)entry.Value; break;
                    case "_z1":
                        Z1 = (EquationContainer)entry.Value; break;
                    case "_x2":
                        X2 = (EquationContainer)entry.Value; break;
                    case "_y2":
                        Y2 = (EquationContainer)entry.Value; break;
                    case "_z2":
                        Z2 = (EquationContainer)entry.Value; break;
                    case "_n1":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueN1)
                            N1 = new EquationContainer(typeof(StringLengthConverter), valueN1);
                        else
                            N1 = (EquationContainer)entry.Value;
                        break;
                    case "_n2":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueN2)
                            N2 = new EquationContainer(typeof(StringLengthConverter), valueN2);
                        else
                            N2 = (EquationContainer)entry.Value;
                        break;
                    case "_n3":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueN3)
                            N3 = new EquationContainer(typeof(StringLengthConverter), valueN3);
                        else
                            N3 = (EquationContainer)entry.Value;
                        break;
                    case "_firstPointPressure":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueP1)
                            FirstPointPressure = new EquationContainer(typeof(StringPressureConverter), valueP1);
                        else
                            FirstPointPressure = (EquationContainer)entry.Value;
                        break;
                    case "_secondPointPressure":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueP2)
                            SecondPointPressure = new EquationContainer(typeof(StringPressureConverter), valueP2);
                        else
                            SecondPointPressure = (EquationContainer)entry.Value;
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private double Check2D(double value)
        {
            if (_twoD) return 0;
            else return value;
        }
        public bool IsProperlyDefined(out string error)
        {
            error = "";
            Vec3D p1 = new Vec3D(_x1.Value, _y1.Value, _z1.Value);
            Vec3D p2 = new Vec3D(_x2.Value, _y2.Value, _z2.Value);
            Vec3D n = new Vec3D(_n1.Value, _n2.Value, _n3.Value);
            //
            if (n.Normalize() > 0)
            {
                double d1 = -Vec3D.DotProduct(n, p1);   // d1: distance of the plane through p1 from origin
                double d2 = -Vec3D.DotProduct(n, p2);   // d2: distance of the plane through p2 from origin
                //
                if (Math.Abs(d1 - d2) < 1E-6)
                {
                    error = "The first and the second point must not be on a plane perpendicular to the pressure direction.";
                    return false;
                }
            }
            else
            {
                error = "The pressure direction is not properly defined.";
                return false;
            }
            //
            return true;
        }
        public FeResults GetPreview(FeMesh targetMesh, string resultName, UnitSystemType unitSystemType)
        {
            //
            PartExchangeData allData = new PartExchangeData();
            targetMesh.GetAllNodesAndCells(out allData.Nodes.Ids, out allData.Nodes.Coor, out allData.Cells.Ids,
                                           out allData.Cells.CellNodeIds, out allData.Cells.Types);
            //
            FeSurface surface = targetMesh.Surfaces[_surfaceName];
            FeNodeSet nodeSet = targetMesh.NodeSets[surface.NodeSetName];
            HashSet<int> nodeIds = new HashSet<int>(nodeSet.Labels);
            //
            float[] values = new float[allData.Nodes.Coor.Length];
            //
            for (int i = 0; i < values.Length; i++)
            {
                if (nodeIds.Contains(allData.Nodes.Ids[i]))
                {
                    values[i] = (float)GetPressureForPoint(allData.Nodes.Coor[i]);
                }
                else
                {
                    values[i] = float.NaN;
                }
            }
            //
            Dictionary<int, int> nodeIdsLookUp = new Dictionary<int, int>();
            for (int i = 0; i < allData.Nodes.Coor.Length; i++) nodeIdsLookUp.Add(allData.Nodes.Ids[i], i);
            FeResults results = new FeResults(resultName);
            results.SetMesh(targetMesh, nodeIdsLookUp);
            // Add distances
            FieldData fieldData = new FieldData(FOFieldNames.Imported);
            fieldData.GlobalIncrementId = 1;
            fieldData.StepType = StepTypeEnum.Static;
            fieldData.Time = 1;
            fieldData.MethodId = 1;
            fieldData.StepId = 1;
            fieldData.StepIncrementId = 1;
            // Add values
            Field field = new Field(fieldData.Name);
            field.AddComponent(FOComponentNames.PRESS, values);
            results.AddField(fieldData, field);
            // Unit system
            results.UnitSystem = new UnitSystem(unitSystemType);
            //
            return results;
        }
        public override double GetPressureForPoint(double[] point)
        {
            Vec3D n = new Vec3D(_n1.Value, _n2.Value, _n3.Value);
            n.Normalize();
            //
            double d1 = -(_x1.Value * n.X + _y1.Value * n.Y + _z1.Value * n.Z);
            double d2 = -(_x2.Value * n.X + _y2.Value * n.Y + _z2.Value * n.Z);
            double d = -(point[0] * n.X + point[1] * n.Y + point[2] * n.Z);
            //
            double p = _firstPointPressure.Value + (_secondPointPressure.Value - _firstPointPressure.Value) / (d2 - d1) * (d - d1);
            //
            return p;
        }

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_x1", _x1, typeof(EquationContainer));
            info.AddValue("_y1", _y1, typeof(EquationContainer));
            info.AddValue("_z1", _z1, typeof(EquationContainer));
            info.AddValue("_x2", _x2, typeof(EquationContainer));
            info.AddValue("_y2", _y2, typeof(EquationContainer));
            info.AddValue("_z2", _z2, typeof(EquationContainer));
            info.AddValue("_n1", _n1, typeof(EquationContainer));
            info.AddValue("_n2", _n2, typeof(EquationContainer));
            info.AddValue("_n3", _n3, typeof(EquationContainer));
            info.AddValue("_firstPointPressure", _firstPointPressure, typeof(EquationContainer));
            info.AddValue("_secondPointPressure", _secondPointPressure, typeof(EquationContainer));
        }
    }
}
