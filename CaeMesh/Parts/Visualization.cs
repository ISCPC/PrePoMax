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
        // edge     - parabolic edge [0, 1, 2] is discretized like 0----2----1
        // face     - cells limited by edges
        // surface  - can be arbitrary combination of cells

        // Variables                                                                                                                
        protected int[][] _cells;
        protected int[] _cellIds;
        protected int[][] _cellIdsByFace;        // coud be hashset<> but serialization coud be bad
        protected double[] _faceAreas;
        protected int[][] _faceEdgeIds;
        protected int[][] _cellNeighboursOverEdge;
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
        /// [0...num. of faces][0...num. of cells] -> local cell id
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
        public int[][] CellNeighboursOverEdge { get { return _cellNeighboursOverEdge; } set { _cellNeighboursOverEdge = value; } }

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
        /// [0...num. of faces]
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
      

        // Constructors                                                                                                             
        public VisualizationData()
        {
            _cells = null;
            _cellIds = null;
            _cellIdsByFace = null;
            _faceAreas = null;
            _cellNeighboursOverEdge = null;
            _edgeCells = null;
            _edgeCellIdsByEdge = null;
            _edgeLengths = null;
            _vertexNodeIds = null;
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

            if (visualization.CellNeighboursOverEdge != null)
            {
                _cellNeighboursOverEdge = new int[visualization.CellNeighboursOverEdge.Length][];
                for (int i = 0; i < _cellNeighboursOverEdge.Length; i++)
                {
                    if (visualization.CellNeighboursOverEdge[i] != null)
                        _cellNeighboursOverEdge[i] = visualization.CellNeighboursOverEdge[i].ToArray();
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
        }

        // Methods
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

    }
}
