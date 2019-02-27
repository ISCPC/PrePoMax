using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CaeGlobals;

namespace PrePoMax.Commands
{
    [Serializable]
    public abstract class Command
    {
        // Variables                                                                                                                
        private string _name;
        private DateTime _dateCreated;


        // Properties                                                                                                               
        public string Name { get { return _name; } }


        // Callbacks
        //public Action CommnadFinished;


        // Constructor                                                                                                              
        public Command(string name)
        {
            _name = name;
            _dateCreated = DateTime.Now;
        }


        // Methods                                                                                                                  
        public virtual bool Execute(Controller receiver)
        {
            //if (CommnadFinished != null) CommnadFinished();
            return true;
        }

       
        public virtual string GetCommandString()
        {
            return _dateCreated.ToString("MM/dd/yy HH:mm:ss") + "   " + _name + ": ";
        }


        protected string GetArrayAsString(string[] array)
        {
            string names = "[";
            int count = 0;
            foreach (string name in array)
            {
                names += name;
                if (++count < array.Length) names += ", ";
            }
            names += "]";

            return names;
        }


    }
}
