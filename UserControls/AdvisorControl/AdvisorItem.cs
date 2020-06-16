using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserControls
{
    [Serializable]
    public class AdvisorItem
    {
        // Variables                                                                                                                
        protected string _text;
        protected int _indentLevel;


        // Properties                                                                                                               
        public string Text { get { return _text; } set { _text = value; } }
        public int IndentLevel
        {
            get { return _indentLevel; }
            set
            {
                _indentLevel = value;
                if (_indentLevel < 0) _indentLevel = 0;
            }
        }


        // Constructors                                                                                                             
        public AdvisorItem()
        {
            _text = "";
            _indentLevel = 0;
        }


        // Methods                                                                                                                  
    }
}
