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

	public override bool CanFireNowSub(IncidentParms parms)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (((Map)parms.target).listerThings.ThingsOfDef(base.def?.mechClusterBuilding).Count > 0)
		{
			return false;
		}
		return true;
	}

	public override bool TryExecuteWorker(IncidentParms parms)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
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
		Thing val2 = ThingMaker.MakeThing(shipPartDef, (ThingDef)null);
		val2.SetFaction(Faction.OfMechanoids, (Pawn)null);
		LordMaker.MakeNewLord(Faction.OfMechanoids, (LordJob)new LordJob_SleepThenMechanoidsDefend(new List<Thing> { val2 }, Faction.OfMechanoids, 28f, val, false, false), map, (IEnumerable<Pawn>)list2);
		DropPodUtility.DropThingsNear(val, map, list2.Cast<Thing>(), 110, false, false, true, true);
		foreach (Pawn item in list2)
		{
			CompCanBeDormant obj = ThingCompUtility.TryGetComp<CompCanBeDormant>((Thing)(object)item);
			if (obj != null)
			{
				obj.ToSleep();
			}
		}
		list.AddRange(((IEnumerable<Pawn>)list2).Select((Func<Pawn, TargetInfo>)((Pawn p) => new TargetInfo((Thing)(object)p))));
		GenSpawn.Spawn((Thing)(object)SkyfallerMaker.MakeSkyfaller(ThingDefOf.MeteoriteIncoming, val2), val, map, (WipeMode)0);
		list.Add(new TargetInfo(val, map, false));
		((IncidentWorker)this).SendStandardLetter(parms, LookTargets.op_Implicit(list), Array.Empty<NamedArgument>());
		return true;
		unsafe bool CanPlaceAt(IntVec3 loc)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			CellRect val3 = GenAdj.OccupiedRect(loc, Rot4.North, ((BuildableDef)shipPartDef).Size);
			if (GridsUtility.Fogged(loc, map) || !((CellRect)(ref val3)).InBounds(map))
			{
				return false;
			}
			if (!DropCellFinder.SkyfallerCanLandAt(loc, map, ((BuildableDef)shipPartDef).Size, (Faction)null))
			{
				return false;
			}
			Enumerator enumerator2 = ((CellRect)(ref val3)).GetEnumerator();
			try
			{
				while (((Enumerator)(ref enumerator2)).MoveNext())
				{
					RoofDef roof = GridsUtility.GetRoof(((Enumerator)(ref enumerator2)).Current, map);
					if (roof != null && roof.isNatural)
					{
						return false;
					}
				}
			}
			finally
			{
				((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to .constrained prefix*/).Dispose();
			}
			return GenConstruct.CanBuildOnTerrain((BuildableDef)(object)shipPartDef, loc, map, Rot4.North, (Thing)null, (ThingDef)null);
		}
	}

	private static IntVec3 FindDropPodLocation(Map map, Predicate<IntVec3> validator)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
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
