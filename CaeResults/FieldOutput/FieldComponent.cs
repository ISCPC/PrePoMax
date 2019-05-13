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
    public class FieldComponent : NamedClass
    {
        // Variables                                                                                                                
        public float[] Values;
        public bool Invariant;
        public IDValuePair Max;
        public IDValuePair Min;

        // Constructor                                                                                                              
        public FieldComponent(string name, float[] values, bool invariant = false)
            : base()
        {
            _checkName = false;
            Name = name;

            Values = values;

            Invariant = invariant;

            Max = new IDValuePair { Id = 0, Value = values[0] };
            Min = new IDValuePair { Id = 0, Value = values[0] };

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > Max.Value)
                {
                    Max.Value = values[i];
                    Max.Id = i;
                }
                else if (values[i] < Min.Value)
                {
                    Min.Value = values[i];
                    Min.Id = i;
                }
            }
        }

        
        // Static methods                                                                                                           
        public static void WriteToFile(FieldComponent component, System.IO.BinaryWriter bw)
        {
            // values
            bw.Write(component.Name);
            bw.Write(component.Invariant);
            bw.Write(component.Values.Length);
            for (int i = 0; i < component.Values.Length; i++)
                bw.Write(component.Values[i]);
        }
        public static FieldComponent ReadFromFile(System.IO.BinaryReader br)
        {
            string name = br.ReadString();
            bool invariant = br.ReadBoolean();
            int numValues = br.ReadInt32();
            float[] values = new float[numValues];
            for (int i = 0; i < numValues; i++)
                values[i] = br.ReadSingle();

            return new FieldComponent(name, values, invariant);
        }


    }
}
