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
        private List<MaterialProperty> _properties;


        // Properties                                                                                                               
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
                //
                foreach (var property in _properties)
                {
                    if (property is Density den)
                    {
                        den.Value = fromSystem.Convert(den.Value, sdc, toSystem);
                    }
                    else if (property is Elastic el)
                    {
                        el.YoungsModulus = fromSystem.Convert(el.YoungsModulus, spc, toSystem);
                        //el.PoissonsRatio = currentSystem.Convert(el.PoissonsRatio, new DoubleConverter(), toSystem);
                    }
                    else if (property is Plastic pl)
                    {
                        for (int i = 0; i < pl.StressStrain.Length; i++)
                        {
                            // Stress
                            pl.StressStrain[i][0] = fromSystem.Convert(pl.StressStrain[i][0], spc, toSystem);
                            // Strain
                            //pl.StressStrain[i][1] = currentSystem.Convert(pl.StressStrain[i][1], new DoubleConverter(), toSystem);
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
                currentSystem.SetConverterUnits();
            }
        }
    }
}
