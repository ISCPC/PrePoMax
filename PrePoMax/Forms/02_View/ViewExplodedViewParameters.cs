using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;
using System.Drawing.Design;

namespace PrePoMax.Forms
{
    [Serializable]
    public enum ExplodedViewStyleEnum
    {
        XYZ
    }

    [Serializable]
    public class ViewExplodedViewParameters
    {
        // Variables                                                                                                                
        private DynamicCustomTypeDescriptor _dctd = null;
        private ExplodedViewStyleEnum _explodedViewStyle;
        private ExplodedViewParameters _explodedViewParameters;


        // Properties                                                                                                               
        [Category("Data")]
        [OrderedDisplayName(0, 10, "Exploded view style")]
        [DescriptionAttribute("Select the exploded view style.")]
        [Id(1, 1)]
        public ExplodedViewStyleEnum ExplodedViewStyle { get { return _explodedViewStyle; } set { _explodedViewStyle = value; } }
        //
        [Category("Data")]
        [OrderedDisplayName(1, 10, "Magnification")]
        [DescriptionAttribute("Select the exploded view magnification (larger than 1).")]
        [Id(2, 1)]
        public double Magnification
        {
            get { return _explodedViewParameters.Magnification; }
            set
            {
                _explodedViewParameters.Magnification = value;
                if (_explodedViewParameters.Magnification < 1) _explodedViewParameters.Magnification = 1;
                else if (_explodedViewParameters.Magnification > 25) _explodedViewParameters.Magnification = 25;
            }
        }
        //
        [Category("Data")]
        [OrderedDisplayName(2, 10, "Scale factor")]
        [DescriptionAttribute("Select the exploded view scale factor [0, 1].")]
        [Id(3, 1)]
        public double ScaleFactor
        {
            get { return _explodedViewParameters.ScaleFactor; }
            set
            {
                _explodedViewParameters.ScaleFactor = value;
                if (_explodedViewParameters.ScaleFactor < 0) _explodedViewParameters.ScaleFactor = 0;
                else if (_explodedViewParameters.ScaleFactor > 1) _explodedViewParameters.ScaleFactor = 1;
            }
        }


        // Constructors                                                                                                             
        public ViewExplodedViewParameters()
        {
            _explodedViewParameters = new ExplodedViewParameters();
            Clear();
            //
            _dctd = ProviderInstaller.Install(this);
            _dctd.CategorySortOrder = CustomSortOrder.AscendingById;
            _dctd.PropertySortOrder = CustomSortOrder.AscendingById;
        }


        // Methods                                                                                                                  
        public void Clear()
        {
            _explodedViewStyle = ExplodedViewStyleEnum.XYZ;
            _explodedViewParameters.ScaleFactor = 0.5;
            _explodedViewParameters.Magnification = 3;
        }
       
    }
}
