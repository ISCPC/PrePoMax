using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CaeGlobals
{
    [Serializable]
    public abstract class NamedClass
    {
        // Variables                                                                                                                
        protected string _name;
        protected bool _active;
        protected bool _visible;
        protected bool _valid;
        protected bool _internal;
        protected bool _checkName;

        // Properties                                                                                                               
        public virtual string Name
        {
            get { return _name; }
            set
            {
                CheckName(value);
                _name = value;
            }
        }
        public virtual bool Active { get { return _active; } set { _active = value; } }
        public virtual bool Visible { get { return _visible; } set { _visible = value; } }
        public virtual bool Valid { get { return _valid; } set { _valid = value; } }
        public virtual bool Internal { get { return _internal; } set { _internal = value; } }


        // Constructors                                                                                                             
        public NamedClass()
            : this("NoName")
        {
        }
        public NamedClass(string name)
        {
            _checkName = true;
            Name = name;

            _active = true;
            _visible = true;
            _valid = true;
            _internal = false;
        }


        // Static methods
        public static string GetNewValueName(string[] existingNames, string nameRoot)
        {
            int max = 0;
            int tmp;
            string[] parts;
            foreach (var names in existingNames)
            {
                parts = names.Split('-');
                if (parts.Length >= 2 && nameRoot.StartsWith(parts[0]))
                {
                    if (int.TryParse(parts.Last(), out tmp))
                    {
                        if (tmp > max) max = tmp;
                    }
                }
            }
            max++;

            return nameRoot + max.ToString();
        }


        // Methods                                                                                                                  
        private void CheckName(string name)
        {
            if (_checkName)
            {
                if (name == "") throw new CaeGlobals.CaeException("The name can not be an empty string.");
                if (name.Contains(' ')) throw new CaeGlobals.CaeException("The name can not contain space characters.");
                if (Char.IsDigit(name[0])) throw new CaeGlobals.CaeException("The name can not start with a digit.");
                if (name == "Missing") throw new CaeGlobals.CaeException("The name 'Missing' is a reserved name.");

                char c;
                int letterCount = 0;
                int digitCount = 0;

                for (int i = 0; i < name.Length; i++)
                {
                    c = (char)name[i];
                    if (Char.IsLetter(c)) letterCount++;
                    else if (Char.IsDigit(c)) digitCount++;
                    else if (c != '_' && c != '-')
                        throw new CaeGlobals.CaeException("The name can only contain a letter, a digit or characters: minus and underscore.");
                }

                if (letterCount <= 0)
                    throw new CaeGlobals.CaeException("The name must contain at least one letter.");
            }
        }
        public override string ToString()
        {
            return _name;
        }
    }
}
