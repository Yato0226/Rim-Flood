using ExtraHives.GenStuff;
using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ExtraHives.HarmonyInstance;

[HarmonyPatch(typeof(SymbolResolver_PawnHiveGroup), "Resolve")]
public static class SymbolResolver_PawnHiveGroup_Resolve_HiveStage_Patch
{
	public static void Prefix(ref ResolveParams rp)
	{
		if (rp.faction == null)
		{
			return;
		}
		Faction faction = rp.faction;
		HiveFactionEvolutionTracker component = Find.World.GetComponent<HiveFactionEvolutionTracker>();
		HiveFactionExtension modExtension = ((Def)faction.def).GetModExtension<HiveFactionExtension>();
		if (component != null && modExtension != null && component.HiveFactionStages.TryGetValue(((object)faction).ToString(), out var _))
		{
			float pointMultipler = modExtension.CurStage.pointMultipler;
			if (rp.pawnGroupMakerParams != null)
			{
				PawnGroupMakerParms pawnGroupMakerParams = rp.pawnGroupMakerParams;
				pawnGroupMakerParams.points *= pointMultipler;
			}
		}
	}
}
