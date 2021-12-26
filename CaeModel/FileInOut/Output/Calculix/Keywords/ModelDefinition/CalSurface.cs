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
        private bool _twoD;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalSurface(FeSurface surface, bool twoD)
        {
            _surface = surface;
            _twoD = twoD;
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
                string faceKey = "";
                foreach (var elementSetEntry in _surface.ElementFaces)
                {
                    if (_twoD)
                    {
                        if (elementSetEntry.Key == FeFaceName.S1) faceKey = "N";
                        else if (elementSetEntry.Key == FeFaceName.S2) faceKey = "P";
                        else if (elementSetEntry.Key == FeFaceName.S3) faceKey = "S1";
                        else if (elementSetEntry.Key == FeFaceName.S4) faceKey = "S2";
                        else if (elementSetEntry.Key == FeFaceName.S5) faceKey = "S3";
                        else if (elementSetEntry.Key == FeFaceName.S6) faceKey = "S4";
                    }
                    else
                    {
                        faceKey = elementSetEntry.Key.ToString();
                    }
                    sb.AppendFormat("{0}, {1}", elementSetEntry.Value, faceKey).AppendLine();
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
