using System.Xml;

namespace Wang
{
    public class XpMultiplierConfig
    {
        public static bool PlayerEnabled { get; set; }

        public static bool TeammateEnabled { get; set; }

        public static int PlayerMultipier { get; set; } = 1;
        public static int TeammateMultipier { get; set; } = 1;

        public static int TeachCompanionXPNumber { get; set; } = 0;


        public static void Init(XmlDocument xmlDocument)
        {

            XmlNode xmlNode = xmlDocument.SelectSingleNode("Config").SelectSingleNode("ExperienceMultiplier");

            PlayerEnabled = bool.Parse(xmlNode.SelectSingleNode("PlayerEnabled").InnerText);
            TeammateEnabled = bool.Parse(xmlNode.SelectSingleNode("TeammateEnabled").InnerText);

            PlayerMultipier = int.Parse(xmlNode.SelectSingleNode("PlayerMultipier").InnerText);
            TeammateMultipier = int.Parse(xmlNode.SelectSingleNode("TeammateMultipier").InnerText);
            TeachCompanionXPNumber = int.Parse(xmlNode.SelectSingleNode("TeachCompanionXPNumber").InnerText);

        }
    }
}
