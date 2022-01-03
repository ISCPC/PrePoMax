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
    internal class CalSurface : CalculixKeyword
    {
        // Variables                                                                                                                
        private FeSurface _surface;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalSurface(FeSurface surface)
        {
            _surface = surface;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Surface, Name={0}, Type={1}{2}", _surface.Name, _surface.Type, Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            if (_surface.Type == FeSurfaceType.Element)
            {
                string key = "";
                foreach (var elementSetEntry in _surface.ElementFaces)
                {
                    if (elementSetEntry.Key == FeFaceName.S1) key = "N";
                    if (elementSetEntry.Key == FeFaceName.S2) key = "P";
                    if (elementSetEntry.Key == FeFaceName.S3) key = "S1";
                    if (elementSetEntry.Key == FeFaceName.S4) key = "S2";
                    if (elementSetEntry.Key == FeFaceName.S5) key = "S3";
                    if (elementSetEntry.Key == FeFaceName.S6) key = "S4";
                    sb.AppendFormat("{0}, {1}", elementSetEntry.Value, key).AppendLine();
                    //sb.AppendFormat("{0}, {1}", elementSetEntry.Value, elementSetEntry.Key).AppendLine();
                }
            }
            else if (_surface.Type == FeSurfaceType.Node)
            {
                sb.AppendFormat("{0}", _surface.NodeSetName).AppendLine();
            }
            else throw new NotImplementedException();
            return sb.ToString();
        }
    }
}
