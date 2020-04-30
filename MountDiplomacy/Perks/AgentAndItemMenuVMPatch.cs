using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.CampaignSystem;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using System.Reflection;
using Helpers;

namespace Wang
{
    public static class PerkHelp
    {
        public static ItemObject.ItemUsageSetFlags GetItemUsageSetFlag(WeaponComponentData item)
        {
            if (!string.IsNullOrEmpty(item.ItemUsage))
            {
                return MBItem.GetItemUsageSetFlags(item.ItemUsage);
            }
            return (ItemObject.ItemUsageSetFlags)0;
        }


        public static void TwoHand3(WeaponStatsData[] weaponStatsData, CharacterObject hero, int i)
        {
            if (hero.GetPerkValue(DefaultPerks.TwoHanded.PowerBasher))
            {
                if (weaponStatsData[i].SwingSpeed < 85)
                {
                    weaponStatsData[i].SwingDamage = (int)(weaponStatsData[i].SwingDamage * 1.1);
                    weaponStatsData[i].SwingSpeed = (int)(weaponStatsData[i].SwingSpeed * 1.05);
                    weaponStatsData[i].ThrustDamage = (int)(weaponStatsData[i].ThrustDamage * 1.1);
                    weaponStatsData[i].ThrustSpeed = (int)(weaponStatsData[i].ThrustSpeed * 1.05);
                }
            }
            else if (hero.GetPerkValue(DefaultPerks.TwoHanded.QuickPlunder))
            {
                if (weaponStatsData[i].SwingSpeed > 100)
                {
                    weaponStatsData[i].SwingDamage = (int)(weaponStatsData[i].SwingDamage * 1.05);
                    weaponStatsData[i].SwingSpeed = (int)(weaponStatsData[i].SwingSpeed * 1.02);
                    weaponStatsData[i].ThrustDamage = (int)(weaponStatsData[i].ThrustDamage * 1.05);
                    weaponStatsData[i].ThrustSpeed = (int)(weaponStatsData[i].ThrustSpeed * 1.02);

                }
            }
        }

    }


    [HarmonyPatch]
    public class AgentAndItemMenuVMPatch
    {
        private static readonly FieldInfo MaxAmmoField = typeof(MissionWeapon).GetField("_maxDataValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

        /// <summary>
        /// 弹药
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Agent), "set_Equipment")]
        public static void Equipment(Agent __instance, MissionEquipment value)
        {
            if (!__instance.IsHero || !(__instance.Character is CharacterObject charObj))
                return;

            for (var i = 0; i < 5; i++)
            {
                MissionWeapon missionWeapon = value[i];

                if (missionWeapon.Weapons.IsEmpty())
                    continue;

                var weaponComponentData = missionWeapon.Weapons[0];
                if (weaponComponentData == null)
                    continue;


                short add = 0;
                if (weaponComponentData.WeaponClass == WeaponClass.Arrow)
                {
                    if (charObj.GetPerkValue(DefaultPerks.Bow.LargeQuiver))
                    {
                        add += 3;
                    }

                    if (__instance.HasMount && charObj.GetPerkValue(DefaultPerks.Riding.SpareArrows))
                    {
                        add += 3;
                    }

                    if (__instance.HasMount && charObj.GetPerkValue(DefaultPerks.Bow.BattleEquipped))
                    {
                        add += 6;
                    }

                    if (!__instance.HasMount && charObj.GetPerkValue(DefaultPerks.Athletics.ExtraArrows))
                    {
                        add += 2;
                    }
                    missionWeapon.Amount += add;
                    short newMaxValue = (short)(missionWeapon.MaxAmount + add);
                    MaxAmmoField.SetValue(missionWeapon, newMaxValue);
                    value[i] = missionWeapon;
                }
                else if (weaponComponentData.WeaponClass == WeaponClass.Crossbow)
                {
                    if (__instance.HasMount && charObj.GetPerkValue(DefaultPerks.Riding.SpareArrows))
                    {
                        add += 3;
                    }
                    if (!__instance.HasMount && charObj.GetPerkValue(DefaultPerks.Athletics.ExtraArrows))
                    {
                        add += 2;
                    }
                    missionWeapon.Amount += add;
                    short newMaxValue = (short)(missionWeapon.MaxAmount + add);
                    MaxAmmoField.SetValue(missionWeapon, newMaxValue);
                    value[i] = missionWeapon;
                }
            }
        }

        /// <summary>
        /// 可以在马上使用弩和长弓
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Agent), "WeaponEquipped")]
        private static void WeaponEquipped(Agent __instance, EquipmentIndex equipmentSlot, ref WeaponData weaponData, WeaponStatsData[] weaponStatsData, ref WeaponData ammoWeaponData, WeaponStatsData[] ammoWeaponStatsData, GameEntity weaponEntity, bool removeOldWeaponFromScene, bool isWieldedOnSpawn)
        {
            if (__instance.IsHero && weaponStatsData != null)
            {
                var hero = ((CharacterObject)__instance.Character);
                for (int i = 0; i < weaponStatsData.Length; i++)
                {
                    switch ((WeaponClass)weaponStatsData[i].WeaponClass)
                    {
                        case WeaponClass.Bow:
                            //弓可以在马上用。
                            if (((CharacterObject)__instance.Character).GetPerkValue(DefaultPerks.Bow.MountedArcher)
                                || ((CharacterObject)__instance.Character).GetPerkValue(DefaultPerks.Riding.BowExpert))
                            {
                                weaponStatsData[i].ItemUsageIndex = MBItem.GetItemUsageIndex("bow");
                            }
                            //精度
                            if (__instance.HasMount && hero.GetPerkValue(DefaultPerks.Riding.Sharpshooter))
                            {
                                weaponStatsData[i].Accuracy = (int)(weaponStatsData[i].Accuracy * 1.15);
                            }

                            break;
                        case WeaponClass.Crossbow:
                            //弩可以在马上用。
                            if (((CharacterObject)__instance.Character).GetPerkValue(DefaultPerks.Crossbow.CrossbowCavalry)
                                || ((CharacterObject)__instance.Character).GetPerkValue(DefaultPerks.Riding.CrossbowExpert))
                            {
                                weaponStatsData[i].WeaponFlags = weaponStatsData[i].WeaponFlags & ~(ulong)WeaponFlags.CantReloadOnHorseback;
                            }



                            //精度
                            if (__instance.HasMount && hero.GetPerkValue(DefaultPerks.Riding.Sharpshooter))
                            {
                                weaponStatsData[i].Accuracy = (int)(weaponStatsData[i].Accuracy * 1.15);
                            }
                            break;

                        case WeaponClass.TwoHandedSword:
                            PerkHelp.TwoHand3(weaponStatsData, hero, i);
                            break;

                        case WeaponClass.TwoHandedAxe:
                            PerkHelp.TwoHand3(weaponStatsData, hero, i);


                            break;
                    }
                }
            }
        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemMenuVM), "AddWeaponItemFlags")]
        private static bool AddWeaponItemFlags(ItemMenuVM __instance, ref BasicCharacterObject ____character, MBBindingList<ItemFlagVM> list, WeaponComponentData weapon)
        {
            var hero = (CharacterObject)____character;
            var blowCanMount = weapon.WeaponClass == WeaponClass.Bow && hero.IsHero && (hero.GetPerkValue(DefaultPerks.Bow.MountedArcher) || hero.GetPerkValue(DefaultPerks.Riding.BowExpert));

            var crossBlowCanReloadMount = weapon.WeaponClass == WeaponClass.Crossbow && hero.IsHero && (hero.GetPerkValue(DefaultPerks.Crossbow.CrossbowCavalry) || hero.GetPerkValue(DefaultPerks.Riding.CrossbowExpert));

            if (weapon.RelevantSkill == DefaultSkills.Bow)
            {
                list.Add(new ItemFlagVM("Weapons\\bow", GameTexts.FindText("str_inventory_flag_bow").ToString()));
            }
            if (weapon.RelevantSkill == DefaultSkills.Crossbow)
            {
                list.Add(new ItemFlagVM("Weapons\\crossbow", GameTexts.FindText("str_inventory_flag_crossbow").ToString()));
            }
            if (weapon.RelevantSkill == DefaultSkills.Polearm)
            {
                list.Add(new ItemFlagVM("Weapons\\polearm", GameTexts.FindText("str_inventory_flag_polearm").ToString()));
            }
            if (weapon.RelevantSkill == DefaultSkills.OneHanded)
            {
                list.Add(new ItemFlagVM("Weapons\\one_handed", GameTexts.FindText("str_inventory_flag_one_handed").ToString()));
            }
            if (weapon.RelevantSkill == DefaultSkills.TwoHanded)
            {
                list.Add(new ItemFlagVM("Weapons\\two_handed", GameTexts.FindText("str_inventory_flag_two_handed").ToString()));
            }
            if (weapon.RelevantSkill == DefaultSkills.Throwing)
            {
                list.Add(new ItemFlagVM("Weapons\\throwing", GameTexts.FindText("str_inventory_flag_throwing").ToString()));
            }
            if (weapon.WeaponFlags.HasAnyFlag(WeaponFlags.CantReloadOnHorseback) && !crossBlowCanReloadMount)
            {
                list.Add(new ItemFlagVM("Weapons\\cant_reload_on_horseback", GameTexts.FindText("str_inventory_flag_cant_reload_on_horseback").ToString()));
            }
            if (weapon.WeaponFlags.HasAnyFlag(WeaponFlags.BonusAgainstShield))
            {
                list.Add(new ItemFlagVM("Weapons\\bonus_against_shield", GameTexts.FindText("str_inventory_flag_bonus_against_shield").ToString()));
            }
            ItemObject.ItemUsageSetFlags p = PerkHelp.GetItemUsageSetFlag(weapon);
            if (p.HasAnyFlag(ItemObject.ItemUsageSetFlags.RequiresNoMount) && !blowCanMount)
            {
                list.Add(new ItemFlagVM("Weapons\\cant_use_with_horse", GameTexts.FindText("str_inventory_flag_cant_use_with_mounts").ToString()));
            }
            if (p.HasAnyFlag(ItemObject.ItemUsageSetFlags.RequiresNoShield))
            {
                list.Add(new ItemFlagVM("Weapons\\cant_use_with_shields", GameTexts.FindText("str_inventory_flag_cant_use_with_shields").ToString()));
            }

            return false;

        }

    }
}
