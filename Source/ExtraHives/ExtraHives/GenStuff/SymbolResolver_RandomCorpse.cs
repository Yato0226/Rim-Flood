using System;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ExtraHives.GenStuff;

internal class SymbolResolver_RandomCorpse : SymbolResolver
{
	public override void Resolve(ResolveParams rp)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		Map map = BaseGen.globalSettings.map;
		Rand.PushState();
		for (int i = 0; i < Rand.RangeInclusive(10, 25); i++)
		{
			IntVec3 randomCell = rp.rect.RandomCell;
			if (!GenGrid.Standable(randomCell, map) || GridsUtility.GetFirstItem(randomCell, map) != null || GridsUtility.GetFirstPawn(randomCell, map) != null || GridsUtility.GetFirstBuilding(randomCell, map) != null)
			{
				continue;
			}
			Pawn val = PawnGenerator.GeneratePawn(PawnKindDefOf.Villager, Find.FactionManager.RandomEnemyFaction(false, false, false, (TechLevel)0));
			((Thing)val).Kill((DamageInfo?)new DamageInfo(DamageDefOf.Cut, 9999f, 0f, -1f, (Thing)null, (BodyPartRecord)null, (ThingDef)null, (DamageInfo.SourceCategory)0, (Thing)null, true, true, (QualityCategory)2, true), (Hediff)null);
			Corpse corpse = val.Corpse;
			corpse.timeOfDeath = 10000;
			ThingCompUtility.TryGetComp<CompRottable>((Thing)corpse).RotImmediately((RotStage)1);
			GenSpawn.Spawn((Thing)corpse, randomCell, map, (WipeMode)0);
			for (int j = 0; j < 5; j++)
			{
				IntVec3 val2 = default(IntVec3);
				RCellFinder.TryFindRandomCellNearWith(randomCell, (Predicate<IntVec3>)((IntVec3 ni) => GenGrid.Walkable(ni, map)), map, out val2, 1, 3);
				GenSpawn.Spawn(DefDatabase<ThingDef>.GetNamed("Filth_CorpseBile"), val2, map, (WipeMode)0);
			}
		}
		Rand.PopState();
	}
}
