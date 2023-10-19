using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;


namespace CaeMesh
{
    [Serializable]
    public class MeshingParameters : MeshSetupItem , ISerializable
    {
        // fineness ... Mesh density: 0...1 (0 => coarse; 1 => fine)
        // grading  ... Mesh grading: 0...1 (0 => uniform mesh; 1 => aggressive local grading)

        // Variables                                                                                                                
        public static readonly double DefaultFactorMax = 0.05;
        public static readonly double DefaultFactorMin = 0.001;
        public static readonly double DefaultFactorHausdorff = 0.001;
        //
        private bool _advancedView;                 //ISerializable
        //
        private bool _relativeSize;                 //ISerializable
        private double _factorMax;                  //ISerializable
        private double _factorMin;                  //ISerializable
        private double _factorHausdorff;            //ISerializable
        //
        private double _maxH;                       //ISerializable
        private double _minH;                       //ISerializable
        private double _fineness;                   //ISerializable
        private double _grading;                    //ISerializable
        private double _elementsPerEdge;            //ISerializable
        private double _elementsPerCurve;           //ISerializable
        private bool _secondOrder;                  //ISerializable
        private bool _quadDominated;                //ISerializable
        private bool _midsideNodesOnGeometry;       //ISerializable
        private int _optimizeSteps2D;               //ISerializable
        private int _optimizeSteps3D;               //ISerializable
        private bool _splitCompoundMesh;            //ISerializable
        private bool _mergeCompoundParts;           //ISerializable
        // mmgPlatform
        private bool _useMmg;                       //ISerializable
        private double _hausdorff;                  //ISerializable  0.01 for objects of size 1; allowed distance from geometry
        private bool _keepModelEdges;               //ISerializable


        // Properties                                                                                                               
        public bool AdvancedView { get { return _advancedView; } set { _advancedView = value; } }
        public bool RelativeSize { get { return _relativeSize; } set { _relativeSize = value; } }
        public double FactorMax 
        {
            get { return _factorMax; }
            set
            {
                if (value <= 0) throw new Exception("The value must be larger than 0.");
                if (value > 1) throw new Exception("The value must be smaller than 1.");
                _factorMax = value;
                if (value < _factorMin) _factorMin = _factorMax;
            }
        }
        public double FactorMin
        {
            get { return _factorMin; }
            set
            {
                if (value <= 0) throw new Exception("The value must be larger than 0.");
                if (value > 1) throw new Exception("The value must be smaller than 1.");
                _factorMin = value;
                if (value > _factorMax) _factorMax = _factorMin;
            }
        }
        public double FactorHausdorff
        {
            get { return _factorHausdorff; }
            set
            {
                if (value <= 0) throw new Exception("The value must be larger than 0.");
                _factorHausdorff = value;
            }
        }
        public double MaxH 
        {
            get { return _maxH; } 
            set
            {
                if (value < 0) throw new Exception("The value must be larger or equal to 0.");
                _maxH = value;
                if (value < _minH) _minH = _maxH;
            } 
        }
        public double MinH 
        { 
            get { return _minH; } 
            set 
            {
                if (value < 0) throw new Exception("The value must be larger or equal to 0.");
                _minH = value;
                if (value > _maxH) _maxH = _minH;
            } 
        }
        public double Fineness
        {
            get { return _fineness; }
            set
            {
                if (value <= 0) throw new Exception("The value must be larger than 0.");
                if (value > 1) throw new Exception("The value must be smaller or equal to 1.");
                _fineness = value;
            }
        }
        public double Grading
        {
            get { return _grading; }
            set
            {
                if (value <= 0) throw new Exception("The value must be larger than 0.");
                if (value > 1) throw new Exception("The value must be smaller or equal to 1.");
                _grading = value;
            }
        }
        public double ElementsPerEdge
        {
            get { return _elementsPerEdge; }
            set
            {
                if (value <= 0) throw new Exception("The value must be larger than 0.");
                _elementsPerEdge = value;
            }
        }
        public double ElementsPerCurve
        {
            get { return _elementsPerCurve; }
            set
            {
                if (value <= 0) throw new Exception("The value must be larger than 0.");
                _elementsPerCurve = value;
            }
        }
        //
        public bool SecondOrder { get { return _secondOrder; } set { _secondOrder = value; } }
        public bool QuadDominated { get { return _quadDominated; } set { _quadDominated = value; } }
        public bool MidsideNodesOnGeometry { get { return _midsideNodesOnGeometry; } set { _midsideNodesOnGeometry = value; } }
        //
        public int OptimizeSteps2D 
        { 
            get { return _optimizeSteps2D; } 
            set 
            {
                if (value < 0) throw new Exception("The value must be larger or equal to 0.");
                _optimizeSteps2D = value; 
            } 
        }
        public int OptimizeSteps3D
        {
            get { return _optimizeSteps3D; }
            set
            {
                if (value < 0) throw new Exception("The value must be larger or equal to 0.");
                _optimizeSteps3D = value;
            }
        }
        //
        public bool SplitCompoundMesh
        {
            get { return _splitCompoundMesh; }
            set
            {
                _splitCompoundMesh = value;
                if (_splitCompoundMesh) _mergeCompoundParts = false;
            }
        }
        public bool MergeCompoundParts
        {
            get { return _mergeCompoundParts; }
            set
            {
                _mergeCompoundParts = value;
                if (_mergeCompoundParts) _splitCompoundMesh = false;
            }
        }
        // mmgPlatform
        public bool UseMmg { get { return _useMmg; } set { _useMmg = value; } }
        public double Hausdorff
        {
            get { return _hausdorff; }
            set
            {
                if (value <= 0) throw new Exception("The value must be larger than 0.");
                _hausdorff = value;
            }
        }
        public bool KeepModelEdges { get { return _keepModelEdges; } set { _keepModelEdges = value; } }


        // Constructors                                                                                                             
        public MeshingParameters(string name)
            : base(name)
        {
            Reset();
        }
        public MeshingParameters(MeshingParameters meshingParameters)
            : base("tmpName")
        {
            CopyFrom(meshingParameters);
        }
        public MeshingParameters(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_advancedView":
                        _advancedView = (bool)entry.Value; break;
                    //
                    case "_relativeSize":
                        _relativeSize = (bool)entry.Value; break;
                    case "_factorMax":
                        _factorMax = (double)entry.Value; break;
                    case "_factorMin":
                        _factorMin = (double)entry.Value; break;
                    case "_factorHausdorff":
                        _factorHausdorff = (double)entry.Value; break;
                    //
                    case "_maxH":
                        _maxH = (double)entry.Value; break;
                    case "_minH":
                        _minH = (double)entry.Value; break;
                    case "_fineness":
                        _fineness = (double)entry.Value; break;
                    case "_grading":
                        _grading = (double)entry.Value; break;
                    case "_elementsPerEdge":
                    case "_elementsperedge":        // Compatibility for version v1.4.1
                        _elementsPerEdge = (double)entry.Value; break;
                    case "_elementsPerCurve":
                    case "_elementspercurve":       // Compatibility for version v1.4.1
                        _elementsPerCurve = (double)entry.Value; break;
                    case "_secondOrder":
                        _secondOrder = (bool)entry.Value; break;
                    case "_quadDominated":
                        _quadDominated = (bool)entry.Value; break;
                    case "_midsideNodesOnGeometry":
                        _midsideNodesOnGeometry = (bool)entry.Value; break;
                    case "_optimizeSteps2D":
                        _optimizeSteps2D = (int)entry.Value; break;
                    case "_optimizeSteps3D":
                        _optimizeSteps3D = (int)entry.Value; break;
                    case "_splitCompoundMesh":
                        _splitCompoundMesh = (bool)entry.Value; break;
                    case "_mergeCompoundParts":
                        _mergeCompoundParts = (bool)entry.Value; break;
                    //
                    case "_useMmg":
                        _useMmg = (bool)entry.Value; break;
                    case "_hausdorff":
                        _hausdorff = (double)entry.Value; break;
                    case "_keepModelEdges":
                        _keepModelEdges = (bool)entry.Value; break;
                    default:
                        break;
                }
            }
        }

        // Methods                                                                                                                  
        public override void Reset()
        {
            base.Reset();
            // Defaults
            _advancedView = false;
            _relativeSize = false;
            _factorMax = DefaultFactorMax;
            _factorMin = DefaultFactorMin;
            _factorHausdorff = DefaultFactorHausdorff;
            //
            _maxH = 1000;
            _minH = 0;
            _fineness = 0.5;        // has no effect
            _grading = 0.3;
            _elementsPerEdge = 2;
            _elementsPerCurve = 2;
            _optimizeSteps2D = 3;
            _optimizeSteps3D = 3;
            _secondOrder = true;
            _quadDominated = false;
            _midsideNodesOnGeometry = false;
            _splitCompoundMesh = false;
            _mergeCompoundParts = false;
            //
            _useMmg = false;
            _hausdorff = 0.01;
            _keepModelEdges = true;
        }
        public void SetCheckName(bool checkName)
        {
            _checkName = checkName;
        }
        public void WriteToFile(string fileName, double bbDiagonal)
        {
            StringBuilder sb = new StringBuilder();
            //
            sb.AppendLine("int      uselocalh                   ... Switch to enable / disable usage of local mesh size modifiers.");
            sb.AppendLine("1");
            sb.AppendLine("double   maxh		                ... Maximum global mesh size allowed.");
            if (_relativeSize) sb.AppendLine((_factorMax * bbDiagonal).ToString());
            else sb.AppendLine(_maxH.ToString());
            sb.AppendLine("double   minh		                ... Minimum global mesh size allowed.");
            if (_relativeSize) sb.AppendLine((_factorMin * bbDiagonal).ToString());
            else sb.AppendLine(_minH.ToString());
            sb.AppendLine("double   fineness	                ... Mesh density: 0...1 (0 => coarse; 1 => fine).");
            sb.AppendLine(_fineness.ToString());
            sb.AppendLine("double   grading                     ... Mesh grading: 0...1 (0 => uniform mesh; 1 => aggressive local grading).");
            sb.AppendLine(_grading.ToString());
            sb.AppendLine("double   elementsperedge	            ... Number of elements to generate per edge of the geometry.");
            sb.AppendLine(_elementsPerEdge.ToString());
            sb.AppendLine("double   elementspercurve	        ... Elements to generate per curvature radius.");
            sb.AppendLine(_elementsPerCurve.ToString());
            sb.AppendLine("int      closeedgeenable		        ... Enable / Disable mesh refinement at close edges.");
            sb.AppendLine("0");
            sb.AppendLine("double 	closeedgefact		        ... Factor to use for refinement at close edges (larger => finer).");
            sb.AppendLine("2.0");
            sb.AppendLine("int      minedgelenenable            ... Enable / Disable user defined minimum edge length for edge subdivision.");
            sb.AppendLine("0");
            sb.AppendLine("double   minedgelen                  ... Minimum edge length to use while subdividing the edges (default = 1e-4).");
            sb.AppendLine("0.0001");
            sb.AppendLine("int      second_order		        ... Generate second-order surface and volume elements.");
            sb.AppendLine(Convert.ToInt32(_secondOrder && _midsideNodesOnGeometry).ToString());
            sb.AppendLine("int      quad_dominated		        ... Creates a Quad-dominated mesh.");
            sb.AppendLine(Convert.ToInt32(_quadDominated).ToString());
            sb.AppendLine("char*    meshsize_filename           ... Optional external mesh size file.");
            sb.AppendLine("");
            sb.AppendLine("int      optsurfmeshenable	        ... Enable / Disable automatic surface mesh optimization.");
            sb.AppendLine("1");
            sb.AppendLine("int      optvolmeshenable	        ... Enable / Disable automatic volume mesh optimization.");
            sb.AppendLine("1");
            sb.AppendLine("int      optsteps_3d			        ... Number of optimize steps to use for 3-D mesh optimization.");
            sb.AppendLine(_optimizeSteps3D.ToString());            
            sb.AppendLine("int      optsteps_2d			        ... Number of optimize steps to use for 2-D mesh optimization.");
            sb.AppendLine(_optimizeSteps2D.ToString());
            sb.AppendLine("int      invert_tets			        ... Invert all the volume elements.");
            sb.AppendLine("0");
            sb.AppendLine("int      invert_trigs		        ... Invert all the surface triangle elements.");
            sb.AppendLine("0");
            sb.AppendLine("int      check_overlap		        ... Check for overlapping surfaces during Surface meshing.");
            sb.AppendLine("1");
            sb.AppendLine("int      check_overlapping_boundary	... Check for overlapping surface elements before volume meshing.");
            sb.AppendLine("1");
            sb.AppendLine("double   deflection	                ... Open cascade visualization deflection.");
            sb.AppendLine("0.01");
            //
            //Encoding encoding = Encoding.Unicode;
            System.IO.File.WriteAllText(fileName, sb.ToString());
        }
        public void CopyFrom(MeshingParameters meshingParameters)
        {
            base.CopyFrom(meshingParameters);
            //
            _advancedView = meshingParameters.AdvancedView;
            //
            _relativeSize = meshingParameters.RelativeSize;
            _factorMax = meshingParameters.FactorMax;
            _factorMin = meshingParameters.FactorMin;
            _factorHausdorff = meshingParameters.FactorHausdorff;
            //
            _maxH = meshingParameters.MaxH;
            _minH = meshingParameters.MinH;
            _fineness = meshingParameters.Fineness;
            _grading = meshingParameters.Grading;
            _elementsPerEdge = meshingParameters.ElementsPerEdge;
            _elementsPerCurve = meshingParameters.ElementsPerCurve;
            _optimizeSteps2D = meshingParameters.OptimizeSteps2D;
            _optimizeSteps3D = meshingParameters.OptimizeSteps3D;
            _secondOrder = meshingParameters.SecondOrder;
            _quadDominated = meshingParameters.QuadDominated;
            _midsideNodesOnGeometry = meshingParameters.MidsideNodesOnGeometry;
            _splitCompoundMesh = meshingParameters.SplitCompoundMesh;
            _mergeCompoundParts = meshingParameters.MergeCompoundParts;
            //
            _useMmg = meshingParameters.UseMmg;
            _hausdorff = meshingParameters.Hausdorff;
            _keepModelEdges = meshingParameters.KeepModelEdges;
        }
        public MeshingParameters DeepCopy()
        {
            return new MeshingParameters(this);
        }
        //
        static public bool Equals(MeshingParameters meshingParameters1, MeshingParameters meshingParameters2)
        {
            if (meshingParameters1 == null) return false;
            if (meshingParameters2 == null) return false;
            //
            if (meshingParameters1._advancedView != meshingParameters2._advancedView) return false;
            if (meshingParameters1._relativeSize != meshingParameters2._relativeSize) return false;
            if (meshingParameters1._factorMax != meshingParameters2._factorMax) return false;
            if (meshingParameters1._factorMin != meshingParameters2._factorMin) return false;
            if (meshingParameters1._factorHausdorff != meshingParameters2._factorHausdorff) return false;
            //
            if (meshingParameters1._maxH != meshingParameters2._maxH) return false;
            if (meshingParameters1._minH != meshingParameters2._minH) return false;
            if (meshingParameters1._fineness != meshingParameters2._fineness) return false;
            if (meshingParameters1._grading != meshingParameters2._grading) return false;
            if (meshingParameters1._elementsPerEdge != meshingParameters2._elementsPerEdge) return false;
            if (meshingParameters1._elementsPerCurve != meshingParameters2._elementsPerCurve) return false;
            if (meshingParameters1._optimizeSteps2D != meshingParameters2._optimizeSteps2D) return false;
            if (meshingParameters1._optimizeSteps3D != meshingParameters2._optimizeSteps3D) return false;
            if (meshingParameters1._secondOrder != meshingParameters2._secondOrder) return false;
            if (meshingParameters1._quadDominated != meshingParameters2._quadDominated) return false;
            if (meshingParameters1._midsideNodesOnGeometry != meshingParameters2._midsideNodesOnGeometry) return false;
            if (meshingParameters1._splitCompoundMesh != meshingParameters2._splitCompoundMesh) return false;
            if (meshingParameters1._mergeCompoundParts != meshingParameters2._mergeCompoundParts) return false;
            //
            if (meshingParameters1._useMmg != meshingParameters2._useMmg) return false;
            if (meshingParameters1._hausdorff != meshingParameters2._hausdorff) return false;
            if (meshingParameters1._keepModelEdges != meshingParameters2._keepModelEdges) return false;
            return true;
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // Using typeof() works also for null fields
            info.AddValue("_advancedView", _advancedView, typeof(bool));
            //
            info.AddValue("_relativeSize", _relativeSize, typeof(bool));
            info.AddValue("_factorMax", _factorMax, typeof(double));
            info.AddValue("_factorMin", _factorMin, typeof(double));
            info.AddValue("_factorHausdorff", _factorHausdorff, typeof(double));
            //
            info.AddValue("_maxH", _maxH, typeof(double));
            info.AddValue("_minH", _minH, typeof(double));
            info.AddValue("_fineness", _fineness, typeof(double));
            info.AddValue("_grading", _grading, typeof(double));
            info.AddValue("_elementsPerEdge", _elementsPerEdge, typeof(double));
            info.AddValue("_elementsPerCurve", _elementsPerCurve, typeof(double));
            info.AddValue("_secondOrder", _secondOrder, typeof(bool));
            info.AddValue("_quadDominated", _quadDominated, typeof(bool));
            info.AddValue("_midsideNodesOnGeometry", _midsideNodesOnGeometry, typeof(bool));
            info.AddValue("_optimizeSteps2D", _optimizeSteps2D, typeof(int));
            info.AddValue("_optimizeSteps3D", _optimizeSteps3D, typeof(int));
            info.AddValue("_splitCompoundMesh", _splitCompoundMesh, typeof(bool));
            info.AddValue("_mergeCompoundParts", _mergeCompoundParts, typeof(bool));
            //
            info.AddValue("_useMmg", _useMmg, typeof(bool));
            info.AddValue("_hausdorff", _hausdorff, typeof(double));
            info.AddValue("_keepModelEdges", _keepModelEdges, typeof(bool));
        }
    }
}
