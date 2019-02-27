using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class MeshingParameters
    {
        // Variables                                                                                                                
        public double _maxH;
        public double _minH;
        public bool _secondOrder;
        public int _optimizeSteps2D;
        public int _optimizeSteps3D;


        // Properties                                                                                                               
        public double MaxH 
        {
            get { return _maxH; } 
            set
            {
                if (value < 0) throw new Exception("The value must be larger or equal to 0.");
                if (value < _minH) throw new Exception("The value must larger than min value.");
                _maxH = value;
            } 
        }
        public double MinH 
        { 
            get { return _minH; } 
            set 
            {
                if (value < 0) throw new Exception("The value must be larger or equal to 0.");
                if (value > _maxH) throw new Exception("The value must smaller than max value.");
                _minH = value; 
            } 
        }
        public bool SecondOrder { get { return _secondOrder; } set { _secondOrder = value; } }
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


        // Constructors                                                                                                             
        public MeshingParameters()
        {
            _maxH = 1000;
            _minH = 0;
            _optimizeSteps2D = 3;
            _optimizeSteps3D = 3;
            _secondOrder = false;
        }


        // Methods                                                                                                                  
        public void WriteToFile(string fileName)
        {
            

            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("int      uselocalh                   ... Switch to enable / disable usage of local mesh size modifiers.");
            sb.AppendLine("1");
            sb.AppendLine("double   maxh		                ... Maximum global mesh size allowed.");
            sb.AppendLine(_maxH.ToString());
            sb.AppendLine("double   minh		                ... Minimum global mesh size allowed.");
            sb.AppendLine(_minH.ToString());
            sb.AppendLine("double   fineness	                ... Mesh density: 0...1 (0 => coarse; 1 => fine)");
            sb.AppendLine("0.3");
            sb.AppendLine("double   grading                     ... Mesh grading: 0...1 (0 => uniform mesh; 1 => aggressive local grading)");
            sb.AppendLine("0.3");
            sb.AppendLine("double   elementsperedge	            ... Number of elements to generate per edge of the geometry.");
            sb.AppendLine("2.0");
            sb.AppendLine("double   elementspercurve	        ... Elements to generate per curvature radius.");
            sb.AppendLine("2.0");
            sb.AppendLine("int      closeedgeenable		        ... Enable / Disable mesh refinement at close edges.");
            sb.AppendLine("0");
            sb.AppendLine("double 	closeedgefact		        ... Factor to use for refinement at close edges (larger => finer)");
            sb.AppendLine("2.0");
            sb.AppendLine("int      minedgelenenable");
            sb.AppendLine("0");
            sb.AppendLine("double   minedgelen");
            sb.AppendLine("0.0001");
            sb.AppendLine("int      second_order		        ... Generate second-order surface and volume elements.");
            sb.AppendLine(Convert.ToInt32(_secondOrder).ToString());
            sb.AppendLine("int      quad_dominated		        ... Creates a Quad-dominated mesh.");
            sb.AppendLine("0");
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

            System.IO.File.WriteAllText(fileName, sb.ToString());
        }
    }
}
