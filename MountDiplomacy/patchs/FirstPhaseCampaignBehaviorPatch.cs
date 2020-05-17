using HarmonyLib;
using StoryMode.Behaviors;
using StoryMode.StoryModePhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace Wang.patchs
{

    [HarmonyPatch(typeof(FirstPhaseCampaignBehavior), "WeeklyTick")]
    class FirstPhaseCampaignBehaviorPatch
    {

        private static bool Prefix()
        {
            if (FirstPhase.Instance != null && SecondPhase.Instance == null && FirstPhase.Instance.FirstPhaseStartTime.ElapsedYearsUntilNow > 100f)
            {
                foreach (QuestBase questBase in Campaign.Current.QuestManager.Quests.ToList<QuestBase>())
                {
                    if (questBase.IsSpecialQuest)
                    {
                        TextObject textObject = new TextObject("{=JTPmw3cb}You couldn't complete the quest in {YEAR} years.", null);
                        textObject.SetTextVariable("YEAR", 100);
                        questBase.CompleteQuestWithFail(textObject);
                    }
                }
            }
            return false;
        }
    }
}
