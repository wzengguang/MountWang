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

        /// <summary>
        /// 战败后被释放，多少天后重新出现。
        /// </summary>
        public static int PrisonerDaysLeftToRespawn { get; set; } = 7;

        public static void Init(XmlDocument xmlDocument)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("Config").SelectSingleNode("DeclareWar");

            TruceDays = int.Parse(xmlNode.SelectSingleNode("TruceDays").InnerText);
            TruceDays = int.Parse(xmlNode.SelectSingleNode("PrisonerDaysLeftToRespawn").InnerText);

        }
    }
}
