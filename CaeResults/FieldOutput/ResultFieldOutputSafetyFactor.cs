using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace CaeResults
{
    [Serializable]
    public enum SafetyFactorBasedOnEnum
    {
        [StandardValue("Parts", DisplayName = "Parts", Description = "Parts")]
        Parts,
        [StandardValue("ElementSets", DisplayName = "Element sets", Description = "Element sets")]
        ElementSets
    }


    [Serializable]
    public class ResultFieldOutputSafetyFactor : ResultFieldOutput
    {
        // Variables                                                                                                                
        public static readonly string AllElementsName = "All elements";
        private string _fieldName;
        private string _componentName;
        private SafetyFactorBasedOnEnum _safetyFactorBasedOn;
        private OrderedDictionary<string, double> _itemNameSafetyLimit;


        // Properties                                                                                                               
        public string FieldName { get { return _fieldName; } set { _fieldName = value; } }
        public string ComponentName { get { return _componentName; } set { _componentName = value; } }
        public SafetyFactorBasedOnEnum SafetyFactorBasedOn
        {
            get { return _safetyFactorBasedOn; }
            set { _safetyFactorBasedOn = value; }
        }
        public OrderedDictionary<string, double> ItemNameSafetyLimit
        {
            get { return _itemNameSafetyLimit; }
            set { _itemNameSafetyLimit = value; }
        }


        // Constructors                                                                                                             
        public ResultFieldOutputSafetyFactor(string name, string filedName, string componentName)
            : base(name)
        {
            _fieldName = filedName;
            _componentName = componentName;
            _safetyFactorBasedOn = SafetyFactorBasedOnEnum.Parts;
            _itemNameSafetyLimit = new OrderedDictionary<string, double>("ItemNameSafetyLimit");
        }


        // Methods                                                                                                                  
        public override string[] GetParentFieldNames()
        {
            return new string[] { _fieldName };
        }
        public override string[] GetParentComponentNames()
        {
            return new string[] { _componentName };
        }
        public override string[] GetComponentNames()
        {
            return new string[] { FOComponentNames.SF };
        }
    }
}
