using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ExtraHives;

public class PawnsArrivalModeWorker_RandomTunnel : PawnsArrivalModeWorker
{
	public override void Arrive(List<Pawn> pawns, IncidentParms parms)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Map val = (Map)parms.target;
		bool canRoofPunch = parms.faction != null && FactionUtility.HostileTo(parms.faction, Faction.OfPlayer);
		for (int i = 0; i < pawns.Count; i++)
		{
			TunnelRaidUtility.DropThingsNear(DropCellFinder.RandomDropSpot(val, true), val, Gen.YieldSingle<Thing>((Thing)(object)pawns[i]), parms.podOpenDelay, canInstaDropDuringInit: false, leaveSlag: true, canRoofPunch);
		}
	}

	public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!parms.raidArrivalModeForQuickMilitaryAid)
		{
			parms.podOpenDelay = 520;
		}
		parms.spawnRotation = Rot4.Random;
		return true;
	}
}
