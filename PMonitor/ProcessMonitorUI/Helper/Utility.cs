using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProcessMonitorUI.Helper
{
    public static class Utility
    {        

        public static void XMLSerialize(string filename, object content)
        {            
            XmlSerializer ser = new XmlSerializer(content.GetType());
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, content);
            writer.Close();
        }

        public static object XMLDeserialze(string filename, Type type)
        {
            XmlSerializer ser = new XmlSerializer(type);
            FileStream reader = new FileStream(filename, FileMode.Open);
            object target = ser.Deserialize(reader);
            reader.Close();
            return target;
        }
    }
}
