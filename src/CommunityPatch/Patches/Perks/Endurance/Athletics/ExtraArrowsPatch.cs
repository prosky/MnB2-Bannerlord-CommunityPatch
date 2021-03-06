﻿#if !AFTER_E1_4_3

using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Endurance.Athletics {

  [PatchObsolete(ApplicationVersionType.EarlyAccess,1,4, 3)]
  public class ExtraArrowsPatch : ExtraAmmoPerksPatch<ExtraArrowsPatch> {

    private static readonly MethodInfo PatchMethodInfo = typeof(ExtraArrowsPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public ExtraArrowsPatch() : base("eQLORdgE") {
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(ExtraAmmoPerksPatch.TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    static bool CanApplyPerk(Hero hero, WeaponComponentData weaponComponentData)
      => WeaponComponentData.GetItemTypeFromWeaponClass(weaponComponentData.WeaponClass) == ItemObject.ItemTypeEnum.Arrows &&
        hero.GetPerkValue(ActivePatch.Perk);

    private static void Postfix(Agent __instance) {
      if (!HasMount(__instance))
        ApplyPerk(__instance, 2, CanApplyPerk);
    }

  }

}

#endif