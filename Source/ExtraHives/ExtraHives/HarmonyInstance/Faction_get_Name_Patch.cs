using HarmonyLib;
using RimWorld;
using Verse;

namespace ExtraHives.HarmonyInstance;

public static class Faction_get_Name_Patch
{
	[HarmonyPostfix]
	public static void Postfix(Faction __instance, ref string __result)
	{
		if (((Def)__instance.def).HasModExtension<HiveFactionExtension>())
		{
			HiveFactionExtension modExtension = ((Def)__instance.def).GetModExtension<HiveFactionExtension>();
			if (modExtension.HasStages)
			{
				__result = __result + " " + modExtension.ActiveStage;
			}
		}
	}
}
