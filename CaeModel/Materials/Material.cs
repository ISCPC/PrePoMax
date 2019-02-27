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
        //public double YoungsModulus 
        //{ 
        //    get 
        //    {
        //        foreach (var property in _properties)
        //        {
        //            var elastic = property as Elastic;
        //            if (elastic != null)
        //            {
        //                return elastic.YoungsModulus;
        //            }
        //        }
        //        return 0;
        //    }
        //}
        //public double PoissonsRatio
        //{
        //    get
        //    {
        //        foreach (var property in _properties)
        //        {
        //            var elastic = property as Elastic;
        //            if (elastic != null)
        //            {
        //                return elastic.PoissonsRatio;
        //            }
        //        }
        //        return 0;
        //    }
        //}


        // Constructors                                                                                                             
        public Material()
        {
            _properties = new List<MaterialProperty>();
        }
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
    }
}
