using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Drawing;

namespace CaeModel
{
    [Serializable]
    public abstract class Constraint : NamedClass
    {
        // Variables                                                                                                                
        protected Color _color;


        // Properties                                                                                                               
        public Color Color
        {
            get
            {
                // Compatibility for version v0.6.0
                if (_color == Color.Empty) _color = Color.Yellow;
                //
                return _color; 
            }
            set { _color = value; }
        }


        // Constructors                                                                                                             
        public Constraint(string name)
            : base(name) 
        {
            _color = Color.Yellow;
        }

        // Methods                                                                                                                  

    }
}
