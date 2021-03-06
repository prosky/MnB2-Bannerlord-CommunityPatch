using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {
  public sealed class FarsightedPatch : PerkPatchBase<FarsightedPatch> {
    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.3.0.228478
        0x20, 0x9C, 0x71, 0x29, 0xC2, 0x4F, 0x22, 0xFB,
        0x9B, 0xAA, 0xBC, 0x55, 0x18, 0x8B, 0xA5, 0x94,
        0x36, 0x76, 0x01, 0x8A, 0x4D, 0xC5, 0x1F, 0x01,
        0x91, 0x77, 0x8D, 0xA7, 0xA3, 0x9B, 0x99, 0xC8
      },
      new byte[] {
        // e1.5.2.241443
        0x9C, 0x5E, 0x8E, 0xF7, 0xAC, 0x77, 0x39, 0x31,
        0x6D, 0x1D, 0x36, 0x04, 0x6A, 0xCD, 0x77, 0x56,
        0xCF, 0x04, 0x03, 0x8A, 0x81, 0x5D, 0x95, 0xBF,
        0xBB, 0x13, 0x66, 0xA4, 0xED, 0xDE, 0x60, 0x1B
      }
    };

    public FarsightedPatch() : base("yqPNKKGb") { }

    [PatchClass(typeof(DefaultMapVisibilityModel))]
    private static void GetPartySpottingDifficultyPostfix(ref float __result) {
      if (MobileParty.MainParty.HasPerk(ActivePatch.Perk)) {
        __result -= __result * ActivePatch.Perk.PrimaryBonus;
      }
    }
  }
}
