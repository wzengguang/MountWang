using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Wang
{
    public class TroopCountLimitConfig
    {

        public static int HideoutLimit { get; set; } = 50;

        public static void Init(XmlDocument xmlDocument)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("Config").SelectSingleNode("TroopCountLimit");

            HideoutLimit = int.Parse(xmlNode.SelectSingleNode("HideoutLimit").InnerText);

        }
    }
}
