using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    public class DistanceWidget : WidgetBase
    {
        // Variables                                                                                                                
        private string _text;
        private double[] _anchorPoint;


        // Properties                                                                                                               
        public string Text { get { return _text; } set { _text = value; } }
        public double[] AnchorPoint { get { return _anchorPoint; } set { _anchorPoint = value; } }


        // Constructors                                                                                                             
        public DistanceWidget(string name, string text, double[] anchorPoint)
            : base(name)
        {
            _text = text;
            _anchorPoint = anchorPoint;
        }

    }
}
