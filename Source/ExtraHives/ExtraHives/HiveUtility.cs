using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ExtraHives;

public static class HiveUtility
{
	private const float HivePreventsClaimingInRadius = 2f;

	public static int TotalSpawnedHivesCount(Map map, bool filterFogged = false, ThingDef thingDef = null)
	{
		ThingDef val = thingDef ?? ThingDefOf.Hive;
		List<Thing> list = map.listerThings.ThingsOfDef(val);
		if (filterFogged)
		{
			return list.Where((Thing h) => !GridsUtility.Fogged(h.Position, h.Map)).Count();
		}
		return list.Count;
	}

	public static int TotalSpawnedHivesCount(Map map, ThingDef thingDef = null)
	{
		ThingDef val = thingDef ?? ThingDefOf.Hive;
		return map.listerThings.ThingsOfDef(val).Count;
	}

	public static bool AnyHivePreventsClaiming(Thing thing)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!thing.Spawned)
		{
			return false;
		}
		int num = GenRadial.NumCellsInRadius(2f);
		for (int i = 0; i < num; i++)
		{
			IntVec3 val = thing.Position + GenRadial.RadialPattern[i];
			if (GenGrid.InBounds(val, thing.Map) && GridsUtility.GetFirstThing<Thing>(val, thing.Map) != null)
			{
				return true;
			}
		}
		return false;
	}

	public static void Notify_HiveDespawned(Hive hive, Map map)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		int num = GenRadial.NumCellsInRadius(2f);
		for (int i = 0; i < num; i++)
		{
			IntVec3 val = ((Thing)hive).Position + GenRadial.RadialPattern[i];
			if (!GenGrid.InBounds(val, map))
			{
				continue;
			}
			List<Thing> thingList = GridsUtility.GetThingList(val, map);
			for (int j = 0; j < thingList.Count; j++)
			{
				if (thingList[j].Faction == Faction.OfInsects && !AnyHivePreventsClaiming(thingList[j]) && !(thingList[j] is Pawn))
				{
					thingList[j].SetFaction((Faction)null, (Pawn)null);
				}
			}
		}
	}
}
