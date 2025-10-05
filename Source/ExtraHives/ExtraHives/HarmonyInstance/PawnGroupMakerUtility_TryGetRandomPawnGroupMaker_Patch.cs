using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ExtraHives.HarmonyInstance;

[HarmonyPatch(typeof(PawnGroupMakerUtility), "TryGetRandomPawnGroupMaker")]
public static class PawnGroupMakerUtility_TryGetRandomPawnGroupMaker_Patch
{
	[HarmonyPrefix]
	public static void Prefix(PawnGroupMakerParms parms, ref PawnGroupMaker pawnGroupMaker)
	{
		Faction faction = parms.faction;
		HiveFactionEvolutionTracker component = Find.World.GetComponent<HiveFactionEvolutionTracker>();
		if (faction == null)
		{
			return;
		}
		HiveFactionExtension modExtension = ((Def)faction.def).GetModExtension<HiveFactionExtension>();
		if (component != null && modExtension != null && component.HiveFactionStages.TryGetValue(((object)faction).ToString(), out var _))
		{
			if (parms.seed.HasValue)
			{
				Rand.PushState(parms.seed.Value);
			}
			if (!GenList.NullOrEmpty<PawnGroupMaker>((IList<PawnGroupMaker>)modExtension.CurStage.pawnGroupMakers))
			{
				string empty = string.Empty;
			}
			bool flag = GenCollection.TryRandomElementByWeight<PawnGroupMaker>((modExtension.CurStage.pawnGroupMakers ?? parms.faction.def.pawnGroupMakers).Where((PawnGroupMaker gm) => gm.kindDef == parms.groupKind && gm.CanGenerateFrom(parms)), (Func<PawnGroupMaker, float>)((PawnGroupMaker gm) => gm.commonality), out pawnGroupMaker);
			if (parms.seed.HasValue)
			{
				Rand.PopState();
			}
		}
	}
}
