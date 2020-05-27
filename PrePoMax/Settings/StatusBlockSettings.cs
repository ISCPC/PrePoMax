﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using System.IO;
using DynamicTypeDescriptor;
using System.Runtime.InteropServices;

namespace PrePoMax
{
    [Serializable]
    public class StatusBlockSettings : ISettings
    {
        // Variables                                                                                                                
        private WidgetBackgroundType _backgroundType;
        private bool _drawBorder;


        // Properties                                                                                                               
        public WidgetBackgroundType BackgroundType
        {
            get { return _backgroundType; }
            set
            {
                if (value != _backgroundType)
                {
                    _backgroundType = value;
                    if (_backgroundType == WidgetBackgroundType.White) _drawBorder = true;
                }
            }
        }
        public bool DrawBorder { get { return _drawBorder; } set { _drawBorder = value; } }


        // Constructors                                                                                                             
        public StatusBlockSettings()
        {
            Reset();
        }


        // Methods                                                                                                                  
        public void CheckValues()
        {
        }
        public void Reset()
        {
            _backgroundType = WidgetBackgroundType.None;
            _drawBorder = true;
        }
    }
}
