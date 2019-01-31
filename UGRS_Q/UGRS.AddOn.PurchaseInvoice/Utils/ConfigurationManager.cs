using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace ContabilidadElectronicaAddOn.Utils
{
    public class ConfigurationManager
    {
        public static string ObtenerConfiguracion(string key)
        {
            string app = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            string file = app + "\\Config.xml";

            file = file.Replace("file:\\", "");
            string res = "";

            if (File.Exists(file))
            {
                XDocument xdoc = XDocument.Load(file);
                var nodes = xdoc.Root.Element("appSettings").Elements("add");
                foreach (var node in nodes)
                {

                    if (node.Attribute("key").Value == key)
                        res = node.Attribute("value").Value;
                }
            }
            return res;
        }
    }
}
