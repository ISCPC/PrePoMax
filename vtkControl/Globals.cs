using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;
using System.Drawing;

namespace vtkControl
{
    internal static class Globals
    {
        // Variables                                                                                                                
        public static char NameSeparator = ':';
        public static string ScalarArrayName = "scalars";
        public static string QuadraticArrayName = "quadratic";
        public static string SectionViewSuffix = "sectionView";
        public static string TransformationSuffix = "transformed";
        //
        public static string MinAnnotationName = "Min_Annotation";      // this is a copy from AnnotationContainer
        public static string MaxAnnotationName = "Max_Annotation";      // this is a copy from AnnotationContainer
        //
        public static Color CurrentMouseHighlightColor = Color.FromArgb(255, 175, 0);
        private static vtkProperty _currentMouseSelectionProperty;
        

        // Properties                                                                                                               
        public static vtkProperty CurrentMouseSelectionProperty 
        { 
            get 
            { 
                vtkProperty prop = vtkProperty.New();
                prop.DeepCopy(_currentMouseSelectionProperty);
                return prop;
            } 
        }


        // Methods                                                                                                                  
        public static void Initialize()
        {
            _currentMouseSelectionProperty = vtkProperty.New();
            _currentMouseSelectionProperty.SetColor(CurrentMouseHighlightColor.R / 255.0, CurrentMouseHighlightColor.G / 255.0, CurrentMouseHighlightColor.B / 255.0);
            _currentMouseSelectionProperty.SetAmbient(0.5);
            _currentMouseSelectionProperty.SetSpecular(0.2);
            _currentMouseSelectionProperty.SetPointSize(7);
            _currentMouseSelectionProperty.SetLineWidth(2);
            _currentMouseSelectionProperty.SetOpacity(1);
            //_currentMouseSelectionProperty.BackfaceCullingOn();
            _currentMouseSelectionProperty.BackfaceCullingOff();
        }
       
    }
}
