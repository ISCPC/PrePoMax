using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PrePoMax
{
    public class Globals
    {
        public static string HomePage = "http://lace.fs.uni-mb.si/wordpress/borovinsek/?page_id=41";
        //
        public static string ProgramName = "PrePoMax v1.0.0";
        //
        public static string ReadyText = "Ready";
        public static string OpeningText = "Opening...";
        public static string ImportingText = "Importing...";
        public static string SavingText = "Saving...";
        public static string SavingAsText = "Saving As...";
        public static string ExportingText = "Exporting...";
        public static string MeshingText = "Meshing...";
        public static string UndoingText = "Undoing...";
        public static string CreatingCompound = "Creating compound...";
        public static string RegeneratingText = "Regenerating history...";
        public static string FlippingNormals = "Flipping normals...";
        public static string SplittingFaces = "Splitting faces...";
        //
        public static string SettingsFileName = "settings.bin";
        public static string MaterialLibraryFileName = "materials.lib";
        //
        public static string HistoryFileName = "history";
        // Settings
        public static string GeneralSettingsName = "General";
        public static string GraphicsSettingsName = "Graphics";
        public static string PreSettingsName = "Pre-processing";
        public static string CalculixSettingsName = "Calculix";
        public static string PostSettingsName = "Post-processing";
        public static string LegendSettingsName = "Legend";
        public static string StatusBlockSettingsName = "Status block";
        public static string UnitSystemSettingsName = "Unit system";
        //
        public static string NetGenMesher = @"\NetGen\NetGenMesher.exe";
        public static string MmgMesher = @"\NetGen\mmgs_O3.exe";
        public static string VisFileName = "geometry.vis";
        public static string BrepFileName = "geometry.brep";
        public static string StlFileName = "geometry.stl";        
        public static string MeshParametersFileName = "meshParameters";
        public static string MeshRefinementFileName = "meshRefinement";
        public static string VolFileName = "geometry.vol";
        public static string MmgMeshFileName = "geometry.mesh";
        public static string EdgeNodesFileName = "edgeNodes";
        //
        public static string NameSeparator = ":";
    }
}
