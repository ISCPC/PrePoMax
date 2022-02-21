﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeResults
{
    public class FOFieldNames
    {
        public const string None = "NONE";
        //
        public const string Disp = "DISP";
        //
        public const string Stress = "STRESS";
        public const string ZZStr = "ZZSTR";
        //
        public const string ToStrain = "TOSTRAIN";
        public const string MeStrain = "MESTRAIN";
        public const string Pe = "PE";
        //
        public const string Forc = "FORC";
        public const string Ener = "ENER";
        public const string Error = "ERROR";
        public const string Contact = "CONTACT";
        //
        public const string SlidingDistance = "SLIDING_DISTANCE";
        public const string SurfaceNormal = "SURFACE_NORMAL";
        public const string Depth = "DEPTH";
        //
        public const string NdTemp = "NDTEMP";
        public const string Flux = "FLUX";
        public const string Rfl = "RFL";
        public const string HError = "HERROR";




        public const string ContactWear = "CONTACT_WEAR";
    }

    public class FOComponentNames
    {
        public const string COpen = "COPEN";
        public const string CSlip1 = "CSLIP1";
        public const string CSlip2 = "CSLIP2";
        //
        public const string CPress = "CPRESS";
        public const string CShear1 = "CSHEAR1";
        public const string CShear2 = "CSHEAR2";
    }
}
