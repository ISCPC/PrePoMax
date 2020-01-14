using CaeGlobals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class VisualizationData
    {
        // vertex
        // edge     - parabolic edge [0, 1, 2] is visually discretized like 0----2----1
        // face     - cells limited by edges
        // surface  - can be arbitrary combination of cells

        // Variables                                                                                                                
        protected int[][] _cells;
        protected int[] _cellIds;
        protected int[][] _cellIdsByFace;        // coud be hashset<> but serialization coud be bad
        protected double[] _faceAreas;
        protected int[][] _faceEdgeIds;
        protected int[][] _cellNeighboursOverCellEdge;
        protected int[][] _edgeCells;
        protected int[][] _edgeCellIdsByEdge;
        protected double[] _edgeLengths;
        protected int[] _vertexNodeIds;


        // Properties                                                                                                               
        /// <summary>
        /// Cells
        /// [0...num. of cells][0...num. of nodes] -> global node id
        /// </summary>
        public int[][] Cells { get { return _cells; } set { _cells = value; } }

        /// <summary>
        /// CellIds
        /// [0...num. of cells] -> global element id
        /// </summary>
        public int[] CellIds { get { return _cellIds; } set { _cellIds = value; } }

        /// <summary>
        /// CellIdsByFace
        /// [0...num. of faces][0...num. of face cells] -> local cell id
        /// </summary>
        public int[][] CellIdsByFace
        {
            get { return _cellIdsByFace; }
            set
            {
                _cellIdsByFace = value;
                if (_cellIdsByFace != null)
                {
                    // sort ids for binary search
                    for (int i = 0; i < _cellIdsByFace.Length; i++) Array.Sort(_cellIdsByFace[i]);
                }
            }
        }

        /// <summary>
        /// FaceAreas
        /// [0...num. of faces]
        /// </summary>
        public double[] FaceAreas
        {
            get { return _faceAreas; }
            set { _faceAreas = value; }
        }

        /// <summary>
        /// FaceEdgeIds
        /// [0...num. of faces][0...num. of edges] -> local edge id
        /// </summary>
        public int[][] FaceEdgeIds { get { return _faceEdgeIds; } set { _faceEdgeIds = value; } }

        /// <summary>
        /// CellNeighboursOverEdge
        /// [0...num. of cells][0...num. of neigh.] -> local cell id
        /// </summary>
        public int[][] CellNeighboursOverCellEdge { get { return _cellNeighboursOverCellEdge; } set { _cellNeighboursOverCellEdge = value; } }

        /// <summary>
        /// EdgeCells
        /// [0...num. of edge cells][0...num. of nodes] -> global node id
        /// </summary>
        public int[][] EdgeCells { get { return _edgeCells; } set { _edgeCells = value; } }

        /// <summary>
        /// EdgeCellIdsByEdge
        /// [0...num. of edges][0...num. of edge cells] -> local edge cell id
        /// </summary>
        public int[][] EdgeCellIdsByEdge { get { return _edgeCellIdsByEdge; } set { _edgeCellIdsByEdge = value; } }

        /// <summary>
        /// EdgeLengths
        /// [0...num. of edges]
        /// </summary>
        public double[] EdgeLengths
        {
            get { return _edgeLengths; }
            set { _edgeLengths = value; }
        }

        /// <summary>
        /// VertexNodeIds
        /// [0...num. of vertices] -> global node id (a vertice is a node where more than two edge cells meet)
        /// </summary>
        public int[] VertexNodeIds { get { return _vertexNodeIds; } set { _vertexNodeIds = value; } }

        [NonSerialized]
        private Dictionary<int[], CellNeighbour> _cellNeighboursOverCell;


        // Constructors                                                                                                             
        public VisualizationData()
        {
            _cells = null;
            _cellIds = null;
            _cellIdsByFace = null;
            _faceAreas = null;
            _cellNeighboursOverCellEdge = null;
            _edgeCells = null;
            _edgeCellIdsByEdge = null;
            _edgeLengths = null;
            _vertexNodeIds = null;
            _cellNeighboursOverCell = null;
        }
        public VisualizationData(VisualizationData visualization)
            : this()
        {
            if (visualization.Cells != null)
            {
                _cells = new int[visualization.Cells.Length][];
                for (int i = 0; i < _cells.Length; i++)
                    _cells[i] = visualization.Cells[i].ToArray();
            }

            _cellIds = visualization.CellIds != null ? visualization.CellIds.ToArray() : null;
            
            if (visualization.CellIdsByFace != null)
            {
                _cellIdsByFace = new int[visualization.CellIdsByFace.Length][];
                for (int i = 0; i < _cellIdsByFace.Length; i++)
                    _cellIdsByFace[i] = visualization.CellIdsByFace[i].ToArray();
            }

            _faceAreas = visualization.FaceAreas != null ? visualization.FaceAreas.ToArray() : null;

            if (visualization.FaceEdgeIds != null)
            {
                _faceEdgeIds = new int[visualization.FaceEdgeIds.Length][];
                for (int i = 0; i < _faceEdgeIds.Length; i++)
                    _faceEdgeIds[i] = visualization.FaceEdgeIds[i].ToArray();
            }

            if (visualization.CellNeighboursOverCellEdge != null)
            {
                _cellNeighboursOverCellEdge = new int[visualization.CellNeighboursOverCellEdge.Length][];
                for (int i = 0; i < _cellNeighboursOverCellEdge.Length; i++)
                {
                    if (visualization.CellNeighboursOverCellEdge[i] != null)
                        _cellNeighboursOverCellEdge[i] = visualization.CellNeighboursOverCellEdge[i].ToArray();
                }
            }

            if (visualization.EdgeCells != null)
            {
                _edgeCells = new int[visualization.EdgeCells.Length][];
                for (int i = 0; i < _edgeCells.Length; i++) _edgeCells[i] = visualization.EdgeCells[i].ToArray();
            }

            if (visualization.EdgeCellIdsByEdge != null)
            {
                _edgeCellIdsByEdge = new int[visualization.EdgeCellIdsByEdge.Length][];
                for (int i = 0; i < _edgeCellIdsByEdge.Length; i++) _edgeCellIdsByEdge[i] = visualization.EdgeCellIdsByEdge[i].ToArray();
            }

            _edgeLengths = visualization.EdgeLengths != null ? visualization.EdgeLengths.ToArray() : null;

            _vertexNodeIds = visualization.VertexNodeIds != null ? visualization.VertexNodeIds.ToArray() : null;

            // Reference exchange - for speed and memory - might be a problem
            _cellNeighboursOverCell = visualization._cellNeighboursOverCell != null ? visualization._cellNeighboursOverCell : null;
        }


        // Methods
        public void ExtractVisualizationCells(Dictionary<int, FeElement> elements, int[] elementIds)
        {
            if (_cellNeighboursOverCell == null) ExtractCellNeighboursOverCell(elements, elementIds);

            List<int[]> visualizationCells = new List<int[]>();
            List<int> visualizationCellsIds = new List<int>();
            foreach (var entry in _cellNeighboursOverCell)
            {
                if (entry.Value.Id2 == -1)
                {
                    visualizationCells.Add(entry.Value.Cell1);
                    visualizationCellsIds.Add(entry.Value.Id1);
                }
            }
            // Save
            _cells = visualizationCells.ToArray();
            _cellIds = visualizationCellsIds.ToArray(); ;
            
        }
        private void ExtractCellNeighboursOverCell(Dictionary<int, FeElement> elements, int[] elementIds)
        {
            int[] sorted;
            CompareIntArray comparer = new CompareIntArray();
            CellNeighbour cellNeighbour;
            _cellNeighboursOverCell = new Dictionary<int[], CellNeighbour>(elementIds.Length, comparer);

            // Parallelizing this loop does not bring any speedup
            foreach (var id in elementIds)
            {
                foreach (int[] cell in ((FeElement3D)elements[id]).GetAllVtkCells())
                {
                    sorted = cell.ToArray();
                    Array.Sort(sorted);

                    if (_cellNeighboursOverCell.TryGetValue(sorted, out cellNeighbour)) cellNeighbour.Id2 = id;
                    else _cellNeighboursOverCell.Add(sorted, new CellNeighbour(id, -1, cell));
                }
            }
        }

        public void RenumberNodes(Dictionary<int, int> newIds)
        {
            // Cells
            if (_cells != null)
            {
                for (int i = 0; i < _cells.Length; i++)
                {
                    for (int j = 0; j < _cells[i].Length; j++)
                    {
                        _cells[i][j] = newIds[_cells[i][j]];
                    }
                }
            }
            // Edge cells
            if (_edgeCells != null)
            {

                for (int i = 0; i < _edgeCells.Length; i++)
                {
                    for (int j = 0; j < _edgeCells[i].Length; j++)
                    {
                        _edgeCells[i][j] = newIds[_edgeCells[i][j]];
                    }
                }
            }
            // Vertex ids
            if (_vertexNodeIds != null)
            {
                for (int i = 0; i < _vertexNodeIds.Length; i++)
                {
                    _vertexNodeIds[i] = newIds[_vertexNodeIds[i]];
                }
            }
        }
        public void RenumberElements(Dictionary<int, int> newIds)
        {
            if (_cellIds != null)
            {
                for (int i = 0; i < _cellIds.Length; i++)
                {
                    _cellIds[i] = newIds[_cellIds[i]];
                }
            }
        }
        public void RenumberSurfaces(int[] orderedSurfaceIds)
        {
            // Surface cells
            int[][] newCellIdsByFace = new int[_cellIdsByFace.Length][];
            for (int i = 0; i < orderedSurfaceIds.Length; i++)
            {
                newCellIdsByFace[i] = _cellIdsByFace[orderedSurfaceIds[i]];
            }
            _cellIdsByFace = newCellIdsByFace;

            // Areas
            double[] newFaceAreas = new double[_faceAreas.Length];
            for (int i = 0; i < orderedSurfaceIds.Length; i++)
            {
                newFaceAreas[i] = _faceAreas[orderedSurfaceIds[i]];
            }
            _faceAreas = newFaceAreas;

            // Edges
            int[][] newFaceEdgeIds = new int[_faceEdgeIds.Length][];
            for (int i = 0; i < orderedSurfaceIds.Length; i++)
            {
                newFaceEdgeIds[i] = _faceEdgeIds[orderedSurfaceIds[i]];
            }
            _faceEdgeIds = newFaceEdgeIds;
        }
        public void RenumberEdges(int[] orderedEdgeIds)
        {
            // inverse map
            int[] map = new int[orderedEdgeIds.Length];
            for (int i = 0; i < orderedEdgeIds.Length; i++)
            {
                map[orderedEdgeIds[i]] = i;
            }
            // Surface edges
            int[][] newFaceEdgeIds = new int[_faceEdgeIds.Length][];
            for (int i = 0; i < _faceEdgeIds.Length; i++)
            {
                newFaceEdgeIds[i] = new int[_faceEdgeIds[i].Length];
                for (int j = 0; j < _faceEdgeIds[i].Length; j++)
                {
                    newFaceEdgeIds[i][j] = map[_faceEdgeIds[i][j]];
                }
            }
            _faceEdgeIds = newFaceEdgeIds;

            // Edge cells
            int[][] newEdgeCellIdsByEdge = new int[_edgeCellIdsByEdge.Length][];
            for (int i = 0; i < orderedEdgeIds.Length; i++)
            {
                newEdgeCellIdsByEdge[i] = _edgeCellIdsByEdge[orderedEdgeIds[i]];
            }
            _edgeCellIdsByEdge = newEdgeCellIdsByEdge;

            // Lengths
            double[] newEdgeLengths = new double[_edgeLengths.Length];
            for (int i = 0; i < orderedEdgeIds.Length; i++)
            {
                newEdgeLengths[i] = _edgeLengths[orderedEdgeIds[i]];
            }
            _edgeLengths = newEdgeLengths;
        }

        public VisualizationData DeepCopy()
        {
            return new VisualizationData(this);
        }


        // Section cut
        public void ApplySectionView(Dictionary<int, FeElement> elements, int[] elementIds, HashSet<int> frontNodes, HashSet<int> backNodes)
        {
            HashSet<int> visibleNodes;
            SectionCutCells(elements, frontNodes, backNodes, out visibleNodes);
            SectionCutEdgeCells(visibleNodes);
            //_cells = new int[0][];
            //_cellIds = new int[0];
            //_cellIdsByFace = new int[0][];
            CreateSectionPlaneCellsAndEdges(elements, elementIds, frontNodes, backNodes);
        }
        public void RemoveSectionView()
        {

        }
        private HashSet<int> SectionCutCells(Dictionary<int, FeElement> elements, HashSet<int> frontNodes, HashSet<int> backNodes,
                                             out HashSet<int> visibleNodes)
        {
            // Split visualization cells                                                    
            FeElement element;
            bool insert;
            int[] cellMap = new int[_cells.Length];
            List<int[]> splitCells = new List<int[]>();
            List<int> splitCellIds = new List<int>();
            visibleNodes = new HashSet<int>();
            for (int i = 0; i < _cells.Length; i++)
            {
                element = elements[CellIds[i]];
                insert = true;
                for (int j = 0; j < element.NodeIds.Length; j++)
                {
                    if (frontNodes.Contains(element.NodeIds[j]))
                    {
                        insert = false;
                        break;
                    }
                }
                if (insert)
                {
                    cellMap[i] = splitCells.Count;
                    splitCells.Add(_cells[i]);
                    splitCellIds.Add(_cellIds[i]);
                    visibleNodes.UnionWith(_cells[i]);
                }
                else
                {
                    cellMap[i] = -1;
                }
            }
            // Renumber cell ids by face
            int cellId;
            List<int> cellIds = new List<int>();
            for (int i = 0; i < _cellIdsByFace.Length; i++)
            {
                cellIds.Clear();
                for (int j = 0; j < _cellIdsByFace[i].Length; j++)
                {
                    cellId = cellMap[_cellIdsByFace[i][j]];
                    if (cellId != -1) cellIds.Add(cellId);
                }
                _cellIdsByFace[i] = cellIds.ToArray();
            }
            // Save
            _cells = splitCells.ToArray();
            _cellIds = splitCellIds.ToArray();

            return visibleNodes;
        }
        private void SectionCutEdgeCells(HashSet<int> visibleNodes)
        {
            // Split visualization edges                                                    
            bool insert;
            int[] cellMap = new int[_edgeCells.Length];
            List<int[]> splitEdgeCells = new List<int[]>();
            for (int i = 0; i < _edgeCells.Length; i++)
            {
                insert = true;
                for (int j = 0; j < _edgeCells[i].Length; j++)
                {
                    if (!visibleNodes.Contains(_edgeCells[i][j]))
                    {
                        insert = false;
                        break;
                    }
                }

                if (insert)
                {
                    cellMap[i] = splitEdgeCells.Count;
                    splitEdgeCells.Add(_edgeCells[i]);
                }
                else
                {
                    cellMap[i] = -1;
                }
            }
            // Renumber edge cell ids by edge
            int cellId;
            List<int> cellIds = new List<int>();
            for (int i = 0; i < _edgeCellIdsByEdge.Length; i++)
            {
                cellIds.Clear();
                for (int j = 0; j < _edgeCellIdsByEdge[i].Length; j++)
                {
                    cellId = cellMap[_edgeCellIdsByEdge[i][j]];
                    if (cellId != -1) cellIds.Add(cellId);
                }
                _edgeCellIdsByEdge[i] = cellIds.ToArray();
            }
            _edgeCells = splitEdgeCells.ToArray();
        }
        private void CreateSectionPlaneCellsAndEdges(Dictionary<int, FeElement> elements, int[] elementIds, HashSet<int> frontNodes, HashSet<int> backNodes)
        {
            List<int[]> sectionPlaneCells;
            List<int> sectionPlaneCellIds;            

            GetSectionElementsAndIds(elements, elementIds, frontNodes, backNodes, out sectionPlaneCells, out sectionPlaneCellIds);
            AddSectionPlaneCellsAndIds(sectionPlaneCells, sectionPlaneCellIds);
            AddSectionPlaneEdgeCellsAndIds(sectionPlaneCells);
        }
        private void GetSectionElementsAndIds(Dictionary<int, FeElement> elements, int[] elementIds, 
                                              HashSet<int> frontNodes, HashSet<int> backNodes,
                                              out List<int[]> sectionPlaneCells, out List<int> sectionPlaneCellIds)
        {
            // Check if cell neighbours exists
            if (_cellNeighboursOverCell == null) ExtractCellNeighboursOverCell(elements, elementIds);

            // Get vtk cells on the plane                                                   
            int countFront;
            int countBack;
            bool insert;
            int id;
            int[] cell;
            int[] sorted;
            int[] sorted2;
            int[][] vtkCells;
            int[][] vtkCells2;
            CellNeighbour cellNeighbour;
            sectionPlaneCells = new List<int[]>();
            sectionPlaneCellIds = new List<int>();
            CompareIntArray comparer = new CompareIntArray();
            for (int i = 0; i < elementIds.Length; i++)
            {
                if (elements[elementIds[i]] is FeElement3D element)
                {
                    countFront = 0;
                    countBack = 0;
                    for (int j = 0; j < element.NodeIds.Length; j++)
                    {
                        if (frontNodes.Contains(element.NodeIds[j])) countFront++;
                        else if (backNodes.Contains(element.NodeIds[j])) countBack++;
                        if (countFront > 0 && countBack > 0)
                        {
                            // The element is on the plane
                            vtkCells = element.GetAllVtkCells();                            
                            for (int k = 0; k < vtkCells.Length; k++)
                            {
                                insert = true;
                                for (int l = 0; l < vtkCells[k].Length; l++)
                                {
                                    if (frontNodes.Contains(vtkCells[k][l]))
                                    {
                                        insert = false;
                                        break;
                                    }
                                }
                                if (insert)
                                {
                                    cell = null;
                                    sorted = vtkCells[k].ToArray();
                                    Array.Sort(sorted);
                                    cellNeighbour =_cellNeighboursOverCell[sorted];
                                    // Add only cells with elements on the other side
                                    if (cellNeighbour.Id2 != -1)
                                    {
                                        // Looking for the other element of the vtkCell where cell orientation is positive
                                        if (cellNeighbour.Id1 == element.Id)
                                        {
                                            id = cellNeighbour.Id2;
                                            // Find the appropriate element cell
                                            element = elements[id] as FeElement3D;
                                            vtkCells2 = element.GetAllVtkCells();
                                            for (int m = 0; m < vtkCells2.Length; m++)
                                            {
                                                sorted2 = vtkCells2[m].ToArray();
                                                Array.Sort(sorted2);

                                                if (comparer.Equals(sorted, sorted2))
                                                {
                                                    cell = vtkCells2[m];
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            id = cellNeighbour.Id1;
                                            cell = cellNeighbour.Cell1;
                                        }
                                        if (cell == null) throw new Exception("The vtk cell was not found.");
                                        // The vtk cell is in front of the plane
                                        sectionPlaneCells.Add(cell);
                                        sectionPlaneCellIds.Add(id);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
        private void AddSectionPlaneCellsAndIds(List<int[]> sectionPlaneCells, List<int> sectionPlaneCellIds)
        {
            // Add section face to visualization                                        
            List<int[]> splitCells = new List<int[]>(_cells);
            List<int> splitCellIds = new List<int>(_cellIds);
            int firstOnPlaneCellLocalId = splitCells.Count;
            splitCells.AddRange(sectionPlaneCells);
            splitCellIds.AddRange(sectionPlaneCellIds);

            // Cell ids by face
            int[][] cellIdsByFace = new int[_cellIdsByFace.Length + 1][];
            for (int i = 0; i < _cellIdsByFace.Length; i++) cellIdsByFace[i] = _cellIdsByFace[i];
            cellIdsByFace[_cellIdsByFace.Length] = new int[sectionPlaneCells.Count];
            for (int i = 0; i < sectionPlaneCells.Count; i++)
                cellIdsByFace[_cellIdsByFace.Length][i] = firstOnPlaneCellLocalId + i;
            
            // Save
            _cells = splitCells.ToArray();
            _cellIds = splitCellIds.ToArray();
            _cellIdsByFace = cellIdsByFace.ToArray();
        }
        private void AddSectionPlaneEdgeCellsAndIds(List<int[]> sectionPlaneCells)
        {
            // Add section edge to visualization                                        
            int[] sorted;
            int[][] cellEdges;
            CompareIntArray comparer = new CompareIntArray();
            Dictionary<int[], int[]> allEdgeCells = new Dictionary<int[], int[]>(comparer);
            foreach (int[] cell in sectionPlaneCells)
            {
                cellEdges = FeMesh.GetVisualizationEdgeCells(cell);
                for (int i = 0; i < cellEdges.Length; i++)
                {
                    sorted = cellEdges[i].ToArray();
                    Array.Sort(sorted);
                    if (!allEdgeCells.Remove(sorted)) allEdgeCells.Add(sorted, cellEdges[i]);
                }
            }
            List<int[]> splitEdgeCells = new List<int[]>(_edgeCells);
            int firstOnPlaneEdgeCellLocalId = splitEdgeCells.Count;
            splitEdgeCells.AddRange(allEdgeCells.Values);

            // Edge cell ids by edge
            int[][] edgeCellIdsByEdge = new int[_edgeCellIdsByEdge.Length + 1][];
            for (int i = 0; i < _edgeCellIdsByEdge.Length; i++) edgeCellIdsByEdge[i] = _edgeCellIdsByEdge[i];
            edgeCellIdsByEdge[_edgeCellIdsByEdge.Length] = new int[allEdgeCells.Count];
            for (int i = 0; i < allEdgeCells.Count; i++)
                edgeCellIdsByEdge[_edgeCellIdsByEdge.Length][i] = firstOnPlaneEdgeCellLocalId + i;

            // Face edge ids
            List<int[]> faceEdgeIds = new List<int[]>(_faceEdgeIds);
            faceEdgeIds.Add(new int[] { edgeCellIdsByEdge.Length - 1 });

            // Save
            _edgeCells = splitEdgeCells.ToArray();
            _edgeCellIdsByEdge = edgeCellIdsByEdge.ToArray();
            _faceEdgeIds = faceEdgeIds.ToArray();
        }

    }
}
