using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace ExtraHives.GenStuff;

public class SymbolResolver_HiveInternals : SymbolResolver
{
	private List<IntVec3> cells = new List<IntVec3>();

	private List<IntVec3> cavecells = new List<IntVec3>();

	private List<IntVec3> bigCaveCenters = new List<IntVec3>();

	private List<IntVec3> smallCaveCenters = new List<IntVec3>();

	private List<IntVec3> cellforbigcave = new List<IntVec3>();

	private List<IntVec3> cellforlittlecave = new List<IntVec3>();

	private List<IntVec3> entranceCaveCenters = new List<IntVec3>();

	private Faction Faction = null;

	private ModuleBase directionNoise;

	private static HashSet<IntVec3> tmpGroupSet = new HashSet<IntVec3>();

	private static readonly FloatRange BranchedTunnelWidthOffset = new FloatRange(0.2f, 0.4f);

	private static readonly SimpleCurve TunnelsWidthPerRockCount;

	private static List<IntVec3> tmpCells;

	private static HashSet<IntVec3> groupSet;

	private static HashSet<IntVec3> groupVisited;

	private static List<IntVec3> subGroup;

	public int SeedPart => 647814558;

	public override void Resolve(ResolveParams rp)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		Faction = rp.faction;
		HiveFactionExtension modExtension = ((Def)Faction.def).GetModExtension<HiveFactionExtension>();
		cells.Clear();
		cavecells.Clear();
		bigCaveCenters.Clear();
		smallCaveCenters.Clear();
		cellforbigcave.Clear();
		cellforlittlecave.Clear();
		entranceCaveCenters.Clear();
		Map map = BaseGen.globalSettings.map;
		IntVec3 CenterCell = rp.rect.CenterCell;
		cavecells.AddRange(GenRadial.RadialCellsAround(CenterCell, 10f, true));
		float dist = IntVec3Utility.DistanceTo(rp.rect.Corners.ToList()[0], rp.rect.Corners.ToList()[2]);
		cells = map.AllCells.Where((IntVec3 x) => IntVec3Utility.DistanceTo(x, CenterCell) <= dist).ToList();
		List<IntVec3> source = cells.Where((IntVec3 x) => IntVec3Utility.DistanceTo(x, CenterCell) > 15f && IntVec3Utility.DistanceTo(x, CenterCell) < dist - 10f).ToList();
		cellforbigcave = source.Where((IntVec3 x) => IntVec3Utility.DistanceTo(x, CenterCell) > 15f && IntVec3Utility.DistanceTo(x, CenterCell) < dist / 2f).ToList();
		cellforlittlecave = source.Where((IntVec3 x) => IntVec3Utility.DistanceTo(x, CenterCell) > dist / 2f && IntVec3Utility.DistanceTo(x, CenterCell) < dist - 10f).ToList();
		float entranceChance = 1f;
		GenerateQuad(CenterCell, Rot4.North, cells.Where((IntVec3 x) => IntVec3Utility.DistanceTo(x, CenterCell) <= dist && NorthQuad(x, CenterCell)).ToList(), entranceChance, dist, modExtension.ActiveStage);
		GenerateQuad(CenterCell, Rot4.South, cells.Where((IntVec3 x) => IntVec3Utility.DistanceTo(x, CenterCell) <= dist && SouthQuad(x, CenterCell)).ToList(), entranceChance, dist, modExtension.ActiveStage);
		GenerateQuad(CenterCell, Rot4.East, cells.Where((IntVec3 x) => IntVec3Utility.DistanceTo(x, CenterCell) <= dist && EastQuad(x, CenterCell)).ToList(), entranceChance, dist, modExtension.ActiveStage);
		GenerateQuad(CenterCell, Rot4.West, cells.Where((IntVec3 x) => IntVec3Utility.DistanceTo(x, CenterCell) <= dist && WestQuad(x, CenterCell)).ToList(), entranceChance, dist, modExtension.ActiveStage);
		foreach (IntVec3 cavecell in cavecells)
		{
			List<Thing> thingList = GridsUtility.GetThingList(cavecell, map);
			for (int num = 0; num < thingList.Count; num++)
			{
				Thing val = thingList[num];
				if (val != null)
				{
					val.Destroy((DestroyMode)0);
				}
			}
		}
		if (modExtension == null)
		{
			return;
		}
		if (modExtension.centerCaveHive != null)
		{
			Thing val2 = GenSpawn.Spawn(ThingMaker.MakeThing(modExtension.centerCaveHive, (ThingDef)null), CenterCell, map, (WipeMode)0);
			val2.SetFaction(Faction, (Pawn)null);
		}
		if (modExtension.smallCaveHive != null)
		{
			foreach (IntVec3 smallCaveCenter in smallCaveCenters)
			{
				Thing val3 = GenSpawn.Spawn(ThingMaker.MakeThing(modExtension.smallCaveHive, (ThingDef)null), smallCaveCenter, map, (WipeMode)0);
				val3.SetFaction(Faction, (Pawn)null);
			}
		}
		if (modExtension.largeCaveHive == null)
		{
			return;
		}
		foreach (IntVec3 bigCaveCenter in bigCaveCenters)
		{
			Thing val4 = GenSpawn.Spawn(ThingMaker.MakeThing(modExtension.largeCaveHive, (ThingDef)null), bigCaveCenter, map, (WipeMode)0);
			val4.SetFaction(Faction, (Pawn)null);
		}
	}

	public void GenerateQuad(IntVec3 CenterCell, Rot4 rot, List<IntVec3> QuadCells, float entranceChance, float radius, int minCaves)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		Map map = BaseGen.globalSettings.map;
		List<IntVec3> list = new List<IntVec3>();
		list.Add(CenterCell);
		Rand.PushState();
		float num = Rand.RangeInclusive(7, 10);
		Rand.PopState();
		IntVec3 val = GenCollection.RandomElement<IntVec3>(cellforbigcave.Where((IntVec3 x) => !GenCollection.Any<IntVec3>(bigCaveCenters, (Predicate<IntVec3>)((IntVec3 y) => IntVec3Utility.DistanceTo(x, y) < 20f))));
		list.Add(val);
		bigCaveCenters.Add(val);
		List<IntVec3> collection = GenRadial.RadialCellsAround(val, num, true).ToList();
		cavecells.AddRange(collection);
		Rand.PushState();
		int num2 = Rand.RangeInclusive(minCaves, (int)radius / 10);
		Rand.PopState();
		for (int num3 = 0; num3 < num2; num3++)
		{
			Rand.PushState();
			float dist = Rand.RangeInclusive(3, 6);
			Rand.PopState();
			IntVec3 val2 = GenCollection.RandomElement<IntVec3>(cellforlittlecave.Where((IntVec3 x) => !GenCollection.Any<IntVec3>(smallCaveCenters, (Predicate<IntVec3>)((IntVec3 y) => IntVec3Utility.DistanceTo(y, x) < dist))));
			list.Add(val2);
			smallCaveCenters.Add(val2);
			List<IntVec3> collection2 = GenRadial.RadialCellsAround(val2, dist, true).ToList();
			cavecells.AddRange(collection2);
		}
		Rand.PushState();
		if (Rand.Chance(entranceChance))
		{
			entranceChance -= 0f;
			float dist2 = Rand.RangeInclusive(3, 10);
			List<IntVec3> list2 = new List<IntVec3>();
			list2.AddRange(cells.Where((IntVec3 x) => InQuad(x, CenterCell, rot) && GenRadial.RadialCellsAround(x, dist2, true).Any((IntVec3 z) => map.reachability.CanReachMapEdge(z, TraverseParms.For((TraverseMode)0, (Danger)3, false, false, false))) && GenRadial.RadialCellsAround(x, dist2, true).Any((IntVec3 z) => QuadCells.Contains(z))));
			IntVec3 val3 = (GenList.NullOrEmpty<IntVec3>((IList<IntVec3>)list2) ? IntVec3.Invalid : GenCollection.RandomElement<IntVec3>((IEnumerable<IntVec3>)list2));
			if (val3 != IntVec3.Invalid)
			{
				List<IntVec3> collection3 = GenRadial.RadialCellsAround(val3, dist2, true).ToList();
				cavecells.AddRange(collection3);
				entranceCaveCenters.Add(val3);
				List<IntVec3> list3 = list;
				List<IntVec3> list4 = new List<IntVec3>();
				IntVec3 prevnode = val3;
				list.OrderByDescending((IntVec3 x) => IntVec3Utility.DistanceTo(x, prevnode));

				for (int num4 = 0; num4 < list.Count; num4++)
				{
					IntVec3 val4 = list[num4];
					IntVec3 val5 = prevnode - val4;
					float num5 = 0f;
					if (GenGeo.MagnitudeHorizontalSquared(prevnode.ToVector3() - val4.ToVector3()) > 0.001f)
					{
						num5 = Vector3Utility.AngleFlat(prevnode.ToVector3() - val4.ToVector3());
					}
					num5 += 90f;
					int num6 = ((prevnode.x < val4.x) ? prevnode.x : val4.x);
					int num7 = ((prevnode.z < val4.z) ? prevnode.z : val4.z);
					CellRect val6 = new CellRect(num6, num7, (int)IntVec3Utility.DistanceTo(val4, prevnode), (int)IntVec3Utility.DistanceTo(val4, prevnode));
					Dig(prevnode, num5, 3f, ((IEnumerable<IntVec3>)(object)val6).ToList(), map, closed: false);
					prevnode = val4;
				}
			}
			else
			{
				Log.Warning("Generatirng " + GenText.CapitalizeFirst(rot.ToStringHuman()) + " Quad Entrance, no suitable cell found out of " + list2.Count + " potential targets");
			}
		}
		Rand.PopState();
		cellforlittlecave.RemoveAll((IntVec3 x) => QuadCells.Contains(x));
		cellforbigcave.RemoveAll((IntVec3 x) => QuadCells.Contains(x));
	}

	private void GeneratePathToCenter(List<IntVec3> group, Map map)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		Rand.PushState();
		int num = GenMath.RoundRandom((float)group.Count * Rand.Range(0.9f, 1.1f) * 5.8f / 10000f);
		Rand.PopState();
		num = Mathf.Min(num, 3);
		if (num > 0)
		{
			Rand.PushState();
			num = Rand.RangeInclusive(1, num);
			Rand.PopState();
		}
		float num2 = TunnelsWidthPerRockCount.Evaluate((float)group.Count);
		for (int i = 0; i < num; i++)
		{
			IntVec3 start = IntVec3.Invalid;
			float num3 = -1f;
			float dir = -1f;
			float num4 = -1f;
			for (int j = 0; j < 10; j++)
			{
				IntVec3 val = FindRandomEdgeCellForTunnel(group, map);
				float distToCave = GetDistToCave(val, group, map, 40f, treatOpenSpaceAsCave: false);
				float dist;
				float num5 = FindBestInitialDir(val, group, out dist);
				if (!start.IsValid || distToCave > num3 || (distToCave == num3 && dist > num4))
				{
					start = val;
					num3 = distToCave;
					dir = num5;
					num4 = dist;
				}
			}
			Rand.PushState();
			float width = Rand.Range(num2 * 0.8f, num2);
			Rand.PopState();
			Dig(start, dir, width, group, map, closed: false);
		}
	}

	public bool InQuad(IntVec3 cell, IntVec3 CenterCell, Rot4 rot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (rot == Rot4.North)
		{
			return NorthQuad(cell, CenterCell);
		}
		if (rot == Rot4.South)
		{
			return SouthQuad(cell, CenterCell);
		}
		if (rot == Rot4.East)
		{
			return EastQuad(cell, CenterCell);
		}
		if (rot == Rot4.West)
		{
			return WestQuad(cell, CenterCell);
		}
		return false;
	}

	public bool EastQuad(IntVec3 cell, IntVec3 CenterCell)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 val = CenterCell - cell;
		if (val.x < ((val.z > 0) ? (val.z - val.z * 2) : val.z))
		{
			return true;
		}
		return false;
	}

	public bool WestQuad(IntVec3 cell, IntVec3 CenterCell)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 val = CenterCell - cell;
		if (val.x > ((val.z < 0) ? (val.z - val.z * 2) : val.z))
		{
			return true;
		}
		return false;
	}

	public bool SouthQuad(IntVec3 cell, IntVec3 CenterCell)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 val = CenterCell - cell;
		if (val.z > ((val.x < 0) ? (val.x - val.x * 2) : val.x))
		{
			return true;
		}
		return false;
	}

	public bool NorthQuad(IntVec3 cell, IntVec3 CenterCell)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 val = CenterCell - cell;
		if (val.z < ((val.x > 0) ? (val.x - val.x * 2) : val.x))
		{
			return true;
		}
		return false;
	}

	public void TrySpawnCave(IntVec3 c, List<IntVec3> cells, float dist, out List<IntVec3> CaveCells)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		CaveCells = new List<IntVec3>();
		Map map = BaseGen.globalSettings.map;
		Rand.PushState();
		directionNoise = (ModuleBase)new Perlin(0.002050000010058284, 2.0, 0.5, 4, Rand.Int, (QualityMode)1);
		Rand.PopState();
		BoolGrid visited = new BoolGrid(map);
		MapGenFloatGrid elevation = new MapGenFloatGrid(map);
		List<IntVec3> group = new List<IntVec3>();
		foreach (IntVec3 CaveCell in CaveCells)
		{
			if (visited[CaveCell])
			{
			}
			if (!IsRock(CaveCell, elevation, map))
			{
			}
			if (IntVec3Utility.DistanceTo(CaveCell, c) < dist && cells.Contains(c))
			{
				elevation[CaveCell] = 1f;
			}
			if (!visited[CaveCell] && IsRock(CaveCell, elevation, map) && cells.Contains(c))
			{
				group.Clear();
				map.floodFiller.FloodFill(CaveCell, (Predicate<IntVec3>)((IntVec3 x) => IsRock(x, elevation, map)), (Action<IntVec3>)delegate(IntVec3 x)
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					visited[x] = true;
					group.Add(x);
				}, int.MaxValue, false, (IEnumerable<IntVec3>)null);
				Trim(group, map);
				RemoveSmallDisconnectedSubGroups(group, map);
				if (group.Count >= 30)
				{
					DoOpenTunnels(group, map);
					DoClosedTunnels(group, map);
					CaveCells.AddRange(group);
				}
			}
		}
	}

	private void TrySpawnCave2(IntVec3 c, List<IntVec3> cells, float dist, out List<IntVec3> CaveCells)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		CaveCells = new List<IntVec3>();
		MapGenFloatGrid elevation = MapGenerator.Elevation;
		Map map = BaseGen.globalSettings.map;
		BoolGrid visited = new BoolGrid(map);
		List<IntVec3> group = new List<IntVec3>();
		Rand.PushState();
		directionNoise = (ModuleBase)new Perlin(0.002050000010058284, 2.0, 0.5, 4, Rand.Int, (QualityMode)1);
		Rand.PopState();
		foreach (IntVec3 cell in cells)
		{
			if (IntVec3Utility.DistanceTo(cell, c) < dist)
			{
				elevation[cell] = 1f;
			}
			if (!visited[cell] && GridsUtility.Filled(cell, map))
			{
				group.Clear();
				map.floodFiller.FloodFill(cell, (Predicate<IntVec3>)((IntVec3 x) => GridsUtility.Filled(x, map)), (Action<IntVec3>)delegate(IntVec3 x)
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					visited[x] = true;
					group.Add(x);
				}, int.MaxValue, false, (IEnumerable<IntVec3>)null);
				Trim(group, map);
				RemoveSmallDisconnectedSubGroups(group, map);
				if (group.Count >= 300)
				{
					DoOpenTunnels(group, map);
					DoClosedTunnels(group, map);
					CaveCells.AddRange(group);
				}
			}
		}
		foreach (IntVec3 cell2 in cells)
		{
			if (!CaveCells.Contains(cell2) && visited[cell2])
			{
				CaveCells.Add(cell2);
			}
		}
	}

	private void Trim(List<IntVec3> group, Map map)
	{
		GenMorphology.Open(group, 6, map);
	}

	private bool IsRock(IntVec3 c, MapGenFloatGrid elevation, Map map)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (GenGrid.InBounds(c, map))
		{
			return elevation[c] > 0.7f;
		}
		return false;
	}

	private void DoOpenTunnels(List<IntVec3> group, Map map)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		Rand.PushState();
		int num = GenMath.RoundRandom((float)group.Count * Rand.Range(0.9f, 1.1f) * 5.8f / 10000f);
		Rand.PopState();
		num = Mathf.Min(num, 3);
		if (num > 0)
		{
			Rand.PushState();
			num = Rand.RangeInclusive(1, num);
			Rand.PopState();
		}
		float num2 = TunnelsWidthPerRockCount.Evaluate((float)group.Count);
		for (int i = 0; i < num; i++)
		{
			IntVec3 start = IntVec3.Invalid;
			float num3 = -1f;
			float dir = -1f;
			float num4 = -1f;
			for (int j = 0; j < 10; j++)
			{
				IntVec3 val = FindRandomEdgeCellForTunnel(group, map);
				float distToCave = GetDistToCave(val, group, map, 40f, treatOpenSpaceAsCave: false);
				float dist;
				float num5 = FindBestInitialDir(val, group, out dist);
				if (!start.IsValid || distToCave > num3 || (distToCave == num3 && dist > num4))
				{
					start = val;
					num3 = distToCave;
					dir = num5;
					num4 = dist;
				}
			}
			Rand.PushState();
			float width = Rand.Range(num2 * 0.8f, num2);
			Rand.PopState();
			Dig(start, dir, width, group, map, closed: false);
		}
	}

	private void DoClosedTunnels(List<IntVec3> group, Map map)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		Rand.PushState();
		int num = GenMath.RoundRandom((float)group.Count * Rand.Range(0.9f, 1.1f) * 2.5f / 10000f);
		Rand.PopState();
		num = Mathf.Min(num, 1);
		if (num > 0)
		{
			Rand.PushState();
			num = Rand.RangeInclusive(0, num);
			Rand.PopState();
		}
		float num2 = TunnelsWidthPerRockCount.Evaluate((float)group.Count);
		for (int i = 0; i < num; i++)
		{
			IntVec3 start = IntVec3.Invalid;
			float num3 = -1f;
			for (int j = 0; j < 7; j++)
			{
				IntVec3 val = GenCollection.RandomElement<IntVec3>((IEnumerable<IntVec3>)group);
				float distToCave = GetDistToCave(val, group, map, 30f, treatOpenSpaceAsCave: true);
				if (!start.IsValid || distToCave > num3)
				{
					start = val;
					num3 = distToCave;
				}
			}
			Rand.PushState();
			float width = Rand.Range(num2 * 0.8f, num2);
			Dig(start, Rand.Range(0f, 360f), width, group, map, closed: true);
			Rand.PopState();
		}
	}

	private IntVec3 FindRandomEdgeCellForTunnel(List<IntVec3> group, Map map)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		MapGenFloatGrid caves = MapGenerator.Caves;
		IntVec3[] cardinalDirections = GenAdj.CardinalDirections;
		tmpCells.Clear();
		tmpGroupSet.Clear();
		GenCollection.AddRange<IntVec3>(tmpGroupSet, group);
		for (int i = 0; i < group.Count; i++)
		{
			if (group[i].DistanceToEdge(map) < 3 || caves[group[i]] > 0f)
			{
				continue;
			}
			for (int j = 0; j < 4; j++)
			{
				IntVec3 item = group[i] + cardinalDirections[j];
				if (!tmpGroupSet.Contains(item))
				{
					tmpCells.Add(group[i]);
					break;
				}
			}
		}
		if (!GenCollection.Any<IntVec3>(tmpCells))
		{
			Log.Warning("Could not find any valid edge cell.");
			return GenCollection.RandomElement<IntVec3>((IEnumerable<IntVec3>)group);
		}
		return GenCollection.RandomElement<IntVec3>((IEnumerable<IntVec3>)tmpCells);
	}

	private float FindBestInitialDir(IntVec3 start, List<IntVec3> group, out float dist)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		float num = GetDistToNonRock(start, group, IntVec3.East, 40);
		float num2 = GetDistToNonRock(start, group, IntVec3.West, 40);
		float num3 = GetDistToNonRock(start, group, IntVec3.South, 40);
		float num4 = GetDistToNonRock(start, group, IntVec3.North, 40);
		float num5 = GetDistToNonRock(start, group, IntVec3.NorthWest, 40);
		float num6 = GetDistToNonRock(start, group, IntVec3.NorthEast, 40);
		float num7 = GetDistToNonRock(start, group, IntVec3.SouthWest, 40);
		float num8 = GetDistToNonRock(start, group, IntVec3.SouthEast, 40);
		dist = Mathf.Max(new float[8] { num, num2, num3, num4, num5, num6, num7, num8 });
		return GenMath.MaxByRandomIfEqual<float>(0f, num + num8 / 2f + num6 / 2f, 45f, num8 + num3 / 2f + num / 2f, 90f, num3 + num8 / 2f + num7 / 2f, 135f, num7 + num3 / 2f + num2 / 2f, 180f, num2 + num7 / 2f + num5 / 2f, 225f, num5 + num4 / 2f + num2 / 2f, 270f, num4 + num6 / 2f + num5 / 2f, 315f, num6 + num4 / 2f + num / 2f, 0.0001f);
	}

	private void Dig(IntVec3 start, float dir, float width, List<IntVec3> group, Map map, bool closed, HashSet<IntVec3> visited = null)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Expected O, but got Unknown
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = start.ToVector3Shifted();
		float num = 0f;
		IntVec3 val2 = start;
		float num2 = 0f;
		MapGenFloatGrid elevation = MapGenerator.Elevation;
		MapGenFloatGrid caves = MapGenerator.Caves;
		bool flag = false;
		bool flag2 = false;
		if (visited == null)
		{
			visited = new HashSet<IntVec3>();
		}
		tmpGroupSet.Clear();
		GenCollection.AddRange<IntVec3>(tmpGroupSet, group);
		int num3 = 0;

		while (true)
		{
			if (closed)
			{
				int num4 = GenRadial.NumCellsInRadius(width / 2f + 1.5f);
				for (int i = 0; i < num4; i++)
				{
					IntVec3 item = val2 + GenRadial.RadialPattern[i];
					if (!visited.Contains(item) && !tmpGroupSet.Contains(item))
					{
						return;
					}
				}
			}
			if (num3 >= 15 && width > 1.8f + BranchedTunnelWidthOffset.max)
			{
				Rand.PushState();
				FloatRange branchedTunnelWidthOffset;
				if (!flag && Rand.Chance(0.05f))
				{
					IntVec3 curIntVec = val2;
					float curDir = dir;
					FloatRange dirOffset = new FloatRange(40f, 90f);
					float num5 = width;
					branchedTunnelWidthOffset = BranchedTunnelWidthOffset;
					DigInBestDirection(curIntVec, curDir, dirOffset, num5 - branchedTunnelWidthOffset.RandomInRange, group, map, closed, visited);
					flag = true;
				}
				if (!flag2 && Rand.Chance(0.05f))
				{
					IntVec3 curIntVec2 = val2;
					float curDir2 = dir;
					FloatRange dirOffset2 = new FloatRange(-90f, -40f);
					float num6 = width;
					branchedTunnelWidthOffset = BranchedTunnelWidthOffset;
					DigInBestDirection(curIntVec2, curDir2, dirOffset2, num6 - branchedTunnelWidthOffset.RandomInRange, group, map, closed, visited);
					flag2 = true;
				}
				Rand.PopState();
			}
			SetCaveAround(val2, width, map, visited, out var hitAnotherTunnel);
			if (hitAnotherTunnel)
			{
				return;
			}
			while (IntVec3Utility.ToIntVec3(val) == val2)
			{
				val += Vector3Utility.FromAngleFlat(dir) * 0.5f;
				num2 += 0.5f;
			}
			if (!tmpGroupSet.Contains(IntVec3Utility.ToIntVec3(val)))
			{
				return;
			}
			IntVec3 val3 = new IntVec3(val2.x, 0, IntVec3Utility.ToIntVec3(val).z);
			if (IsRock(val3, elevation, map))
			{
				caves[val3] = Mathf.Max(caves[val3], width);
				visited.Add(val3);
			}
			cavecells.Add(val2);
			val2 = IntVec3Utility.ToIntVec3(val);
			if (directionNoise == null)
			{
				Rand.PushState();
				directionNoise = (ModuleBase)new Perlin(0.002050000010058284, 2.0, 0.5, 4, Rand.Int, (QualityMode)1);
				Rand.PopState();
			}
			dir += (float)directionNoise.GetValue((double)(num2 * 60f), (double)((float)start.x * 200f), (double)((float)start.z * 200f)) * 8f;
			width -= 0.005f;
			if (width < 1.4f)
			{
				break;
			}
			num3++;
		}
		num = IntVec3Utility.DistanceTo(start, val2);
	}

	private void DigInBestDirection(IntVec3 curIntVec, float curDir, FloatRange dirOffset, float width, List<IntVec3> group, Map map, bool closed, HashSet<IntVec3> visited = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		float dir = -1f;
		for (int i = 0; i < 6; i++)
		{
			float num2 = curDir + dirOffset.RandomInRange;
			int distToNonRock = GetDistToNonRock(curIntVec, group, num2, 50);
			if (distToNonRock > num)
			{
				num = distToNonRock;
				dir = num2;
			}
		}
		if (num >= 18)
		{
			Dig(curIntVec, dir, width, group, map, closed, visited);
		}
	}

	private void SetCaveAround(IntVec3 around, float tunnelWidth, Map map, HashSet<IntVec3> visited, out bool hitAnotherTunnel)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		hitAnotherTunnel = false;
		int num = GenRadial.NumCellsInRadius(tunnelWidth / 2f);
		MapGenFloatGrid elevation = MapGenerator.Elevation;
		MapGenFloatGrid caves = MapGenerator.Caves;
		for (int i = 0; i < num; i++)
		{
			IntVec3 val = around + GenRadial.RadialPattern[i];
			if (IsRock(val, elevation, map))
			{
				if (caves[val] > 0f && !visited.Contains(val))
				{
					hitAnotherTunnel = true;
				}
				caves[val] = Mathf.Max(caves[val], tunnelWidth);
				visited.Add(val);
			}
			cavecells.Add(val);
		}
	}

	private int GetDistToNonRock(IntVec3 from, List<IntVec3> group, IntVec3 offset, int maxDist)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		groupSet.Clear();
		GenCollection.AddRange<IntVec3>(groupSet, group);
		for (int i = 0; i <= maxDist; i++)
		{
			IntVec3 item = from + offset * i;
			if (!groupSet.Contains(item))
			{
				return i;
			}
		}
		return maxDist;
	}

	private int GetDistToNonRock(IntVec3 from, List<IntVec3> group, float dir, int maxDist)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		groupSet.Clear();
		GenCollection.AddRange<IntVec3>(groupSet, group);
		Vector3 val = Vector3Utility.FromAngleFlat(dir);
		for (int i = 0; i <= maxDist; i++)
		{
			IntVec3 item = IntVec3Utility.ToIntVec3(from.ToVector3Shifted() + val * (float)i);
			if (!groupSet.Contains(item))
			{
				return i;
			}
		}
		return maxDist;
	}

	private float GetDistToCave(IntVec3 cell, List<IntVec3> group, Map map, float maxDist, bool treatOpenSpaceAsCave)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		MapGenFloatGrid caves = MapGenerator.Caves;
		tmpGroupSet.Clear();
		GenCollection.AddRange<IntVec3>(tmpGroupSet, group);
		int num = GenRadial.NumCellsInRadius(maxDist);
		IntVec3[] radialPattern = GenRadial.RadialPattern;
		for (int i = 0; i < num; i++)
		{
			IntVec3 val = cell + radialPattern[i];
			if ((treatOpenSpaceAsCave && !tmpGroupSet.Contains(val)) || (GenGrid.InBounds(val, map) && caves[val] > 0f))
			{
				return IntVec3Utility.DistanceTo(cell, val);
			}
		}
		return maxDist;
	}

	private void RemoveSmallDisconnectedSubGroups(List<IntVec3> group, Map map)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		groupSet.Clear();
		GenCollection.AddRange<IntVec3>(groupSet, group);
		groupVisited.Clear();
		for (int i = 0; i < group.Count; i++)
		{
			if (groupVisited.Contains(group[i]) || !groupSet.Contains(group[i]))
			{
				continue;
			}
			subGroup.Clear();
			map.floodFiller.FloodFill(group[i], (Predicate<IntVec3>)((IntVec3 x) => groupSet.Contains(x)), (Action<IntVec3>)delegate(IntVec3 x)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				subGroup.Add(x);
				groupVisited.Add(x);
			}, int.MaxValue, false, (IEnumerable<IntVec3>)null);
			if (subGroup.Count < 300 || (float)subGroup.Count < 0.05f * (float)group.Count)
			{
				for (int num = 0; num < subGroup.Count; num++)
				{
					groupSet.Remove(subGroup[num]);
				}
			}
		}
		group.Clear();
		group.AddRange(groupSet);
	}

	static SymbolResolver_HiveInternals()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		SimpleCurve val = new SimpleCurve();
		val.Add(new CurvePoint(100f, 2f), true);
		val.Add(new CurvePoint(300f, 4f), true);
		val.Add(new CurvePoint(3000f, 5.5f), true);
		TunnelsWidthPerRockCount = val;
		tmpCells = new List<IntVec3>();
		groupSet = new HashSet<IntVec3>();
		groupVisited = new HashSet<IntVec3>();
		subGroup = new List<IntVec3>();
	}
}
