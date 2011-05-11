using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ContactApi
{
    public static class ContactXml
    {
        public static string Serialize (object value)
        {
            var result = new StringBuilder();
            using (var stringWriter = new StringWriter(result))
            {
                var settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                };

                using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    var serializer = new XmlSerializer(value.GetType());
                    serializer.Serialize(xmlWriter, value, ns);
                }//using
            }//using

            return result.ToString();
        }
    }
}
