using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using System.Xml.Linq;
using System.Numerics;

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
        private OrderedDictionary<string,  FieldComponent> _components;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public Field(string name)
            : base(name)
        {
            _components = new OrderedDictionary<string, FieldComponent>("Components");
        }
        public Field(Field field)
            : base(field)
        {
            _components = new OrderedDictionary<string, FieldComponent>("Components");
            foreach (var entry in field._components) _components.Add(entry.Key, new FieldComponent(entry.Value));   // copy
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
                    //
                    bw.Write(field._components.Count);
                    foreach (var entry in field._components) FieldComponent.WriteToFile(entry.Value, bw);
                }
                //
                bw.Write(field.Active);
                bw.Write(field.Visible);
                bw.Write(field.Valid);
                bw.Write(field.Internal);
            }
        }
        public static Field ReadFromFile(System.IO.BinaryReader br, int version)
        {
            int numItems;
            FieldComponent component;
            //
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
                    //
                    field._components = new OrderedDictionary<string, FieldComponent>("Components");
                    for (int i = 0; i < numItems; i++)
                    {
                        component = FieldComponent.ReadFromFile(br);
                        field._components.Add(component.Name, component);
                    }
                }
                //
                if (version >= 1_004_000)
                {
                    field.Active = br.ReadBoolean();
                    field.Visible = br.ReadBoolean();
                    field.Valid = br.ReadBoolean();
                    field.Internal = br.ReadBoolean();
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
        public string[] GetComponentNames()
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
            float max = Math.Abs(_components[name].Max.Value);
            float min = Math.Abs(_components[name].Min.Value);
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
        public void ReplaceComponent(string name, FieldComponent newComponent)
        {
            _components.Replace(name, name, newComponent);
        }
        public void RemoveComponent(string name)
        {
            _components.Remove(name);
        }
        //
        public void ComputeInvariants()
        {
            bool isComplex = FOFieldNames.IsComplex(_name);
            ComputeInvariants(isComplex);
        }
        public void ClearInvariants()
        {
            ComputeInvariants(true);
        }
        public void ComputeInvariants(bool setToNaN)
        {
            switch (_name)
            {
                case FOFieldNames.Disp:
                //case FOFieldNames.PDisp:
                case FOFieldNames.Forc:
                //
                case FOFieldNames.Velo:
                // Thermal
                case FOFieldNames.Flux:
                // Sensitivity
                case FOFieldNames.Norm:
                    ComputeVectorFieldInvariantF(setToNaN);
                    break;
                case FOFieldNames.DispI:
                case FOFieldNames.ForcI:
                    ComputeVectorFieldInvariantF(true);  // always set to NaN
                    break;
                case FOFieldNames.Stress:
                //case FOFieldNames.PStress:
                case FOFieldNames.ToStrain:
                case FOFieldNames.MeStrain:
                // Error
                case FOFieldNames.ZZStr:
                    ComputeTensorFieldInvariantF(setToNaN);
                    break;
                case FOFieldNames.StressI:
                case FOFieldNames.ToStraiI:
                case FOFieldNames.MeStraiI:
                case FOFieldNames.ZZStrI:
                    ComputeTensorFieldInvariantF(true);  // always set to NaN
                    break;
                default:
                    break;
            }
        }
        private void ComputeVectorFieldInvariant(bool setToNaN)
        {
            _components.Remove(FOComponentNames.All);
            if (_components.Count != 3) throw new NotSupportedException();
            //
            if (setToNaN) return;
            //
            int count = 0;
            float[][] values = new float[3][];
            foreach (var entry in _components)
            {
                values[count++] = entry.Value.Values;
            }
            //
            
            float[] magnitude = new float[values[0].Length];
            for (int i = 0; i < magnitude.Length; i++)
            {
                magnitude[i] = (float)Math.Sqrt(Math.Pow(values[0][i], 2) +
                                                Math.Pow(values[1][i], 2) +
                                                Math.Pow(values[2][i], 2));
                    
            }
            _components.Insert(0, FOComponentNames.All, new FieldComponent(FOComponentNames.All, magnitude, true));
        }
        private void ComputeVectorFieldInvariantF(bool setToNaN)
        {
            _components.Remove(FOComponentNames.All);
            if (_components.Count != 3) throw new NotSupportedException();
            //
            if (setToNaN) return;
            //
            int count = 0;
            float[][] values = new float[3][];
            foreach (var entry in _components)
            {
                values[count++] = entry.Value.Values;
            }
            //
            float[] magnitude = new float[values[0].Length];
            for (int i = 0; i < magnitude.Length; i++)
            {
                magnitude[i] = (float)Math.Sqrt(values[0][i] * values[0][i] +
                                                values[1][i] * values[1][i] +
                                                values[2][i] * values[2][i]);

            }
            _components.Insert(0, FOComponentNames.All, new FieldComponent(FOComponentNames.All, magnitude, true));
        }
        private void ComputeTensorFieldInvariant(bool setToNaN)
        {
            _components.Remove(FOComponentNames.Mises);
            _components.Remove(FOComponentNames.Tresca);
            _components.Remove(FOComponentNames.SgnMaxAbsPri);
            _components.Remove(FOComponentNames.PrincipalMax);
            _components.Remove(FOComponentNames.PrincipalMid);
            _components.Remove(FOComponentNames.PrincipalMin);
            if (_components.Count != 6) throw new NotSupportedException();
            //
            if (setToNaN) return;

            int count = 0;
            float[][] values = new float[6][];
            foreach (var entry in _components)
            {
                values[count++] = entry.Value.Values;
            }
            //
            float[] vonMises = new float[values[0].Length];
            for (int i = 0; i < vonMises.Length; i++)
            {
                vonMises[i] = (float)Math.Sqrt(0.5f * (Math.Pow(values[0][i] - values[1][i], 2) +
                                                       Math.Pow(values[1][i] - values[2][i], 2) +
                                                       Math.Pow(values[2][i] - values[0][i], 2) +
                                                       6 * (Math.Pow(values[3][i], 2) +
                                                            Math.Pow(values[4][i], 2) +
                                                            Math.Pow(values[5][i], 2))));
            }
            // Mises
            _components.Insert(0, FOComponentNames.Mises, new FieldComponent(FOComponentNames.Mises, vonMises, true));
            // Principal
            ComputeAndAddPrincipalInvariants(values);
            // Tresca
            float[] tresca = new float[values[0].Length];
            float[] eMax = GetComponentValues(FOComponentNames.PrincipalMax);
            float[] eMin = GetComponentValues(FOComponentNames.PrincipalMin);
            for (int i = 0; i < tresca.Length; i++) tresca[i] = eMax[i] - eMin[i];
            //
            _components.Insert(1, FOComponentNames.Tresca, new FieldComponent(FOComponentNames.Tresca, tresca, true));
        }
        private void ComputeTensorFieldInvariantF(bool setToNaN)
        {
            _components.Remove(FOComponentNames.Mises);
            _components.Remove(FOComponentNames.Tresca);
            _components.Remove(FOComponentNames.SgnMaxAbsPri);
            _components.Remove(FOComponentNames.PrincipalMax);
            _components.Remove(FOComponentNames.PrincipalMid);
            _components.Remove(FOComponentNames.PrincipalMin);
            if (_components.Count != 6) throw new NotSupportedException();
            //
            if (setToNaN) return;

            int count = 0;
            float[][] values = new float[6][];
            foreach (var entry in _components)
            {
                values[count++] = entry.Value.Values;
            }
            //
            float[] vonMises = new float[values[0].Length];
            float a, b, c;
            for (int i = 0; i < vonMises.Length; i++)
            {
                a = values[0][i] - values[1][i];
                b = values[1][i] - values[2][i];
                c = values[2][i] - values[0][i];
                vonMises[i] = (float)Math.Sqrt(0.5f * (a * a + b * b + c * c +
                              6 * (values[3][i] * values[3][i] + values[4][i] * values[4][i] + values[5][i] * values[5][i])));
            }
            // Mises
            _components.Insert(0, FOComponentNames.Mises, new FieldComponent(FOComponentNames.Mises, vonMises, true));
            // Principal
            ComputeAndAddPrincipalInvariantsF(values);
            // Tresca
            float[] tresca = new float[values[0].Length];
            float[] eMax = GetComponentValues(FOComponentNames.PrincipalMax);
            float[] eMin = GetComponentValues(FOComponentNames.PrincipalMin);
            for (int i = 0; i < tresca.Length; i++) tresca[i] = eMax[i] - eMin[i];
            //
            _components.Insert(1, FOComponentNames.Tresca, new FieldComponent(FOComponentNames.Tresca, tresca, true));
        }
        private void ComputeAndAddPrincipalInvariants(float[][] values)
        {
            // https://en.wikipedia.org/wiki/Cubic_function#General_solution_to_the_cubic_equation_with_arbitrary_coefficients
            // https://en.wikiversity.org/wiki/Principal_stresses
            //
            float[] s0 = new float[values[0].Length];
            float[] s1 = new float[values[0].Length];
            float[] s2 = new float[values[0].Length];
            float[] s3 = new float[values[0].Length];
            //
            float s11;
            float s22;
            float s33;
            float s12;
            float s23;
            float s31;
            //
            double I1;
            double I2;
            double I3;
            //
            double sp1, sp2, sp3;
            sp1 = sp2 = sp3 = 0;
            //
            for (int i = 0; i < s1.Length; i++)
            {
                s11 = values[0][i];
                s22 = values[1][i];
                s33 = values[2][i];
                s12 = values[3][i];
                s23 = values[4][i];
                s31 = values[5][i];
                //
                I1 = s11 + s22 + s33;
                I2 = s11 * s22 + s22 * s33 + s33 * s11 - Math.Pow(s12, 2.0) - Math.Pow(s23, 2.0) - Math.Pow(s31, 2.0);
                I3 = s11 * s22 * s33 - s11 * Math.Pow(s23, 2.0) - s22 * Math.Pow(s31, 2.0) - s33 * Math.Pow(s12, 2.0) +
                        2.0 * s12 * s23 * s31;
                //
                Tools.SolveQubicEquationDepressedCubic(1.0, -I1, I2, -I3, ref sp1, ref sp2, ref sp3);
                Tools.Sort3_descending(ref sp1, ref sp2, ref sp3);
                //
                s0[i] = Math.Abs(sp1) > Math.Abs(sp3) ? (float)sp1 : (float)sp3;
                s1[i] = (float)sp1;
                s2[i] = (float)sp2;
                s3[i] = (float)sp3;
                //
                if (float.IsNaN(s0[i])) s0[i] = 0;
                if (float.IsNaN(s1[i])) s1[i] = 0;
                if (float.IsNaN(s2[i])) s2[i] = 0;
                if (float.IsNaN(s3[i])) s3[i] = 0;
            }
            //
            AddComponent(FOComponentNames.SgnMaxAbsPri, s0, true);
            AddComponent(FOComponentNames.PrincipalMax, s1, true);
            AddComponent(FOComponentNames.PrincipalMid, s2, true);
            AddComponent(FOComponentNames.PrincipalMin, s3, true);
        }
        private void ComputeAndAddPrincipalInvariantsF(float[][] values)
        {
            // https://en.wikipedia.org/wiki/Cubic_function#General_solution_to_the_cubic_equation_with_arbitrary_coefficients
            // https://en.wikiversity.org/wiki/Principal_stresses
            //
            float[] s0 = new float[values[0].Length];
            float[] s1 = new float[values[0].Length];
            float[] s2 = new float[values[0].Length];
            float[] s3 = new float[values[0].Length];
            //
            float s11;
            float s22;
            float s33;
            float s12;
            float s23;
            float s31;
            //
            float I1;
            float I2;
            float I3;
            //
            float sp1, sp2, sp3;
            sp1 = sp2 = sp3 = 0;
            //
            for (int i = 0; i < s1.Length; i++)
            {
                s11 = values[0][i];
                s22 = values[1][i];
                s33 = values[2][i];
                s12 = values[3][i];
                s23 = values[4][i];
                s31 = values[5][i];
                //
                I1 = s11 + s22 + s33;
                I2 = s11 * s22 + s22 * s33 + s33 * s11 - s12 * s12 - s23 * s23 - s31 * s31;
                I3 = s11 * s22 * s33 - s11 * s23 * s23 - s22 * s31 * s31 - s33 * s12 * s12 + 2f * s12 * s23 * s31;
                //
                Tools.SolveQubicEquationDepressedCubicF(1f, -I1, I2, -I3, ref sp1, ref sp2, ref sp3);
                Tools.Sort3_descending(ref sp1, ref sp2, ref sp3);
                //
                s0[i] = Math.Abs(sp1) > Math.Abs(sp3) ? sp1 : sp3;
                s1[i] = sp1;
                s2[i] = sp2;
                s3[i] = sp3;
                //
                if (float.IsNaN(s0[i])) s0[i] = 0;
                if (float.IsNaN(s1[i])) s1[i] = 0;
                if (float.IsNaN(s2[i])) s2[i] = 0;
                if (float.IsNaN(s3[i])) s3[i] = 0;
            }
            //
            AddComponent(FOComponentNames.SgnMaxAbsPri, s0, true);
            AddComponent(FOComponentNames.PrincipalMax, s1, true);
            AddComponent(FOComponentNames.PrincipalMid, s2, true);
            AddComponent(FOComponentNames.PrincipalMin, s3, true);
        }
    }
}
