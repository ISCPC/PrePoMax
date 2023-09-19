using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using CaeResults;
using System.Runtime.Serialization;
using DynamicTypeDescriptor;

namespace CaeModel
{
    [Serializable]
    public enum HydrostaticPressureCutoffEnum
    {
        [StandardValue("None", Description = "None", DisplayName = "None")]
        None,
        [StandardValue("Positive", Description = "Positive cutoff", DisplayName = "Positive cutoff")]
        Positive,
        [StandardValue("Negative", Description = "Negative cutoff", DisplayName = "Negative cutoff")]
        Negative
    }

    [Serializable]
    public class HydrostaticPressure : VariablePressure, IPreviewable, ISerializable
    {
        // Variables                                                                                                                
        private EquationContainer _x1;                                      //ISerializable
        private EquationContainer _y1;                                      //ISerializable
        private EquationContainer _z1;                                      //ISerializable
        private EquationContainer _x2;                                      //ISerializable
        private EquationContainer _y2;                                      //ISerializable
        private EquationContainer _z2;                                      //ISerializable
        private EquationContainer _n1;                                      //ISerializable
        private EquationContainer _n2;                                      //ISerializable
        private EquationContainer _n3;                                      //ISerializable
        private EquationContainer _firstPointPressure;                      //ISerializable
        private EquationContainer _secondPointPressure;                     //ISerializable
        private HydrostaticPressureCutoffEnum _hydrostaticPressureCutoff;   //ISerializable


        // Properties                                                                                                               
        public EquationContainer X1 { get { return _x1; } set { SetX1(value); } }
        public EquationContainer Y1 { get { return _y1; } set { SetY1(value); } }
        public EquationContainer Z1 { get { return _z1; } set { SetZ1(value); } }
        public EquationContainer X2 { get { return _x2; } set { SetX2(value); } }
        public EquationContainer Y2 { get { return _y2; } set { SetY2(value); } }
        public EquationContainer Z2 { get { return _z2; } set { SetZ2(value); } }
        public EquationContainer N1 { get { return _n1; } set { SetN1(value); } }
        public EquationContainer N2 { get { return _n2; } set { SetN2(value); } }
        public EquationContainer N3 { get { return _n3; } set { SetN3(value); } }
        public EquationContainer FirstPointPressure
        {
            get { return _firstPointPressure; }
            set { SetFirstPointPressure(value); }
        }
        public EquationContainer SecondPointPressure
        {
            get { return _secondPointPressure; }
            set { SetSecondPointPressure(value); }
        }
        public HydrostaticPressureCutoffEnum HydrostaticPressureCutoff
        {
            get { return _hydrostaticPressureCutoff; }
            set { _hydrostaticPressureCutoff = value; }
        }


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
            //
            _hydrostaticPressureCutoff = HydrostaticPressureCutoffEnum.None;
        }
        public HydrostaticPressure(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Compatibility for version v1.4.0
            if (_regionType == RegionTypeEnum.PartName) _regionType = RegionTypeEnum.Selection;
            _hydrostaticPressureCutoff = HydrostaticPressureCutoffEnum.None;
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
                        SetX1((EquationContainer)entry.Value, false); break;
                    case "_y1":
                        SetY1((EquationContainer)entry.Value, false); break;
                    case "_z1":
                        SetZ1((EquationContainer)entry.Value, false); break;
                    case "_x2":
                        SetX2((EquationContainer)entry.Value, false); break;
                    case "_y2":
                        SetY2((EquationContainer)entry.Value, false); break;
                    case "_z2":
                        SetZ2((EquationContainer)entry.Value, false); break;
                    case "_n1":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueN1)
                            N1 = new EquationContainer(typeof(StringLengthConverter), valueN1);
                        else
                            SetN1((EquationContainer)entry.Value, false);
                        break;
                    case "_n2":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueN2)
                            N2 = new EquationContainer(typeof(StringLengthConverter), valueN2);
                        else
                            SetN2((EquationContainer)entry.Value, false);
                        break;
                    case "_n3":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueN3)
                            N3 = new EquationContainer(typeof(StringLengthConverter), valueN3);
                        else
                            SetN3((EquationContainer)entry.Value, false);
                        break;
                    case "_firstPointPressure":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueP1)
                            FirstPointPressure = new EquationContainer(typeof(StringPressureConverter), valueP1);
                        else
                            SetFirstPointPressure((EquationContainer)entry.Value, false);
                        break;
                    case "_secondPointPressure":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueP2)
                            SecondPointPressure = new EquationContainer(typeof(StringPressureConverter), valueP2);
                        else
                            SetSecondPointPressure((EquationContainer)entry.Value, false);
                        break;
                    case "_hydrostaticPressureCutoff":
                        _hydrostaticPressureCutoff = (HydrostaticPressureCutoffEnum)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        
        private void SetX1(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _x1, value, null, checkEquation);
        }
        private void SetY1(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _y1, value, null, checkEquation);
        }
        private void SetZ1(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _z1, value, Check2D, checkEquation);
        }
        private void SetX2(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _x2, value, null, checkEquation);
        }
        private void SetY2(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _y2, value, null, checkEquation);
        }
        private void SetZ2(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _z2, value, Check2D, checkEquation);
        }
        private void SetN1(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _n1, value, null, checkEquation);
        }
        private void SetN2(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _n2, value, null, checkEquation);
        }
        private void SetN3(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _n3, value, Check2D, checkEquation);
        }
        private void SetFirstPointPressure(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _firstPointPressure, value, null, checkEquation);
        }
        private void SetSecondPointPressure(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _secondPointPressure, value, null, checkEquation);
        }
        //
        private double Check2D(double value)
        {
            if (_twoD) return 0;
            else return value;
        }        
        // IContainsEquations
        public override void CheckEquations()
        {
            base.CheckEquations();
            //
            _x1.CheckEquation();
            _y1.CheckEquation();
            _z1.CheckEquation();
            _x2.CheckEquation();
            _y2.CheckEquation();
            _z2.CheckEquation();
            _n1.CheckEquation();
            _n2.CheckEquation();
            _n3.CheckEquation();
            _firstPointPressure.CheckEquation();
            _secondPointPressure.CheckEquation();
        }
        //
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
            if (_hydrostaticPressureCutoff == HydrostaticPressureCutoffEnum.None) return p;
            else if (_hydrostaticPressureCutoff == HydrostaticPressureCutoffEnum.Positive) return p > 0 ? 0 : p;
            else if (_hydrostaticPressureCutoff == HydrostaticPressureCutoffEnum.Negative) return p < 0 ? 0 : p;
            else throw new NotSupportedException();
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
            info.AddValue("_hydrostaticPressureCutoff", _hydrostaticPressureCutoff, typeof(HydrostaticPressureCutoffEnum));
        }
    }
}
