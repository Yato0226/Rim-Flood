using HarmonyLib;
using RimWorld;
using Verse;

namespace ExtraHives.HarmonyInstance;

[HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker")]
public static class IncidentWorker_RaidEnemy_TryExecuteWorker_Patch
{
	[HarmonyPrefix]
	[HarmonyPriority(800)]
	public static void Prefix(ref IncidentParms parms)
	{
		if (!(parms.target is Map) || !((Map)parms.target).IsPlayerHome || parms.faction == null || !((Def)parms.faction.def).HasModExtension<HiveFactionExtension>())
		{
			return;
		}
		float num = 1f;
		int value = 0;
		HiveFactionEvolutionTracker component = Find.World.GetComponent<HiveFactionEvolutionTracker>();
		HiveFactionExtension modExtension = ((Def)parms.faction.def).GetModExtension<HiveFactionExtension>();
		if (component != null)
		{
			if (component.HiveFactionStages.TryGetValue(((object)parms.faction).ToString(), out value))
			{
				num = modExtension.CurStage.pointMultipler;
			}
			else
			{
				value = modExtension.ActiveStage;
				GenCollection.SetOrAdd<string, int>(component.HiveFactionStages, ((object)parms.faction).ToString(), value);
				num = modExtension.CurStage.pointMultipler;
			}
		}
		parms.points *= num;
	}
}
