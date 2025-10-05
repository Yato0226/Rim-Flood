using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ExtraHives;

public class PawnsArrivalModeWorker_EdgeTunnelGroups : PawnsArrivalModeWorker
{
	public override void Arrive(List<Pawn> pawns, IncidentParms parms)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		Map map = (Map)parms.target;
		bool canRoofPunch = parms.faction != null && FactionUtility.HostileTo(parms.faction, Faction.OfPlayer);
		List<Pair<List<Pawn>, IntVec3>> list = PawnsArrivalModeWorkerUtility.SplitIntoRandomGroupsNearMapEdge(pawns, map, arriveInPods: true);
		PawnsArrivalModeWorkerUtility.SetPawnGroupsInfo(parms, list);
		for (int i = 0; i < list.Count; i++)
		{
			TunnelRaidUtility.DropThingsNear(list[i].Second, map, list[i].First.Cast<Thing>(), parms.podOpenDelay, canInstaDropDuringInit: false, leaveSlag: true, canRoofPunch);
		}
	}

	public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		parms.spawnRotation = Rot4.Random;
		return true;
	}
}
