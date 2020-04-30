using EnhanceLordTroop;
using System.Linq;
using System.Xml;

namespace Wang
{
    public class Settings
    {
        /// <summary>
        /// 停战时间
        /// </summary>
        public static int TruceDays { get; set; } = 7;

        public static void Init(XmlDocument xmlDocument)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("Config").SelectSingleNode("DeclareWar");

            TruceDays = int.Parse(xmlNode.SelectSingleNode("TruceDays").InnerText);

        }
    }
}
