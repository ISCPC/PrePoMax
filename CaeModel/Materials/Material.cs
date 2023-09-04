using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class Material : NamedClass
    {
        // Variables                                                                                                                
        private string _description;
        private bool _temperatureDependent;
        private List<MaterialProperty> _properties;


        // Properties                                                                                                               
        public string Description { get { return _description; } set { _description = value; } }
        public bool TemperatureDependent { get { return _temperatureDependent; } set { _temperatureDependent = value; } }
        public List<MaterialProperty> Properties { get { return _properties; } }


        // Constructors                                                                                                             
        public Material(string name)
            : base(name)
        {
            _properties = new List<MaterialProperty>();
        }


        // Methods                                                                                                                  
        public void AddProperty(MaterialProperty property)
        {
            _properties.Add(property);
        }
        public MaterialProperty GetProperty<T>()
        {
            foreach (MaterialProperty property in _properties)
            {
                if (property is T) return property;
            }
            return null;
        }
        public void ConvertUnits(UnitSystem currentSystem, UnitSystem fromSystem, UnitSystem toSystem)
        {
            try
            {
                StringDensityConverter sdc = new StringDensityConverter();
                StringPressureConverter spc = new StringPressureConverter();
                StringTemperatureConverter stc = new StringTemperatureConverter();
                //
                StringThermalExpansionConverter stec = new StringThermalExpansionConverter();
                StringThermalConductivityConverter stcc = new StringThermalConductivityConverter();
                StringSpecificHeatConverter sshc = new StringSpecificHeatConverter();
                //
                foreach (var property in _properties)
                {
                    if (property is Density den)
                    {
                        for (int i = 0; i < den.DensityTemp.Length; i++)
                        {
                            // Check
                            if (den.DensityTemp[i][0].IsEquation()) throw new NotSupportedException();
                            if (den.DensityTemp[i][1].IsEquation()) throw new NotSupportedException();
                            // Desity
                            den.DensityTemp[i][0].SetEquationFromValue
                                (fromSystem.Convert(den.DensityTemp[i][0].Value, sdc, toSystem));
                            // Temp
                            den.DensityTemp[i][1].SetEquationFromValue
                                (fromSystem.Convert(den.DensityTemp[i][1].Value, stc, toSystem));
                        }
                    }
                    else if (property is SlipWear sw)
                    {
                        // Check
                        if (sw.Hardness.IsEquation()) throw new NotSupportedException();
                        if (sw.WearCoefficient.IsEquation()) throw new NotSupportedException();
                        // Hardness
                        sw.Hardness.SetEquationFromValue(fromSystem.Convert(sw.Hardness.Value, spc, toSystem));
                        // Wear coefficient
                    }
                    else if (property is Elastic el)
                    {
                        for (int i = 0; i < el.YoungsPoissonsTemp.Length; i++)
                        {
                            // Check
                            if (el.YoungsPoissonsTemp[i][0].IsEquation()) throw new NotSupportedException();
                            if (el.YoungsPoissonsTemp[i][1].IsEquation()) throw new NotSupportedException();
                            if (el.YoungsPoissonsTemp[i][2].IsEquation()) throw new NotSupportedException();
                            // Youngs modulus
                            el.YoungsPoissonsTemp[i][0].SetEquationFromValue
                                (fromSystem.Convert(el.YoungsPoissonsTemp[i][0].Value, spc, toSystem));
                            // Poissons ratio
                            // Temp
                            el.YoungsPoissonsTemp[i][2].SetEquationFromValue
                                (fromSystem.Convert(el.YoungsPoissonsTemp[i][2].Value, stc, toSystem));
                        }
                    }
                    else if (property is ElasticWithDensity ewd)
                    {
                        // Check
                        if (ewd.YoungsModulus.IsEquation()) throw new NotSupportedException();
                        if (ewd.PoissonsRatio.IsEquation()) throw new NotSupportedException();
                        if (ewd.Density.IsEquation()) throw new NotSupportedException();
                        // Youngs modulus
                        ewd.YoungsModulus.SetEquationFromValue(fromSystem.Convert(ewd.YoungsModulus.Value, spc, toSystem));
                        // Poissons ratio
                        // Density
                        ewd.Density.SetEquationFromValue(fromSystem.Convert(ewd.Density.Value, sdc, toSystem));
                    }
                    else if (property is Plastic pl)
                    {
                        for (int i = 0; i < pl.StressStrainTemp.Length; i++)
                        {
                            // Check
                            if (pl.StressStrainTemp[i][0].IsEquation()) throw new NotSupportedException();
                            if (pl.StressStrainTemp[i][1].IsEquation()) throw new NotSupportedException();
                            if (pl.StressStrainTemp[i][2].IsEquation()) throw new NotSupportedException();
                            // Stress
                            pl.StressStrainTemp[i][0].SetEquationFromValue
                                (fromSystem.Convert(pl.StressStrainTemp[i][0].Value, spc, toSystem));
                            // Strain
                            // Temp
                            pl.StressStrainTemp[i][2].SetEquationFromValue
                                (fromSystem.Convert(pl.StressStrainTemp[i][2].Value, stc, toSystem));
                        }
                    }
                    else if (property is ThermalExpansion te)
                    {
                        for (int i = 0; i < te.ThermalExpansionTemp.Length; i++)
                        {
                            // Check
                            if (te.ThermalExpansionTemp[i][0].IsEquation()) throw new NotSupportedException();
                            if (te.ThermalExpansionTemp[i][1].IsEquation()) throw new NotSupportedException();
                            // Expansion
                            te.ThermalExpansionTemp[i][0].SetEquationFromValue
                                (fromSystem.Convert(te.ThermalExpansionTemp[i][0].Value, stec, toSystem));
                            // Temp
                            te.ThermalExpansionTemp[i][1].SetEquationFromValue
                                (fromSystem.Convert(te.ThermalExpansionTemp[i][1].Value, stc, toSystem));
                        }
                    }
                    else if (property is ThermalConductivity tc)
                    { 
                        for (int i = 0; i < tc.ThermalConductivityTemp.Length; i++)
                        {
                            // Check
                            if (tc.ThermalConductivityTemp[i][0].IsEquation()) throw new NotSupportedException();
                            if (tc.ThermalConductivityTemp[i][1].IsEquation()) throw new NotSupportedException();
                            // Conductivity
                            tc.ThermalConductivityTemp[i][0].SetEquationFromValue
                                (fromSystem.Convert(tc.ThermalConductivityTemp[i][0].Value, stcc, toSystem));
                            // Temp
                            tc.ThermalConductivityTemp[i][1].SetEquationFromValue
                                (fromSystem.Convert(tc.ThermalConductivityTemp[i][1].Value, stc, toSystem));
                        }
                    }
                    else if (property is SpecificHeat sh)
                    {
                        for (int i = 0; i < sh.SpecificHeatTemp.Length; i++)
                        {
                            // Check
                            if (sh.SpecificHeatTemp[i][0].IsEquation()) throw new NotSupportedException();
                            if (sh.SpecificHeatTemp[i][1].IsEquation()) throw new NotSupportedException();
                            // Conductivity
                            sh.SpecificHeatTemp[i][0].SetEquationFromValue
                                (fromSystem.Convert(sh.SpecificHeatTemp[i][0].Value, sshc, toSystem));
                            // Temp
                            sh.SpecificHeatTemp[i][1].SetEquationFromValue
                                (fromSystem.Convert(sh.SpecificHeatTemp[i][1].Value, stc, toSystem));
                        }
                    }
                    else throw new NotSupportedException();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex); 
            }
            finally
            {
                // fromSystem.Convert calls changes converter units - reset it here
                currentSystem.SetConverterUnits();
            }
        }
        public bool ContainsEquation()
        {
            foreach (var property in _properties)
            {
                if (property is Density den)
                {
                    for (int i = 0; i < den.DensityTemp.Length; i++)
                    {
                        // Check
                        if (den.DensityTemp[i][0].IsEquation()) return true;
                        if (den.DensityTemp[i][1].IsEquation()) return true;
                    }
                }
                else if (property is SlipWear sw)
                {
                    // Check
                    if (sw.Hardness.IsEquation()) return true;
                    if (sw.WearCoefficient.IsEquation()) return true;
                }
                else if (property is Elastic el)
                {
                    for (int i = 0; i < el.YoungsPoissonsTemp.Length; i++)
                    {
                        // Check
                        if (el.YoungsPoissonsTemp[i][0].IsEquation()) return true;
                        if (el.YoungsPoissonsTemp[i][1].IsEquation()) return true;
                        if (el.YoungsPoissonsTemp[i][2].IsEquation()) return true;
                    }
                }
                else if (property is ElasticWithDensity ewd)
                {
                    // Check
                    if (ewd.YoungsModulus.IsEquation()) return true;
                    if (ewd.PoissonsRatio.IsEquation()) return true;
                    if (ewd.Density.IsEquation()) return true;
                }
                else if (property is Plastic pl)
                {
                    for (int i = 0; i < pl.StressStrainTemp.Length; i++)
                    {
                        // Check
                        if (pl.StressStrainTemp[i][0].IsEquation()) return true;
                        if (pl.StressStrainTemp[i][1].IsEquation()) return true;
                        if (pl.StressStrainTemp[i][2].IsEquation()) return true;
                    }
                }
                else if (property is ThermalExpansion te)
                {
                    for (int i = 0; i < te.ThermalExpansionTemp.Length; i++)
                    {
                        // Check
                        if (te.ThermalExpansionTemp[i][0].IsEquation()) return true;
                        if (te.ThermalExpansionTemp[i][1].IsEquation()) return true;
                    }
                }
                else if (property is ThermalConductivity tc)
                {
                    for (int i = 0; i < tc.ThermalConductivityTemp.Length; i++)
                    {
                        // Check
                        if (tc.ThermalConductivityTemp[i][0].IsEquation()) return true;
                        if (tc.ThermalConductivityTemp[i][1].IsEquation()) return true;
                    }
                }
                else if (property is SpecificHeat sh)
                {
                    for (int i = 0; i < sh.SpecificHeatTemp.Length; i++)
                    {
                        // Check
                        if (sh.SpecificHeatTemp[i][0].IsEquation()) return true;
                        if (sh.SpecificHeatTemp[i][1].IsEquation()) return true;
                    }
                }
                else throw new NotSupportedException();
            }
            return false;
        }
    }
}
