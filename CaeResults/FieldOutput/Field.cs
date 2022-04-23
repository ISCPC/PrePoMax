using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace CaeResults
{
    [Serializable]
    public struct IDValuePair
    {
        public int Id;
        public float Value;
    }

    [Serializable]
    public class Field : NamedClass
    {
        // Variables                                                                                                                
        private Dictionary<string,  FieldComponent> _components;


        // Constructor                                                                                                              
        public Field(string name)
            : base(name)
        {
            _components = new Dictionary<string, FieldComponent>();
        }


        // Static methods                                                                                                           
        public static void WriteToFile(Field field, System.IO.BinaryWriter bw)
        {
            if (field == null)
            {
                bw.Write((int)0);
            }
            else
            {
                bw.Write((int)1);
                
                // Name
                bw.Write(field._name);

                // Components
                if (field._components == null) bw.Write((int)0);
                else
                {
                    bw.Write((int)1);

                    bw.Write(field._components.Count);
                    foreach (var entry in field._components)
                        FieldComponent.WriteToFile(entry.Value, bw);
                }
            }
        }
        public static Field ReadFromFile(System.IO.BinaryReader br)
        {
            int numItems;
            FieldComponent component;

            int exists = br.ReadInt32();
            if (exists == 1)
            {
                string name = br.ReadString();
                Field field = new Field(name);

                // Components
                exists = br.ReadInt32();
                if (exists == 1)
                {
                    numItems = br.ReadInt32();

                    field._components = new Dictionary<string, FieldComponent>();
                    for (int i = 0; i < numItems; i++)
                    {
                        component = FieldComponent.ReadFromFile(br);
                        field._components.Add(component.Name, component);
                    }
                }

                return field;
            }
            return null;
        }


        // Methods                                                                                                                  
        public void AddComponent(FieldComponent component)
        {
            _components.Add(component.Name, component);
        }
        public void AddComponent(string name, float[] values, bool invariant = false)
        {
            _components.Add(name, new FieldComponent(name, values, invariant));
        }
        public string[] GetCmponentNames()
        {
            return _components.Keys.ToArray();
        }
        public bool ContainsComponent(string name)
        {
            return _components.ContainsKey(name);
        }
        public bool IsComponentInvariant(string name)
        {
            return _components[name].Invariant;
        }
        public float[] GetComponentValues(string name)
        {
            if (_components.ContainsKey(name)) return _components[name].Values;
            else return null;
        }
        public float GetComponentAbsMax(string name)
        {
            float max = _components[name].Max.Value;
            float min = _components[name].Min.Value;
            return Math.Max(min, max);
        }
        public float GetComponentMax(string name)
        {
            return _components[name].Max.Value;
        }
        public int GetComponentMaxId(string name)
        {
            return _components[name].Max.Id;
        }
        public float GetComponentMin(string name)
        {
            return _components[name].Min.Value;
        }
        public int GetComponentMinId(string name)
        {
            return _components[name].Min.Id;
        }
        public void RemoveComponent(string name)
        {
            _components.Remove(name);
        }
    }
}
