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
        public static bool GeometryId(this vtkSelectBy selectBy)
        {
            return (selectBy == vtkSelectBy.QuerySurface ||
                    selectBy == vtkSelectBy.QueryEdge ||
                    selectBy == vtkSelectBy.Geometry ||
                    selectBy == vtkSelectBy.GeometryEdgeAngle ||
                    selectBy == vtkSelectBy.GeometrySurfaceAngle);
        }
    }
}
