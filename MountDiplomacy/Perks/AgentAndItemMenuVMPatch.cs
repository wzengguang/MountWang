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
using SandBox;

namespace Wang
{

    //MissionEquipment：Agent的武器。 Agent.Equipment->属性
    //MissionEquipment的字段 MissionWeapon[]。表示所有的武器槽里的武器。
    //MissionWeapon的字段_dataValue，_maxDataValue表示弹药数量。根据weaponComponentData.MaxDataValue产生。

    //Equipment表示单位的所有装备，包括护甲马武器。Agent.SpawnEquipment 表示Agent实例化时chractorObject自带的装备。
    //Equipment的字段 EquipmentElement[]表示所有装备槽里的武器。EquipmentElement是Struct

    //EquipmentElement的属性 ItemObject。表示对应的物品。

    //ItemObject的属性 ItemComponent表示物品的性质，WeaponComponent、HorseComponent、ArmorComponent、SaddleComponent表示装备。
    //WeaponComponent 的 WeaponComponentData[]表示武器的参数。实际集合中只有一个值。

    //WeaponStatsData 结构，表示agent装备武器时，产生的新的武器的状态。WeaponStatsData根据WeaponComponentData产生。



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


        //public static void TwoHand3(WeaponStatsData[] weaponStatsData, CharacterObject hero, int i, Agent agent)
        //{
        //    if (hero.GetPerkValue(DefaultPerks.TwoHanded.PowerBasher) && weaponStatsData[i].SwingSpeed < 85)
        //    {

        //        weaponStatsData[i].SwingDamage += (int)(weaponStatsData[i].SwingDamage * DefaultPerks.TwoHanded.PowerBasher.PrimaryBonus * 4);
        //        weaponStatsData[i].SwingSpeed += (int)(weaponStatsData[i].SwingSpeed * DefaultPerks.TwoHanded.PowerBasher.SecondaryBonus * 4);
        //        weaponStatsData[i].ThrustDamage += (int)(weaponStatsData[i].ThrustDamage * DefaultPerks.TwoHanded.PowerBasher.PrimaryBonus * 4);
        //        weaponStatsData[i].ThrustSpeed += (int)(weaponStatsData[i].ThrustSpeed * DefaultPerks.TwoHanded.PowerBasher.SecondaryBonus * 4);

        //    }
        //    else if (hero.GetPerkValue(DefaultPerks.TwoHanded.QuickPlunder) && weaponStatsData[i].SwingSpeed > 100)
        //    {

        //        weaponStatsData[i].SwingDamage += (int)(weaponStatsData[i].SwingDamage * DefaultPerks.TwoHanded.QuickPlunder.PrimaryBonus * 4);
        //        weaponStatsData[i].SwingSpeed += (int)(weaponStatsData[i].SwingSpeed * DefaultPerks.TwoHanded.QuickPlunder.SecondaryBonus * 4);
        //        weaponStatsData[i].ThrustDamage += (int)(weaponStatsData[i].ThrustDamage * DefaultPerks.TwoHanded.QuickPlunder.PrimaryBonus * 4);
        //        weaponStatsData[i].ThrustSpeed += (int)(weaponStatsData[i].ThrustSpeed * DefaultPerks.TwoHanded.QuickPlunder.SecondaryBonus * 4);

        //    }

        //    if (hero.GetPerkValue(DefaultPerks.TwoHanded.EdgePlacement))
        //    {
        //        weaponStatsData[i].SwingDamage += (int)(weaponStatsData[i].ThrustDamage * DefaultPerks.TwoHanded.EdgePlacement.PrimaryBonus * 4);
        //    }
        //    if (hero.GetPerkValue(DefaultPerks.TwoHanded.ExtraDamage))
        //    {
        //        weaponStatsData[i].SwingDamage += (int)(weaponStatsData[i].ThrustDamage * DefaultPerks.TwoHanded.ExtraDamage.PrimaryBonus * 4);
        //        weaponStatsData[i].ThrustDamage += (int)(weaponStatsData[i].ThrustDamage * DefaultPerks.TwoHanded.ExtraDamage.PrimaryBonus * 4);
        //    }

        //    if (agent.IsMount && hero.GetPerkValue(DefaultPerks.TwoHanded.MountedTwoHanded))
        //    {
        //        weaponStatsData[i].WeaponBalance += (int)(weaponStatsData[i].WeaponBalance * DefaultPerks.TwoHanded.ExtraDamage.PrimaryBonus);
        //    }
        //}

    }


    [HarmonyPatch]
    public static class AgentAndItemMenuVMPatch
    {
        private static readonly FieldInfo MaxAmmoField = typeof(MissionWeapon).GetField("_maxDataValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

        /// <summary>
        /// Mission.SpawnAgent调用Agent.InitializeMissionEquipment,给Equipment赋值。
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
        /// Agent装备（或捡起）Weapon时调用。agent丢掉装备的时候也调用。
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
                            break;
                        case WeaponClass.Crossbow:
                            //弩可以在马上用。
                            if (((CharacterObject)__instance.Character).GetPerkValue(DefaultPerks.Crossbow.CrossbowCavalry)
                                || ((CharacterObject)__instance.Character).GetPerkValue(DefaultPerks.Riding.CrossbowExpert))
                            {
                                weaponStatsData[i].WeaponFlags = weaponStatsData[i].WeaponFlags & ~(ulong)WeaponFlags.CantReloadOnHorseback;
                            }
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
