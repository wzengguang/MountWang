using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Wang
{
    public class BanditConfig
    {

        public static int NumberOfMaximumLooterParties { get; set; } = 300;
        public static int NumberOfMinimumBanditPartiesInAHideoutToInfestIt { get; set; } = 2;

        public static int NumberOfMaximumBanditPartiesInEachHideout { get; set; } = 4;
        public static int NumberOfMaximumBanditPartiesAroundEachHideout { get; set; } = 8;
        public static int NumberOfMaximumHideoutsAtEachBanditFaction { get; set; } = 10;
        public static int NumberOfInitialHideoutsAtEachBanditFaction { get; set; } = 1;

        public static int BanditMultiple { get; set; } = 1;

        public static void Init(XmlDocument xmlDocument)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("Config").SelectSingleNode("Bandit");

            NumberOfMaximumLooterParties = int.Parse(xmlNode.SelectSingleNode("NumberOfMaximumLooterParties").InnerText);
            NumberOfMinimumBanditPartiesInAHideoutToInfestIt = int.Parse(xmlNode.SelectSingleNode("NumberOfMinimumBanditPartiesInAHideoutToInfestIt").InnerText);
            NumberOfMaximumBanditPartiesInEachHideout = int.Parse(xmlNode.SelectSingleNode("NumberOfMaximumBanditPartiesInEachHideout").InnerText);
            NumberOfMaximumBanditPartiesAroundEachHideout = int.Parse(xmlNode.SelectSingleNode("NumberOfMaximumBanditPartiesAroundEachHideout").InnerText);
            NumberOfMaximumHideoutsAtEachBanditFaction = int.Parse(xmlNode.SelectSingleNode("NumberOfMaximumHideoutsAtEachBanditFaction").InnerText);
            NumberOfInitialHideoutsAtEachBanditFaction = int.Parse(xmlNode.SelectSingleNode("NumberOfInitialHideoutsAtEachBanditFaction").InnerText);
            BanditMultiple = int.Parse(xmlNode.SelectSingleNode("BanditMultiple").InnerText);

        }
    }
}
