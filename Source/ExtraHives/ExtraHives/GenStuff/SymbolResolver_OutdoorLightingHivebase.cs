using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ExtraHives.GenStuff;

internal class SymbolResolver_OutdoorLightingHivebase : SymbolResolver
{
	private static List<CompGlower> nearbyGlowers = new List<CompGlower>();

	private const float Margin = 2f;

	public override void Resolve(ResolveParams rp)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		Map map = BaseGen.globalSettings.map;
		ThingDef glowPod = DefDatabase<ThingDef>.GetNamed("GlowPod");
		FindNearbyGlowers(rp.rect);
		for (int i = 0; i < rp.rect.Area / 4; i++)
		{
			IntVec3 randomCell = rp.rect.RandomCell;
			if (!GenGrid.Standable(randomCell, map) || GridsUtility.GetFirstItem(randomCell, map) != null || GridsUtility.GetFirstPawn(randomCell, map) != null || GridsUtility.GetFirstBuilding(randomCell, map) != null)
			{
				continue;
			}
			Region region = GridsUtility.GetRegion(randomCell, map, (RegionType)14);
			if (region != null && region.Room.PsychologicallyOutdoors && region.Room.UsesOutdoorTemperature && !AnyGlowerNearby(randomCell) && !BaseGenUtility.AnyDoorAdjacentCardinalTo(randomCell, map))
			{
				if (!rp.spawnBridgeIfTerrainCantSupportThing.HasValue || rp.spawnBridgeIfTerrainCantSupportThing.Value)
				{
					BaseGenUtility.CheckSpawnBridgeUnder(glowPod, randomCell, Rot4.North);
				}
				Thing val = GenSpawn.Spawn(glowPod, randomCell, map, (WipeMode)0);
				if (val.def.CanHaveFaction && val.Faction != rp.faction)
				{
					val.SetFaction(rp.faction, (Pawn)null);
				}
				nearbyGlowers.Add(ThingCompUtility.TryGetComp<CompGlower>(val));
			}
		}
		nearbyGlowers.Clear();
	}

	private unsafe void FindNearbyGlowers(CellRect rect)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		Map map = BaseGen.globalSettings.map;
		nearbyGlowers.Clear();
		rect = rect.ExpandedBy(4);
		rect = rect.ClipInsideMap(map);
		foreach (IntVec3 current in rect)
		{
			Region region = GridsUtility.GetRegion(current, map, (RegionType)14);
			if (region == null || !region.Room.PsychologicallyOutdoors)
			{
				continue;
			}
			List<Thing> thingList = GridsUtility.GetThingList(current, map);
			for (int i = 0; i < thingList.Count; i++)
			{
				CompGlower val = ThingCompUtility.TryGetComp<CompGlower>(thingList[i]);
				if (val != null)
				{
					nearbyGlowers.Add(val);
				}
			}
		}
	}

	private bool AnyGlowerNearby(IntVec3 c)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < nearbyGlowers.Count; i++)
		{
			if (c.InHorDistOf(((Thing)((ThingComp)nearbyGlowers[i]).parent).Position, nearbyGlowers[i].Props.glowRadius + 2f))
			{
				return true;
			}
		}
		return false;
	}
}
