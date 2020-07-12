﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;
using System.Drawing.Design;
using CaeResults;

namespace PrePoMax.Forms
{
    [Serializable]
    public abstract class ViewTransformation
    {
        // Variables                                                                                                                
        protected DynamicCustomTypeDescriptor _dctd = null;
        protected ItemSetData _startPointItemSetData;
        protected ItemSetData _endPointItemSetData;


        // Properties                                                                                                               
        [Browsable(false)]
        public abstract string Name { get; }


        // Constructors                                                                                                             


        // Methods                                                                                                                  
        [Browsable(false)]
        public abstract Transformation Base { get; }
    }
}
