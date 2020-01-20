using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;
using CaeGlobals;

namespace vtkControl
{
    public class vtkMaxActor
    {
        // Variables                                                                                                                
        private string _name;
        private vtkMaxExtreemeNode _minNode;
        private vtkMaxExtreemeNode _maxNode;
        private vtkActor _geometry;
        private vtkPointData _geometryPointData;
        private vtkActor _elementEdges;
        private vtkActor _modelEdges;
        private vtkCellLocator _cellLocator;          // for surface picking
        private vtkCellLocator _frustumCellLocator;   // for volume picking
        private vtkPointData _frustumPointData;
        public vtkMaxActorRepresentation _actorRepresentation;
        private bool _visible;
        private bool _backfaceCulling;
        private System.Drawing.Color _color;
        private double _ambient;
        private double _diffuse;
        private bool _colorContours;
        private bool _sectionViewPossible;
        private bool _drawOnGeometry;
        

        // Properties                                                                                                               
        public string Name { get { return _name; } set { _name = value; } }
        public vtkMaxExtreemeNode MinNode { get { return _minNode; } set { _minNode = value; } }
        public vtkMaxExtreemeNode MaxNode { get { return _maxNode; } set { _maxNode = value; } }
        public vtkActor Geometry { get { return _geometry; } set { _geometry = value; } }
        public vtkPointData GeometryPointData { get { return _geometryPointData; } }
        public vtkActor ElementEdges { get { return _elementEdges; } set { _elementEdges = value; } }
        public vtkActor ModelEdges { get { return _modelEdges; } set { _modelEdges = value; } }
        public vtkCellLocator CellLocator { get { return _cellLocator; } set { _cellLocator = value; } }
        public vtkCellLocator FrustumCellLocator { get { return _frustumCellLocator; } set { _frustumCellLocator = value; } }
        public vtkPointData FrustumPointData { get { return _frustumPointData; } }
        public vtkMaxActorRepresentation ActorRepresentation { get { return _actorRepresentation; } set { _actorRepresentation = value; } }
        public bool VtkMaxActorVisible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                _geometry.SetVisibility(_visible ? 1 : 0);    // to base vtkActor
            }
        }
        public bool BackfaceCulling { get { return _backfaceCulling; } set { _backfaceCulling = value; } }
        public System.Drawing.Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                UpdateColor();
            }
        }
        public double Ambient
        {
            get { return _ambient; }
            set
            {
                _ambient = value;
                UpdateColor();
            }
        }
        public double Diffuse
        {
            get { return _diffuse; }
            set
            {
                _diffuse = value;
                UpdateColor();
            }
        }
        public bool ColorContours { get { return _colorContours; } set { _colorContours = value; } }
        public bool SectionViewPossible { get { return _sectionViewPossible; } set { _sectionViewPossible = value; } }
        public bool DrawOnGeometry { get { return _drawOnGeometry; } set { _drawOnGeometry = value; } }


        // Constructors                                                                                                             
        public vtkMaxActor()
        {
            _name = null;
            _minNode = null;
            _maxNode = null;
            _geometry = new vtkActor();
            _elementEdges = null;
            _modelEdges = null;
            _cellLocator = null;
            _frustumCellLocator = null;
            _actorRepresentation = vtkMaxActorRepresentation.Unknown;
            _visible = true;
            _backfaceCulling = true;
            _color = System.Drawing.Color.Yellow;
            _ambient = 0.5;
            _diffuse = 0.5;
            _colorContours = false;
            _sectionViewPossible = true;
            _drawOnGeometry = false;
            //
            UpdateColor();
        }
        public vtkMaxActor(vtkUnstructuredGrid source)
            : this ()
        {
            // Unstructured grid
            vtkUnstructuredGrid uGridActor = vtkUnstructuredGrid.New();
            vtkUnstructuredGrid uGridEdges = vtkUnstructuredGrid.New();
            // Add the points
            uGridActor.SetPoints(source.GetPoints());
            uGridEdges.SetPoints(source.GetPoints());
            // Add the cells
            AddCellDataToGrid(source, uGridActor, uGridEdges);
            // Update grid
            uGridActor.Update();
            uGridEdges.Update();
            // Actor
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInput(uGridActor);
            _geometry.SetMapper(mapper);
            _geometry.PickableOff();
            // Edges
            _elementEdges = GetActorEdgesFromGridByVisualizationSurfaceExtraction(uGridEdges);
        }
        public vtkMaxActor(vtkMaxActorData data)
            : this(data, false, false)
        {
        }
        public vtkMaxActor(vtkMaxActorData data, vtkMapper mapper)
            : this()
        {
            this._name = data.Name;
            _geometry.SetMapper(mapper);
            _geometry.SetPickable(data.Pickable ? 1 : 0);
            this._actorRepresentation = data.ActorRepresentation;
            this._backfaceCulling = data.BackfaceCulling;
            this._color = data.Color;
            this._ambient = data.Ambient;
            this._diffuse = data.Diffuse;
            this._colorContours = data.ColorContours;
            this._sectionViewPossible = data.SectionViewPossible;
            this._drawOnGeometry = data.DrawOnGeometry;
            //
            UpdateColor();
        }
        public vtkMaxActor(vtkMaxActorData data, bool extractVisualizationSurface, bool createNodalActor)
            : this()
        {
            this._name = data.Name;            
            //
            if (createNodalActor)
                CreateNodalActor(data);
            else
            {
                if (extractVisualizationSurface) CreateFromDataByVisualizationSurfaceExtraction(data);
                else CreateFromData(data);
                //
                if (data.Pickable && _geometry.GetMapper().GetInputAsDataSet().GetCellData().GetGlobalIds() != null)
                {
                    _geometry.SetPickable(1);
                    _cellLocator = vtkCellLocator.New();
                    _cellLocator.SetDataSet(_geometry.GetMapper().GetInputAsDataSet());
                    _cellLocator.LazyEvaluationOn();
                }
                else
                {
                    _geometry.SetPickable(0);
                }
            }
            //
            this._actorRepresentation = data.ActorRepresentation;
            this._backfaceCulling = data.BackfaceCulling;
            this._color = data.Color;
            this._ambient = data.Ambient;
            this._diffuse = data.Diffuse;
            this._colorContours = data.ColorContours;
            this._sectionViewPossible = data.SectionViewPossible;
            this._drawOnGeometry = data.DrawOnGeometry;
            //
            UpdateColor();
        }
        public vtkMaxActor(vtkMaxActor sourceActor)
          : this()
        {
            this._name = sourceActor.Name;           
            //
            if (sourceActor.MinNode != null) _minNode = new vtkMaxExtreemeNode(sourceActor.MinNode);
            if (sourceActor.MaxNode != null) _maxNode = new vtkMaxExtreemeNode(sourceActor.MaxNode);
            //
            // Geometry                                                         
            // Polydata
            vtkPolyData polydata = vtkPolyData.New();
            polydata.DeepCopy(sourceActor.Geometry.GetMapper().GetInput());
            // Mapper
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInput(polydata);
            // Actor
            _geometry.SetMapper(mapper);
            _geometry.GetProperty().DeepCopy(sourceActor.Geometry.GetProperty());
            _geometry.SetVisibility(sourceActor.Geometry.GetVisibility());
            _geometry.SetPickable(sourceActor.Geometry.GetPickable());
            //
            // Cell Locator
            if (sourceActor.CellLocator != null)
            {
                _cellLocator = vtkCellLocator.New();
                _cellLocator.SetDataSet(_geometry.GetMapper().GetInputAsDataSet());
                _cellLocator.LazyEvaluationOn();
            }
            // Frustum Locator
            if (sourceActor.FrustumCellLocator != null)
            {
                vtkUnstructuredGrid grid = vtkUnstructuredGrid.New();
                grid.DeepCopy(sourceActor.FrustumCellLocator.GetDataSet());
                //
                _frustumCellLocator = vtkCellLocator.New();
                _frustumCellLocator.SetDataSet(grid);
                _frustumCellLocator.LazyEvaluationOn();
            }
            //
            // Element edges                                                    
            if (sourceActor.ElementEdges != null)
            {
                // Polydata
                polydata = vtkPolyData.New();
                polydata.DeepCopy(sourceActor.ElementEdges.GetMapper().GetInput());
                // Mapper
                mapper = vtkDataSetMapper.New();
                mapper.SetInput(polydata);
                // Actor
                _elementEdges = new vtkActor();
                _elementEdges.SetMapper(mapper);
                _elementEdges.GetProperty().DeepCopy(sourceActor.ElementEdges.GetProperty());
                _elementEdges.SetVisibility(sourceActor.ElementEdges.GetVisibility());
                _elementEdges.SetPickable(sourceActor.ElementEdges.GetPickable());
            }
            //
            // Model edges                                                      
            if (sourceActor.ModelEdges != null)
            {
                // Polydata
                polydata = vtkPolyData.New();
                polydata.DeepCopy(sourceActor.ModelEdges.GetMapper().GetInput());
                // Mapper
                mapper = vtkDataSetMapper.New();
                mapper.SetInput(polydata);
                // Actor
                _modelEdges = vtkActor.New();
                _modelEdges.SetMapper(mapper);
                _modelEdges.GetProperty().DeepCopy(sourceActor.ModelEdges.GetProperty());
                _modelEdges.SetVisibility(sourceActor.ModelEdges.GetVisibility());
                _modelEdges.SetPickable(sourceActor.ModelEdges.GetPickable());
            }
            //
            this._actorRepresentation = sourceActor.ActorRepresentation;
            this.VtkMaxActorVisible = sourceActor.VtkMaxActorVisible;
            this._backfaceCulling = sourceActor.BackfaceCulling;
            this._color = sourceActor.Color;
            this._ambient = sourceActor.Ambient;
            this._diffuse = sourceActor.Diffuse;
            this._colorContours = sourceActor.ColorContours;
            this._sectionViewPossible = sourceActor.SectionViewPossible;
            this._drawOnGeometry = sourceActor.DrawOnGeometry;
            //
            UpdateColor();
        }

        // Methods                                                                                                                  
        private void AddCellDataToGrid(vtkUnstructuredGrid source, vtkUnstructuredGrid uGridActor, vtkUnstructuredGrid uGridEdges)
        {
            List<int> actorCellIds = new List<int>();

            int cellType;
            bool isEdge;
            vtkIdList list = vtkIdList.New();

            uGridActor.ShallowCopy(source);

            vtkUnsignedCharArray types = source.GetCellTypesArray();

            for (int i = 0; i < types.GetNumberOfTuples(); i++)
            {
                cellType = (int)types.GetTuple1(i);
                isEdge = cellType == (int)vtkCellType.VTK_LINE || cellType == (int)vtkCellType.VTK_QUADRATIC_EDGE;

                if (!isEdge)
                {
                    source.GetCellPoints(i, list);
                    uGridEdges.InsertNextCell(cellType, list);
                }
            }
        }
        private vtkActor GetActorEdgesFromGridByVisualizationSurfaceExtraction(vtkUnstructuredGrid uGridEdges)
        {
            if (uGridEdges.GetNumberOfCells() <= 0) return null;

            // extract visualization surface of the unstructured grid
            vtkUnstructuredGridGeometryFilter filter = vtkUnstructuredGridGeometryFilter.New();
            filter.SetInput(uGridEdges);
            filter.Update();

            // extract edges of the visualization surface
            vtkExtractEdges extractEdges = vtkExtractEdges.New();
            extractEdges.SetInput(uGridEdges);
            extractEdges.Update();

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(extractEdges.GetOutput());

            vtkActor edges = vtkActor.New();
            edges.SetMapper(mapper);
            edges.PickableOff();

            return edges;
        }
        //                          
        public void CreateNodalActor(vtkMaxActorData data)
        {
            // Create the geometry of a point (the coordinate)
            vtkPoints points = vtkPoints.New();

            // Create the topology of the point (a vertex)
            vtkCellArray vertices = vtkCellArray.New();
            for (int i = 0; i < data.Geometry.Nodes.Coor.GetLength(0); i++)
            {
                points.InsertNextPoint(data.Geometry.Nodes.Coor[i][0], data.Geometry.Nodes.Coor[i][1], data.Geometry.Nodes.Coor[i][2]);
                vertices.InsertNextCell(1);
                vertices.InsertCellPoint(i);
            }

            // Create a polythis object
            vtkPolyData pointsPolythis = vtkPolyData.New();

            // Set the points and vertices created as the geometry and topology of the polythis
            pointsPolythis.SetPoints(points);
            pointsPolythis.SetVerts(vertices);

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(pointsPolythis);


            _geometry.SetMapper(mapper);

            _geometry.GetProperty().SetColor(_color.R / 255d, _color.G / 255d, _color.B / 255d);
            _geometry.GetProperty().SetPointSize(data.NodeSize);
        }
        private void CreateFromDataByVisualizationSurfaceExtraction(vtkMaxActorData data)
        {
            // Unstructured grid
            vtkUnstructuredGrid uGridActor;
            vtkUnstructuredGrid uGridEdges;

            bool addBackFace = data.Layer == vtkRendererLayer.Selection;
            addBackFace = false;

            AddNodeAndCellDataToGrid(data.Geometry, out uGridActor, out uGridEdges, false, addBackFace);

            // extract visualization surface of the unstructured grid - filter only keeps PedigreeIds and not GlobalIds
            vtkUnstructuredGridGeometryFilter filter = vtkUnstructuredGridGeometryFilter.New();
            filter.SetInput(uGridActor);
            filter.PassThroughPointIdsOn();
            filter.PassThroughCellIdsOn();
            filter.Update();

            // Actor
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInput(filter.GetOutput());

            _geometry.SetMapper(mapper);
            _geometry.GetProperty().SetColor(_color.R / 255d, _color.G / 255d, _color.B / 255d);
            _geometry.GetProperty().SetOpacity(_color.A / 255d);

            if (data.Pickable) _geometry.PickableOn();
            else _geometry.PickableOff();

            // Edges
            if (data.CanHaveElementEdges)
                this._elementEdges = GetActorEdgesFromGridByVisualizationSurfaceExtraction(uGridEdges);
        }
        private void CreateFromData(vtkMaxActorData data)
        {
            // Unstructured grid
            vtkUnstructuredGrid uGridActor;
            vtkUnstructuredGrid uGridEdges;

            bool addBackFace = data.Layer == vtkRendererLayer.Selection;
            addBackFace = false;

            AddNodeAndCellDataToGrid(data.Geometry, out uGridActor, out uGridEdges, true, addBackFace);

            // Add scalars
            if (data.Geometry.Nodes.Values != null)
            {
                vtkFloatArray scalars = vtkFloatArray.New();
                scalars.SetName(Globals.ScalarArrayName);
                scalars.SetNumberOfValues(data.Geometry.Nodes.Values.Length);
                for (int i = 0; i < data.Geometry.Nodes.Values.Length; i++)
                {
                    scalars.SetValue(i, data.Geometry.Nodes.Values[i]);
                }
                // Set scalars
                uGridActor.GetPointData().SetScalars(scalars);
                // Set extreme nodes data
                if (data.Geometry.ExtremeNodes.Ids != null)
                {
                    _minNode = new vtkMaxExtreemeNode(data.Geometry.ExtremeNodes.Ids[0], data.Geometry.ExtremeNodes.Coor[0], data.Geometry.ExtremeNodes.Values[0]);
                    _maxNode = new vtkMaxExtreemeNode(data.Geometry.ExtremeNodes.Ids[1], data.Geometry.ExtremeNodes.Coor[1], data.Geometry.ExtremeNodes.Values[1]);
                }
            }

            // Actor                                                                                   
            
            // Create polydata from unstructured grid

            vtkGeometryFilter geometryFilter = vtkGeometryFilter.New();
            geometryFilter.SetInput(uGridActor);
            geometryFilter.Update();
            vtkPolyData polydata = geometryFilter.GetOutput();
            polydata.GetPointData().CopyScalarsOn();
            // Normals
            if (data.SmoothShaded) polydata.GetPointData().SetNormals(ComputeNormals(polydata));

            

            //// Transform
            //double[] position = _geometry.GetPosition();
            //double[] center = _geometry.GetCenter();
            //double[] delta = new double[] { center[0] - position[0], center[1] - position[1], center[2] - position[2] };
            //double scale = 2;
            //center[0] *= scale;
            //center[1] *= scale;
            //center[2] *= scale;
            //position[0] = center[0] - delta[0];
            //position[1] = center[1] - delta[1];
            //position[2] = center[2] - delta[2];
            ////
            //double[] bounds = polydata.GetBounds();

            //delta[0] = (bounds[0] + bounds[1]) / 2 * (scale - 1);
            //delta[1] = (bounds[2] + bounds[3]) / 2 * (scale - 1);
            //delta[2] = (bounds[4] + bounds[5]) / 2 * (scale - 1);
            ////
            //vtkTransform translation = vtkTransform.New();
            //translation.Translate(delta[0], delta[1], delta[2]);
            ////
            //vtkTransformPolyDataFilter transformFilter = vtkTransformPolyDataFilter.New();
            //transformFilter.SetInput(polydata);
            //transformFilter.SetTransform(translation);
            //transformFilter.Update();




            // Mapper
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            //mapper.SetInputConnection(transformFilter.GetOutputPort());
            mapper.SetInput(polydata);
            // Actor
            _geometry.SetMapper(mapper);
            _geometryPointData = polydata.GetPointData();
            //
            if (data.Pickable) _geometry.PickableOn();
            else _geometry.PickableOff();
            // Frustum locator
            //if (data.Layer == vtkRendererLayer.Base && data.Pickable && data.CellLocator != null)
            if (data.Layer == vtkRendererLayer.Base && data.CellLocator != null)
            { 
                vtkUnstructuredGrid uGridCellLocator;

                AddNodeAndCellDataToGrid(data.CellLocator, out uGridCellLocator);

                _frustumCellLocator = vtkCellLocator.New();
                _frustumCellLocator.SetDataSet(uGridCellLocator);
                _frustumCellLocator.LazyEvaluationOn();
                _frustumCellLocator.GetDataSet().GetPointData().SetGlobalIds((vtkUnsignedLongArray)uGridCellLocator.GetPointData().GetGlobalIds());
                _frustumCellLocator.GetDataSet().GetCellData().SetGlobalIds((vtkUnsignedLongArray)uGridCellLocator.GetCellData().GetGlobalIds());

                // Add scalars
                if (data.CellLocator.Nodes.Values != null)
                {
                    vtkFloatArray scalars = vtkFloatArray.New();
                    scalars.SetName(Globals.ScalarArrayName);
                    scalars.SetNumberOfValues(data.CellLocator.Nodes.Values.Length);
                    for (int i = 0; i < data.CellLocator.Nodes.Values.Length; i++)
                    {
                        scalars.SetValue(i, data.CellLocator.Nodes.Values[i]);
                    }
                    // Set scalars
                    _frustumPointData = _frustumCellLocator.GetDataSet().GetPointData();
                    _frustumPointData.SetScalars(scalars);

                    // Set extreme nodes data
                    //if (data.Geometry.ExtremeNodes.Ids != null)
                    //{
                    //   _minNode = new vtkMaxExtreemeNode(data.Geometry.ExtremeNodes.Ids[0], data.Geometry.ExtremeNodes.Coor[0], data.Geometry.ExtremeNodes.Values[0]);
                    //   _maxNode = new vtkMaxExtreemeNode(data.Geometry.ExtremeNodes.Ids[1], data.Geometry.ExtremeNodes.Coor[1], data.Geometry.ExtremeNodes.Values[1]);
                    // }
                }
            }

            // Element edges
            if (data.CanHaveElementEdges)
            {
                geometryFilter = vtkGeometryFilter.New();
                geometryFilter.SetInput(uGridEdges);
                geometryFilter.Update();
                polydata = vtkPolyData.New();
                polydata = geometryFilter.GetOutput();
                // Mapper
                mapper = vtkDataSetMapper.New();
                mapper.SetInput(polydata);
                // Actor
                _elementEdges = new vtkActor();
                _elementEdges.SetMapper(mapper);
                _elementEdges.GetProperty().SetLighting(true);
                _elementEdges.PickableOff();
            }

            // Model edges          
            if (data.ModelEdges != null)
            {               
                AddNodeAndCellDataToGrid(data.ModelEdges, out uGridActor);
                // Polydata
                geometryFilter = vtkGeometryFilter.New();
                geometryFilter.SetInput(uGridActor);
                geometryFilter.Update();
                polydata = vtkPolyData.New();
                polydata = geometryFilter.GetOutput();

                // Mapper
                mapper = vtkDataSetMapper.New();
                mapper.SetInput(polydata);
                // Actor
                _modelEdges = vtkActor.New();
                _modelEdges.SetMapper(mapper);
                _modelEdges.GetProperty().SetLighting(true);
                _modelEdges.PickableOff();

                if (!data.CanHaveElementEdges)
                {
                    // recreate data
                    polydata.DeepCopy(polydata);
                    // Mapper
                    mapper = vtkDataSetMapper.New();
                    mapper.SetInput(polydata);
                    // Actor
                    _elementEdges = new vtkActor();
                    _elementEdges.SetMapper(mapper);
                    _elementEdges.GetProperty().SetLighting(true);
                    _elementEdges.PickableOff();
                }
            }

           
        }
        

        //                          
        private vtkActor GetActorEdgesFromGrid_(vtkUnstructuredGrid uGridEdges)
        {
            // SHOWS INTERNAL EDGES OF PARABOLIC ELEMENTS
            if (uGridEdges.GetNumberOfCells() <= 0) return null;

            //vtkLookupTable table = vtkLookupTable.New();
            //table.SetNumberOfColors(1);
            //table.SetTableValue(0, 0, 0, 0, 0);

            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInput(uGridEdges);
            //mapper.SetLookupTable(table);

            vtkProperty prop = vtkProperty.New();
            prop.SetRepresentationToWireframe();
            prop.SetColor(0, 0, 0);
            prop.SetLighting(false);

            vtkActor edges = vtkActor.New();
            edges.SetMapper(mapper);
            edges.SetProperty(prop);
            edges.PickableOff();

            return edges;
        }
        private vtkActor ExtractActorEdges_(vtkActor actor)
        {
            // Edges
            vtkExtractEdges extractEdges = vtkExtractEdges.New();
            extractEdges.SetInput(actor.GetMapper().GetInput());
            extractEdges.Update();

            // Visualize edges
            vtkPolyDataMapper mapperEdges = vtkPolyDataMapper.New();
            mapperEdges.SetInputConnection(extractEdges.GetOutputPort());
            vtkActor actorEdges = vtkActor.New();
            actorEdges.SetMapper(mapperEdges);
            actorEdges.GetProperty().SetColor(0, 0, 0);
            actorEdges.GetProperty().SetLineWidth(1);
            actorEdges.GetProperty().EdgeVisibilityOn();
            actorEdges.PickableOff();

            return actorEdges;
        }
        private vtkActor GetActorEdgesFromGrid__(vtkUnstructuredGrid uGridEdges)
        {
            if (uGridEdges.GetNumberOfCells() <= 0) return null;

            //vtkLookupTable table = vtkLookupTable.New();
            //table.SetNumberOfColors(1);
            //table.SetTableValue(0, 0, 0, 0, 0);

            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInput(uGridEdges);
            //mapper.SetLookupTable(table);

            vtkProperty prop = vtkProperty.New();
            prop.SetRepresentationToWireframe();
            prop.SetColor(0, 0, 0);
            prop.SetLighting(false);

            vtkActor edges = vtkActor.New();
            edges.SetMapper(mapper);
            edges.SetProperty(prop);
            edges.PickableOff();

            return edges;
        }
        private vtkActor GetActorEdgesFromGrid___(vtkUnstructuredGrid uGridEdges)
        {
            if (uGridEdges.GetNumberOfCells() <= 0) return null;


            vtkDataSetSurfaceFilter surfaceFilter = vtkDataSetSurfaceFilter.New();
            surfaceFilter.SetInput(uGridEdges);
            //surfaceFilter.SetInputData(unstructuredGrid);
            surfaceFilter.Update();

            vtkPolyData polydata = surfaceFilter.GetOutput();

            vtkFeatureEdges featureEdges = vtkFeatureEdges.New();
            featureEdges.SetInput(polydata);
            featureEdges.BoundaryEdgesOn();
            featureEdges.FeatureEdgesOff();
            featureEdges.ManifoldEdgesOff();
            featureEdges.NonManifoldEdgesOn();
            featureEdges.Update();

            //vtkDataSetMapper mapper = vtkDataSetMapper.New();
            //mapper.SetInput(uGridEdges);

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(featureEdges.GetOutputPort());

            vtkProperty prop = vtkProperty.New();
            prop.SetRepresentationToWireframe();
            prop.SetColor(0, 0, 0);
            prop.SetLighting(false);

            vtkActor edges = vtkActor.New();
            edges.SetMapper(mapper);
            edges.SetProperty(prop);
            edges.PickableOff();

            return edges;
        }
        private void GetActorEdgesFromGrid_(vtkUnstructuredGrid uGridEdges, ref vtkActor actorEdges)
        {
            actorEdges = GetActorEdgesFromGridByVisualizationSurfaceExtraction(uGridEdges);
        }

       
        private void AddNodeAndCellDataToGrid(PartExchangeData data, out vtkUnstructuredGrid uGridActor)
        {           
            vtkUnstructuredGrid uGridEdges;
            AddNodeAndCellDataToGrid(data, out uGridActor, out uGridEdges, false, false);
        }
        private void AddNodeAndCellDataToGrid(PartExchangeData data, out vtkUnstructuredGrid uGridActor, out vtkUnstructuredGrid uGridEdges, 
                                              bool extractEdges, bool addBackFace)
        {
            // Unstructured grid
            uGridActor = vtkUnstructuredGrid.New();
            uGridEdges = vtkUnstructuredGrid.New();

            // Add the points
            vtkPoints points = vtkPoints.New();
            points.SetNumberOfPoints(data.Nodes.Coor.Length);

            for (int i = 0; i < data.Nodes.Coor.Length; i++)
            {
                points.SetPoint(i, data.Nodes.Coor[i][0], data.Nodes.Coor[i][1], data.Nodes.Coor[i][2]);
            }
            uGridActor.SetPoints(points);
            uGridEdges.SetPoints(points);

            // Add the cells
            AddCellDataToGrid(data, ref uGridActor, ref uGridEdges, extractEdges, addBackFace);

            // Add node ids to points in the grid
            if (data.Nodes.Ids != null)
            {
                vtkUnsignedLongArray pointIds = vtkUnsignedLongArray.New();
                pointIds.SetNumberOfValues(data.Nodes.Ids.Length);
                for (int i = 0; i < data.Nodes.Ids.Length; i++)
                {
                    pointIds.SetValue(i, (uint)data.Nodes.Ids[i]);
                }
                uGridActor.GetPointData().SetGlobalIds(pointIds);
            }

            // Add cell ids to cells in the grid
            if (data.Cells.Ids != null)
            {
                vtkUnsignedLongArray cellIds = vtkUnsignedLongArray.New();
                cellIds.SetNumberOfValues(data.Cells.Ids.Length);
                for (int i = 0; i < data.Cells.Ids.Length; i++)
                {
                    cellIds.SetValue(i, (uint)data.Cells.Ids[i]);
                }
                uGridActor.GetCellData().SetGlobalIds(cellIds);
            }

            // Update grid
            uGridActor.Update();
            uGridEdges.Update();                        
        }
        private void AddCellDataToGrid(PartExchangeData data, ref vtkUnstructuredGrid uGridActor, ref vtkUnstructuredGrid uGridEdges, 
                                       bool extractEdges, bool addBackFace)
        {
            int[] nodeIds;
            int cellType;
            bool isEdge;
            bool isSurface;
            vtkIdList nodeList = vtkIdList.New();
            vtkIdList edgeNodeList = vtkIdList.New();
            List<int> actorCellIds = new List<int>();

            for (int i = 0; i < data.Cells.CellNodeIds.GetLength(0); i++)
            {
                nodeIds = data.Cells.CellNodeIds[i];
                cellType = data.Cells.Types[i];
                isEdge = cellType == (int)vtkCellType.VTK_LINE || cellType == (int)vtkCellType.VTK_QUADRATIC_EDGE;
                isSurface = cellType == (int)vtkCellType.VTK_TRIANGLE || cellType == (int)vtkCellType.VTK_QUAD
                                        || cellType == (int)vtkCellType.VTK_QUADRATIC_TRIANGLE || cellType == (int)vtkCellType.VTK_QUADRATIC_QUAD;

                // Node list
                nodeList.SetNumberOfIds(nodeIds.Length);
                for (int j = 0; j < nodeIds.Length; j++) nodeList.SetId(j, nodeIds[j]);

                // Cell edges                                                                                                       
                if (!isEdge && uGridEdges != null)
                {
                    if (!extractEdges) uGridEdges.InsertNextCell(cellType, nodeList);
                    else
                    {
                        if (cellType == (int)vtkCellType.VTK_TRIANGLE)
                        {
                            edgeNodeList.SetNumberOfIds(2);
                            for (int j = 0; j < 3; j++)
                            {
                                edgeNodeList.SetId(0, nodeIds[j]);
                                edgeNodeList.SetId(1, nodeIds[(j + 1) % 3]);
                                uGridEdges.InsertNextCell((int)vtkCellType.VTK_LINE, edgeNodeList);
                            }
                        }
                        else if (cellType == (int)vtkCellType.VTK_QUAD)
                        {
                            edgeNodeList.SetNumberOfIds(2);
                            for (int j = 0; j < 4; j++)
                            {
                                edgeNodeList.SetId(0, nodeIds[j]);
                                edgeNodeList.SetId(1, nodeIds[(j + 1) % 4]);
                                uGridEdges.InsertNextCell((int)vtkCellType.VTK_LINE, edgeNodeList);
                            }
                        }
                        else if (cellType == (int)vtkCellType.VTK_QUADRATIC_TRIANGLE)
                        {
                            edgeNodeList.SetNumberOfIds(3);
                            for (int j = 0; j < 3; j++)
                            {
                                edgeNodeList.SetId(0, nodeIds[j]);
                                edgeNodeList.SetId(1, nodeIds[(j + 1) % 3]);
                                edgeNodeList.SetId(2, nodeIds[j + 3]);
                                uGridEdges.InsertNextCell((int)vtkCellType.VTK_QUADRATIC_EDGE, edgeNodeList);
                            }
                        }
                        else if (cellType == (int)vtkCellType.VTK_QUADRATIC_QUAD)
                        {
                            edgeNodeList.SetNumberOfIds(3);
                            for (int j = 0; j < 4; j++)
                            {
                                edgeNodeList.SetId(0, nodeIds[j]);
                                edgeNodeList.SetId(1, nodeIds[(j + 1) % 4]);
                                edgeNodeList.SetId(2, nodeIds[j + 4]);
                                uGridEdges.InsertNextCell((int)vtkCellType.VTK_QUADRATIC_EDGE, edgeNodeList);
                            }
                        }
                    }
                }

                // Cells                                                                                                            
                uGridActor.InsertNextCell(cellType, nodeList);
                if (data.Cells.Ids != null) actorCellIds.Add(data.Cells.Ids[i]);

                // Cells backface
                if (isSurface && addBackFace) 
                {
                    nodeList.SetNumberOfIds(nodeIds.Length);
                    if (cellType == (int)vtkCellType.VTK_TRIANGLE)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[2]);
                        nodeList.SetId(2, nodeIds[1]);
                    }
                    else if (cellType == (int)vtkCellType.VTK_QUAD)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[3]);
                        nodeList.SetId(2, nodeIds[2]);
                        nodeList.SetId(3, nodeIds[1]);
                    }
                    else if (cellType == (int)vtkCellType.VTK_QUADRATIC_TRIANGLE)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[2]);
                        nodeList.SetId(2, nodeIds[1]);
                        nodeList.SetId(3, nodeIds[5]);
                        nodeList.SetId(4, nodeIds[4]);
                        nodeList.SetId(5, nodeIds[3]);
                    }
                    else if (cellType == (int)vtkCellType.VTK_QUADRATIC_QUAD)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[3]);
                        nodeList.SetId(2, nodeIds[2]);
                        nodeList.SetId(3, nodeIds[1]);
                        nodeList.SetId(4, nodeIds[7]);
                        nodeList.SetId(5, nodeIds[6]);
                        nodeList.SetId(6, nodeIds[5]);
                        nodeList.SetId(7, nodeIds[4]);
                    }
                    else throw new NotSupportedException();

                    uGridActor.InsertNextCell(cellType, nodeList);
                    if (data.Cells.Ids != null) actorCellIds.Add(data.Cells.Ids[i]);
                }
            }

            if (data.Cells.Ids != null) data.Cells.Ids = actorCellIds.ToArray();
        }        

        // Color and visuals
        public void UpdateColor()
        {
            double opacity = _color.A / 255d;

            vtkProperty property = _geometry.GetProperty();
            property.SetAmbient(_ambient);
            property.SetDiffuse(_diffuse);
            property.SetAmbientColor(1, 1, 1);  // also reset part color highlight
            //
            property.SetOpacity(opacity);
            //
            if (!ColorContours)
            {
                property.SetColor(_color.R / 255d, _color.G / 255d, _color.B / 255d);
                property.SetSpecular(0.6);
                property.SetSpecularColor(1, 1, 1);
                property.SetSpecularPower(100);
            }
            //
            if (_modelEdges != null)
            {
                property = _modelEdges.GetProperty();
                property.SetColor(0, 0, 0);
                property.SetLighting(false);
                property.SetLineWidth(1.2f);
                property.SetOpacity(opacity * 0.7);
            }
            //if (opacity < 1) opacity = Math.Min(0.2, opacity);
            if (_elementEdges != null)
            {
                property = _elementEdges.GetProperty();
                property.SetColor(0, 0, 0);
                property.SetLighting(false);
                property.SetLineWidth(0.5f);
                property.SetOpacity(opacity * 0.4);
            }
        }
        public void Highlight()
        {
            _geometry.GetProperty().SetAmbientColor(1, 0, 0);
        }
        private vtkDataArray ComputeNormals(vtkPointSet pointSet)
        {
            // recompute polydata normals
            vtkPolyDataNormals normalGenerator = vtkPolyDataNormals.New();
            normalGenerator.SetInput(pointSet);
            normalGenerator.SplittingOff(); // prevents changes in topology
            //normalGenerator.SetFeatureAngle(0.001 * Math.PI / 180);
            normalGenerator.ComputePointNormalsOn();
            normalGenerator.ComputeCellNormalsOff();
            normalGenerator.Update();
            return normalGenerator.GetOutput().GetPointData().GetNormals();
        }


        // Animation                                                                                                                
        public void SetAnimationFrame(vtkMaxActorData data, int frameNumber)
        {
            if (data.Geometry.NodesAnimation != null)
            {
                if (frameNumber < 0 || frameNumber >= data.Geometry.NodesAnimation.Length) throw new Exception("The animation frame can not be set.");

                vtkMaxActorAnimationData animationData = new vtkMaxActorAnimationData(data.Geometry.NodesAnimation[frameNumber].Coor, 
                                                                                      data.ModelEdges.NodesAnimation[frameNumber].Coor,
                                                                                      data.Geometry.NodesAnimation[frameNumber].Values,
                                                                                      data.Geometry.ExtremeNodesAnimation[frameNumber],
                                                                                      data.CellLocator.NodesAnimation[frameNumber].Coor,
                                                                                      data.CellLocator.NodesAnimation[frameNumber].Values);
                // actor points
                if (animationData.Points != null)
                {
                    vtkPointSet pointSet = _geometry.GetMapper().GetInput() as vtkPointSet;
                    pointSet.SetPoints(animationData.Points);
                    (_elementEdges.GetMapper().GetInput() as vtkPointSet).SetPoints(animationData.Points);

                    // normals
                    if (pointSet.GetPointData().GetNormals() != null && animationData.PointNormals == null)
                        animationData.PointNormals = ComputeNormals(pointSet);
                    pointSet.GetPointData().SetNormals(animationData.PointNormals);
                }
                
                // actor edges points
                if (animationData.ModelEdgesPoints != null && _modelEdges != null)
                    (_modelEdges.GetMapper().GetInput() as vtkPointSet).SetPoints(animationData.ModelEdgesPoints);

                // values
                if (animationData.Values != null) _geometry.GetMapper().GetInput().GetPointData().SetScalars(animationData.Values);
                
                // min/max
                if (animationData.MinNode != null) _minNode = animationData.MinNode;
                if (animationData.MaxNode != null) _maxNode = animationData.MaxNode;

                // locator points
                if (animationData.LocatorPoints != null)
                {
                    vtkPointSet pointSet = _frustumCellLocator.GetDataSet() as vtkPointSet;
                    pointSet.SetPoints(animationData.LocatorPoints);
                }

                // locator values
                if (animationData.LocatorValues != null) _frustumCellLocator.GetDataSet().GetPointData().SetScalars(animationData.LocatorValues);
            }
        }
    }
}
