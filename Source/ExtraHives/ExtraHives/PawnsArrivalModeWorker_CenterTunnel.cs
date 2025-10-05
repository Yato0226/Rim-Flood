using System.Collections.Generic;
using RimWorld;
using Verse;
using RimWorld.Planet;

namespace ExtraHives;

public class PawnsArrivalModeWorker_CenterTunnel : PawnsArrivalModeWorker
{
	public const int PodOpenDelay = 520;

	public override void Arrive(List<Pawn> pawns, IncidentParms parms)
	{
		PawnsArrivalModeWorkerUtility.PlaceInTunnelsNearSpawnCenter(parms, pawns);
	}

	public override void TravellingTransportersArrived(List<ActiveTransporterInfo> dropPods, Map map)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 near = default(IntVec3);
		if (!DropCellFinder.TryFindRaidDropCenterClose(out near, map, true, true, true, -1))
		{
			near = DropCellFinder.FindRaidDropCenterDistant(map, false);
		}
		TunnelsArrivalActionUtility.PlaceTravelingTunnelers(dropPods, near, map);
	}

	public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		Map val = (Map)parms.target;
		if (!parms.raidArrivalModeForQuickMilitaryAid)
		{
			parms.podOpenDelay = 520;
		}
		parms.spawnRotation = Rot4.Random;
		if (!parms.spawnCenter.IsValid)
		{
			bool flag = parms.faction == Faction.OfMechanoids;
			bool flag2 = parms.faction != null && FactionUtility.HostileTo(parms.faction, Faction.OfPlayer);
			Rand.PushState();
			if (Rand.Chance(0.4f) && !flag && val.listerBuildings.ColonistsHaveBuildingWithPowerOn(DefDatabase<ThingDef>.GetNamed("OrbitalTradeBeacon")))
			{
				parms.spawnCenter = DropCellFinder.TradeDropSpot(val);
			}
			else if (!DropCellFinder.TryFindRaidDropCenterClose(out parms.spawnCenter, val, !flag && flag2, !flag, true, -1))
			{
				parms.raidArrivalMode = (Rand.Chance(0.75f) ? PawnsArrivalModeDefOf.EdgeTunnelIn_ExtraHives : PawnsArrivalModeDefOf.EdgeTunnelInGroups_ExtraHives);
				return parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms);
			}
			Rand.PopState();
		}
		return true;
	}
}
