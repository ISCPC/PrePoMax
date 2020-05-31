using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using UnitsNet;
using UnitsNet.Units;

namespace CaeModel
{
    public class StringAngleDegConverter : StringAngleConverter
    {
        // Variables                                                                                                                
        protected new static AngleUnit _angleUnit = AngleUnit.Radian;                       // a new variable is needed


        // Properties                                                                                                               
        public new static string SetUnit { set { _angleUnit = Angle.ParseUnit(value); } }   // a new method is needed


        // Constructors                                                                                                             
        public StringAngleDegConverter()
        {
        }


        // Methods                                                                                                                  
        
    }
}