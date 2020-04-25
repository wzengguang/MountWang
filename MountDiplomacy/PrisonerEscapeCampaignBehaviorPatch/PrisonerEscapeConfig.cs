using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Wang
{
    public class PrisonerEscapeConfig
    {

        public static int BaseLine { get; set; } = 1000;

        public static void Init(XmlDocument xmlDocument)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("Config").SelectSingleNode("PrisonerEscape");

            BaseLine = int.Parse(xmlNode.SelectSingleNode("BaseLine").InnerText);

        }
    }
}
