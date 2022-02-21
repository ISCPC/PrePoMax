using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeResults
{
    class HOFieldNames
    {
        public const string Time = "TIME";
        // Nodal
        public const string Displacements = "DISPLACEMENTS";
        // Contact
        public const string RelativeContactDisplacement = "RELATIVE CONTACT DISPLACEMENT";
        public const string CenterOgGravityCG = "CENTER OF GRAVITY CG";
        public const string MeanSurfaceNormal = "MEAN SURFACE NORMAL";
        public const string SurfaceArea = "SURFACE AREA";
        //
        public const string Volume = "VOLUME";
        public const string TotalVolume = "TOTAL VOLUME";


    }




    //// Nodal                                                                                                        
    //private static readonly string nameForces = "Forces";
    //private static readonly string nameTotalForce = "Total force";
    //private static readonly string nameStresses = "Stresses";
    //private static readonly string nameStrains = "Strains";
    //private static readonly string nameMechanicalStrains = "Mechanical strains";
    //private static readonly string nameEquivalentPlasticStrains = "Equivalent plastic strain";
    //private static readonly string nameInternalEnergyDensity = "Internal energy density";
    //// Thermal
    //private static readonly string nameTemperatures = "Temperatures";
    //private static readonly string nameHeatGeneration = "Heat generation";
    //private static readonly string nameTotalHeatGeneration = "Total heat generation";
    //// Contact                                                                                                              
    //private static readonly string nameContactStress = "Contact stress";
    //private static readonly string nameContactPrintEnergy = "Contact print energy";
    //private static readonly string nameTotalNumberOfContactElements = "Total number of contact elements";
    //private static readonly string nameContactStatistics = "Statistics for slave set";
    //private static readonly string nameTotalSurfaceForce = "Total surface force";
    //private static readonly string nameMomentAboutOrigin = "Moment about origin";
    //private static readonly string nameMomentAboutCG = "Moment about CG";
    //private static readonly string nameNormalSurfaceForce = "Normal surface force";
    //private static readonly string nameShearSurfaceForce = "Shear surface force";
    //// Element                                                                                                      
    //private static readonly string nameVolume = "Volume";
    //private static readonly string nameTotalVolume = "Total volume";
    //private static readonly string nameInternalEnergy = "Internal energy";
    //private static readonly string nameTotalInternalEnergy = "Total internal energy";
    //// Thermal
    //private static readonly string nameHeatFlux = "Heat flux";
    //private static readonly string nameBodyHeating = "Body heating";
    //private static readonly string nameTotalBodyHeating = "Total body heating";
    //// Error                                                                                                        
    //private static readonly string nameError = "Error";
}
