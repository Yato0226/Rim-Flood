using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using Verse;

namespace ExtraHives.GenStuff;

public class SymbolResolver_SingleHivePawn : SymbolResolver
{
	public override bool CanResolve(ResolveParams rp)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 cell;
		return ((SymbolResolver)this).CanResolve(rp) && ((rp.singlePawnToSpawn != null && ((Thing)rp.singlePawnToSpawn).Spawned) || TryFindSpawnCell(rp, out cell));
	}

	public override void Resolve(ResolveParams rp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		if (rp.singlePawnToSpawn != null && ((Thing)rp.singlePawnToSpawn).Spawned)
		{
			return;
		}
		Map map = BaseGen.globalSettings.map;
		if (!TryFindSpawnCell(rp, out var cell))
		{
			if (rp.singlePawnToSpawn != null)
			{
				Find.WorldPawns.PassToWorld(rp.singlePawnToSpawn, (PawnDiscardDecideMode)0);
			}
			return;
		}
		Pawn val4;
		if (rp.singlePawnToSpawn == null)
		{
			PawnGenerationRequest value = default(PawnGenerationRequest);
			if (rp.singlePawnGenerationRequest.HasValue)
			{
				value = rp.singlePawnGenerationRequest.Value;
			}
			else
			{
				PawnKindDef val;
				if ((val = rp.singlePawnKindDef) == null)
				{
					val = GenCollection.RandomElement<PawnKindDef>(DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef x) => x.defaultFactionDef == null || !x.defaultFactionDef.isPlayer));
				}
				PawnKindDef val2 = val;
				Faction val3 = rp.faction;
				if (val3 == null && val2.RaceProps.Humanlike)
				{
					if (val2.defaultFactionDef != null)
					{
						val3 = FactionUtility.DefaultFactionFrom(val2.defaultFactionDef);
						if (val3 == null)
						{
							return;
						}
					}
					else if (!GenCollection.TryRandomElement<Faction>(Find.FactionManager.AllFactions.Where((Faction x) => !x.IsPlayer).ToHashSet(), out val3))
					{
						return;
					}
				}
				value = new PawnGenerationRequest(val2, val3, (PawnGenerationContext)2, map.Tile, false, false, false, false, true, 1f, false, true, true, true, false, false, false, false, false, 0f, 0f, (Pawn)null, 1f, (Predicate<Pawn>)null, (Predicate<Pawn>)null, (IEnumerable<TraitDef>)null, (IEnumerable<TraitDef>)null, (float?)null, (float?)null, (float?)null, (Gender?)null, (string)null, (string)null, (RoyalTitleDef)null, (Ideo)null, false, false, false, false, (List<GeneDef>)null, (List<GeneDef>)null, (XenotypeDef)null, (CustomXenotype)null, (List<XenotypeDef>)null, 0f, (DevelopmentalStage)8, (Func<XenotypeDef, PawnKindDef>)null, (FloatRange?)null, (FloatRange?)null, false, false, false, -1, 0, false);
			}
			val4 = PawnGenerator.GeneratePawn(value);
			if (rp.postThingGenerate != null)
			{
				rp.postThingGenerate((Thing)(object)val4);
			}
		}
		else
		{
			val4 = rp.singlePawnToSpawn;
		}
		if (!val4.Dead && rp.disableSinglePawn.HasValue && rp.disableSinglePawn.Value)
		{
			val4.mindState.Active = false;
		}
		GenSpawn.Spawn((Thing)(object)val4, cell, map, (WipeMode)0);
		if (rp.singlePawnLord != null)
		{
			rp.singlePawnLord.AddPawn(val4);
		}
		if (rp.postThingSpawn != null)
		{
			rp.postThingSpawn((Thing)(object)val4);
		}
	}

	public static bool TryFindSpawnCell(ResolveParams rp, out IntVec3 cell)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Map map = BaseGen.globalSettings.map;
		return CellFinder.TryFindRandomCellInsideWith(rp.rect, (Predicate<IntVec3>)((IntVec3 x) => GenGrid.Standable(x, map) && (rp.singlePawnSpawnCellExtraPredicate == null || rp.singlePawnSpawnCellExtraPredicate(x))), out cell);
	}
}
