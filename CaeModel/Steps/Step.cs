using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.Runtime.Serialization;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace CaeModel
{
    [Serializable]
    public enum IncrementationTypeEnum
    {
        Default,
        Automatic,
        Direct
    }
    //
    [Serializable]
    public enum SolverTypeEnum
    {
        Default,
        PaStiX,
        Pardiso,
        Spooles,
        //
        [StandardValue("IterativeScaling", DisplayName = "Iterative scaling")]
        IterativeScaling,
        //
        [StandardValue("IterativeCholesky", DisplayName = "Iterative Cholesky")]
        IterativeCholesky
    }

    [Serializable]
    public abstract class Step : NamedClass, ISerializable
    {
        // Variables                                                                                                                
        protected OrderedDictionary<string, HistoryOutput> _historyOutputs;             //ISerializable
        protected OrderedDictionary<string, FieldOutput> _fieldOutputs;                 //ISerializable
        protected OrderedDictionary<string, BoundaryCondition> _boundayConditions;      //ISerializable
        protected OrderedDictionary<string, Load> _loads;                               //ISerializable
        protected OrderedDictionary<string, DefinedField> _definedFields;               //ISerializable
        protected bool _perturbation;                                                   //ISerializable
        protected bool _nlgeom;                                                         //ISerializable
        protected int _maxIncrements;                                                   //ISerializable
        protected bool _supportsLoads;                                                  //ISerializable
        protected IncrementationTypeEnum _incrementationType;                           //ISerializable
        protected SolverTypeEnum _solverType;                                           //ISerializable


        // Properties                                                                                                               
        public OrderedDictionary<string, HistoryOutput> HistoryOutputs { get { return _historyOutputs; } }
        public OrderedDictionary<string, FieldOutput> FieldOutputs { get { return _fieldOutputs; } }
        public OrderedDictionary<string, BoundaryCondition> BoundaryConditions { get { return _boundayConditions; } }
        public OrderedDictionary<string, Load> Loads { get { return _loads; } }
        public OrderedDictionary<string, DefinedField> DefinedFields { get { return _definedFields; } }
        public bool Perturbation { get { return _perturbation; } set { _perturbation = value; } }
        public bool Nlgeom { get { return _nlgeom; } set { _nlgeom = value; } }
        public int MaxIncrements { get { return _maxIncrements; } set { _maxIncrements = Math.Max(value, 1); } }
        public bool SupportsLoads { get { return _supportsLoads; } }
        public IncrementationTypeEnum IncrementationType { get { return _incrementationType; } set { _incrementationType = value; } }
        public SolverTypeEnum SolverType { get { return _solverType; } set { _solverType = value; } }


        // Constructors                                                                                                             
        public Step()
            :this("Step")
        { 
        }
        public Step(string name)
            : base(name) 
        {
            _historyOutputs = new OrderedDictionary<string, HistoryOutput>();
            _fieldOutputs = new OrderedDictionary<string, FieldOutput>();
            _boundayConditions = new OrderedDictionary<string, BoundaryCondition>();
            _loads = new OrderedDictionary<string, Load>();
            _definedFields = new OrderedDictionary<string, DefinedField>();
            _perturbation = false;
            _nlgeom = false;
            _maxIncrements = 100;
            _supportsLoads = true;
            _incrementationType = IncrementationTypeEnum.Default;
        }
        public Step(SerializationInfo info, StreamingContext context)
            :base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                _incrementationType = IncrementationTypeEnum.Automatic;         // Compatibility for version v.0.9.0
                
                //
                switch (entry.Name)
                {
                    case "_historyOutputs":
                        _historyOutputs = (OrderedDictionary<string, HistoryOutput>)entry.Value; break;
                    case "_fieldOutputs":
                        _fieldOutputs = (OrderedDictionary<string, FieldOutput>)entry.Value; break;
                    case "_boundayConditions":
                        _boundayConditions = (OrderedDictionary<string, BoundaryCondition>)entry.Value; break;
                    case "_loads":
                        _loads = (OrderedDictionary<string, Load>)entry.Value; break;
                    case "_definedFields":
                        _definedFields = (OrderedDictionary<string, DefinedField>)entry.Value; break;
                    case "_perturbation":
                        _perturbation = (bool)entry.Value; break;
                    case "_nlgeom":
                        _nlgeom = (bool)entry.Value; break;
                    case "_maxIncrements":
                        _maxIncrements = (int)entry.Value; break;
                    case "_supportsLoads":
                        _supportsLoads = (bool)entry.Value; break;
                    case "_incrementationType":
                        _incrementationType = (IncrementationTypeEnum)entry.Value; break;
                    case "_solverType":
                        _solverType = (SolverTypeEnum)entry.Value; break;
                        //default:
                        //    throw new NotSupportedException();
                }
            }
            // Compatibility for version v.1.0.0
            if (_definedFields == null) _definedFields = new OrderedDictionary<string, DefinedField>(); 
        }


        // Methods                                                                                                                  
        public void AddHistoryOutput(HistoryOutput historyOutput)
        {
            _historyOutputs.Add(historyOutput.Name, historyOutput);
        }
        public void AddFieldOutput(FieldOutput fieldOutput)
        {
            _fieldOutputs.Add(fieldOutput.Name, fieldOutput);
        }
        public void AddBoundaryCondition(BoundaryCondition boundaryCondition)
        {
            _boundayConditions.Add(boundaryCondition.Name, boundaryCondition);
        }
        public void AddLoad(Load load)
        {
            _loads.Add(load.Name, load);
        }
        public void AddDefinedField(DefinedField definedField)
        {
            _definedFields.Add(definedField.Name, definedField);
        }

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_historyOutputs", _historyOutputs, typeof(OrderedDictionary<string, HistoryOutput>));
            info.AddValue("_fieldOutputs", _fieldOutputs, typeof(OrderedDictionary<string, FieldOutput>));
            info.AddValue("_boundayConditions", _boundayConditions, typeof(OrderedDictionary<string, BoundaryCondition>));
            info.AddValue("_loads", _loads, typeof(OrderedDictionary<string, Load>));
            info.AddValue("_definedFields", _definedFields, typeof(OrderedDictionary<string, DefinedField>));
            info.AddValue("_perturbation", _perturbation, typeof(bool));
            info.AddValue("_nlgeom", _nlgeom, typeof(bool));
            info.AddValue("_maxIncrements", _maxIncrements, typeof(int));
            info.AddValue("_supportsLoads", _supportsLoads, typeof(bool));
            info.AddValue("_incrementationType", _incrementationType, typeof(IncrementationTypeEnum));
            info.AddValue("_solverType", _solverType, typeof(SolverTypeEnum));
        }
    }
}
