using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExtraHives;

public class IncidentWorker_GrowZoneInfestation : IncidentWorker
{
	private static List<Zone_Growing> tmpZones = new List<Zone_Growing>();

	private const float MinPointsFactor = 0.3f;

	private const float MaxPointsFactor = 0.6f;

	private const float MinPoints = 200f;

	private const float MaxPoints = 1000f;

	protected override bool CanFireNowSub(IncidentParms parms)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		if (base.def.mechClusterBuilding == null)
		{
			Log.Error("Hivedef (def.mechClusterBuilding) not set");
			return false;
		}
		if (!((Def)base.def.mechClusterBuilding).HasModExtension<HiveDefExtension>())
		{
			Log.Error("Hivedef (def.mechClusterBuilding) missing HiveExtension");
			return false;
		}
		if (!base.CanFireNowSub(parms))
		{
			Log.Error("!base.CanFireNowSub");
			return false;
		}
		Map map = (Map)parms.target;
		tmpZones.Clear();
		ExtraInfestationIncidentUtility.GetUsableGrowZones(map, tmpZones);
		return true;
	}

	protected override bool TryExecuteWorker(IncidentParms parms)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		Map map = (Map)parms.target;
		if (base.def.mechClusterBuilding == null)
		{
			Log.Error("Hivedef (def.mechClusterBuilding) not set");
			return false;
		}
		if (!((Def)base.def.mechClusterBuilding).HasModExtension<HiveDefExtension>())
		{
			Log.Error("Hivedef (def.mechClusterBuilding) missing HiveExtension");
			return false;
		}
		ThingDef hiveDef = base.def.mechClusterBuilding;
		HiveDefExtension hive = ((Def)hiveDef).GetModExtension<HiveDefExtension>();
		if (hive == null)
		{
			return false;
		}
		if (parms.faction == null)
		{
			try
			{
				parms.faction = GenCollection.RandomElement<Faction>(Find.FactionManager.AllFactions.Where((Faction x) => ((Def)x.def).defName.Contains(((Def)hive.Faction).defName)));
			}
			catch (Exception)
			{
				parms.faction = Find.FactionManager.FirstFactionOfDef(hive.Faction);
			}
		}
		ThingDef val = hive.TunnelDef ?? ThingDefOf.Tunneler_ExtraHives;
		tmpZones.Clear();
		ExtraInfestationIncidentUtility.GetUsableGrowZones(map, tmpZones);
		IntVec3 val2 = IntVec3.Invalid;
		Zone_Growing val3 = default(Zone_Growing);
		if (GenCollection.TryRandomElementByWeight<Zone_Growing>((IEnumerable<Zone_Growing>)tmpZones, (Func<Zone_Growing, float>)((Zone_Growing x) => ((Zone)x).Cells.Count), out val3))
		{
			val2 = CellFinder.FindNoWipeSpawnLocNear(GenCollection.RandomElement<IntVec3>((IEnumerable<IntVec3>)((Zone)val3).Cells), map, val, Rot4.North, 2, (Predicate<IntVec3>)((IntVec3 x) => GenGrid.Walkable(x, map) && GridsUtility.GetFirstThing(x, map, hiveDef) == null && GridsUtility.GetFirstThingWithComp<ThingComp>(x, map) == null && GridsUtility.GetFirstThing(x, map, ThingDefOf.Hive) == null && GridsUtility.GetFirstThing(x, map, ThingDefOf.Tunneler_ExtraHives) == null && !GridsUtility.Roofed(x, map) && GridsUtility.UsesOutdoorTemperature(x, map)));
			if (val2 == ((Zone)val3).Position)
			{
				Log.Error("intVec == growZone.Position");
				return false;
			}
		}
		else
		{
			RCellFinder.TryFindRandomPawnEntryCell(out val2, map, 0f, false, (Predicate<IntVec3>)null);
			if (RCellFinder.TryFindRandomSpotJustOutsideColony(val2, map, out val2))
			{
				Log.Warning("Found spot outside colony");
			}
			else
			{
				Log.Warning("failed to find interesting location, use map edge");
			}
		}
		if (val2 == IntVec3.Invalid)
		{
			Log.Error("intVec == IntVec3.Invalid");
			return false;
		}
		float num = ((float?)base.def.mechClusterBuilding.GetCompProperties<CompProperties_SpawnerPawn>()?.initialPawnsPoints) ?? 250f;
		Thing val4 = InfestationUtility.SpawnTunnels(base.def.mechClusterBuilding, Mathf.Max(GenMath.RoundRandom(parms.points / num), 1), map, val2, spawnAnywhereIfNoGoodCell: true, ignoreRoofedRequirement: true, null, parms.faction);
		base.SendStandardLetter(parms, new TargetInfo(val2, map, false), Array.Empty<NamedArgument>());
		return true;
	}
}
