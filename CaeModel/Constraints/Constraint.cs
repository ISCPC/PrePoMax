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
        protected Color _masterColor;
        protected Color _slaveColor;


        // Properties                                                                                                               
        public Color MasterColor
        {
            get
            {
                // Compatibility for version v0.6.0
                if (_masterColor == Color.Empty) _masterColor = Color.Yellow;
                //
                return _masterColor;
            }
            set { _masterColor = value; }
        }
        public Color SlaveColor
        {
            get
            {
                // Compatibility for version v0.6.0
                if (_slaveColor == Color.Empty) _slaveColor = Color.Yellow;
                //
                return _slaveColor;
            }
            set { _slaveColor = value; }
        }


        // Constructors                                                                                                             
        public Constraint(string name)
            : base(name) 
        {
            _masterColor = Color.Yellow;
            _slaveColor = Color.Yellow;
        }

        // Methods                                                                                                                  

    }
}
