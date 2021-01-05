using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.Runtime.Serialization;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public enum IncrementationTypeEnum
    {
        Default,
        Automatic,
        Direct
    }

    [Serializable]
    public abstract class Step : NamedClass, ISerializable
    {
        // Variables                                                                                                                
        protected OrderedDictionary<string, BoundaryCondition> _boundayConditions;      //ISerializable
        protected OrderedDictionary<string, Load> _loads;                               //ISerializable
        protected OrderedDictionary<string, FieldOutput> _fieldOutputs;                 //ISerializable
        protected OrderedDictionary<string, HistoryOutput> _historyOutputs;             //ISerializable
        protected bool _perturbation;                                                   //ISerializable
        protected bool _nlgeom;                                                         //ISerializable
        protected int _maxIncrements;                                                   //ISerializable
        protected bool _supportsLoads;                                                  //ISerializable
        protected IncrementationTypeEnum _incrementationType;                           //ISerializable


        // Properties                                                                                                               
        public OrderedDictionary<string, BoundaryCondition> BoundaryConditions { get { return _boundayConditions; } }
        public OrderedDictionary<string, Load> Loads { get { return _loads; } }
        public OrderedDictionary<string, FieldOutput> FieldOutputs { get { return _fieldOutputs; } }
        public OrderedDictionary<string, HistoryOutput> HistoryOutputs { get { return _historyOutputs; } }
        public bool Perturbation { get { return _perturbation; } set { _perturbation = value; } }
        public bool Nlgeom { get { return _nlgeom; } set { _nlgeom = value; } }
        public int MaxIncrements { get { return _maxIncrements; } set { _maxIncrements = Math.Max(value, 1); } }
        public bool SupportsLoads { get { return _supportsLoads; } }
        public IncrementationTypeEnum IncrementationType { get { return _incrementationType; } set { _incrementationType = value; } }


        // Constructors                                                                                                             
        public Step()
            :this("Step")
        { 
        }
        public Step(string name)
            : base(name) 
        {
            _boundayConditions = new OrderedDictionary<string, BoundaryCondition>();
            _loads = new OrderedDictionary<string, Load>();
            _fieldOutputs = new OrderedDictionary<string, FieldOutput>();
            _historyOutputs = new OrderedDictionary<string, HistoryOutput>();
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
                // Compatibility for version v.0.9.0
                _incrementationType = IncrementationTypeEnum.Automatic;
                //
                switch (entry.Name)
                {
                    case "_boundayConditions":
                        if (entry.Value is Dictionary<string, BoundaryCondition> bc)
                        {
                            // Compatibility for version v.0.5.2
                            bc.OnDeserialization(null);
                            _boundayConditions = new OrderedDictionary<string, BoundaryCondition>(bc);
                        }
                        else if (entry.Value is OrderedDictionary<string, BoundaryCondition> bcod) _boundayConditions = bcod;
                        else if (entry.Value == null) _boundayConditions = null;
                        else throw new NotSupportedException();
                        break;
                    case "_loads":
                        if (entry.Value is Dictionary<string, Load> l)
                        {
                            // Compatibility for version v.0.5.2
                            l.OnDeserialization(null);
                            _loads = new OrderedDictionary<string, Load>(l);
                        }
                        else if (entry.Value is OrderedDictionary<string, Load> lod) _loads = lod;
                        else if (entry.Value == null) _loads = null;
                        else throw new NotSupportedException();
                        break;
                    case "_fieldOutputs":
                        if (entry.Value is Dictionary<string, FieldOutput> fo)
                        {
                            // Compatibility for version v.0.5.2
                            fo.OnDeserialization(null);
                            _fieldOutputs = new OrderedDictionary<string, FieldOutput>(fo);
                        }
                        else if (entry.Value is OrderedDictionary<string, FieldOutput> food) _fieldOutputs = food;
                        else if (entry.Value == null) _fieldOutputs = null;
                        else throw new NotSupportedException();
                        break;
                    case "_historyOutputs":
                        if (entry.Value is Dictionary<string, HistoryOutput> ho)
                        {
                            // Compatibility for version v.0.5.2
                            ho.OnDeserialization(null);
                            _historyOutputs = new OrderedDictionary<string, HistoryOutput>(ho);
                        }
                        else if (entry.Value is OrderedDictionary<string, HistoryOutput> hood) _historyOutputs = hood;
                        else if (entry.Value == null) _historyOutputs = null;
                        else throw new NotSupportedException();
                        break;
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
                    //default:
                    //    throw new NotSupportedException();
                }
            }
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

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_boundayConditions", _boundayConditions, typeof(OrderedDictionary<string, BoundaryCondition>));
            info.AddValue("_loads", _loads, typeof(OrderedDictionary<string, Load>));
            info.AddValue("_fieldOutputs", _fieldOutputs, typeof(OrderedDictionary<string, FieldOutput>));
            info.AddValue("_historyOutputs", _historyOutputs, typeof(OrderedDictionary<string, HistoryOutput>));
            info.AddValue("_perturbation", _perturbation, typeof(bool));
            info.AddValue("_nlgeom", _nlgeom, typeof(bool));
            info.AddValue("_maxIncrements", _maxIncrements, typeof(int));
            info.AddValue("_supportsLoads", _supportsLoads, typeof(bool));
            info.AddValue("_incrementationType", _incrementationType, typeof(IncrementationTypeEnum));
        }
    }
}
