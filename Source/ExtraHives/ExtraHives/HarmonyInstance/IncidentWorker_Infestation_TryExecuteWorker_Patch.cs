using HarmonyLib;
using RimWorld;
using Verse;

namespace ExtraHives.HarmonyInstance;

[HarmonyPatch(typeof(IncidentWorker_Infestation), "TryExecuteWorker")]
public static class IncidentWorker_Infestation_TryExecuteWorker_Patch
{
	private static readonly IntRange RaidDelay = new IntRange(1000, 2000);

	[HarmonyPrefix]
	public static void Prefix(ref IncidentParms parms)
	{
		if (!(parms.target is Map) || !((Map)parms.target).IsPlayerHome)
		{
			return;
		}
		Faction faction = parms.faction;
		HiveFactionEvolutionTracker component = Find.World.GetComponent<HiveFactionEvolutionTracker>();
		HiveFactionExtension hiveFactionExtension = ((faction != null) ? ((Def)faction.def).GetModExtension<HiveFactionExtension>() : null);
		if (faction != null)
		{
			IIncidentTarget target = parms.target;
			Map val = (Map)(object)((target is Map) ? target : null);
			if (val != null && component != null && hiveFactionExtension != null && component.HiveFactionStages.TryGetValue(((object)faction).ToString(), out var _))
			{
				float pointMultipler = hiveFactionExtension.CurStage.pointMultipler;
				IncidentParms obj = parms;
				obj.points *= pointMultipler;
			}
		}
	}
}
