using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Wang
{
    public class SettlementMillitiaConfig
    {

        public static int TownRemainMillitiaNumber { get; set; } = 200;
        public static int CastleRemainMillitiaNumber { get; set; } = 100;

        public static float EliteTroopRate { get; set; } = 50f;

        public static void Init(XmlDocument xmlDocument)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("Config").SelectSingleNode("Settlement");

            TownRemainMillitiaNumber = int.Parse(xmlNode.SelectSingleNode("TownRemainMillitiaNumber").InnerText);
            CastleRemainMillitiaNumber = int.Parse(xmlNode.SelectSingleNode("CastleRemainMillitiaNumber").InnerText);
            EliteTroopRate = float.Parse(xmlNode.SelectSingleNode("EliteTroopRate").InnerText);

        }
    }
}
