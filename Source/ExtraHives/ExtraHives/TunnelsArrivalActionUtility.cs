using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ExtraHives;

internal class TunnelsArrivalActionUtility
{
	public static void PlaceTravelingTunnelers(List<RimWorld.ActiveTransporterInfo> dropPods, IntVec3 near, Map map)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		TransportersArrivalActionUtility.RemovePawnsFromWorldPawns(dropPods);
		IntVec3 c = default(IntVec3);
		for (int i = 0; i < dropPods.Count; i++)
		{
			DropCellFinder.TryFindDropSpotNear(near, map, out c, false, true, true, (IntVec2?)null, true);
			TunnelRaidUtility.MakeTunnelAt(c, map, dropPods[i]);
		}
	}
}
