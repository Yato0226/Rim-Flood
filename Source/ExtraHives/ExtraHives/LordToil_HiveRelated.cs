using System;
using System.Collections.Generic;
using ExtraHives.ExtensionMethods;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ExtraHives;

public abstract class LordToil_HiveRelated : LordToil
{
	private LordToil_HiveRelatedData Data => (LordToil_HiveRelatedData)(object)base.data;

	public LordToil_HiveRelated()
	{
		base.data = (LordToilData)(object)new LordToil_HiveRelatedData();
	}

	protected void FilterOutUnspawnedHives()
	{
		GenCollection.RemoveAll<Pawn, Hive>(Data.assignedHives, (Predicate<KeyValuePair<Pawn, Hive>>)((KeyValuePair<Pawn, Hive> x) => x.Value == null || !((Thing)x.Value).Spawned));
	}

	protected Hive GetHiveFor(Pawn pawn)
	{
		if (Data.assignedHives.TryGetValue(pawn, out var value))
		{
			return value;
		}
		value = FindClosestHive(pawn);
		if (value != null)
		{
			Data.assignedHives.Add(pawn, value);
		}
		return value;
	}

	private Hive FindClosestHive(Pawn pawn)
	{
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		ThingDef val = ThingDefOf.Hive;
		bool flag = ((Thing)pawn).Faction != null;

		FactionDef val2 = (flag ? ((Thing)pawn).Faction.def : null);
		if (val2 == null)
		{
			return null;
		}
		List<ThingDef> list = val2.HivedefsFor();
		if (((Thing)pawn).Faction != null && !GenList.NullOrEmpty<ThingDef>((IList<ThingDef>)list))
		{
			foreach (ThingDef item in list)
			{
				val = item;
				if (GenClosest.ClosestThingReachable(((Thing)pawn).Position, ((Thing)pawn).Map, ThingRequest.ForDef(val), (PathEndMode)2, TraverseParms.For(pawn, (Danger)3, (TraverseMode)0, false, false, false), 30f, (Predicate<Thing>)((Thing x) => x.Faction == ((Thing)pawn).Faction), (IEnumerable<Thing>)null, 0, 30, false, (RegionType)14, false) is Hive result)
				{
					return result;
				}
			}
		}
		return GenClosest.ClosestThingReachable(((Thing)pawn).Position, ((Thing)pawn).Map, ThingRequest.ForDef(val), (PathEndMode)2, TraverseParms.For(pawn, (Danger)3, (TraverseMode)0, false, false, false), 30f, (Predicate<Thing>)((Thing x) => x.Faction == ((Thing)pawn).Faction), (IEnumerable<Thing>)null, 0, 30, false, (RegionType)14, false) as Hive;
	}
}
