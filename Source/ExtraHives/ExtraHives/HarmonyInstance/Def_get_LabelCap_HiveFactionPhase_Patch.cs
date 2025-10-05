using HarmonyLib;
using Verse;

namespace ExtraHives.HarmonyInstance;

[HarmonyPatch(typeof(Def), "get_LabelCap")]
public static class Def_get_LabelCap_HiveFactionPhase_Patch
{
	[HarmonyPostfix]
	public static void Postfix(Def __instance, ref TaggedString __result)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (__instance.HasModExtension<HiveFactionExtension>())
		{
			HiveFactionExtension modExtension = __instance.GetModExtension<HiveFactionExtension>();
			if (modExtension.HasStages && modExtension.showStageInName)
			{
				__result += TranslatorFormattedStringExtensions.Translate(modExtension.stageKey, modExtension.ActiveStage);
			}
		}
	}
}
