using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;

namespace UserControls
{
    [Serializable]
    public class ViewMaterial
    {
        // Variables                                                                                                                
        CaeModel.Material _material;

        // Properties                                                                                                               

        [CategoryAttribute("Data"),
        DisplayName("\tYoung's modulus"),
        DescriptionAttribute("The value of the Young's modulus.")]
        public double YoungsModulus 
        { 
            get 
            {
                foreach (var property in _material.Properties)
                {
                    var elastic = property as CaeModel.Elastic;
                    if (elastic != null)
                    {
                        return elastic.YoungsModulus;
                    }
                }
                return 0;
            }
        }

        [CategoryAttribute("Data"),
        DisplayName("Poisson's ratio"),
        DescriptionAttribute("The value of the Poisson's ratio.")]
        public double PoissonsRatio
        {
            get
            {
                foreach (var property in _material.Properties)
                {
                    var elastic = property as CaeModel.Elastic;
                    if (elastic != null)
                    {
                        return elastic.PoissonsRatio;
                    }
                }
                return 0;
            }
        }


        // Constructors                                                                                                             
        public ViewMaterial(CaeModel.Material material)
        {
            _material = material;
        }


        // Methods                                                                                                                  
    }
}
