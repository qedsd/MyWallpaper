using System;
using System.IO;
using System.Xml.Serialization;

namespace MyWallpaper
{
    public static class XmlSerializerHelper
    {
        /// <summary>
        /// serialize object to xml file.
        /// </summary>
        /// <param name="path">the path to save the xml file</param>
        /// <param name="obj">the object you want to serialize</param>
        public static void SerializeToXml(string path, object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            string content = string.Empty;
            //serialize
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                content = writer.ToString();
            }
            //save to file
            using (StreamWriter stream_writer = new StreamWriter(path))
            {
                stream_writer.Write(content);
            }
        }

        /// <summary>
        /// deserialize xml file to object
        /// </summary>
        /// <param name="path">the path of the xml file</param>
        /// <param name="object_type">the object type you want to deserialize</param>
        public static object DeserializeFromXml(string path, Type object_type)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(object_type);
                object result = null;
                using (StreamReader reader = new StreamReader(path))
                {
                    result= serializer.Deserialize(reader);
                }
                return result;
            }
            catch (Exception er) 
            {
                return null;
            }
        }
    }
}
