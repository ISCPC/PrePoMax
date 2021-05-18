using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    public abstract class CalculixKeyword
    {
        // Variables                                                                                                                
        protected List<CalculixKeyword> _keywords;


        // Properties                                                                                                               
        public List<CalculixKeyword> Keywords { get { return _keywords; } }


        // Constructor                                                                                                              
        public CalculixKeyword()
        {
            _keywords = new List<CalculixKeyword>();
        }


        // Methods                                                                                                                  
        public void AddKeyword(CalculixKeyword keyword)
        {
            _keywords.Add(keyword);
        }
        public void ClearKeywords()
        {
            _keywords.Clear();
        }
        public abstract string GetKeywordString();
        public abstract string GetDataString();

        
        
    }
}
