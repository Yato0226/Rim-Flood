using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExtraHives;

public class IncidentWorker_InfestedMeteoriteImpact : IncidentWorker
{
	protected override bool CanFireNowSub(IncidentParms parms)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		Map map = (Map)parms.target;
		if (base.def.mechClusterBuilding == null)
		{
			return false;
		}
		if (!((Def)base.def.mechClusterBuilding).HasModExtension<HiveDefExtension>())
		{
			return false;
		}
		ThingDef mechClusterBuilding = base.def.mechClusterBuilding;
		HiveDefExtension modExtension = ((Def)mechClusterBuilding).GetModExtension<HiveDefExtension>();
		IntVec3 cell;
		return TryFindCell(out cell, map);
	}

	protected override bool TryExecuteWorker(IncidentParms parms)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		Map val = (Map)parms.target;
		if (base.def.mechClusterBuilding == null)
		{
			return false;
		}
		if (!((Def)base.def.mechClusterBuilding).HasModExtension<HiveDefExtension>())
		{
			return false;
		}
		ThingDef mechClusterBuilding = base.def.mechClusterBuilding;
		HiveDefExtension modExtension = ((Def)mechClusterBuilding).GetModExtension<HiveDefExtension>();
		Faction val2 = null;
		if (parms.faction != null)
		{
			val2 = parms.faction;
		}
		else
		{
			if (modExtension.Faction == null)
			{
				return false;
			}
			IEnumerable<Faction> enumerable = CandidateFactions(val, ((Def)modExtension.Faction).defName);
			val2 = (GenCollection.EnumerableNullOrEmpty<Faction>(enumerable) ? Find.FactionManager.FirstFactionOfDef(modExtension.Faction) : GenCollection.RandomElement<Faction>(enumerable));
		}
		if (!TryFindCell(out var cell, val))
		{
			return false;
		}
		List<Thing> list = new List<Thing>();
		TunnelRaidSpawner tunnelRaidSpawner = (TunnelRaidSpawner)(object)ThingMaker.MakeThing(ThingDefOf.Tunneler_ExtraHives, (ThingDef)null);
		tunnelRaidSpawner.spawnHive = false;
		Rand.PushState();
		tunnelRaidSpawner.initialPoints = Mathf.Max(parms.points * Rand.Range(0.3f, 0.6f), 200f);
		Rand.PopState();
		tunnelRaidSpawner.spawnedByInfestationThingComp = true;
		tunnelRaidSpawner.ResultSpawnDelay = new FloatRange(0.1f, 0.5f);
		tunnelRaidSpawner.spawnablePawnKinds = GenCollection.RandomElement<PawnGroupMaker>(val2.def.pawnGroupMakers.Where((PawnGroupMaker x) => x.kindDef == PawnGroupKindDefOf.Hive_ExtraHives || x.kindDef == PawnGroupKindDefOf.Tunneler_ExtraHives)).options;
		if (tunnelRaidSpawner.SpawnedFaction == null && val2 != null)
		{
			tunnelRaidSpawner.SpawnedFaction = val2;
		}
		list.Add((Thing)(object)tunnelRaidSpawner);
		Generate(out var outThings);
		list.AddRange(outThings);
		SkyfallerMaker.SpawnSkyfaller(ThingDefOf.InfestedMeteoriteIncoming_ExtraHives, (IEnumerable<Thing>)list, cell, val);
		LetterDef val3 = (list[list.Count - 1].def.building.isResourceRock ? LetterDefOf.PositiveEvent : LetterDefOf.NeutralEvent);
		string text = GenText.CapitalizeFirst(string.Format(base.def.letterText, ((Def)list[list.Count - 1].def).label));
		base.SendStandardLetter(base.def.letterLabel + ": " + ((Def)list[list.Count - 1].def).LabelCap, text, val3, parms, new TargetInfo(cell, val, false), Array.Empty<NamedArgument>());
		return true;
	}

	protected void Generate(out List<Thing> outThings)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		outThings = new List<Thing>();
		IntRange mineablesCountRange = ThingSetMaker_Meteorite.MineablesCountRange;
		int randomInRange = mineablesCountRange.RandomInRange;
		ThingDef mineableComponentsIndustrial = RimWorld.ThingDefOf.MineableComponentsIndustrial;
		for (int i = 0; i < randomInRange; i++)
		{
			Building val = (Building)ThingMaker.MakeThing(mineableComponentsIndustrial, (ThingDef)null);
			val.canChangeTerrainOnDestroyed = false;
			outThings.Add((Thing)(object)val);
		}
	}

	private unsafe bool TryFindCell(out IntVec3 cell, Map map)
	{
		cell = IntVec3.Invalid;
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			int maxMineables = ThingSetMaker_Meteorite.MineablesCountRange.max;
			return CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.InfestedMeteoriteIncoming_ExtraHives, map, TerrainAffordanceDefOf.Light, out cell, minDistToEdge: 10, nearLoc: default(IntVec3), nearLocMaxDist: -1, allowRoofedCells: true, allowCellsWithItems: false, allowCellsWithBuildings: false, colonyReachable: false, avoidColonistsIfExplosive: true, alwaysAvoidColonists: true, extraValidator: (Predicate<IntVec3>)delegate (IntVec3 x)
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				int num = Mathf.CeilToInt(Mathf.Sqrt((float)maxMineables)) + 2;
				CellRect val = CellRect.CenteredOn(x, num, num);
				int num2 = 0;
				foreach (IntVec3 current in val)
				{
					if (GenGrid.InBounds(current, map) && GenGrid.Standable(current, map))
					{
						num2++;
					}
				}
				return num2 >= maxMineables;
			});
		} 
	}

	protected IEnumerable<Faction> CandidateFactions(Map map, string Contains, bool desperate = false)
	{
		return Find.FactionManager.AllFactions.Where((Faction f) => FactionCanBeGroupSource(f, map, desperate) && (GenText.NullOrEmpty(Contains) || ((Def)f.def).defName.Contains(Contains)));
	}

	protected virtual bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
	{
		return !f.IsPlayer && !f.defeated && !f.temporary && (desperate || (f.def.allowedArrivalTemperatureRange.Includes(map.mapTemperature.OutdoorTemp) && f.def.allowedArrivalTemperatureRange.Includes(map.mapTemperature.SeasonalTemp) && (float)GenDate.DaysPassed >= f.def.earliestRaidDays));
	}
}
