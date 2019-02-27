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
        // Variables                                                                                                                
        protected int[][] _cells;
        protected int[] _cellIds;
        protected int[][] _cellIdsBySurface;        // coud be hashset<> but serialization coud be bad
        protected int[][] _surfaceEdgeIds;
        protected int[][] _cellNeighboursOverEdge;
        protected int[][] _edgeCells;
        protected int[][] _edgeCellIdsByEdge;
        protected int[] _vertexNodeIds;


        // Properties                                                                                                               
        /// <summary>
        /// // [0...num. of cells][0...num. of nodes] -> global node id
        /// </summary>
        public int[][] Cells { get { return _cells; } set { _cells = value; } }
        /// <summary>
        /// // [0...num. of cells] -> global element id
        /// </summary>
        public int[] CellIds { get { return _cellIds; } set { _cellIds = value; } }
        /// <summary>
        /// // [0...num. of surfaces][0...num. of cells] -> local cell id
        /// </summary>
        public int[][] CellIdsBySurface
        {
            get { return _cellIdsBySurface; }
            set
            {
                _cellIdsBySurface = value;
                if (_cellIdsBySurface != null)
                {
                    // sort ids for binary search
                    for (int i = 0; i < _cellIdsBySurface.Length; i++) Array.Sort(_cellIdsBySurface[i]);
                }
            }
        }
        /// <summary>
        /// // [0...num. of surfaces][0...num. of edges] -> local edge id
        /// </summary>
        public int[][] SurfaceEdgeIds { get { return _surfaceEdgeIds; } set { _surfaceEdgeIds = value; } }
        /// <summary>
        /// // [0...num. of cells][0...num. of neigh.] -> local cell id
        /// </summary>
        public int[][] CellNeighboursOverEdge { get { return _cellNeighboursOverEdge; } set { _cellNeighboursOverEdge = value; } }
        /// <summary>
        /// // [0...num. of edge cells][0...num. of nodes] -> global node id
        /// </summary>
        public int[][] EdgeCells { get { return _edgeCells; } set { _edgeCells = value; } }
        /// <summary>
        /// // [0...num. of edges][0...num. of edge cells] -> local edge cell id
        /// </summary>
        public int[][] EdgeCellIdsByEdge { get { return _edgeCellIdsByEdge; } set { _edgeCellIdsByEdge = value; } }
        /// <summary>
        /// // [0...num. of vertices] -> global node id
        /// </summary>
        public int[] VertexNodeIds { get { return _vertexNodeIds; } set { _vertexNodeIds = value; } }
      

        // Constructors                                                                                                             
        public VisualizationData()
        {
            _cells = null;
            _cellIds = null;
            _cellIdsBySurface = null;
            _cellNeighboursOverEdge = null;
            _edgeCells = null;
            _edgeCellIdsByEdge = null;
            _vertexNodeIds = null;
        }

        public VisualizationData(VisualizationData visualization)
        {
            _cells = null;
            if (visualization.Cells != null)
            {
                _cells = new int[visualization.Cells.Length][];
                for (int i = 0; i < _cells.Length; i++)
                    _cells[i] = visualization.Cells[i].ToArray();
            }

            _cellIds = visualization.CellIds != null ? visualization.CellIds.ToArray() : null;

            _cellIdsBySurface = null;
            if (visualization.CellIdsBySurface != null)
            {
                _cellIdsBySurface = new int[visualization.CellIdsBySurface.Length][];
                for (int i = 0; i < _cellIdsBySurface.Length; i++)
                    _cellIdsBySurface[i] = visualization.CellIdsBySurface[i].ToArray();
            }

            _surfaceEdgeIds = null;
            if (visualization.SurfaceEdgeIds != null)
            {
                _surfaceEdgeIds = new int[visualization.SurfaceEdgeIds.Length][];
                for (int i = 0; i < _surfaceEdgeIds.Length; i++)
                    _surfaceEdgeIds[i] = visualization.SurfaceEdgeIds[i].ToArray();
            }

            _cellNeighboursOverEdge = null;
            if (visualization.CellNeighboursOverEdge != null)
            {
                _cellNeighboursOverEdge = new int[visualization.CellNeighboursOverEdge.Length][];
                for (int i = 0; i < _cellNeighboursOverEdge.Length; i++)
                {
                    if (visualization.CellNeighboursOverEdge[i] != null)
                        _cellNeighboursOverEdge[i] = visualization.CellNeighboursOverEdge[i].ToArray();
                }
            }

            _edgeCells = null;
            if (visualization.EdgeCells != null)
            {
                _edgeCells = new int[visualization.EdgeCells.Length][];
                for (int i = 0; i < _edgeCells.Length; i++) _edgeCells[i] = visualization.EdgeCells[i].ToArray();
            }

            _edgeCellIdsByEdge = null;
            if (visualization.EdgeCellIdsByEdge != null)
            {
                _edgeCellIdsByEdge = new int[visualization.EdgeCellIdsByEdge.Length][];
                for (int i = 0; i < _edgeCellIdsByEdge.Length; i++) _edgeCellIdsByEdge[i] = visualization.EdgeCellIdsByEdge[i].ToArray();
            }

            _vertexNodeIds = visualization.VertexNodeIds != null ? visualization.VertexNodeIds.ToArray() : null;
        }

    }
}
