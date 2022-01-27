using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Kitware.VTK;
using CaeGlobals;

namespace vtkControl
{
    public enum vtkMaxActorRepresentation
    {
        Solid,
        SolidAsShell,
        Shell,
        Wire,
        Unknown
    }
    public class vtkMaxActorData
    {
        // Variables                                                                                                                
        public string Name;
        public float NodeSize;
        public float LineWidth;
        public Color Color;
        public Color BackfaceColor;
        public Color[] ColorTable;
        public double Ambient;
        public double Diffuse;
        public vtkRendererLayer Layer;
        public bool CanHaveElementEdges;
        public bool Pickable;
        public bool BackfaceCulling;
        public bool ColorContours;
        public bool SmoothShaded;
        public bool DrawOnGeometry;
        public bool UseSecondaryHighightColor;
        public vtkMaxActorRepresentation ActorRepresentation;
        public bool SectionViewPossible;
        //
        public PartExchangeData Geometry;
        public PartExchangeData ModelEdges;
        public PartExchangeData CellLocator;
        

        // Constructors                                                                                                             
        public vtkMaxActorData()
        {
            Name = null;
            NodeSize = 1;
            LineWidth = 1;
            Color = Color.Empty;
            BackfaceColor = Color.Empty;
            ColorTable = null;
            Ambient = 0.1;
            Diffuse = 0.1;
            Layer = vtkRendererLayer.Base;
            CanHaveElementEdges = false;
            Pickable = false;
            BackfaceCulling = true;
            ColorContours = false;
            SmoothShaded = false;
            DrawOnGeometry = false;
            UseSecondaryHighightColor = false;
            ActorRepresentation = vtkMaxActorRepresentation.Unknown;
            SectionViewPossible = true;
            //
            Geometry = new PartExchangeData();
            ModelEdges = null;
            CellLocator = null;
        }

        // Methods                                                                                                                  
        public double[][][] GetStlTriangles()
        {
            vtkMaxActor actor = new vtkMaxActor(this);
            return actor.GetStlTriangles();
        }
    }
}
