using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class GeometryPart : BasePart
    {
        // Variables                                                                                                                
        private MeshingParameters _meshingParameters;
        private int[] _errorElementIds;
        private int[] _errorNodeIds;
        private string _cadFileData;


        // Properties                                                                                                               
        public MeshingParameters MeshingParameters { get { return _meshingParameters; } set { _meshingParameters = value; } }
        public int[] ErrorElementIds { get { return _errorElementIds; } set { _errorElementIds = value; } }
        public int[] ErrorNodeIds { get { return _errorNodeIds; } set { _errorNodeIds = value; } }
        public string CADFileData
        {
            get
            {
                if (_cadFileData != null) return StringCompressor.DecompressString(_cadFileData);
                else return _cadFileData;
            }
            set
            {
                if (value == null) _cadFileData = value;
                else _cadFileData = CaeGlobals.StringCompressor.CompressString(value);
            }
        }


        // Constructors                                                                                                             
        public GeometryPart(string name, int partId,  int[] nodeLabels, int[] elementLabels, Type[] elementTypes)
            : base(name, partId, nodeLabels, elementLabels, elementTypes)
        {
            _meshingParameters = null;
            _errorElementIds = null;
            _errorNodeIds = null;
            _cadFileData = null;
        }
        public GeometryPart(BasePart part)
            : base(part)
        {
            _meshingParameters = null;
            _errorElementIds = null;
            _errorNodeIds = null;
            _cadFileData = null;
        }
        public GeometryPart(GeometryPart part)
            : base((BasePart)part)
        {
            _meshingParameters = part.MeshingParameters != null ? part.MeshingParameters.DeepClone() : null;
            _errorElementIds = part.ErrorElementIds != null ? part.ErrorElementIds.ToArray() : null;
            _errorNodeIds = part.ErrorNodeIds != null ? part.ErrorNodeIds.ToArray() : null;
            _cadFileData = part._cadFileData != null ? _cadFileData : null;
        }


        // Methods                                                                                                                  
        public override BasePart DeepCopy()
        {
            return new GeometryPart(this);
        }
        public override PartProperties GetProperties()
        {
            PartProperties properties = base.GetProperties();
            //if (_meshingParameters != null) properties.MeshingParameters = _meshingParameters.DeepClone();
            //else properties.MeshingParameters = null;
            return properties;
        }
        public override void SetProperties(PartProperties properties)
        {
            base.SetProperties(properties);

            //if (properties.MeshingParameters != null) _meshingParameters = properties.MeshingParameters.DeepClone();
            //else _meshingParameters = null;
        }
        public override void RenumberVisualizationElements(Dictionary<int, int> newIds)
        {
            base.RenumberVisualizationElements(newIds);

            if (_errorElementIds != null)
            {
                for (int i = 0; i < _errorElementIds.Length; i++)
                {
                    _errorElementIds[i] = newIds[_errorElementIds[i]];
                }
            }
        }
        public override void RenumberVisualizationNodes(Dictionary<int, int> newIds)
        {
            base.RenumberVisualizationNodes(newIds);

            if (_errorNodeIds != null)
            {
                for (int i = 0; i < _errorNodeIds.Length; i++)
                {
                    _errorNodeIds[i] = newIds[_errorNodeIds[i]];
                }
            }
        }
    }
}
