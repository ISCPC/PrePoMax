using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace CaeGlobals
{
    public static class ExtensionMethods
    {
        // Faster serialization
        // https://github.com/tomba/netserializer/blob/master/Doc.md
        //

        // Deep clone
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        // Save clone to File
        public static void DumpToFile<T>(this T a, string fileName)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;

                FileStream fs = new FileStream(fileName, FileMode.Create);

                stream.WriteTo(fs);
                fs.Close();
            }
        }
        public static void DumpToStream<T>(this T a, BinaryWriter bw)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;

                long length = stream.Length;
                bw.Write(length);
                //stream.WriteTo(bw.BaseStream);
                stream.CopyTo(bw.BaseStream);
            }
        }
        public static string SerializeToXML<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
        public static T GetNewObject<T>()
        {
            try
            {
                return (T)typeof(T).GetConstructor(new Type[] { }).Invoke(new object[] { });
            }
            catch
            {
                return default(T);
            }
        }

        // Dictionary
        public static bool ContainsValidKey<T>(this IDictionary<string, T> dictionary, string key)
        {
            T value;
            if (dictionary.TryGetValue(key, out value))
            {
                if (value is NamedClass) return (value as NamedClass).Valid;
                else return true;  // act as ordinary ContainsKey
            }
            else return false;
        }
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
        {
            foreach (var item in dicToAdd) dic.Add(item.Key, item.Value);
        }
        public static string GetNextNumberedKey<T>(this IDictionary<string, T> dictionary, string keyBase)
        {
            int n = 0;
            bool contains = true;
            while (contains)
            {
                n++;
                contains = false;
                foreach (var entry in dictionary)
                {
                    if (entry.Key.StartsWith(keyBase + "-" + n + "_"))
                    {
                        contains = true;
                        break;
                    }
                }
            }
            return keyBase + "-" + n + "_";
        }

        // Property grid items
        public static IEnumerable<GridItem> EnumerateAllItems(this PropertyGrid grid)
        {
            if (grid == null)
                yield break;

            // get to root item
            GridItem start = grid.SelectedGridItem;
            while (start.Parent != null)
            {
                start = start.Parent;
            }

            foreach (GridItem item in start.EnumerateAllItems())
            {
                yield return item;
            }
        }
        public static IEnumerable<GridItem> EnumerateAllItems(this GridItem item)
        {
            if (item == null)
                yield break;

            yield return item;
            foreach (GridItem child in item.GridItems)
            {
                foreach (GridItem gc in child.EnumerateAllItems())
                {
                    yield return gc;
                }
            }
        }

        // String Array
        public static string ToUTF8(this string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Encoding.Default.GetString(bytes);
        }
        public static string ToUnicode(this string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            return Encoding.Default.GetString(bytes);
        }
        public static string ToASCII(this string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            return Encoding.Default.GetString(bytes);
        }

        public static string ToShortString(this string[] stringArray)
        {
            string allNames = null;
            if (stringArray != null)
            {
                if (stringArray.Length >= 1) allNames = stringArray[0];
                if (stringArray.Length >= 2) allNames += ", ...";
            }
            return allNames;
        }

        // vtkSelectBy
        public static bool IsGeometryBased(this vtkSelectBy selectBy)
        {
            return (selectBy == vtkSelectBy.QuerySurface ||
                    selectBy == vtkSelectBy.QueryEdge ||
                    selectBy == vtkSelectBy.Geometry ||
                    selectBy == vtkSelectBy.GeometryEdgeAngle ||
                    selectBy == vtkSelectBy.GeometrySurfaceAngle);
        }

        // String[]
        public static string ToRows(this string[] names)
        {
            string rows = "";
            for (int i = 0; i < names.Length; i++)
            {
                rows += names[i];
                if (i < names.Length - 1) rows += Environment.NewLine;
            }
            return rows;
        }
        //
        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }
    }
}
