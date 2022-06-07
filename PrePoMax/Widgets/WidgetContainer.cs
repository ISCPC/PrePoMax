using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using CaeMesh;

namespace PrePoMax
{
    [Serializable]
    public class WidgetContainer
    {
        // Variables                                                                                                                
        private Dictionary<string, WidgetBase> _geometryWidgets;
        private Dictionary<string, WidgetBase> _modelWidgets;
        private Dictionary<string, WidgetBase> _resultsWidgets;


        // Constructors                                                                                                             
        public WidgetContainer(Controller controller)
        {
            WidgetBase.Controller = controller;
            //
            _geometryWidgets = new Dictionary<string, WidgetBase>();
            _modelWidgets = new Dictionary<string, WidgetBase>();
            _resultsWidgets = new Dictionary<string, WidgetBase>();
        }
        public WidgetContainer(WidgetContainer widgetContainer, Controller controller)
        {
            WidgetBase.Controller = controller;
            //
            if (widgetContainer == null)    // compatibility v1.3.1
            {
                _geometryWidgets = new Dictionary<string, WidgetBase>();
                _modelWidgets = new Dictionary<string, WidgetBase>();
                _resultsWidgets = new Dictionary<string, WidgetBase>();
            }
            else
            {
                _geometryWidgets = widgetContainer._geometryWidgets;
                _modelWidgets = widgetContainer._modelWidgets;
                _resultsWidgets = widgetContainer._resultsWidgets;
            }
        }


        // Methods                                                                                                                  
        public string GetFreeWidgetName()
        {
            string prefix = "";
            if (WidgetBase.Controller.CurrentView == ViewGeometryModelResults.Geometry) prefix = "G_";
            else if (WidgetBase.Controller.CurrentView == ViewGeometryModelResults.Model) prefix = "M_";
            else if (WidgetBase.Controller.CurrentView == ViewGeometryModelResults.Results) prefix = "R_";
            else throw new NotSupportedException();
            //
            return GetCurrentWidgets().GetNextNumberedKey(prefix + "Widget");
        }
        public string[] GetCurrentWidgetNames()
        {
            return GetCurrentWidgets().Keys.ToArray();
        }
        public string[] GetAllWidgetNames()
        {
            List<string> allNames = new List<string>();
            allNames.AddRange(_geometryWidgets.Keys);
            allNames.AddRange(_modelWidgets.Keys);
            allNames.AddRange(_resultsWidgets.Keys);
            return allNames.ToArray();
        }
        //
        public void AddWidget(WidgetBase widget)
        {
            GetCurrentWidgets().Add(widget.Name, widget);
        }
        //
        public WidgetBase GetCurrentWidget(string name)
        {
            return GetCurrentWidgets()[name];
        }
        public Dictionary<string, WidgetBase> GetCurrentWidgets()
        {
            return GetWidgets(WidgetBase.Controller.CurrentView);
        }
        public Dictionary<string, WidgetBase> GetWidgets(ViewGeometryModelResults view)
        {
            if (view == ViewGeometryModelResults.Geometry) return _geometryWidgets;
            else if (view == ViewGeometryModelResults.Model) return _modelWidgets;
            else if (view == ViewGeometryModelResults.Results) return _resultsWidgets;
            else throw new NotSupportedException();
        }
        //
        public string GetWidgetText(string data)
        {
            int itemId;
            string text;
            WidgetBase widget;
            //
            int.TryParse(data, out itemId);
            //
            if (WidgetBase.Controller.SelectBy == vtkSelectBy.QueryNode)
            {
                widget = new NodeWidget("tmp", itemId);
            }
            else if (WidgetBase.Controller.SelectBy == vtkSelectBy.QueryElement)
            {
                widget = new ElementWidget("tmp", itemId);
            }
            else if (WidgetBase.Controller.SelectBy == vtkSelectBy.QueryEdge)
            {
                widget = new EdgeWidget("tmp", itemId);
            }
            else if (WidgetBase.Controller.SelectBy == vtkSelectBy.QuerySurface)
            {
                widget = new SurfaceWidget("tmp", itemId);
            }
            else if (WidgetBase.Controller.SelectBy == vtkSelectBy.QueryPart)
            {
                widget = new PartWidget("tmp", data);
            }
            else throw new NotSupportedException();
            //
            widget.GetWidgetData(out text, out double[] coor);
            //
            return text;
        }
        //
        public void SuppressCurrentWidgets()
        {
            foreach (var entry in GetCurrentWidgets()) entry.Value.Valid = false;
        }
        public void ResumeCurrentWidgets()
        {
            foreach (var entry in GetCurrentWidgets()) entry.Value.Valid = true;
        }
        //
        public void RemoveCurrentArrowWidget(string widgetName)
        {
            GetCurrentWidgets().Remove(widgetName);
        }
        public void RemoveCurrentArrowWidgets(string[] widgetNames)
        {
            foreach (var widgetName in widgetNames) GetCurrentWidgets().Remove(widgetName);
        }
        public void RemoveCurrentArrowWidgetsByParts(BasePart[] parts, ViewGeometryModelResults view)
        {
            FeMesh mesh;
            if (view == ViewGeometryModelResults.Geometry) mesh = WidgetBase.Controller.Model.Geometry;
            else if (view == ViewGeometryModelResults.Model) mesh = WidgetBase.Controller.Model.Mesh;
            else if (view == ViewGeometryModelResults.Results) mesh = WidgetBase.Controller.Results.Mesh;
            else throw new NotSupportedException();
            //
            Dictionary<string, WidgetBase> widgets;
            HashSet<int> partIds = new HashSet<int>();
            if (mesh != null)
            {
                foreach (var part in parts) partIds.Add(part.PartId);
                //
                widgets = GetWidgets(view);
                List<WidgetBase> widgetsToRemove = new List<WidgetBase>();
                foreach (var entry in widgets)
                {
                    if (partIds.Contains(entry.Value.PartId)) widgetsToRemove.Add(entry.Value);
                }
                // Remove
                foreach (var widget in widgetsToRemove) widgets.Remove(widget.Name);
            }
        }
        public void RemoveCurrentArrowWidgetsByParts(string[] partNames, ViewGeometryModelResults view)
        {
            FeMesh mesh;
            if (view == ViewGeometryModelResults.Geometry) mesh = WidgetBase.Controller.Model.Geometry;
            else if (view == ViewGeometryModelResults.Model) mesh = WidgetBase.Controller.Model.Mesh;
            else if (view == ViewGeometryModelResults.Results) mesh = WidgetBase.Controller.Results.Mesh;
            else throw new NotSupportedException();
            //
            BasePart part;
            List<BasePart> parts = new List<BasePart>();
            foreach (var name in partNames)
            {
                if (mesh.Parts.TryGetValue(name, out part)) parts.Add(part);
            }
            //
            RemoveCurrentArrowWidgetsByParts(parts.ToArray(), view);
        }
        public void RemoveCurrentArrowWidgets()
        {
            GetCurrentWidgets().Clear();
        }
        public void RemoveAllArrowWidgets()
        {
            _geometryWidgets.Clear();
            _modelWidgets.Clear();
            _resultsWidgets.Clear();
        }
    }
}
