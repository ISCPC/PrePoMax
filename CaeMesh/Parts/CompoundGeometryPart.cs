using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class CompoundGeometryPart : GeometryPart
    {
        // Variables                                                                                                                
        private string[] _subPartNames;


        // Properties                                                                                                               
        public string[] SubPartNames { get { return _subPartNames; } set { _subPartNames = value; } }


        // Constructors                                                                                                             
        public CompoundGeometryPart(string name, string[] subParts)
            : base(name, -1, new int[0], new int[0], new Type[0])
        {
            _subPartNames = subParts;
            _partType = PartType.Compound;
        }

        public CompoundGeometryPart(CompoundGeometryPart part)
            : this(part.Name, part.SubPartNames)
        {
        }


        // Methods                                                                                                                  
        public override BasePart DeepCopy()
        {
            return new GeometryPart(this);
        }
        public override PartProperties GetProperties()
        {
            PartProperties properties = base.GetProperties();
            return properties;
        }
        public override void SetProperties(PartProperties properties)
        {
            base.SetProperties(properties);
        }
        
        
    }
}
