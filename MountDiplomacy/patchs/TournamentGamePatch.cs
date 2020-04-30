using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Wang
{
    [HarmonyPatch(typeof(TournamentGame))]
    class TournamentGamePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("GetTournamentPrize")]
        private static bool GetTournamentPrize(TournamentGame __instance, ref ItemObject __result)
        {
            string[] e = new string[28]
            {
            "winds_fury_sword_t3",
            "bone_crusher_mace_t3",
            "tyrhung_sword_t3",
            "pernach_mace_t3",
            "early_retirement_2hsword_t3",
            "black_heart_2haxe_t3",
            "knights_fall_mace_t3",
            "the_scalpel_sword_t3",
            "judgement_mace_t3",
            "dawnbreaker_sword_t3",
            "ambassador_sword_t3",
            "heavy_nasalhelm_over_imperial_mail",
            "closed_desert_helmet",
            "sturgian_helmet_closed",
            "full_helm_over_laced_coif",
            "desert_mail_coif",
            "heavy_nasalhelm_over_imperial_mail",
            "plumed_nomad_helmet",
            "eastern_studded_shoulders",
            "ridged_northernhelm",
            "armored_bearskin",
            "noble_horse_southern",
            "noble_horse_imperial",
            "noble_horse_western",
            "noble_horse_eastern",
            "noble_horse_battania",
            "noble_horse_northern",
            "special_camel"
            };

            ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>(e.GetRandomElement());
            var culture = __instance.Town.Culture;

            var days = Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow;
            var mul = (days + 100) / 100;
            var min = (days + 200) / 200;

            ItemObject itemObject = MBRandom.ChooseWeighted(ItemObject.All, (ItemObject item) =>
            (!(item.Value > min * 1600f * (item.IsMountable ? 0.5f : 1f)) ||
            !(item.Value < mul * 5000f * (item.IsMountable ? 0.5f : 1f)) ||
            item.Culture != culture || (!item.IsCraftedWeapon && !item.IsMountable && item.ArmorComponent == null)) ? 0f : 1f);
            if (itemObject == null)
            {
                itemObject = MBRandom.ChooseWeighted(ItemObject.All, (ItemObject item) => (!((float)item.Value > min * 1600f * (item.IsMountable ? 0.5f : 1f)) || !((float)item.Value < mul * 5000f * (item.IsMountable ? 0.5f : 1f)) || (!item.IsCraftedWeapon && !item.IsMountable && item.ArmorComponent == null)) ? 0f : 1f);
            }
            if (itemObject == null)
            {
                __result = @object;
                return false;
            }
            __result = itemObject;
            return false;
        }
    }
}
