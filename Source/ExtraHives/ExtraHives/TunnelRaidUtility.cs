using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using RimWorld.Planet;

namespace ExtraHives;

public static class TunnelRaidUtility
{
	private static List<List<Thing>> tempList = new List<List<Thing>>();

	public static string TunnelerArrivalmode()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		return Translator.Translate("HE_Tunnelers");
	}

	public static void MakeTunnelAt(IntVec3 c, Map map, ActiveTransporterInfo info, Faction faction = null)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		ThingDef tunneler_ExtraHives = ThingDefOf.Tunneler_ExtraHives;
		TunnelRaidSpawner tunnelRaidSpawner = (TunnelRaidSpawner)(object)ThingMaker.MakeThing(tunneler_ExtraHives, (ThingDef)null);
		if (tunnelRaidSpawner.SpawnedFaction == null)
		{
			tunnelRaidSpawner.SpawnedFaction = faction;
			if (tunnelRaidSpawner.SpawnedFaction == null)
			{
			}
		}
		foreach (Thing item in (IEnumerable<Thing>)info.innerContainer)
		{
			tunnelRaidSpawner.GetDirectlyHeldThings().TryAddOrTransfer(item, false);
		}
		GenSpawn.Spawn((Thing)(object)tunnelRaidSpawner, c, map, (WipeMode)0);
	}

	public static void DropThingsNear(IntVec3 dropCenter, Map map, IEnumerable<Thing> things, int openDelay = 110, bool canInstaDropDuringInit = false, bool leaveSlag = false, bool canRoofPunch = true, Faction faction = null)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		tempList.Clear();
		foreach (Thing thing in things)
		{
			List<Thing> item = new List<Thing> { thing };
			tempList.Add(item);
		}
		DropThingGroupsNear(dropCenter, map, tempList, openDelay, canInstaDropDuringInit, leaveSlag, canRoofPunch, faction);
		tempList.Clear();
	}

	public static void DropThingGroupsNear(IntVec3 dropCenter, Map map, List<List<Thing>> thingsGroups, int openDelay = 110, bool instaDrop = false, bool leaveSlag = false, bool canRoofPunch = true, Faction faction = null)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 val = default(IntVec3);
		foreach (List<Thing> thingsGroup in thingsGroups)
		{
			List<Thing> list = thingsGroup.Where((Thing x) => x.def.thingClass == typeof(Pawn)).ToList();
			if (!DropCellFinder.TryFindDropSpotNear(dropCenter, map, out val, true, canRoofPunch, true, (IntVec2?)null, true))
			{
				Log.Warning(string.Concat("DropThingsNear failed to find a place to drop ", thingsGroup.FirstOrDefault(), " near ", dropCenter, ". Dropping on random square instead."));
				val = CellFinderLoose.RandomCellWith((Predicate<IntVec3>)((IntVec3 c) => GenGrid.Walkable(c, map) && GridsUtility.Roofed(c, map) && GridsUtility.GetRoof(c, map) != RoofDefOf.RoofRockThick), map, 1000);
			}
			for (int num = 0; num < thingsGroup.Count; num++)
			{
				ForbidUtility.SetForbidden(thingsGroup[num], true, false);
			}
			if (instaDrop)
			{
				foreach (Thing item in thingsGroup)
				{
					GenPlace.TryPlaceThing(item, val, map, (ThingPlaceMode)1, (Action<Thing, int>)null, (Predicate<IntVec3>)null, default(Rot4));
				}
				continue;
			}
			ActiveTransporterInfo val2 = new ActiveTransporterInfo();
			foreach (Thing item2 in thingsGroup)
			{
				val2.innerContainer.TryAddOrTransfer(item2, true);
			}
			val2.openDelay = openDelay;
			val2.leaveSlag = leaveSlag;
			MakeTunnelAt(val, map, val2, faction);
		}
	}

	public static void MakeTunnelAt(IntVec3 c, Map map, List<Thing> info)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		ThingDef named = DefDatabase<ThingDef>.GetNamed("TunnelerDef", true);
		TunnelRaidSpawner tunnelRaidSpawner = (TunnelRaidSpawner)(object)ThingMaker.MakeThing(named, (ThingDef)null);
		foreach (Thing item in info)
		{
			tunnelRaidSpawner.GetDirectlyHeldThings().TryAddOrTransfer(item, false);
		}
		GenSpawn.Spawn((Thing)(object)tunnelRaidSpawner, c, map, (WipeMode)0);
	}
}
