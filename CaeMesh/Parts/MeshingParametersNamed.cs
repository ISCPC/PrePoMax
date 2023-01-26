using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;


namespace CaeMesh
{
    [Serializable]
    public class MeshingParametersNamed : NamedClass
    {
        private MeshingParameters _meshingParameters;


        // Properties                                                                                                               
        public double MaxH
        {
            get { return _meshingParameters.MaxH; }
            set { _meshingParameters.MaxH = value; }
        }
        public double MinH
        {
            get { return _meshingParameters.MinH; }
            set { _meshingParameters.MinH = value; }
        }
        public double Fineness
        {
            get { return _meshingParameters.Fineness; }
            set { _meshingParameters.Fineness = value; }
        }
        public double Grading
        {
            get { return _meshingParameters.Grading; }
            set { _meshingParameters.Grading = value; }
        }
        public double Elementsperedge
        {
            get { return _meshingParameters.Elementsperedge; }
            set { _meshingParameters.Elementsperedge = value; }
        }
        public double Elementspercurve
        {
            get { return _meshingParameters.Elementspercurve; }
            set { _meshingParameters.Elementspercurve = value; }
        }
        //
        public bool SecondOrder { get { return _meshingParameters.SecondOrder; } set { _meshingParameters.SecondOrder = value; } }
        public bool QuadDominated { get { return _meshingParameters.QuadDominated; } set { _meshingParameters.QuadDominated = value; } }
        public bool MidsideNodesOnGeometry { get { return _meshingParameters.MidsideNodesOnGeometry; } set { _meshingParameters.MidsideNodesOnGeometry = value; } }
        //
        public int OptimizeSteps2D
        {
            get { return _meshingParameters.OptimizeSteps2D; }
            set { _meshingParameters.OptimizeSteps2D = value; }
        }
        public int OptimizeSteps3D
        {
            get { return _meshingParameters.OptimizeSteps3D; }
            set { _meshingParameters.OptimizeSteps3D = value; }
        }
        //
        public bool SplitCompoundMesh { get { return _meshingParameters.SplitCompoundMesh; } set { _meshingParameters.SplitCompoundMesh = value; } }
        // mmgPlatform
        public bool UseMmg { get { return _meshingParameters.UseMmg; } set { _meshingParameters.UseMmg = value; } }
        public double Hausdorff
        {
            get { return _meshingParameters.Hausdorff; }
            set { _meshingParameters.Hausdorff = value; }
        }
        public bool KeepModelEdges { get { return _meshingParameters.KeepModelEdges; } set { _meshingParameters.KeepModelEdges = value; } }
        //
        public int[] CreationIds { get { return _meshingParameters.CreationIds; } set { _meshingParameters.CreationIds = value; } }
        public Selection CreationData { get { return _meshingParameters.CreationData; } set { _meshingParameters.CreationData = value; } }
        //
        public new string Name
        {
            get { return _meshingParameters.Name; }
            set { _meshingParameters.Name = value; }
        }
        public new bool Active { get { return _meshingParameters.Active; } set { _meshingParameters.Active = value; } }
        public new bool Visible { get { return _meshingParameters.Visible; } set { _meshingParameters.Visible = value; } }
        public new bool Valid
        {
            get { return _meshingParameters.Valid; }
            set { _meshingParameters.Valid = value; }
        }
        public new bool Internal { get { return _meshingParameters.Internal; } set { _meshingParameters.Internal = value; } }



        // Constructors                                                                                                             
        public MeshingParametersNamed(MeshingParameters meshingParameters)
            : base(meshingParameters.Name)
        {
            _meshingParameters = meshingParameters;
            Reset();
        }
       

        // Methods                                                                                                                  
        public void Reset()
        {
            // defaults
            MaxH = 1000;
            MinH = 0;
            Fineness = 0.5;        // has no effect
            Grading = 0.3;
            Elementsperedge = 2;
            Elementspercurve = 2;
            OptimizeSteps2D = 3;
            OptimizeSteps3D = 3;
            SecondOrder = true;
            QuadDominated = false;
            MidsideNodesOnGeometry = false;
            SplitCompoundMesh = false;
            //
            UseMmg = false;
            Hausdorff = 0.01;
            KeepModelEdges = true;
        }
        public void WriteToFile(string fileName)
        {
            throw new NotSupportedException();

            //StringBuilder sb = new StringBuilder();
            ////
            //sb.AppendLine("int      uselocalh                   ... Switch to enable / disable usage of local mesh size modifiers.");
            //sb.AppendLine("1");
            //sb.AppendLine("double   maxh		                ... Maximum global mesh size allowed.");
            //sb.AppendLine(_maxH.ToString());
            //sb.AppendLine("double   minh		                ... Minimum global mesh size allowed.");
            //sb.AppendLine(_minH.ToString());
            //sb.AppendLine("double   fineness	                ... Mesh density: 0...1 (0 => coarse; 1 => fine).");
            //sb.AppendLine(_fineness.ToString());
            //sb.AppendLine("double   grading                     ... Mesh grading: 0...1 (0 => uniform mesh; 1 => aggressive local grading).");
            //sb.AppendLine(_grading.ToString());
            //sb.AppendLine("double   elementsperedge	            ... Number of elements to generate per edge of the geometry.");
            //sb.AppendLine(_elementsperedge.ToString());
            //sb.AppendLine("double   elementspercurve	        ... Elements to generate per curvature radius.");
            //sb.AppendLine(_elementspercurve.ToString());
            //sb.AppendLine("int      closeedgeenable		        ... Enable / Disable mesh refinement at close edges.");
            //sb.AppendLine("0");
            //sb.AppendLine("double 	closeedgefact		        ... Factor to use for refinement at close edges (larger => finer).");
            //sb.AppendLine("2.0");
            //sb.AppendLine("int      minedgelenenable            ... Enable / Disable user defined minimum edge length for edge subdivision.");
            //sb.AppendLine("0");
            //sb.AppendLine("double   minedgelen                  ... Minimum edge length to use while subdividing the edges (default = 1e-4).");
            //sb.AppendLine("0.0001");
            //sb.AppendLine("int      second_order		        ... Generate second-order surface and volume elements.");
            //sb.AppendLine(Convert.ToInt32(_secondOrder && _midsideNodesOnGeometry).ToString());
            //sb.AppendLine("int      quad_dominated		        ... Creates a Quad-dominated mesh.");
            //sb.AppendLine(Convert.ToInt32(_quadDominated).ToString());
            //sb.AppendLine("char*    meshsize_filename           ... Optional external mesh size file.");
            //sb.AppendLine("");
            //sb.AppendLine("int      optsurfmeshenable	        ... Enable / Disable automatic surface mesh optimization.");
            //sb.AppendLine("1");
            //sb.AppendLine("int      optvolmeshenable	        ... Enable / Disable automatic volume mesh optimization.");
            //sb.AppendLine("1");
            //sb.AppendLine("int      optsteps_3d			        ... Number of optimize steps to use for 3-D mesh optimization.");
            //sb.AppendLine(_optimizeSteps3D.ToString());            
            //sb.AppendLine("int      optsteps_2d			        ... Number of optimize steps to use for 2-D mesh optimization.");
            //sb.AppendLine(_optimizeSteps2D.ToString());
            //sb.AppendLine("int      invert_tets			        ... Invert all the volume elements.");
            //sb.AppendLine("0");
            //sb.AppendLine("int      invert_trigs		        ... Invert all the surface triangle elements.");
            //sb.AppendLine("0");
            //sb.AppendLine("int      check_overlap		        ... Check for overlapping surfaces during Surface meshing.");
            //sb.AppendLine("1");
            //sb.AppendLine("int      check_overlapping_boundary	... Check for overlapping surface elements before volume meshing.");
            //sb.AppendLine("1");
            //sb.AppendLine("double   deflection	                ... Open cascade visualization deflection.");
            //sb.AppendLine("0.01");
            ////
            ////Encoding encoding = Encoding.Unicode;
            //System.IO.File.WriteAllText(fileName, sb.ToString());
        }
        static public bool Equals(MeshingParametersNamed meshingParameters1, MeshingParametersNamed meshingParameters2)
        {
            throw new NotSupportedException();

            //if (meshingParameters1 == null) return false;
            //if (meshingParameters2 == null) return false;
            //if (meshingParameters1._maxH != meshingParameters2._maxH) return false;
            //if (meshingParameters1._minH != meshingParameters2._minH) return false;
            //if (meshingParameters1._fineness != meshingParameters2._fineness) return false;
            //if (meshingParameters1._grading != meshingParameters2._grading) return false;
            //if (meshingParameters1._elementsperedge != meshingParameters2._elementsperedge) return false;
            //if (meshingParameters1._elementspercurve != meshingParameters2._elementspercurve) return false;
            //if (meshingParameters1._optimizeSteps2D != meshingParameters2._optimizeSteps2D) return false;
            //if (meshingParameters1._optimizeSteps3D != meshingParameters2._optimizeSteps3D) return false;
            //if (meshingParameters1._secondOrder != meshingParameters2._secondOrder) return false;
            //if (meshingParameters1._quadDominated != meshingParameters2._quadDominated) return false;
            //if (meshingParameters1._midsideNodesOnGeometry != meshingParameters2._midsideNodesOnGeometry) return false;
            //if (meshingParameters1._splitCompoundMesh != meshingParameters2._splitCompoundMesh) return false;
            ////
            //if (meshingParameters1._useMmg != meshingParameters2._useMmg) return false;
            //if (meshingParameters1._hausdorff != meshingParameters2._hausdorff) return false;
            //if (meshingParameters1._keepModelEdges != meshingParameters2._keepModelEdges) return false;
            //return true;
        }
    }
}
