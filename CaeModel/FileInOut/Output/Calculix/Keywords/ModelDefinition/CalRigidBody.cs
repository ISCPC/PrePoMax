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
    internal class CalRigidBody : CalculixKeyword
    {
        // Variables                                                                                                                
        private RigidBody _rigidBody;
        private Dictionary<string, int[]> _referencePointsNodeIds;
        private string _surfaceNodeSetName;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalRigidBody(RigidBody rigidBody, Dictionary<string, int[]> referencePointsNodeIds, string surfaceNodeSetName)
        {
            _rigidBody = rigidBody;
            _referencePointsNodeIds = referencePointsNodeIds;
            _surfaceNodeSetName = surfaceNodeSetName;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            //*RIGID BODY,NSET=rigid1,REF NODE=100,ROT NODE=101
            string nodeSetName;
            if (_rigidBody.RegionType == CaeGlobals.RegionTypeEnum.NodeSetName) nodeSetName = _rigidBody.RegionName;
            else if (_rigidBody.RegionType == CaeGlobals.RegionTypeEnum.SurfaceName) nodeSetName = _surfaceNodeSetName;
            else throw new NotSupportedException();
            //
            return string.Format("*Rigid body, Nset={0}, Ref node={1}, Rot node={2}{3}", nodeSetName,
                                 _referencePointsNodeIds[_rigidBody.ReferencePointName][0],
                                 _referencePointsNodeIds[_rigidBody.ReferencePointName][1],
                                 Environment.NewLine);
        }
        public override string GetDataString()
        {
            return "";
        }
    }
}
