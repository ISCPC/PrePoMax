﻿using System;
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
        private GmshAlgorithmMesh2DEnum _algorithmMesh2D;               // ISerializable
        private GmshAlgorithmMesh3DEnum _algorithmMesh3D;               // ISerializable
        private GmshAlgorithmRecombineEnum _algorithmRecombine;         // ISerializable
        private double _recombineMinQuality;                            // ISerializable
        private bool _transfinite;                                      // ISerializable
        private double _transfiniteAngleDeg;                            // ISerializable
        private ElementSizeTypeEnum _elementSizeType;                   // ISerializable
        private int _numberOfLayers;                                    // ISerializable
        private double _elementScaleFactor;                             // ISerializable
        private int _numberOfThreads;                                   // ISerializable


        // Properties                                                                                                               
        public GmshAlgorithmMesh2DEnum AlgorithmMesh2D { get { return _algorithmMesh2D; } set { _algorithmMesh2D = value; } }
        public GmshAlgorithmMesh3DEnum AlgorithmMesh3D { get { return _algorithmMesh3D; } set { _algorithmMesh3D = value; } }
        public GmshAlgorithmRecombineEnum AlgorithmRecombine
        {
            get { return _algorithmRecombine; }
            set { _algorithmRecombine = value; }
        }
        public double RecombineMinQuality { get { return _recombineMinQuality; } set { _recombineMinQuality = value; } }
        public bool Transfinite { get { return _transfinite; } set { _transfinite = value; } }
        public double TransfiniteAngleDeg { get { return _transfiniteAngleDeg; } set { _transfiniteAngleDeg = value; } }
        public double TransfiniteAngleRad { get { return _transfiniteAngleDeg * Math.PI / 180; } }
        public ElementSizeTypeEnum ElementSizeType
        {
            get { return _elementSizeType; }
            set { _elementSizeType = value; }
        }
        public int NumberOfLayers
        {
            get { return _numberOfLayers; }
            set
            {
                _numberOfLayers = value;
                if (_numberOfLayers < 1) _numberOfLayers = 1;
            }
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
                    case "_transfinite":
                        _transfinite = (bool)entry.Value; break;
                    case "_transfiniteAngleDeg":
                        _transfiniteAngleDeg = (double)entry.Value; break;
                    case "_elementSizeType":
                        _elementSizeType = (ElementSizeTypeEnum)entry.Value; break;
                    case "_numberOfLayers":
                        _numberOfLayers = (int)entry.Value; break;
                    case "_elementScaleFactor":
                        _elementScaleFactor = (double)entry.Value; break;
                    case "_numberOfThreads":
                        _numberOfThreads = (int)entry.Value; break;
                    default:
                        break;
                }
            }
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
            _transfinite = true;
            _transfiniteAngleDeg = 135;
            _elementSizeType = ElementSizeTypeEnum.ScaleFactor;
            _numberOfLayers = 1;
            _elementScaleFactor = 1;
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
            _transfinite = gmshSetupItem._transfinite;
            _transfiniteAngleDeg = gmshSetupItem._transfiniteAngleDeg;
            _elementSizeType = gmshSetupItem._elementSizeType;
            _numberOfLayers = gmshSetupItem._numberOfLayers;
            _elementScaleFactor = gmshSetupItem._elementScaleFactor;
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
            info.AddValue("_transfinite", _transfinite, typeof(bool));
            info.AddValue("_transfiniteAngleDeg", _transfiniteAngleDeg, typeof(double));
            info.AddValue("_elementSizeType", _elementSizeType, typeof(ElementSizeTypeEnum));
            info.AddValue("_numberOfLayers", _numberOfLayers, typeof(int));
            info.AddValue("_elementScaleFactor", _elementScaleFactor, typeof(double));
            info.AddValue("_numberOfThreads", _numberOfThreads, typeof(int));
        }
    }
}