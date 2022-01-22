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
        private string[] _createdFromPartNames;
        private string[] _subPartNames;


        // Properties                                                                                                               
        public string[] CreatedFromPartNames { get { return _createdFromPartNames; } set { _createdFromPartNames = value; } }
        public string[] SubPartNames { get { return _subPartNames; } set { _subPartNames = value; } }


        // Constructors                                                                                                             
        public CompoundGeometryPart(string name, string[] createdFromPartNames, string[] subPartNames)
            : base(name, -111, new int[0], new int[0], new Type[0])
        {
            _createdFromPartNames = createdFromPartNames;
            _subPartNames = subPartNames;
            _partType = PartType.Compound;
        }

        public CompoundGeometryPart(CompoundGeometryPart part)
            : this(part.Name, part.CreatedFromPartNames, part.SubPartNames)
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
