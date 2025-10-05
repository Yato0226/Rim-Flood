using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace ExtraHives.HarmonyInstance;

[HarmonyPatch(typeof(Settlement), "get_Label")]
public static class Settlement_Label_HiveFactionPhase_Patch
{
	[HarmonyPostfix]
	public static void Postfix(Settlement __instance, ref string __result)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (((__instance != null) ? ((WorldObject)__instance).Faction : null) != null && ((Def)((WorldObject)__instance).Faction.def).HasModExtension<HiveFactionExtension>())
		{
			HiveFactionExtension modExtension = ((Def)((WorldObject)__instance).Faction.def).GetModExtension<HiveFactionExtension>();
			if (modExtension.HasStages && modExtension.showStageInName)
			{
				__result = __result + (" " + TranslatorFormattedStringExtensions.Translate(modExtension.stageKey, modExtension.ActiveStage));
			}
		}
	}
}
