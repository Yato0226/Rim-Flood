using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace CrashedShipsExtension;

public class IncidentWorker_CrashedShipPart : IncidentWorker
{
	private const float ShipPointsFactor = 0.9f;

	private const int IncidentMinimumPoints = 300;

	private const float DefendRadius = 28f;

	protected override bool CanFireNowSub(IncidentParms parms)
	{
		if (((Map)parms.target).listerThings.ThingsOfDef(base.def?.mechClusterBuilding).Count > 0)
		{
			return false;
		}
		return true;
	}

	protected override bool TryExecuteWorker(IncidentParms parms)
	{
		Map map = (Map)parms.target;
		List<TargetInfo> list = new List<TargetInfo>();
		ThingSetMakerDefOf.Meteorite.root.Generate();
		ThingDef shipPartDef = base.def?.mechClusterBuilding;
		IntVec3 val = FindDropPodLocation(map, (IntVec3 spot) => CanPlaceAt(spot));
		if (val == IntVec3.Invalid)
		{
			return false;
		}
		float points = Mathf.Max(parms.points * 0.9f, 300f);
		List<Pawn> list2 = PawnGroupMakerUtility.GeneratePawns(new PawnGroupMakerParms
		{
			groupKind = PawnGroupKindDefOf.Combat,
			tile = map.Tile,
			faction = Faction.OfMechanoids,
			points = points
		}, true).ToList();
		Thing val2 = ThingMaker.MakeThing(shipPartDef);
		val2.SetFaction(Faction.OfMechanoids);
		LordMaker.MakeNewLord(Faction.OfMechanoids, new LordJob_SleepThenMechanoidsDefend(new List<Thing> { val2 }, Faction.OfMechanoids, 28f, val, false, false), map, list2);
		DropPodUtility.DropThingsNear(val, map, list2.Cast<Thing>(), 110, false, false, true, true);
		foreach (Pawn item in list2)
		{
			CompCanBeDormant obj = item.GetComp<CompCanBeDormant>();
			if (obj != null)
			{
				obj.ToSleep();
			}
		}
		list.AddRange(list2.Select(p => new TargetInfo(p)));
		GenSpawn.Spawn(SkyfallerMaker.MakeSkyfaller(ThingDefOf.MeteoriteIncoming, val2), val, map, WipeMode.Vanish);
		list.Add(new TargetInfo(val, map, false));
		SendStandardLetter(parms, new LookTargets(list), Array.Empty<NamedArgument>());
		return true;
		unsafe bool CanPlaceAt(IntVec3 loc)
		{
			CellRect val3 = GenAdj.OccupiedRect(loc, Rot4.North, shipPartDef.Size);
			if (GridsUtility.Fogged(loc, map) || !val3.InBounds(map))
			{
				return false;
			}
			if (!DropCellFinder.SkyfallerCanLandAt(loc, map, shipPartDef.Size, (Faction)null))
			{
				return false;
			}
			foreach (IntVec3 c in val3)
			{
				RoofDef roof = c.GetRoof(map);
				if (roof != null && roof.isNatural)
				{
					return false;
				}
			}
			return GenConstruct.CanBuildOnTerrain(shipPartDef, loc, map, Rot4.North);
		}
	}

	private static IntVec3 FindDropPodLocation(Map map, Predicate<IntVec3> validator)
	{
		for (int i = 0; i < 200; i++)
		{
			IntVec3 val = RCellFinder.FindSiegePositionFrom(DropCellFinder.FindRaidDropCenterDistant(map, true), map, true);
			if (validator(val))
			{
				return val;
			}
		}
		return IntVec3.Invalid;
	}
}
