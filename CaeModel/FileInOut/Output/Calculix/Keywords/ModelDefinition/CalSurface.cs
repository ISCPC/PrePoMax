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
        public override object GetBase { get { return _surface; } }


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
                foreach (var elementSetEntry in _surface.ElementFaces)
                {
                    sb.AppendFormat("{0}, {1}", elementSetEntry.Value, elementSetEntry.Key).AppendLine();
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
