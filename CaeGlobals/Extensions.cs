using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;

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

        public static bool ContainsValidKey<T>(this Dictionary<string, T> dictionary, string key)
        {
            T value;
            if (dictionary.TryGetValue(key, out value))
            {
                if (value is NamedClass) return (value as NamedClass).Valid;
                else return true;  // act as ordinary ContainsKey
            }
            else return false;
        }
    }
}
