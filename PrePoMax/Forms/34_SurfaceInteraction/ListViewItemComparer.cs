using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrePoMax.Forms
{ 
    class ListViewItemComparer : System.Collections.IComparer
    {
        //  Variables                                                                                                               
        public Dictionary<string, int> namePosition = new Dictionary<string, int>();        
        private int col;


        //  Constructors                                                                                                            
        public ListViewItemComparer()
            :this (0)
        {
        }
        public ListViewItemComparer(int column)            
        {
            col = column;
            //
            namePosition.Add("Surface behavior", 0);
            namePosition.Add("Friction", 1);
            namePosition.Add("Gap conductance", 2);
        }
        public int Compare(object x, object y)
        {
            int xPos = namePosition[((ListViewItem)x).Text];
            int yPos = namePosition[((ListViewItem)y).Text];
            //
            if (xPos > yPos) return 1;
            else if (xPos < yPos) return -1;
            return 0;
        }
    }
}
