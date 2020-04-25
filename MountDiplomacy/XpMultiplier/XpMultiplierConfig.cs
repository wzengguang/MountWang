using EnhanceLordTroop;
using System.Linq;
using System.Xml;

namespace Wang
{
    public class XpMultiplierConfig : XpMultiplierConfigBase
    {
        public static bool PlayerEnabled { get; set; }

        public static bool TeammateEnabled { get; set; }

        public static int PlayerMultipier { get; set; } = 1;
        public static int TeammateMultipier { get; set; } = 1;

        public static int LearningXPMultipier { get; set; } = 0;

        public static int CombatTips { get; set; } = 1;
        public static int RaiseTheMeek { get; set; } = 1;

        public static new void Init(XmlDocument xmlDocument)
        {
            XpMultiplierConfigBase.Init(xmlDocument);

            XmlNode xmlNode = xmlDocument.SelectSingleNode("Config").SelectSingleNode("ExperienceMultiplier");

            PlayerEnabled = bool.Parse(xmlNode.SelectSingleNode("PlayerEnabled").InnerText);
            TeammateEnabled = bool.Parse(xmlNode.SelectSingleNode("TeammateEnabled").InnerText);

            PlayerMultipier = int.Parse(xmlNode.SelectSingleNode("PlayerMultipier").InnerText);
            TeammateMultipier = int.Parse(xmlNode.SelectSingleNode("TeammateMultipier").InnerText);
            LearningXPMultipier = int.Parse(xmlNode.SelectSingleNode("LearningXPMultipier").InnerText);

            CombatTips = int.Parse(xmlNode.SelectSingleNode("CombatTips").InnerText);
            RaiseTheMeek = int.Parse(xmlNode.SelectSingleNode("RaiseTheMeek").InnerText);

        }
    }
}
