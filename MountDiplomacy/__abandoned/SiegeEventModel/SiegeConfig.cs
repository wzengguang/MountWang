using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Wang
{
    public class SiegeConfig
    {
        public static float ConstructionProgressPerHourMutiplier { get; set; } = 3f;

        public static void Init(XmlDocument xmlDocument)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("Config").SelectSingleNode("Siege");

            ConstructionProgressPerHourMutiplier = float.Parse(xmlNode.SelectSingleNode("ConstructionProgressPerHourMutiplier").InnerText);

            if (ConstructionProgressPerHourMutiplier < 0)
            {
                ConstructionProgressPerHourMutiplier = 1f;
            }



        }
    }
}
