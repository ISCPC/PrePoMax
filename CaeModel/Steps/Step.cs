using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public abstract class Step : NamedClass
    {
        // Variables                                                                                                                
        private Dictionary<string, BoundaryCondition> _boundayConditions;
        private Dictionary<string, Load> _loads;
        private Dictionary<string, FieldOutput> _fieldOutputs;
        private bool _nlgeom;
        private int _maxIncrements;


        // Properties                                                                                                               
        public Dictionary<string, BoundaryCondition> BoundaryConditions { get { return _boundayConditions; } }
        public Dictionary<string, Load> Loads { get { return _loads; } }
        public Dictionary<string, FieldOutput> FieldOutputs { get { return _fieldOutputs; } }
        public bool Nlgeom { get { return _nlgeom; } set { _nlgeom = value; } }
        public int MaxIncrements { get { return _maxIncrements; } set { _maxIncrements = Math.Max(value, 1); } }


        // Constructors                                                                                                             
        public Step(string name)
            : base(name) 
        {
            _boundayConditions = new Dictionary<string, BoundaryCondition>();
            _loads = new Dictionary<string, Load>();
            _fieldOutputs = new Dictionary<string, FieldOutput>();
            _nlgeom = false;
            _maxIncrements = 100;
        }

        // Methods                                                                                                                  
        public void AddBoundaryCondition(BoundaryCondition boundaryCondition)
        {
            _boundayConditions.Add(boundaryCondition.Name, boundaryCondition);
        }

        public void AddLoad(Load load)
        {
            _loads.Add(load.Name, load);
        }

        public void AddFieldOutput(FieldOutput fieldOutput)
        {
            _fieldOutputs.Add(fieldOutput.Name, fieldOutput);
        }
    }
}
