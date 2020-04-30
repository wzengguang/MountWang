using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Wang
{
    public class SortPartyConfig
    {

        public static TroopSortType SortOrder
        {
            get;
            set;
        } = TroopSortType.MountRangeTierDesc;


        public static void init(XmlDocument xmlDocument)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("Config").SelectSingleNode("SortParty");

            switch (xmlNode.SelectSingleNode("sortOrder").InnerText.Trim())
            {
                case "TierDesc":
                    SortOrder = TroopSortType.TierDesc;
                    break;
                case "TierAsc":
                    SortOrder = TroopSortType.TierAsc;
                    break;
                case "TierDescType":
                    SortOrder = TroopSortType.TierDescType;
                    break;
                case "TierAscType":
                    SortOrder = TroopSortType.TierAscType;
                    break;
                case "MountRangeTierDesc":
                    SortOrder = TroopSortType.MountRangeTierDesc;
                    break;
                case "MountRangeTierAsc":
                    SortOrder = TroopSortType.MountRangeTierAsc;
                    break;
                case "CultureTierDesc":
                    SortOrder = TroopSortType.CultureTierDesc;
                    break;
                case "CultureTierAsc":
                    SortOrder = TroopSortType.CultureTierAsc;
                    break;
                default:
                    SortOrder = TroopSortType.None;
                    break;

            }
        }
    }
}
