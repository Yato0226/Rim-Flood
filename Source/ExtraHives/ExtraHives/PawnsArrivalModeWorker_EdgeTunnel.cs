using System.Collections.Generic;
using RimWorld;
using Verse;
using RimWorld.Planet;

namespace ExtraHives;

public class PawnsArrivalModeWorker_EdgeTunnel : PawnsArrivalModeWorker
{
	public override void Arrive(List<Pawn> pawns, IncidentParms parms)
	{
		PawnsArrivalModeWorkerUtility.PlaceInTunnelsNearSpawnCenter(parms, pawns);
	}

	public override void TravellingTransportersArrived(List<ActiveTransporterInfo> dropPods, Map map)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 near = DropCellFinder.FindRaidDropCenterDistant(map, false);
		TunnelsArrivalActionUtility.PlaceTravelingTunnelers(dropPods, near, map);
	}

	public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Map val = (Map)parms.target;
		if (!parms.spawnCenter.IsValid)
		{
			parms.spawnCenter = DropCellFinder.FindRaidDropCenterDistant(val, false);
		}
		parms.spawnRotation = Rot4.Random;
		return true;
	}
}
