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
        public void ConvertUnits(UnitSystem currentSystem, UnitSystem fromSystem, UnitSystem toSystem)
        {
            try
            {
                StringDensityConverter sdc = new StringDensityConverter();
                StringPressureConverter spc = new StringPressureConverter();
                StringTemperatureConverter stc = new StringTemperatureConverter();
                //
                foreach (var property in _properties)
                {
                    if (property is Density den)
                    {
                        for (int i = 0; i < den.DensityTemp.Length; i++)
                        {
                            // Desity
                            den.DensityTemp[i][0] = fromSystem.Convert(den.DensityTemp[i][0], sdc, toSystem);
                            // Temp
                            den.DensityTemp[i][1] = fromSystem.Convert(den.DensityTemp[i][1], stc, toSystem);
                        }
                    }
                    else if (property is Elastic el)
                    {
                        for (int i = 0; i < el.YoungsPoissonsTemp.Length; i++)
                        {
                            // Youngs modulus
                            el.YoungsPoissonsTemp[i][0] = fromSystem.Convert(el.YoungsPoissonsTemp[i][0], spc, toSystem);
                            // Temp
                            el.YoungsPoissonsTemp[i][2] = fromSystem.Convert(el.YoungsPoissonsTemp[i][2], stc, toSystem);
                        }
                    }
                    else if (property is ElasticWithDensity ewd)
                    {
                        ewd.YoungsModulus = fromSystem.Convert(ewd.YoungsModulus, spc, toSystem);
                        ewd.Density = fromSystem.Convert(ewd.Density, sdc, toSystem);
                    }
                    else if (property is Plastic pl)
                    {
                        for (int i = 0; i < pl.StressStrainTemp.Length; i++)
                        {
                            // Stress
                            pl.StressStrainTemp[i][0] = fromSystem.Convert(pl.StressStrainTemp[i][0], spc, toSystem);
                            // Temp
                            pl.StressStrainTemp[i][2] = fromSystem.Convert(pl.StressStrainTemp[i][2], stc, toSystem);
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
    }
}
