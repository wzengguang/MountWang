using System.Linq;
using System.Xml;

namespace EnhanceLordTroop
{
    public class XpMultiplierConfigBase
    {


        public static bool AddTroopXpEnabled { get; set; } = true;

        public static float[] PartyTroopRatio { get; set; } = new float[10] { 0.05f, 0.15f, 0.3f, 0.2f, 0.15f, 0.1f, 0.05f, 0f, 0f, 0f };

        public static int[] TierXps { get; set; } = new int[7] { 50, 100, 100, 100, 100, 100, 100 };

        public static void Init(XmlDocument xmlDocument)
        {

            XmlNode xmlNode = xmlDocument.SelectSingleNode("Config").SelectSingleNode("ExperienceMultiplier");

            AddTroopXpEnabled = bool.Parse(xmlNode.SelectSingleNode("AddTroopXpEnabled").InnerText);
            var ratio = xmlNode.SelectSingleNode("PartyTroopRatio").InnerText.Split(',').Select(a => float.Parse(a.Trim())).ToArray();
            for (int i = 0; i < ratio.Length; i++)
            {
                PartyTroopRatio[i] = ratio[i];
            }
            TierXps = xmlNode.SelectSingleNode("TierXps").InnerText.Split(',').Select(a => int.Parse(a.Trim())).ToArray();
        }
    }
}
