﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using CaeMesh;
using CaeResults;

namespace CaeModel
{
    [Serializable]
    public enum ModelType
    {
        [DynamicTypeDescriptor.StandardValue("General", DisplayName = "General model")]
        GeneralModel,
        [DynamicTypeDescriptor.StandardValue("Submodel", DisplayName = "Submodel")]
        Submodel,
        [DynamicTypeDescriptor.StandardValue("SlipWear", DisplayName = "Slip wear model")]
        SlipWearModel
    }

    [Serializable]
    public enum ModelSpaceEnum
    {
        [DynamicTypeDescriptor.StandardValue("Undefined", Visible = false)]
        Undefined = 0,
        [DynamicTypeDescriptor.StandardValue("ThreeD", DisplayName = "3D")]
        ThreeD = 1,
        [DynamicTypeDescriptor.StandardValue("PlaneStress", DisplayName = "2D plane stress")]
        PlaneStress = 2,
        [DynamicTypeDescriptor.StandardValue("PlaneStrain", DisplayName = "2D plane strain")]
        PlaneStrain = 3,
        [DynamicTypeDescriptor.StandardValue("Axisymmetric", DisplayName = "2D axisymmetric")]
        Axisymmetric = 4
    }    
    public static class ExtensionMethods
    {
        // ModelSpaceEnum
        public static bool IsTwoD(this ModelSpaceEnum modelSpace)
        {
            return (int)modelSpace > 1; // 2, 3 or 4 is 2D
        }
        //
        public static Dictionary<Type, HashSet<Enum>> GetAvailableElementTypes(this ModelSpaceEnum modelSpace)
        {            
            List<Type> elementTypes = new List<Type>();
            elementTypes.Add(typeof(FeElementTypeLinearTria));
            elementTypes.Add(typeof(FeElementTypeParabolicTria));
            elementTypes.Add(typeof(FeElementTypeLinearQuad));
            elementTypes.Add(typeof(FeElementTypeParabolicQuad));
            elementTypes.Add(typeof(FeElementTypeLinearTetra));
            elementTypes.Add(typeof(FeElementTypeParabolicTetra));
            elementTypes.Add(typeof(FeElementTypeLinearWedge));
            elementTypes.Add(typeof(FeElementTypeParabolicWedge));
            elementTypes.Add(typeof(FeElementTypeLinearHexa));
            elementTypes.Add(typeof(FeElementTypeParabolicHexa));
            //
            int type = 0;
            if (modelSpace == ModelSpaceEnum.ThreeD) type = 1;
            else if (modelSpace == ModelSpaceEnum.PlaneStress) type = 2;
            else if (modelSpace == ModelSpaceEnum.PlaneStrain) type = 3;
            else if (modelSpace == ModelSpaceEnum.Axisymmetric) type = 4;
            HashSet<Enum> elementEnums;
            Dictionary<Type, HashSet<Enum>> elementTypeEnums = new Dictionary<Type, HashSet<Enum>>();
            //
            foreach (Type elementType in elementTypes)
            {
                foreach (var item in Enum.GetValues(elementType))
                {
                    if ((int)item / 10 == type)
                    {
                        if (elementTypeEnums.TryGetValue(elementType, out elementEnums)) elementEnums.Add((Enum)item);
                        else elementTypeEnums.Add(elementType, new HashSet<Enum>() { (Enum)item });
                    }
                }
            }
            //
            return elementTypeEnums;
        }
        //
        public static Dictionary<Type, HashSet<string>> GetUnavailableElementTypeNames(this ModelSpaceEnum modelSpace)
        {
            List<Type> elementTypes = new List<Type>();
            elementTypes.Add(typeof(FeElementTypeLinearTria));
            elementTypes.Add(typeof(FeElementTypeParabolicTria));
            elementTypes.Add(typeof(FeElementTypeLinearQuad));
            elementTypes.Add(typeof(FeElementTypeParabolicQuad));
            elementTypes.Add(typeof(FeElementTypeLinearTetra));
            elementTypes.Add(typeof(FeElementTypeParabolicTetra));
            elementTypes.Add(typeof(FeElementTypeLinearWedge));
            elementTypes.Add(typeof(FeElementTypeParabolicWedge));
            elementTypes.Add(typeof(FeElementTypeLinearHexa));
            elementTypes.Add(typeof(FeElementTypeParabolicHexa));
            //
            int type = 0;
            if (modelSpace == ModelSpaceEnum.ThreeD) type = 1;
            else if (modelSpace == ModelSpaceEnum.PlaneStress) type = 2;
            else if (modelSpace == ModelSpaceEnum.PlaneStrain) type = 3;
            else if (modelSpace == ModelSpaceEnum.Axisymmetric) type = 4;
            HashSet<string> elementEnums;
            Dictionary<Type, HashSet<string>> unavailableElementTypeNames = new Dictionary<Type, HashSet<string>>();
            //
            foreach (Type elementType in elementTypes)
            {
                foreach (var item in Enum.GetValues(elementType))
                {
                    if ((int)item > 0 && (int)item / 10 != type)
                    {
                        if (unavailableElementTypeNames.TryGetValue(elementType, out elementEnums))
                            elementEnums.Add(item.ToString());
                        else
                            unavailableElementTypeNames.Add(elementType, new HashSet<string>() { item.ToString() });
                    }
                }
            }
            //
            return unavailableElementTypeNames;
        }
    }

    [Serializable]
    public class ModelProperties
    {
        // Variables                                                                                                                
        public ModelSpaceEnum ModelSpace;
        public ModelType ModelType;
        // Submodel
        public string GlobalResultsFileName;
        // Slip wear model
        public SlipWearResultsEnum SlipWearResults;
        private int _numberOfCycles;
        private bool _enforceZeroBoundaryConditions;
        private bool _meshUpdate;
        //
        public double AbsoluteZero;
        public double StefanBoltzmann;
        public double NewtonGravity;


        // Properties                                                                                                               
        public int NumberOfCycles
        {
            get { return _numberOfCycles; }
            set
            {
                _numberOfCycles = value;
                if (_numberOfCycles < 1) _numberOfCycles = 1;
            }
        }
        public bool EnforceZeroBoundaryConditions
        {
            get { return _enforceZeroBoundaryConditions; }
            set { _enforceZeroBoundaryConditions = value; }
        }
        public bool MeshUpdate
        {
            get { return _meshUpdate; }
            set { _meshUpdate = value; }
        }


        // Constructors                                                                                                             
        public ModelProperties()
        {
            ModelSpace = ModelSpaceEnum.Undefined;
            ModelType = ModelType.GeneralModel;
            // Submodel
            GlobalResultsFileName = null;
            // Slip wear model
            SlipWearResults = SlipWearResultsEnum.All;
            _numberOfCycles = 1;
            _enforceZeroBoundaryConditions = true;
            _meshUpdate = false;
            //
            AbsoluteZero = double.PositiveInfinity;
            StefanBoltzmann = double.PositiveInfinity;
            NewtonGravity = double.PositiveInfinity;
        }


        // Methods                                                                                                                  

    }
}
