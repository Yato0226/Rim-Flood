using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExtraHives;

public static class PawnsArrivalModeWorkerUtility
{
	private const int MaxGroupsCount = 3;

	public static void PlaceInTunnelsNearSpawnCenter(IncidentParms parms, List<Pawn> pawns)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Map map = (Map)parms.target;
		bool flag = parms.faction != null && FactionUtility.HostileTo(parms.faction, Faction.OfPlayer);
		TunnelRaidUtility.DropThingsNear(parms.spawnCenter, map, pawns.Cast<Thing>(), parms.podOpenDelay, canInstaDropDuringInit: false, leaveSlag: true, flag || parms.raidArrivalModeForQuickMilitaryAid, parms.faction);
	}

	public static List<Pair<List<Pawn>, IntVec3>> SplitIntoRandomGroupsNearMapEdge(List<Pawn> pawns, Map map, bool arriveInPods)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		List<Pair<List<Pawn>, IntVec3>> list = new List<Pair<List<Pawn>, IntVec3>>();
		if (!GenCollection.Any<Pawn>(pawns))
		{
			return list;
		}
		int maxGroupsCount = GetMaxGroupsCount(pawns.Count);
		Rand.PushState();
		int num = ((maxGroupsCount == 1) ? 1 : Rand.RangeInclusive(2, maxGroupsCount));
		Rand.PopState();
		Pair<List<Pawn>, IntVec3> item = default(Pair<List<Pawn>, IntVec3>);
		for (int i = 0; i < num; i++)
		{
			IntVec3 val = FindNewMapEdgeGroupCenter(map, list, arriveInPods);
			item = new Pair<List<Pawn>, IntVec3>(new List<Pawn>(), val);
			item.First.Add(pawns[i]);
			list.Add(item);
		}
		for (int j = num; j < pawns.Count; j++)
		{
			GenCollection.RandomElement<Pair<List<Pawn>, IntVec3>>((IEnumerable<Pair<List<Pawn>, IntVec3>>)list).First.Add(pawns[j]);
		}
		return list;
	}

	private static IntVec3 FindNewMapEdgeGroupCenter(Map map, List<Pair<List<Pawn>, IntVec3>> groups, bool arriveInPods)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 result = IntVec3.Invalid;
		float num = 0f;
		IntVec3 val = default(IntVec3);
		for (int i = 0; i < 4; i++)
		{
			if (arriveInPods)
			{
				val = DropCellFinder.FindRaidDropCenterDistant(map, false);
			}
			else if (!RCellFinder.TryFindRandomPawnEntryCell(out val, map, CellFinder.EdgeRoadChance_Hostile, false, (Predicate<IntVec3>)null))
			{
				val = DropCellFinder.FindRaidDropCenterDistant(map, false);
			}
			if (!GenCollection.Any<Pair<List<Pawn>, IntVec3>>(groups))
			{
				result = val;
				break;
			}
			float num2 = float.MaxValue;
			for (int j = 0; j < groups.Count; j++)
			{
				float num3 = IntVec3Utility.DistanceToSquared(val, groups[j].Second);
				if (num3 < num2)
				{
					num2 = num3;
				}
			}
			if (!result.IsValid || num2 > num)
			{
				num = num2;
				result = val;
			}
		}
		return result;
	}

	private static int GetMaxGroupsCount(int pawnsCount)
	{
		if (pawnsCount <= 1)
		{
			return 1;
		}
		return Mathf.Clamp(pawnsCount / 2, 2, 3);
	}

	public static void SetPawnGroupsInfo(IncidentParms parms, List<Pair<List<Pawn>, IntVec3>> groups)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		parms.pawnGroups = new Dictionary<Pawn, int>();
		for (int i = 0; i < groups.Count; i++)
		{
			for (int j = 0; j < groups[i].First.Count; j++)
			{
				parms.pawnGroups.Add(groups[i].First[j], i);
			}
		}
	}
}
