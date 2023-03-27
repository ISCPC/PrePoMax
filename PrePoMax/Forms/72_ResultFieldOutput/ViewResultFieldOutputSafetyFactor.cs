using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using CaeResults;
using CaeModel;

namespace PrePoMax
{
    [Serializable]
    public class ViewResultFieldOutputSafetyFactor : ViewResultFieldOutput
    {
        // Variables                                                                                                                
        private ResultFieldOutputSafetyFactor _resultFieldOutput;
        private List<SafetyFactorPartDataPoint> _partPoints;
        private List<SafetyFactorElementSetDataPoint> _elementSetPoints;
        private Dictionary<string, string[]> _filedNameComponentNames;


        // Properties                                                                                                               
        public override string Name { get { return _resultFieldOutput.Name; } set { _resultFieldOutput.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Field name")]
        [DescriptionAttribute("Filed name for the field output.")]
        public string FieldName
        {
            get { return _resultFieldOutput.FieldName; }
            set
            {
                if (_resultFieldOutput.FieldName != value)
                {
                    _resultFieldOutput.FieldName = value;
                    UpdateComponents();
                }
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Component name")]
        [DescriptionAttribute("Component name for the field output.")]
        public string ComponentName
        {
            get { return _resultFieldOutput.ComponentName; }
            set { _resultFieldOutput.ComponentName = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "SF based on")]
        [DescriptionAttribute("Select how the safety limit values will be defined for the field output.")]
        public SafetyFactorBasedOnEnum SafetyFactorBasedOn
        {
            get { return _resultFieldOutput.SafetyFactorBasedOn; }
            set { _resultFieldOutput.SafetyFactorBasedOn = value; }
        }
        //
        [Browsable(false)]
        public List<SafetyFactorPartDataPoint> PartPoints
        {
            get { return _partPoints; }
            set { _partPoints = value; }
        }
        //
        [Browsable(false)]
        public List<SafetyFactorElementSetDataPoint> ElementSetPoints
        {
            get { return _elementSetPoints; }
            set { _elementSetPoints = value; }
        }
        [Browsable(false)]
        public object DataPoints
        {
            get
            {
                if (SafetyFactorBasedOn == SafetyFactorBasedOnEnum.Parts) return _partPoints;
                if (SafetyFactorBasedOn == SafetyFactorBasedOnEnum.ElementSets) return _elementSetPoints;
                else throw new NotSupportedException();
            }
        }


        // Constructors                                                                                                             
        public ViewResultFieldOutputSafetyFactor(ResultFieldOutputSafetyFactor resultFieldOutput, string[] partNames,
                                                 string[] elementSetNames, ref bool propertyChanged)
        {
            // The order is important
            _resultFieldOutput = resultFieldOutput;
            // Parts
            bool valid = true;
            double safetyLimit;
            _partPoints = new List<SafetyFactorPartDataPoint>();
            foreach (var partName in partNames)
            {
                safetyLimit = 0;
                valid &= _resultFieldOutput.ItemNameSafetyLimit.TryGetValue(partName, out safetyLimit);
                _partPoints.Add(new SafetyFactorPartDataPoint(partName, safetyLimit));
            }
            // Element sets
            if (elementSetNames.Length == 0) elementSetNames = new string[] { ResultFieldOutputSafetyFactor.AllElementsName };
            //
            _elementSetPoints = new List<SafetyFactorElementSetDataPoint>();
            foreach (var elementSetName in elementSetNames)
            {
                safetyLimit = 0;
                valid &= _resultFieldOutput.ItemNameSafetyLimit.TryGetValue(elementSetName, out safetyLimit);
                _elementSetPoints.Add(new SafetyFactorElementSetDataPoint(elementSetName, safetyLimit));
            }
            //
            if (!valid) propertyChanged = true;
            //
            _dctd = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override ResultFieldOutput GetBase()
        {
            _resultFieldOutput.ItemNameSafetyLimit.Clear();
            //
            if (SafetyFactorBasedOn == SafetyFactorBasedOnEnum.Parts)
            {
                foreach (var point in _partPoints)
                {
                    _resultFieldOutput.ItemNameSafetyLimit.Add(point.PartName, point.SafetyLimit);
                }
            }
            else if (SafetyFactorBasedOn == SafetyFactorBasedOnEnum.ElementSets)
            {
                foreach (var point in _elementSetPoints)
                {
                    _resultFieldOutput.ItemNameSafetyLimit.Add(point.ElementSetName, point.SafetyLimit);
                }
            }
            else throw new NotSupportedException();
            //
            return _resultFieldOutput;
        }
        public void PopulateDropDownLists(Dictionary<string, string[]> filedNameComponentNames)
        {
            _filedNameComponentNames = filedNameComponentNames;
            _dctd.PopulateProperty(nameof(FieldName), _filedNameComponentNames.Keys.ToArray());
            UpdateComponents();
        }
        private void UpdateComponents()
        {
            string[] componentNames;
            if (_filedNameComponentNames != null && _filedNameComponentNames.TryGetValue(FieldName, out componentNames) &&
                componentNames != null && componentNames.Length > 0)
            {
                _dctd.PopulateProperty(nameof(ComponentName), componentNames);
                if (!componentNames.Contains(ComponentName)) ComponentName = componentNames[0];
            }
        }
        private void UpdateVisibility()
        {
           
        }
    }



   
}
