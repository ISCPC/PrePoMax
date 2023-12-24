using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using CaeMesh.Meshing;
using DynamicTypeDescriptor;
using GmshCommon;

namespace CaeMesh
{
    [Serializable]
    public class GmshSetupItem : MeshSetupItem, ISerializable
    {
        // Variables                                                                                                                
        private GmshAlgorithmMesh2DEnum _algorithmMesh2D;                   // ISerializable
        private GmshAlgorithmMesh3DEnum _algorithmMesh3D;                   // ISerializable
        private GmshAlgorithmRecombineEnum _algorithmRecombine;             // ISerializable
        private double _recombineMinQuality;                                // ISerializable
        private bool _transfiniteThreeSided;                                // ISerializable
        private bool _transfiniteFourSided;                                 // ISerializable
        private double _transfiniteAngleDeg;                                // ISerializable
        private GmshOptimizeFirstOrderSolidEnum _optimizeFirstOrderSolid;   // ISerializable
        private GmshOptimizeHighOrderEnum _optimizeHighOrder;               // ISerializable
        private ElementSizeTypeEnum _elementSizeType;                       // ISerializable
        private double _elementScaleFactor;                                 // ISerializable
        private int _numberOfElements;                                      // ISerializable
        private double[] _normalizedLayerSizes;                             // ISerializable
        private int[] _numOfElementsPerLayer;                               // ISerializable
        private int _numberOfThreads;                                       // ISerializable


        // Properties                                                                                                               
        public GmshAlgorithmMesh2DEnum AlgorithmMesh2D { get { return _algorithmMesh2D; } set { _algorithmMesh2D = value; } }
        public GmshAlgorithmMesh3DEnum AlgorithmMesh3D { get { return _algorithmMesh3D; } set { _algorithmMesh3D = value; } }
        public GmshAlgorithmRecombineEnum AlgorithmRecombine
        {
            get { return _algorithmRecombine; }
            set { _algorithmRecombine = value; }
        }
        public double RecombineMinQuality { get { return _recombineMinQuality; } set { _recombineMinQuality = value; } }
        public bool TransfiniteThreeSided { get { return _transfiniteThreeSided; } set { _transfiniteThreeSided = value; } }
        public bool TransfiniteFourSided { get { return _transfiniteFourSided; } set { _transfiniteFourSided = value; } }
        public double TransfiniteAngleDeg { get { return _transfiniteAngleDeg; } set { _transfiniteAngleDeg = value; } }
        public double TransfiniteAngleRad { get { return _transfiniteAngleDeg * Math.PI / 180; } }
        public GmshOptimizeFirstOrderSolidEnum OptimizeFirstOrderSolid
        {
            get { return _optimizeFirstOrderSolid; }
            set { _optimizeFirstOrderSolid = value; }
        }
        public GmshOptimizeHighOrderEnum OptimizeHighOrder
        {
            get { return _optimizeHighOrder; }
            set { _optimizeHighOrder = value; }
        }
        public ElementSizeTypeEnum ElementSizeType
        {
            get { return _elementSizeType; }
            set { _elementSizeType = value; }
        }
        public double ElementScaleFactor
        {
            get { return _elementScaleFactor; }
            set
            {
                _elementScaleFactor = value;
                if (_elementScaleFactor < 0) _elementScaleFactor = 1;
            }
        }
        public int NumberOfElements
        {
            get { return _numberOfElements; }
            set
            {
                _numberOfElements = value;
                if (_numberOfElements < 1) _numberOfElements = 1;
            }
        }
        public double[] NormalizedLayerSizes
        {
            get { return _normalizedLayerSizes; }
            set { _normalizedLayerSizes = value; }
        }
        public int[] NumOfElementsPerLayer
        {
            get { return _numOfElementsPerLayer; }
            set { _numOfElementsPerLayer = value; }
        }
        public int NumberOfThreads
        {
            get { return _numberOfThreads; }
            set
            {
                _numberOfThreads = value;
                if (_numberOfThreads < 1) _numberOfThreads = 1;
            }
        }


        // Constructors                                                                                                             
        public GmshSetupItem(string name)
            : base(name)
        {
            Reset();
        }
        public GmshSetupItem(ExtrudeMesh extrudeMesh)
            : base("tmpName")
        {
            CopyFrom(extrudeMesh);
        }
        public GmshSetupItem(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_algorithmMesh2D":
                        _algorithmMesh2D = (GmshAlgorithmMesh2DEnum)entry.Value; break;
                    case "_algorithmMesh3D":
                        _algorithmMesh3D = (GmshAlgorithmMesh3DEnum)entry.Value; break;
                    case "_algorithmRecombine":
                        _algorithmRecombine = (GmshAlgorithmRecombineEnum)entry.Value; break;
                    case "_recombineMinQuality":
                        _recombineMinQuality = (double)entry.Value; break;
                    case "_transfiniteThreeSided":
                        _transfiniteThreeSided = (bool)entry.Value; break;
                    case "_transfinite":
                    case "_transfiniteFourSided":
                        _transfiniteFourSided = (bool)entry.Value; break;
                    case "_transfiniteAngleDeg":
                        _transfiniteAngleDeg = (double)entry.Value; break;
                    case "_optimizeFirstOrderSolid":
                        _optimizeFirstOrderSolid = (GmshOptimizeFirstOrderSolidEnum)entry.Value; break;
                    case "_optimizeHighOrder":
                        _optimizeHighOrder = (GmshOptimizeHighOrderEnum)entry.Value; break;
                    case "_elementSizeType":
                        _elementSizeType = (ElementSizeTypeEnum)entry.Value; break;
                    case "_elementScaleFactor":
                        _elementScaleFactor = (double)entry.Value; break;
                    case "_numberOfLayers":     // Compatibility for version v1.5.7
                    case "_numberOfElements":
                        _numberOfElements = (int)entry.Value; break;
                    case "_normalizedLayerSizes":
                        _normalizedLayerSizes = (double[])entry.Value; break;
                    case "_numOfElementsPerLayer":
                        _numOfElementsPerLayer = (int[])entry.Value; break;
                    case "_numberOfThreads":
                        _numberOfThreads = (int)entry.Value; break;
                    default:
                        break;
                }
            }
            // Compatibility for version v1.5.7
            if (_normalizedLayerSizes == null) _normalizedLayerSizes = new double[] { 1 };
            if (_numOfElementsPerLayer == null) _numOfElementsPerLayer = new int[] { 1 };
        }


        // Methods                                                                                                                  
        public override void Reset()
        {
            base.Reset();
            //
            _algorithmMesh2D = GmshAlgorithmMesh2DEnum.FrontalDelaunay;
            _algorithmMesh3D = GmshAlgorithmMesh3DEnum.Delaunay;
            _algorithmRecombine = GmshAlgorithmRecombineEnum.None;
            _recombineMinQuality = 0.01;
            _transfiniteThreeSided = true;
            _transfiniteFourSided = true;
            _transfiniteAngleDeg = 135;
            _optimizeFirstOrderSolid = GmshOptimizeFirstOrderSolidEnum.None;
            _optimizeHighOrder = GmshOptimizeHighOrderEnum.None;
            _elementSizeType = ElementSizeTypeEnum.ScaleFactor;
            _elementScaleFactor = 1;
            _numberOfElements = 1;
            _normalizedLayerSizes = new double[] { 1 };
            _numOfElementsPerLayer = new int[] { 1 };
            _numberOfThreads = 1;
        }
        public void CopyFrom(GmshSetupItem gmshSetupItem)
        {
            base.CopyFrom(gmshSetupItem);
            //
            _algorithmMesh2D = gmshSetupItem._algorithmMesh2D;
            _algorithmMesh3D = gmshSetupItem._algorithmMesh3D;
            _algorithmRecombine = gmshSetupItem._algorithmRecombine;
            _recombineMinQuality = gmshSetupItem._recombineMinQuality;
            _transfiniteThreeSided = gmshSetupItem._transfiniteThreeSided;
            _transfiniteFourSided = gmshSetupItem._transfiniteFourSided;
            _transfiniteAngleDeg = gmshSetupItem._transfiniteAngleDeg;
            _optimizeFirstOrderSolid = gmshSetupItem.OptimizeFirstOrderSolid;
            _optimizeHighOrder = gmshSetupItem.OptimizeHighOrder;
            _elementSizeType = gmshSetupItem._elementSizeType;
            _elementScaleFactor = gmshSetupItem._elementScaleFactor;
            _numberOfElements = gmshSetupItem._numberOfElements;
            if (gmshSetupItem._normalizedLayerSizes != null) _normalizedLayerSizes = gmshSetupItem._normalizedLayerSizes.ToArray();
            else _normalizedLayerSizes = null;
            if (gmshSetupItem._numOfElementsPerLayer != null) _numOfElementsPerLayer = gmshSetupItem._numOfElementsPerLayer.ToArray();
            else _numOfElementsPerLayer = null;
            _numberOfThreads = gmshSetupItem._numberOfThreads;
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // Using typeof() works also for null fields
            info.AddValue("_algorithmMesh2D", _algorithmMesh2D, typeof(GmshAlgorithmMesh2DEnum));
            info.AddValue("_algorithmMesh3D", _algorithmMesh3D, typeof(GmshAlgorithmMesh3DEnum));
            info.AddValue("_algorithmRecombine", _algorithmRecombine, typeof(GmshAlgorithmRecombineEnum));
            info.AddValue("_recombineMinQuality", _recombineMinQuality, typeof(double));
            info.AddValue("_transfiniteThreeSided", _transfiniteThreeSided, typeof(bool));
            info.AddValue("_transfiniteFourSided", _transfiniteFourSided, typeof(bool));
            info.AddValue("_transfiniteAngleDeg", _transfiniteAngleDeg, typeof(double));
            info.AddValue("_optimizeFirstOrderSolid", _optimizeFirstOrderSolid, typeof(GmshOptimizeFirstOrderSolidEnum));
            info.AddValue("_optimizeHighOrder", _optimizeHighOrder, typeof(GmshOptimizeHighOrderEnum));
            info.AddValue("_elementSizeType", _elementSizeType, typeof(ElementSizeTypeEnum));
            info.AddValue("_elementScaleFactor", _elementScaleFactor, typeof(double));
            info.AddValue("_numberOfElements", _numberOfElements, typeof(int));
            info.AddValue("_normalizedLayerSizes", _normalizedLayerSizes, typeof(double[]));
            info.AddValue("_numOfElementsPerLayer", _numOfElementsPerLayer, typeof(int[]));
            info.AddValue("_numberOfThreads", _numberOfThreads, typeof(int));
        }
    }
}
