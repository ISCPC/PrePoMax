using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class BoundingBox
    {
        public double MinX;
        public double MinY;
        public double MinZ;

        public double MaxX;
        public double MaxY;
        public double MaxZ;

        public BoundingBox()
        {
            Reset();
        }

        public void Reset()
        {
            MinX = double.MaxValue;
            MinY = double.MaxValue;
            MinZ = double.MaxValue;
            MaxX = -double.MaxValue;
            MaxY = -double.MaxValue;
            MaxZ = -double.MaxValue;
        }
        public void SetMin(FeNode node)
        {
            MinX = node.X;
            MinY = node.Y;
            MinZ = node.Z;
        }
        public void SetMax(FeNode node)
        {
            MaxX = node.X;
            MaxY = node.Y;
            MaxZ = node.Z;
        }
        public void CheckNode(FeNode node)
        {
            if (node.X > MaxX) MaxX = node.X;
            else if (node.X < MinX) MinX = node.X;

            if (node.Y > MaxY) MaxY = node.Y;
            else if (node.Y < MinY) MinY = node.Y;

            if (node.Z > MaxZ) MaxZ = node.Z;
            else if (node.Z < MinZ) MinZ = node.Z;
        }
        public double GetDiagonal()
        {
            return Math.Sqrt(Math.Pow(MaxX - MinX, 2) + Math.Pow(MaxY - MinY, 2) + Math.Pow(MaxZ - MinZ, 2));
        }

        public bool IsEqual(BoundingBox boundingBox)
        {
            double diagonal = Math.Pow(MinX - MaxX, 2) + Math.Pow(MinY - MaxY, 2) + Math.Pow(MinZ - MaxZ, 2);
            double bbDiagonal = Math.Pow(boundingBox.MinX - boundingBox.MaxX, 2) + Math.Pow(boundingBox.MinY - boundingBox.MaxY, 2) + Math.Pow(boundingBox.MinZ - boundingBox.MaxZ, 2);

            if (diagonal == 0 && bbDiagonal == 0) return true;
            else if (diagonal != 0 && bbDiagonal != 0) return Math.Abs(diagonal - bbDiagonal) / Math.Max(diagonal, bbDiagonal) < 0.001 ? true : false;
            return false;
        }
    }


    [Serializable]

    public class BasePart : FeGroup
    {
        // Variables                                                                                                                
        protected int _partId;
        protected PartType _partType;
        protected System.Drawing.Color _color;
        protected Type[] _elementTypes;
        protected VisualizationData _visualization;
        protected int[] _nodeLabels;
        protected bool _smoothShaded;
        protected BoundingBox _boundingBox;


        // Properties                                                                                                               
        public int PartId { get { return _partId; } set { _partId = value; } }
        public PartType PartType { get { return _partType; } }
        public System.Drawing.Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        public Type[] ElementTypes { get { return _elementTypes; } }
        public VisualizationData Visualization  { get { return _visualization; } set { _visualization = value; } }
        public int[] NodeLabels { get { return _nodeLabels; } set { _nodeLabels = value; } }
        public bool SmoothShaded { get { return _smoothShaded; } set { _smoothShaded = value; } }
        public BoundingBox BoundingBox { get { return _boundingBox; } set { _boundingBox = value; } }


        // Constructors                                                                                                             
        public BasePart(string name, int partId, int[] nodeLabels, int[] elementLabels, Type[] elementTypes)
            : base(name, elementLabels)
        {
            _partId = partId;
            _nodeLabels = nodeLabels;
            _color = System.Drawing.Color.Gray;
            _elementTypes = elementTypes;
            _visualization = new VisualizationData();
            _smoothShaded = false;
            _boundingBox = new BoundingBox();

            if (IsSolid()) _partType = PartType.Solid;
            else if (IsShell()) _partType = PartType.Shell;
            else if (IsBeam()) _partType = PartType.Wire;
            else _partType = PartType.Unknown;
        }
        public BasePart(BasePart part)
            : base(part.Name, part.Labels.ToArray())
        {
            _partId = part.PartId;

            _partType = part.PartType;

            _color = part.Color;

            _elementTypes = part.ElementTypes != null ? part.ElementTypes.ToArray() : null;

            _visualization = new VisualizationData(part.Visualization);
            
            _nodeLabels = part.NodeLabels != null ? part.NodeLabels.ToArray() : null;

            _smoothShaded = part.SmoothShaded;

            _boundingBox = part.BoundingBox.DeepClone();
        }


        // Methods                                                                                                                  
        private bool IsSolid()
        {
            if (_elementTypes == null) return false;

            foreach (var type in _elementTypes)
            {
                if (type != typeof(LinearTetraElement) && type != typeof(LinearWedgeElement) && type != typeof(LinearHexaElement) &&
                    type != typeof(ParabolicTetraElement) && type != typeof(ParabolicWedgeElement) && type != typeof(ParabolicHexaElement)) return false;
            }
            return true;
        }
        private bool IsShell()
        {
            if (_elementTypes == null) return false;

            foreach (var type in _elementTypes)
            {
                if (type != typeof(LinearTriangleElement) && type != typeof(LinearQuadrilateralElement) &&
                    type != typeof(ParabolicTriangleElement) && type != typeof(ParabolicQuadrilateralElement)) return false;
            }
            return true;
        }
        private bool IsBeam()
        {
            if (_elementTypes == null) return false;

            foreach (var type in _elementTypes)
            {
                if (type != typeof(LinearBeamElement) && type != typeof(ParabolicBeamElement)) return false;
            }
            return true;
        }

        public virtual BasePart DeepCopy()
        {
            return new BasePart(this);
        }
        public virtual PartProperties GetProperties()
        {
            PartProperties properties = new PartProperties();
            properties.Name = Name;
            properties.PartType = _partType;
            properties.Color = _color;
            properties.NumberOfNodes = _nodeLabels.Length;
            properties.NumberOfElements = Labels.Length;
            return properties;
        }
        public virtual void SetProperties(PartProperties properties)
        {
            Name = properties.Name;
            _color = properties.Color;
        }
        public virtual void RenumberElements(Dictionary<int, int> newIds)
        {
            Visualization.RenumberElements(newIds);
        }
        public virtual void RenumberNodes(Dictionary<int, int> newIds)
        {
            Visualization.RenumberNodes(newIds);
        }

        public bool IsEqual(BasePart part)
        {
            if (_nodeLabels.Length != part.NodeLabels.Length) return false;
            if (_boundingBox != null && part.BoundingBox != null && !_boundingBox.IsEqual(part.BoundingBox)) return false;
            //for (int i = 0; i < _nodeLabels.Length; i++)
            //{
            //    if (_nodeLabels[i] != part.NodeLabels[i])
            //        return false;
            //}

            return true;
        }
    }
}
