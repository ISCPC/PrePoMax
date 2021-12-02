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


        // Constructor                                                                                                              
        public CalNode(FeModel model, IDictionary<string, int[]> referencePointsNodeIds)
        {
            _referencePoints = model.Mesh.ReferencePoints;
            _nodes = model.Mesh.Nodes;
            _modelSpace = model.Properties.ModelSpace;
            _referencePointsNodeIds = referencePointsNodeIds;
        }


        // Properties                                                                                                               


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Node{0}", Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            FeNode node;
            List<int> idsSorted = _nodes.Keys.ToList();
            idsSorted.Sort();
            foreach (int nodeId in idsSorted)
            {
                node = _nodes[nodeId];
                if (_modelSpace == ModelSpaceEnum.Three_D)
                    sb.AppendFormat("{0}, {1:E8}, {2:E8}, {3:E8}", node.Id, node.X, node.Y, node.Z).AppendLine();
                else if (_modelSpace == ModelSpaceEnum.Two_D)
                    sb.AppendFormat("{0}, {1:E8}, {2:E8}", node.Id, node.X, node.Y).AppendLine();
            }
            //
            FeReferencePoint rp;
            foreach (var entry in _referencePointsNodeIds)
            {
                if (_modelSpace == ModelSpaceEnum.Three_D)
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
                else if (_modelSpace == ModelSpaceEnum.Two_D)
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
            }
            return sb.ToString();
        }
    }
}
