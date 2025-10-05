using System;
using RimWorld;
using Verse;

namespace ExtraHives;

public static class InfestationUtility
{
	public static Thing SpawnTunnels(ThingDef hiveDef, int hiveCount, Map map, bool spawnAnywhereIfNoGoodCell = false, bool ignoreRoofedRequirement = false, string questTag = null, Faction faction = null)
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		ThingDef val = hiveDef ?? ThingDefOf.Hive;
		HiveDefExtension modExtension = ((Def)val).GetModExtension<HiveDefExtension>();
		ThingDef val2 = modExtension?.TunnelDef ?? ExtraHives.ThingDefOf.Tunneler_ExtraHives;
		ExtraHives.InfestationCellFinder_ExtraHives.InfestationParms parms = new ExtraHives.InfestationCellFinder_ExtraHives.InfestationParms(modExtension);
		if (!ExtraHives.InfestationCellFinder_ExtraHives.TryFindCell(out var cell, map, parms))
		{
			if (!spawnAnywhereIfNoGoodCell)
			{
				return null;
			}
			if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((Predicate<IntVec3>)delegate(IntVec3 x)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				if (!GenGrid.Standable(x, map) || GridsUtility.Fogged(x, map))
				{
					return false;
				}
				bool flag = false;
				int num2 = GenRadial.NumCellsInRadius(3f);
				for (int i = 0; i < num2; i++)
				{
					IntVec3 val4 = x + GenRadial.RadialPattern[i];
					if (GenGrid.InBounds(val4, map))
					{
						RoofDef roof = GridsUtility.GetRoof(val4, map);
						if (roof != null && roof.isThickRoof)
						{
							flag = true;
							break;
						}
					}
				}
				return flag ? true : false;
			}, map, out cell))
			{
				return null;
			}
		}
		if (modExtension?.TunnelDef != null)
		{
			val2 = modExtension.TunnelDef;
		}
		Thing val3 = GenSpawn.Spawn(ThingMaker.MakeThing(val2, (ThingDef)null), cell, map, (WipeMode)1);
		TunnelHiveSpawner tunnelHiveSpawner = val3 as TunnelHiveSpawner;
		if (tunnelHiveSpawner != null && tunnelHiveSpawner.faction == null && faction != null)
		{
			tunnelHiveSpawner.faction = faction;
		}
		CompSpawnerPawn compSpawnerPawn = ThingCompUtility.TryGetComp<CompSpawnerPawn>(val3);
		if (compSpawnerPawn != null && Prefs.DevMode)
		{
			Log.Message($"{((Entity)val3).LabelCap} spawnerPawn initialPawnsPoints: {compSpawnerPawn.initialPawnsPoints}");
		}
		if (tunnelHiveSpawner.SpawnedFaction != null)
		{
		}
		QuestUtility.AddQuestTag((object)val3, questTag);
		for (int num = 0; num < hiveCount - 1; num++)
		{
			cell = CompSpawnerHives.FindChildHiveLocation(val3.Position, map, val, val.GetCompProperties<CompProperties_SpawnerHives>(), ignoreRoofedRequirement, allowUnreachable: true);
			if (cell.IsValid)
			{
				val3 = GenSpawn.Spawn(ThingMaker.MakeThing(val2, (ThingDef)null), cell, map, (WipeMode)1);
				tunnelHiveSpawner = val3 as TunnelHiveSpawner;
				if (tunnelHiveSpawner != null && tunnelHiveSpawner.faction == null && faction != null)
				{
					tunnelHiveSpawner.faction = faction;
				}
				if (tunnelHiveSpawner.SpawnedFaction != null)
				{
				}
				QuestUtility.AddQuestTag((object)val3, questTag);
			}
		}
		return val3;
	}

	public static Thing SpawnTunnels(ThingDef hiveDef, int hiveCount, Map map, IntVec3 cell, bool spawnAnywhereIfNoGoodCell = false, bool ignoreRoofedRequirement = false, string questTag = null, Faction faction = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		ThingDef val = hiveDef ?? ThingDefOf.Hive;
		ThingDef val2 = ((Def)val).GetModExtension<HiveDefExtension>()?.TunnelDef ?? ExtraHives.ThingDefOf.Tunneler_ExtraHives;
		HiveDefExtension modExtension = ((Def)val).GetModExtension<HiveDefExtension>();
		if (modExtension != null && modExtension.TunnelDef != null)
		{
			val2 = modExtension.TunnelDef;
		}
		Thing val3 = GenSpawn.Spawn(ThingMaker.MakeThing(val2, (ThingDef)null), cell, map, (WipeMode)1);
		TunnelHiveSpawner tunnelHiveSpawner = val3 as TunnelHiveSpawner;
		if (tunnelHiveSpawner != null && tunnelHiveSpawner.faction == null && faction != null)
		{
			tunnelHiveSpawner.faction = faction;
		}
		if (tunnelHiveSpawner.SpawnedFaction != null)
		{
		}
		QuestUtility.AddQuestTag((object)val3, questTag);
		CompSpawnerHives compSpawnerHives = ThingCompUtility.TryGetComp<CompSpawnerHives>(val3);
		if (compSpawnerHives?.Props.tunnelDef != null)
		{
			val2 = compSpawnerHives.Props.tunnelDef;
		}
		for (int i = 0; i < hiveCount - 1; i++)
		{
			cell = CompSpawnerHives.FindChildHiveLocation(val3.Position, map, val, val.GetCompProperties<CompProperties_SpawnerHives>(), ignoreRoofedRequirement, allowUnreachable: true);
			if (cell.IsValid)
			{
				val3 = GenSpawn.Spawn(ThingMaker.MakeThing(val2, (ThingDef)null), cell, map, (WipeMode)1);
				tunnelHiveSpawner = val3 as TunnelHiveSpawner;
				if (tunnelHiveSpawner != null && tunnelHiveSpawner.faction == null && faction != null)
				{
					tunnelHiveSpawner.faction = faction;
				}
				if (tunnelHiveSpawner.SpawnedFaction != null)
				{
				}
				QuestUtility.AddQuestTag((object)val3, questTag);
			}
		}
		return val3;
	}
}
