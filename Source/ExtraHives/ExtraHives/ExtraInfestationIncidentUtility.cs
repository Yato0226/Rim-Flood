using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ExtraHives;

public static class ExtraInfestationIncidentUtility
{
	public static void GetUsableDeepDrills(Map map, List<Thing> outDrills)
	{
		outDrills.Clear();
		List<Thing> list = map.listerThings.ThingsInGroup((ThingRequestGroup)49);
		Faction ofPlayer = Faction.OfPlayer;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Faction == ofPlayer && ThingCompUtility.TryGetComp<CompCreatesInfestations>(list[i]).CanCreateInfestationNow)
			{
				outDrills.Add(list[i]);
			}
		}
	}

	public static void GetUsableGrowZones(Map map, List<Zone_Growing> outGrowZones)
	{
		outGrowZones.Clear();
		List<Zone> allZones = map.zoneManager.AllZones;
		Faction ofPlayer = Faction.OfPlayer;
		for (int i = 0; i < allZones.Count; i++)
		{
			if (allZones[i] is Zone_Growing && GenCollection.Any<IntVec3>(allZones[i].Cells, (Predicate<IntVec3>)((IntVec3 x) => GridsUtility.UsesOutdoorTemperature(x, map) || !GridsUtility.Roofed(x, map))))
			{
				Zone obj = allZones[i];
				outGrowZones.Add((Zone_Growing)(object)((obj is Zone_Growing) ? obj : null));
			}
		}
	}
}
