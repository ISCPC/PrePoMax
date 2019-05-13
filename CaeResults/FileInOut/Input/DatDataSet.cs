using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeResults
{
    struct DatDataSet
    {
        public double Time;
        public string FieldName;
        public string SetName;
        public string[] ComponentNames;
        /// <summary>
        /// ComponentNames [0 ... num. of values][0...num. of components] -> value
        /// </summary>
        public double[][] Values;
    }
}
