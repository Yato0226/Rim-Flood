using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExtraHives.HarmonyInstance;

[HarmonyPatch(typeof(PawnGroupMakerUtility), "MaxPawnCost")]
public static class PawnGroupMakerUtility_MaxPawnCost_Patch
{
	[HarmonyPrefix]
	public static bool Prefix(Faction faction, float totalPoints, RaidStrategyDef raidStrategy, PawnGroupKindDef groupKind, ref float __result)
	{
		HiveFactionEvolutionTracker component = Find.World.GetComponent<HiveFactionEvolutionTracker>();
		HiveFactionExtension modExtension = ((Def)faction.def).GetModExtension<HiveFactionExtension>();
		if (faction != null && component != null && modExtension != null && component.HiveFactionStages.TryGetValue(((object)faction).ToString(), out var _))
		{
			SimpleCurve val = modExtension.CurStage.maxPawnCostPerTotalPointsCurve ?? faction.def.maxPawnCostPerTotalPointsCurve;
			float num = val.Evaluate(totalPoints);
			if (raidStrategy != null)
			{
				num = Mathf.Min(num, totalPoints / raidStrategy.minPawns);
			}
			num = Mathf.Max(num, faction.def.MinPointsToGeneratePawnGroup(groupKind, (PawnGroupMakerParms)null) * 1.2f);
			if (raidStrategy != null)
			{
				num = Mathf.Max(num, raidStrategy.Worker.MinMaxAllowedPawnGenOptionCost(faction, groupKind) * 1.2f);
			}
			__result = num;
			return false;
		}
		return true;
	}
}
