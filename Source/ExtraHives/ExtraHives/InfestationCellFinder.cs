using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExtraHives;

public static class InfestationCellFinder_ExtraHives
{
	private struct LocationCandidate
	{
		public IntVec3 cell;

		public float score;

		public LocationCandidate(IntVec3 cell, float score)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			this.cell = cell;
			this.score = score;
		}
	}

	public class InfestationParms
	{
		public int minClosedAreaSize;

		public bool requiresRoofed;

		public bool mustBeThickRoof;

		public bool mustBeVisable;

		public bool mustBeWalkable;

		public bool mustBeNearColony;

		public float maxColonyDistance;

		public float minColonyDistance;

		public float minMountainouseness;

		public float minTemp;

		public float maxTemp;

		public float bonusTempScore;

		public float? bonusAboveTemp;

		public float? bonusBelowTemp;

		public InfestationParms(HiveDefExtension HiveExt)
		{
			minClosedAreaSize = HiveExt.minClosedAreaSize;
			requiresRoofed = HiveExt.requiresRoofed;
			mustBeThickRoof = HiveExt.mustBeThickRoof;
			mustBeVisable = HiveExt.mustBeVisable;
			mustBeWalkable = HiveExt.mustBeWalkable;
			mustBeNearColony = HiveExt.mustBeNearColony;
			maxColonyDistance = HiveExt.maxColonyDistance;
			minColonyDistance = HiveExt.minColonyDistance;
			minMountainouseness = HiveExt.minMountainouseness;
			minTemp = HiveExt.minTemp ?? 17f;
			maxTemp = HiveExt.maxTemp ?? 40f;
			bonusTempScore = HiveExt.bonusTempScore;
			bonusAboveTemp = HiveExt.bonusAboveTemp;
			bonusBelowTemp = HiveExt.bonusBelowTemp;
		}

		public InfestationParms(HivelikeIncidentDef HiveExt)
		{
			minClosedAreaSize = HiveExt.minClosedAreaSize;
			requiresRoofed = HiveExt.requiresRoofed;
			mustBeThickRoof = HiveExt.mustBeThickRoof;
			mustBeVisable = HiveExt.mustBeVisable;
			mustBeWalkable = HiveExt.mustBeWalkable;
			mustBeNearColony = HiveExt.mustBeNearColony;
			maxColonyDistance = HiveExt.maxColonyDistance;
			minColonyDistance = HiveExt.minColonyDistance;
			minMountainouseness = HiveExt.minMountainouseness;
			minTemp = HiveExt.minTemp ?? 17f;
			maxTemp = HiveExt.maxTemp ?? 40f;
			bonusTempScore = HiveExt.bonusTempScore;
			bonusAboveTemp = HiveExt.bonusAboveTemp;
			bonusBelowTemp = HiveExt.bonusBelowTemp;
		}

		public virtual bool ExtraReqs(Map map, IntVec3 cell)
		{
			return true;
		}

		public virtual float Modifier(Map map, IntVec3 cell)
		{
			return 1f;
		}
	}

	private static List<LocationCandidate> locationCandidates = new List<LocationCandidate>();

	private static Dictionary<Region, float> regionsDistanceToUnroofed = new Dictionary<Region, float>();

	private static ByteGrid closedAreaSize;

	private static HashSet<Region> tempUnroofedRegions = new HashSet<Region>();

	public static bool TryFindCell(out IntVec3 cell, Map map, InfestationParms parms)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		CalculateLocationCandidates(map, parms);
		LocationCandidate locationCandidate = default(LocationCandidate);
		if (!GenCollection.TryRandomElementByWeight<LocationCandidate>((IEnumerable<LocationCandidate>)locationCandidates, (Func<LocationCandidate, float>)((LocationCandidate x) => x.score), out locationCandidate))
		{
			cell = IntVec3.Invalid;
			return false;
		}
		cell = CellFinder.FindNoWipeSpawnLocNear(locationCandidate.cell, map, ThingDefOf.Hive, Rot4.North, 2, (Predicate<IntVec3>)((IntVec3 x) => GetScoreAt(x, map, parms) > 0f && GridsUtility.GetFirstThing(x, map, ThingDefOf.Hive) == null && GridsUtility.GetFirstThing(x, map, ExtraHives.ThingDefOf.Tunneler_ExtraHives) == null));
		return true;
	}

	private static float GetScoreAt(IntVec3 cell, Map map, InfestationParms parms)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		return GetScoreAt(cell, map, parms.minClosedAreaSize, parms.requiresRoofed, parms.mustBeThickRoof, parms.mustBeVisable, parms.mustBeWalkable, parms.mustBeNearColony, parms.maxColonyDistance, parms.minColonyDistance, parms.minMountainouseness, parms.minTemp, parms.maxTemp, parms.bonusTempScore, parms.bonusAboveTemp, parms.bonusBelowTemp) * parms.Modifier(map, cell);
	}

	private static float GetScoreAt(IntVec3 cell, Map map, int minClosedAreaSize, bool requiresRoofed, bool mustBeThickRoof, bool mustBeVisable, bool mustBeWalkable, bool mustBeNearColony, float maxColonyDistance, float minColonyDistance, float minMountainouseness = 0f, float minTemp = 17f, float maxTemp = 40f, float bonusTempScore = 0f, float? bonusAboveTemp = null, float? bonusBelowTemp = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		if (mustBeNearColony && (float)(int)CellFinderUtility.DistToColonyBuilding[cell] > maxColonyDistance)
		{
			return 0f;
		}
		if ((float)(int)CellFinderUtility.DistToColonyBuilding[cell] < minColonyDistance)
		{
			return 0f;
		}
		if (!GenGrid.Walkable(cell, map) && mustBeWalkable)
		{
			return 0f;
		}
		if (GridsUtility.Fogged(cell, map) && mustBeVisable)
		{
			return 0f;
		}
		if (CellHasBlockingThings(cell, map))
		{
			return 0f;
		}
		if (requiresRoofed)
		{
			if (!GridsUtility.Roofed(cell, map))
			{
				return 0f;
			}
			if (mustBeThickRoof && !GridsUtility.GetRoof(cell, map).isThickRoof)
			{
				return 0f;
			}
		}
		Region region = GridsUtility.GetRegion(cell, map, (RegionType)14);
		if (region == null)
		{
			return 0f;
		}
		if (closedAreaSize[cell] < minClosedAreaSize)
		{
			return 0f;
		}
		float temperature = GridsUtility.GetTemperature(cell, map);
		if (temperature < minTemp)
		{
			return 0f;
		}
		if (temperature > maxTemp)
		{
			return 0f;
		}
		float num = 0f;
		if (bonusTempScore != 0f)
		{
			if (bonusAboveTemp.HasValue && temperature > bonusAboveTemp.Value)
			{
				num = bonusTempScore;
			}
			if (bonusBelowTemp.HasValue && temperature < bonusBelowTemp.Value)
			{
				num = bonusTempScore;
			}
		}
		float num2 = GetMountainousnessScoreAt(cell, map);
		if (minMountainouseness == 0f)
		{
			num2 = 1f;
		}
		else if (num2 < minMountainouseness)
		{
			return 0f;
		}
		int num3 = StraightLineDistToUnroofed(cell, map);
		float num4 = (regionsDistanceToUnroofed.TryGetValue(region, out num4) ? Mathf.Min(num4, (float)num3 * 4f) : ((float)num3 * 1.15f));
		num4 = Mathf.Pow(num4, 1.55f);
		float num5 = Mathf.InverseLerp(0f, 12f, (float)num3);
		float num6 = Mathf.Lerp(1f, 0.18f, map.glowGrid.GroundGlowAt(cell, false, false));
		float num7 = 1f - Mathf.Clamp(DistToBlocker(cell, map) / 11f, 0f, 0.6f);
		float num8 = Mathf.InverseLerp(-17f, -7f, temperature);
		float num9 = num4 * num5 * num7 * num2 * num6 * num8;
		num9 = Mathf.Pow(num9, 1.2f);
		if (num9 < 7.5f)
		{
			return 0f;
		}
		return num9;
	}

	private static void CalculateLocationCandidates(Map map, InfestationParms parms)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		locationCandidates.Clear();
		CalculateTraversalDistancesToUnroofed(map);
		CalculateClosedAreaSizeGrid(map);
		CellFinderUtility.CalculateDistanceToColonyBuildingGrid(map);
		IntVec3 cell = default(IntVec3);
		for (int i = 0; i < map.Size.z; i++)
		{
			for (int j = 0; j < map.Size.x; j++)
			{
				cell = new IntVec3(j, 0, i);
				float scoreAt = GetScoreAt(cell, map, parms);
				if (!(scoreAt <= 0f))
				{
					locationCandidates.Add(new LocationCandidate(cell, scoreAt));
				}
			}
		}
	}

	private static bool CellHasBlockingThings(IntVec3 cell, Map map)
	{
		List<Thing> thingList = cell.GetThingList(map);
		for (int i = 0; i < thingList.Count; i++)
		{
			if (thingList[i] is Pawn || thingList[i] is Hive || thingList[i] is TunnelHiveSpawner)
			{
				return true;
			}
			if (thingList[i].def.category == ThingCategory.Building && thingList[i].def.passability == Traversability.Impassable && GenSpawn.SpawningWipes(ThingDefOf.Hive, thingList[i].def))
			{
				return true;
			}
		}
		return false;
	}

	private static int StraightLineDistToUnroofed(IntVec3 cell, Map map)
	{
		int num = int.MaxValue;
		for (int i = 0; i < 4; i++)
		{
			int num2 = 0;
			IntVec3 facingCell = new Rot4(i).FacingCell;
			int num3 = 0;
			while (true)
			{
				IntVec3 intVec = cell + facingCell * num3;
				if (!intVec.InBounds(map))
				{
					num2 = int.MaxValue;
					break;
				}
				num2 = num3;
				if (NoRoofAroundAndWalkable(intVec, map))
				{
					break;
				}
				num3++;
			}
			if (num2 < num)
			{
				num = num2;
			}
		}
		if (num == int.MaxValue)
		{
			return map.Size.x;
		}
		return num;
	}

	private static bool NoRoofAroundAndWalkable(IntVec3 cell, Map map)
	{
		if (!cell.Walkable(map))
		{
			return false;
		}
		if (cell.Roofed(map))
		{
			return false;
		}
		for (int i = 0; i < 4; i++)
		{
			IntVec3 c = new Rot4(i).FacingCell + cell;
			if (c.InBounds(map) && c.Roofed(map))
			{
				return false;
			}
		}
		return true;
	}

	private static float DistToBlocker(IntVec3 cell, Map map)
	{
		int num = int.MinValue;
		int num2 = int.MinValue;
		for (int i = 0; i < 4; i++)
		{
			int num3 = 0;
			IntVec3 facingCell = new Rot4(i).FacingCell;
			int num4 = 0;
			while (true)
			{
				IntVec3 c = cell + facingCell * num4;
				num3 = num4;
				if (!c.InBounds(map) || !c.Walkable(map))
				{
					break;
				}
				num4++;
			}
			if (num3 > num)
			{
				num2 = num;
				num = num3;
			}
			else if (num3 > num2)
			{
				num2 = num3;
			}
		}
		return Mathf.Min(num, num2);
	}

	private static float GetMountainousnessScoreAt(IntVec3 cell, Map map)
	{
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < 700; i += 10)
		{
			IntVec3 c = cell + GenRadial.RadialPattern[i];
			if (c.InBounds(map))
			{
				Building edifice = c.GetEdifice(map);
				if (edifice != null && edifice.def.category == ThingCategory.Building && edifice.def.building.isNaturalRock)
				{
					num += 1f;
				}
				else if (c.Roofed(map) && c.GetRoof(map).isThickRoof)
				{
					num += 0.5f;
				}
				num2++;
			}
		}
		return num / (float)num2;
	}

	private static void CalculateTraversalDistancesToUnroofed(Map map)
	{
		tempUnroofedRegions.Clear();
		for (int i = 0; i < map.Size.z; i++)
		{
			for (int j = 0; j < map.Size.x; j++)
			{
				IntVec3 intVec = new IntVec3(j, 0, i);
				Region region = intVec.GetRegion(map);
				if (region != null && NoRoofAroundAndWalkable(intVec, map))
				{
					tempUnroofedRegions.Add(region);
				}
			}
		}
		Dijkstra<Region>.Run(tempUnroofedRegions, (Region x) => x.Neighbors, (Region a, Region b) => Mathf.Sqrt(a.extentsClose.CenterCell.DistanceToSquared(b.extentsClose.CenterCell)), regionsDistanceToUnroofed);
		tempUnroofedRegions.Clear();
	}

	private static void CalculateClosedAreaSizeGrid(Map map)
	{
		if (closedAreaSize == null)
		{
			closedAreaSize = new ByteGrid(map);
		}
		else
		{
			closedAreaSize.ClearAndResizeTo(map);
		}
		for (int i = 0; i < map.Size.z; i++)
		{
			for (int j = 0; j < map.Size.x; j++)
			{
				IntVec3 intVec = new IntVec3(j, 0, i);
				if (closedAreaSize[j, i] == 0 && !intVec.Impassable(map))
				{
					int area = 0;
					map.floodFiller.FloodFill(intVec, (Predicate<IntVec3>)((IntVec3 c) => !c.Impassable(map)), (Action<IntVec3>)delegate
					{
						area++;
					}, int.MaxValue, rememberParents: false, (IEnumerable<IntVec3>)null);
					area = Mathf.Min(area, 255);
					map.floodFiller.FloodFill(intVec, (IntVec3 c) => !c.Impassable(map), delegate(IntVec3 c)
					{
						closedAreaSize[c] = (byte)area;
					});
				}
			}
		}
	}

}
