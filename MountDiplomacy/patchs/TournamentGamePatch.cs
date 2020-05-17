using HarmonyLib;
using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation.Tags;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Wang
{
    [HarmonyPatch]
    class TournamentGamePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TournamentGame), "GetTournamentPrize")]
        private static bool GetTournamentPrize(TournamentGame __instance, ref ItemObject __result)
        {
            string[] e = new string[]
            {
                "noble_horse_southern",
                "noble_horse_imperial",
                "noble_horse_western",
                "noble_horse_eastern",
                "noble_horse_battania",
                "noble_horse_northern",
                "special_camel",
                "noble_horse"
            };

            ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>(e.GetRandomElement());

            if (MBRandom.RandomFloat < 0.15f)
            {
                __result = @object;
                return false;
            }

            var culture = __instance.Town.Culture;

            var days = Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow;
            var mul = Math.Sqrt((days + 100) / 100);


            ItemObject itemObject = MBRandom.ChooseWeighted(ItemObject.All, delegate (ItemObject item)
            {
                if ((float)item.Value <= 1600f * (item.IsMountable ? 0.5f : mul * 1f) || (float)item.Value >= 5000f * (item.IsMountable ? 0.5f : mul * 1f) || item.Culture != __instance.Town.Culture || item.IsPlayerCraft() || (!item.IsCraftedWeapon && !item.IsMountable && item.ArmorComponent == null))
                {
                    return 0f;
                }
                return 1f;
            });
            //if (itemObject == null)
            //{
            //    itemObject = MBRandom.ChooseWeighted<ItemObject>(ItemObject.All, delegate (ItemObject item)
            //    {
            //        if ((float)item.Value <= mul * 1600f * (item.IsMountable ? 0.5f : 1f) || (float)item.Value >= mul * 5000f * (item.IsMountable ? 0.5f : 1f) || item.IsPlayerCraft() || item.IsCraftedWeapon || (!item.IsCraftedWeapon && !item.IsMountable && item.ArmorComponent == null))
            //        {
            //            return 0f;
            //        }
            //        return 1f;
            //    });
            //}
            if (itemObject == null)
            {
                __result = @object;
                return false;
            }
            __result = itemObject;
            return false;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(TournamentBehavior), "CalculateBet")]
        private static void CalculateBet(ref TournamentBehavior __instance)
        {
            if (__instance.CurrentMatch == null || __instance.CurrentMatch.Teams == null)
            {
                return;
            }
            var arr = new int[__instance.CurrentMatch.Teams.Count()];
            var atIndex = -1;

            var num = 0;
            foreach (var team in __instance.CurrentMatch.Teams)
            {
                foreach (var participant in team.Participants)
                {
                    if (participant.Character == CharacterObject.PlayerCharacter)
                    {
                        atIndex = num;
                    }
                    arr[num] += participant.Character.Level;
                }
                num++;
            }

            var other = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (i != atIndex)
                {
                    other += arr[i];
                }
            }

            if (atIndex == -1)
            {
                return;
            }

            other /= (arr.Length - 1);

            var ratio = 2f * other / arr[atIndex];

            ratio *= 1 + (arr.Length - 2) * 0.2f;

            var odd = MathF.Clamp(ratio, 1.5f, 8f);

            Traverse.Create(__instance).Property("BetOdd").SetValue(odd);

        }


    }



}
