using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class WarRationsPatch : PerkPatchBase<WarRationsPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultMobilePartyFoodConsumptionModel).GetMethod("CalculateDailyFoodConsumptionf", Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(WarRationsPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    internal static bool QuartermasterIsClanWide {
      get => CommunityPatchSubModule.Options.Get<bool>(nameof(QuartermasterIsClanWide));
      set => CommunityPatchSubModule.Options.Set(nameof(QuartermasterIsClanWide), value);
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x03, 0xA6, 0xD1, 0x58, 0xEA, 0x4A, 0x70, 0xB5,
        0xFD, 0x39, 0xA9, 0x33, 0x66, 0xAB, 0x92, 0x37,
        0xD5, 0xDB, 0xE7, 0x8E, 0x30, 0xF3, 0x9C, 0x5A,
        0xFC, 0x20, 0xF8, 0x64, 0xC5, 0x19, 0x19, 0x1C
      },
      new byte[] {
        // e1.4.1.230527
        0xAA, 0x08, 0xEB, 0xB8, 0x40, 0x06, 0xA6, 0xA5,
        0xED, 0xC8, 0x35, 0xB4, 0x22, 0x75, 0xAB, 0x08,
        0xEB, 0xA7, 0x50, 0x52, 0x1B, 0x4E, 0x72, 0xD3,
        0xA0, 0xDF, 0x65, 0x4A, 0xE2, 0x1E, 0x4E, 0x60
      },
      new byte[] {
        // e1.4.3.237794
        0x69, 0xAC, 0x0B, 0x27, 0xAF, 0xD6, 0xC8, 0x8F,
        0x16, 0xB7, 0x4B, 0xC6, 0x4D, 0x0A, 0xF7, 0x35,
        0x7C, 0xB6, 0x86, 0x8A, 0x15, 0x0A, 0xDA, 0xD4,
        0xB6, 0xFE, 0x6A, 0xA9, 0xB0, 0xD9, 0x73, 0x22
      }
    };

    public WarRationsPatch() : base("sLv7MMJf") {
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public override bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    // ReSharper disable once InconsistentNaming

    private static void Postfix(ref float __result, MobileParty party, StatExplainer explainer) {
      var qm = party?.LeaderHero?.Clan?.GetEffectiveQuartermaster();

      if (qm == null)
        return;

      if (
        // qm is not clan-wide
        !QuartermasterIsClanWide
        // qm is not leading party
        && party.LeaderHero != qm
        // qm is not in party
        && party.MemberRoster.All(element => element.Character?.HeroObject != qm)
      )
        return;

      var perk = ActivePatch.Perk;
      if (!qm.GetPerkValue(perk))
        return;

      var explainedNumber = new ExplainedNumber(__result, explainer);
      explainer?.Lines.RemoveAt(explainer.Lines.Count - 1);
#if AFTER_E1_4_2
      switch (perk.PrimaryIncrementType) {
#else
      switch (perk.IncrementType) {
#endif
        case SkillEffect.EffectIncrementType.Add:
          explainedNumber.Add(perk.PrimaryBonus, perk.Name);
          break;

        case SkillEffect.EffectIncrementType.AddFactor:
          explainedNumber.AddFactor(perk.PrimaryBonus, perk.Name);
          break;
      }

      __result = explainedNumber.ResultNumber;
    }

  }

}