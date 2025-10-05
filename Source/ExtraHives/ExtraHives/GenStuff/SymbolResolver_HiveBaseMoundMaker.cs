using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ExtraHives.GenStuff;

public class SymbolResolver_HiveBaseMoundMaker : SymbolResolver
{
	private List<IntVec3> cells = new List<IntVec3>();

	private Faction Faction = null;

	public override void Resolve(ResolveParams rp)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		Faction = rp.faction;
		HiveFactionExtension modExtension = ((Def)Faction.def).GetModExtension<HiveFactionExtension>();
		cells.Clear();
		Map map = BaseGen.globalSettings.map;
		IntVec3 CenterCell = rp.rect.CenterCell;
		float dist = IntVec3Utility.DistanceTo(rp.rect.Corners.ToList()[0], rp.rect.Corners.ToList()[2]);
		cells = map.AllCells.Where((IntVec3 x) => IntVec3Utility.DistanceTo(x, CenterCell) <= dist).ToList();
		RoofGrid roofGrid = BaseGen.globalSettings.map.roofGrid;
		RoofDef val = rp.roofDef ?? RoofDefOf.RoofRockThick;
		List<IntVec3> list = cells.Where((IntVec3 x) => IntVec3Utility.DistanceTo(x, CenterCell) < dist - 10f).ToList();
		List<IntVec3> list2 = cells.Where((IntVec3 x) => IntVec3Utility.DistanceTo(x, CenterCell) > dist - 5f && IntVec3Utility.DistanceTo(x, CenterCell) < dist).ToList();
		for (int num = 0; (float)num < dist / 5f; num++)
		{
			IntVec3 ce = GenCollection.RandomElement<IntVec3>((IEnumerable<IntVec3>)list2);
			Rand.PushState();
			float size = Rand.Range(5, 10);
			Rand.PopState();
			cells.RemoveAll((IntVec3 x) => IntVec3Utility.DistanceTo(ce, x) < size);
		}
		foreach (IntVec3 cell in cells)
		{
			roofGrid.SetRoof(cell, val);
			map.terrainGrid.SetTerrain(cell, TerrainDefOf.Gravel);
			TrySpawnWall(cell, rp);
		}
	}

	private Thing TrySpawnWall(IntVec3 c, ResolveParams rp)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		Map map = BaseGen.globalSettings.map;
		List<Thing> thingList = GridsUtility.GetThingList(c, map);
		for (int i = 0; i < thingList.Count; i++)
		{
			if (!thingList[i].def.destroyable)
			{
				return null;
			}
		}
		if (IntVec3Utility.DistanceTo(rp.rect.CenterCell, c) < 10f)
		{
			return null;
		}
		for (int num = thingList.Count - 1; num >= 0; num--)
		{
			thingList[num].Destroy((DestroyMode)0);
		}
		Rand.PushState();
		bool flag = Rand.Chance(rp.chanceToSkipWallBlock.Value);
		Rand.PopState();
		if (rp.chanceToSkipWallBlock.HasValue && flag)
		{
			return null;
		}
		Thing val = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ExtraHive_Hive_Wall", true), (ThingDef)null);
		val.SetFaction(rp.faction, (Pawn)null);
		return GenSpawn.Spawn(val, c, map, (WipeMode)0);
	}
}
