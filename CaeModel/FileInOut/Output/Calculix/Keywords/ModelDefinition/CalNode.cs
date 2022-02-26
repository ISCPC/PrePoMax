using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalNode : CalculixKeyword
    {
        // Variables                                                                                                                
        private IDictionary<string, FeReferencePoint> _referencePoints;
        private IDictionary<int, FeNode> _nodes;
        private ModelSpaceEnum _modelSpace;
        private IDictionary<string, int[]> _referencePointsNodeIds;
        private Dictionary<int, double[]> _deformations;


        // Constructor                                                                                                              
        public CalNode(FeModel model, IDictionary<string, int[]> referencePointsNodeIds,
                       Dictionary<int, double[]> deformations)
        {
            _referencePoints = model.Mesh.ReferencePoints;
            _nodes = model.Mesh.Nodes;
            _modelSpace = model.Properties.ModelSpace;
            _referencePointsNodeIds = referencePointsNodeIds;
            _deformations = deformations;
        }


        // Properties                                                                                                               


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Node{0}", Environment.NewLine);
        }
        public override string GetDataString()
        {
            FeNode node;
            double[] def;
            StringBuilder sb = new StringBuilder();
            List<int> sortedIds = _nodes.Keys.ToList();
            sortedIds.Sort();
            foreach (int nodeId in sortedIds)
            {
                node = _nodes[nodeId];
                //
                if (_deformations != null && _deformations.TryGetValue(nodeId, out def))
                {
                    node.X += def[0];
                    node.Y += def[1];
                    node.Z += def[2];
                }
                //
                if (_modelSpace == ModelSpaceEnum.ThreeD)
                    sb.AppendFormat("{0}, {1:E8}, {2:E8}, {3:E8}", node.Id, node.X, node.Y, node.Z).AppendLine();
                else if (_modelSpace.IsTwoD())
                    sb.AppendFormat("{0}, {1:E8}, {2:E8}", node.Id, node.X, node.Y).AppendLine();
                else throw new NotSupportedException();
            }
            //
            FeReferencePoint rp;
            foreach (var entry in _referencePointsNodeIds)
            {
                if (_modelSpace == ModelSpaceEnum.ThreeD)
                {
                    if (_referencePoints.TryGetValue(entry.Key, out rp))
                    {
                        sb.AppendFormat("{0}, {1:E8}, {2:E8}, {3:E8}", entry.Value[0], rp.X, rp.Y, rp.Z).AppendLine();
                        sb.AppendFormat("{0}, {1:E8}, {2:E8}, {3:E8}", entry.Value[1], rp.X, rp.Y, rp.Z).AppendLine();
                    }
                    else
                    {
                        sb.AppendFormat("{0}, {1:E8}, {2:E8}, {3:E8}", entry.Value[0], 0, 0, 0).AppendLine();
                    }
                }
                else if (_modelSpace.IsTwoD())
                {
                    if (_referencePoints.TryGetValue(entry.Key, out rp))
                    {
                        sb.AppendFormat("{0}, {1:E8}, {2:E8}", entry.Value[0], rp.X, rp.Y).AppendLine();
                        sb.AppendFormat("{0}, {1:E8}, {2:E8}", entry.Value[1], rp.X, rp.Y).AppendLine();
                    }
                    else
                    {
                        sb.AppendFormat("{0}, {1:E8}, {2:E8}", entry.Value[0], 0, 0).AppendLine();
                    }
                }
                else throw new NotSupportedException();
            }
            return sb.ToString();
        }
    }
}
