using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeResults
{
    [Serializable]
    public class ResultsCollection
    {
        // Variables                                                                                                                
        private FeResults _currentResult;
        private Dictionary<string, FeResults> _results;


        // Properties                                                                                                               
        public int Count { get { return _results.Count; } }
        public FeResults CurrentResult { get { return _currentResult; } }


        //  Constructors                                                                                                            
        public ResultsCollection()
        {
            Clear();
        }


        // Metods
        public static void WriteToFile(ResultsCollection allResults, System.IO.BinaryWriter bw)
        {
            bw.Write(allResults.Count);
            //
            foreach (var entry in allResults._results)
            {
                bw.Write(entry.Key);
                FeResults.WriteToFile(entry.Value, bw);
            }
        }
        public static void ReadFromFile(ResultsCollection allResults, System.IO.BinaryReader br, int version)
        {
            string resultsName;
            int numResults = br.ReadInt32();
            //
            if (numResults > 0)
            {
                resultsName = br.ReadString();
                allResults.SetCurrentResult(resultsName);
                if (allResults.CurrentResult != null) FeResults.ReadFromFile(allResults.CurrentResult, br, version);
            }
        }
        public void Clear()
        {
            _currentResult = null;
            _results = new Dictionary<string, FeResults>();
        }
        public bool Add(string name, FeResults result)
        {
            if (_results.ContainsKey(name))
            {
                throw new NotSupportedException();
                //return false;
            }
            else
            {
                _results.Add(name, result);
                _currentResult = result;
                return true;
            }
        }
        public void Remove(string name)
        {
            _results.Remove(name);
        }
        public void SetCurrentResult(string name)
        {
            _results.TryGetValue(name, out _currentResult);
        }
        
        
    }
}
