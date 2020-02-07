using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.Serialization;


namespace CaeGlobals
{
    [Serializable]
    public abstract class NamedClass //: ISerializable - this would mean that all derived classes must be Serializable !!!
    {
        // Variables                                                                                                                
        protected string _name;             //ISerializable
        protected bool _active;             //ISerializable
        protected bool _visible;            //ISerializable
        protected bool _valid;              //ISerializable
        protected bool _internal;           //ISerializable
        protected bool _checkName;          //ISerializable


        // Properties                                                                                                               
        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (_checkName) CheckNameForErrors(value);
                _name = value;
            }
        }
        public virtual bool Active { get { return _active; } set { _active = value; } }
        public virtual bool Visible { get { return _visible; } set { _visible = value; } }
        public virtual bool Valid { get { return _valid; }
            set { _valid = value; } }
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
        public NamedClass(SerializationInfo info, StreamingContext context)
        {
            int count = 0;
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_name":
                        _name = (string)entry.Value; count++; break;
                    case "_active":
                        _active = (bool)entry.Value; count++; break;
                    case "_visible":
                        _visible = (bool)entry.Value; count++; break;
                    case "_valid":
                        _valid = (bool)entry.Value; count++; break;
                    case "_internal":
                        _internal = (bool)entry.Value; count++; break;
                    case "_checkName":
                        _checkName = (bool)entry.Value; count++; break;
                }
            }
            if (count != 6) throw new NotSupportedException();
        }


        // Static methods
        public static string GetNewValueName(ICollection<string> existingNames, string nameRoot)
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

        public static string GetNameWithoutLastValue(string name)
        {
            int tmp;
            string[] parts;
            parts = name.Split('-');
            int numOfParts = parts.Length;
            //
            if (int.TryParse(parts.Last(), out tmp)) numOfParts--;
            //
            string newName = "";
            for (int i = 0; i < numOfParts; i++)
            {
                newName += parts[i] + "-";
            }
            return newName;
        }


        // Methods                                                                                                                  
        public static bool CheckName(string name)
        {
            try
            {
                CheckNameForErrors(name);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static string CheckNameError(string name)
        {
            try
            {
                CheckNameForErrors(name);
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private static void CheckNameForErrors(string name)
        {
            if (name == null) throw new CaeGlobals.CaeException("The name can not be null.");
            if (name == "") throw new CaeGlobals.CaeException("The name can not be an empty string.");
            if (name.Contains(' ')) throw new CaeGlobals.CaeException("The name can not contain space characters: '" + name + "'.");
            if (Char.IsDigit(name[0])) throw new CaeGlobals.CaeException("The name can not start with a digit: '" + name + "'.");
            if (name == "Missing") throw new CaeGlobals.CaeException("The name 'Missing' is a reserved name.");

            char c;
            int letterCount = 0;
            int digitCount = 0;

            for (int i = 0; i < name.Length; i++)
            {
                c = (char)name[i];
                if (Char.IsLetter(c)) letterCount++;
                else if (Char.IsDigit(c)) digitCount++;
                else if (c != '_' && c != '-' && c != '(' && c != ')')
                    throw new CaeGlobals.CaeException("The name can only contain a letter, a digit or characters: minus, underscore and parenthesis: '" + name + "'.");
            }

            if (letterCount <= 0)
                throw new CaeGlobals.CaeException("The name must contain at least one letter: '" + name + "'.");
        }
        //
        public override string ToString()
        {
            return _name;
        }

        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // using typeof() works also for null fields
            info.AddValue("_name", _name, typeof(string));
            info.AddValue("_active", _active, typeof(bool));
            info.AddValue("_visible", _visible, typeof(bool));
            info.AddValue("_valid", _valid, typeof(bool));
            info.AddValue("_internal", _internal, typeof(bool));
            info.AddValue("_checkName", _checkName, typeof(bool));
        }
    }
}
