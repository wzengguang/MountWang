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

        public static SortType SortOrder
        {
            get;
            set;
        } = SortType.MountRangeTierDesc;


        public static void init(XmlDocument xmlDocument)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("Config").SelectSingleNode("SortParty");

            switch (xmlNode.SelectSingleNode("sortOrder").InnerText.Trim())
            {
                case "TierDesc":
                    SortOrder = SortType.TierDesc;
                    break;
                case "TierAsc":
                    SortOrder = SortType.TierAsc;
                    break;
                case "TierDescType":
                    SortOrder = SortType.TierDescType;
                    break;
                case "TierAscType":
                    SortOrder = SortType.TierAscType;
                    break;
                case "MountRangeTierDesc":
                    SortOrder = SortType.MountRangeTierDesc;
                    break;
                case "MountRangeTierAsc":
                    SortOrder = SortType.MountRangeTierAsc;
                    break;
                case "CultureTierDesc":
                    SortOrder = SortType.CultureTierDesc;
                    break;
                case "CultureTierAsc":
                    SortOrder = SortType.CultureTierAsc;
                    break;
                default:
                    SortOrder = SortType.None;
                    break;

            }
        }
    }
}
