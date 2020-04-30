using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Wang
{
    public class RecruitConfig
    {
        public static float[] RecruitChange { get; set; }

        public static void Init(XmlDocument xmlDocument)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("Config").SelectSingleNode("Recruit");

            RecruitChange = xmlNode.SelectSingleNode("RecruitChange").InnerText.Split(',').Select(a => float.Parse(a)).ToArray();

        }
    }
}
