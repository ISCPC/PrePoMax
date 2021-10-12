using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    public class MasterSlaveItem
    {
        // Variables                                                                                                                
        private string _masterName;
        private string _slaveName;
        private HashSet<int> _masterGeometryIds;
        private HashSet<int> _slaveGeometryIds;
        private bool _unresolved;


        // Properties                                                                                                               
        public string Name
        {
            get
            {
                if (_unresolved) return _masterName;
                else return _masterName +  Globals.MasterSlaveSeparator + _slaveName;
            }
        }
        public string MasterName { get { return _masterName; } }
        public string SlaveName { get { return _slaveName; } }
        public HashSet<int> MasterGeometryIds { get { return _masterGeometryIds; } set { _masterGeometryIds = value; } }
        public HashSet<int> SlaveGeometryIds { get { return _slaveGeometryIds; } set { _slaveGeometryIds = value; } }
        public bool Unresolved { get { return _unresolved; } set { _unresolved = value; } }
        public string GeometryTypeName
        {
            get
            {
                string name = GetGeometryTypeName(_masterGeometryIds);
                if (name.Length > 0 && _slaveGeometryIds != null && _slaveGeometryIds.Count > 0) name += "-";
                name += GetGeometryTypeName(_slaveGeometryIds);
                return name;
            }
        }


        // Constructors                                                                                                             
        public MasterSlaveItem(string masterName, string slaveName,
                               HashSet<int> masterGeometryIds, HashSet<int> slaveGeometryIds)
        {
            _masterName = masterName;
            _slaveName = slaveName;
            _masterGeometryIds = masterGeometryIds;
            _slaveGeometryIds = slaveGeometryIds;
            _unresolved = false;
        }
        public void SwapMasterSlave()
        {
            string tmpName = _masterName;
            _masterName = _slaveName;
            _slaveName = tmpName;
            //
            HashSet<int> tmpGeometryIds = _masterGeometryIds;
            _masterGeometryIds = _slaveGeometryIds;
            _slaveGeometryIds = tmpGeometryIds;
        }
        public static string GetGeometryTypeName(HashSet<int> geometryIds)
        {
            if (geometryIds == null) return "";
            //
            int typeId;
            string name = "";
            HashSet<int> typeIds = new HashSet<int>();
            //
            foreach (int geometryId in geometryIds) typeIds.Add(FeMesh.GetItemTypePartIdsFromGeometryId(geometryId)[1]);
            //
            if (typeIds.Count == 0) { }
            else if (typeIds.Count == 1)
            {
                typeId = typeIds.First();
                if (typeId == (int)GeometryType.SolidSurface) return "Solid";
                else if (typeId == (int)GeometryType.ShellFrontSurface ||
                         typeId == (int)GeometryType.ShellBackSurface) name += "Shell";
                else if (typeId == (int)GeometryType.ShellEdgeSurface) name += "Edge";
                else throw new NotSupportedException();
            }
            else if (typeIds.Count == 2)
            {
                if (typeIds.Contains((int)GeometryType.ShellFrontSurface) &&
                    typeIds.Contains((int)GeometryType.ShellBackSurface)) name += "Shell";
            }
            else name += "Mixed";
            //
            return name;
        }
    }
}
