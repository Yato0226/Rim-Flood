using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExtraHives;

public class IncidentWorker_DeepDrillInfestation : IncidentWorker
{
	private static List<Thing> tmpDrills = new List<Thing>();

	private const float MinPointsFactor = 0.3f;

	private const float MaxPointsFactor = 0.6f;

	private const float MinPoints = 200f;

	private const float MaxPoints = 1000f;

	protected override bool CanFireNowSub(IncidentParms parms)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		if (base.def.mechClusterBuilding == null)
		{
			return false;
		}
		if (!((Def)base.def.mechClusterBuilding).HasModExtension<HiveDefExtension>())
		{
			return false;
		}
		if (!this.CanFireNowSub(parms))
		{
			return false;
		}
		Map val = (Map)parms.target;
		tmpDrills.Clear();
		DeepDrillInfestationIncidentUtility.GetUsableDeepDrills(val, tmpDrills);
		return GenCollection.Any<Thing>(tmpDrills);
	}

	protected override bool TryExecuteWorker(IncidentParms parms)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		Map map = (Map)parms.target;
		if (base.def.mechClusterBuilding == null)
		{
			return false;
		}
		if (!((Def)base.def.mechClusterBuilding).HasModExtension<HiveDefExtension>())
		{
			return false;
		}
		ThingDef mechClusterBuilding = base.def.mechClusterBuilding;
		HiveDefExtension modExtension = ((Def)mechClusterBuilding).GetModExtension<HiveDefExtension>();
		if (modExtension == null)
		{
			return false;
		}
		HiveDefExtension ext = ((Def)base.def.mechClusterBuilding).GetModExtension<HiveDefExtension>();
		if (parms.faction == null)
		{
			try
			{
				parms.faction = GenCollection.RandomElement<Faction>(Find.FactionManager.AllFactions.Where((Faction x) => ((Def)x.def).defName.Contains(((Def)ext.Faction).defName)));
			}
			catch (Exception)
			{
				parms.faction = Find.FactionManager.FirstFactionOfDef(ext.Faction);
			}
		}
		ThingDef val = modExtension.TunnelDef ?? FloodDefOf.Floodtunnel;
		tmpDrills.Clear();
		DeepDrillInfestationIncidentUtility.GetUsableDeepDrills(map, tmpDrills);
		Thing deepDrill = default(Thing);
		if (!GenCollection.TryRandomElement<Thing>(tmpDrills, out deepDrill))
		{
			return false;
		}
		IntVec3 val2 = CellFinder.FindNoWipeSpawnLocNear(deepDrill.Position, map, val, Rot4.North, 2, (Predicate<IntVec3>)((IntVec3 x) => GenGrid.Walkable(x, map) && GridsUtility.GetFirstThing(x, map, deepDrill.def) == null && GridsUtility.GetFirstThingWithComp<ThingComp>(x, map) == null && GridsUtility.GetFirstThing(x, map, ThingDefOf.Hive) == null && GridsUtility.GetFirstThing(x, map, FloodDefOf.Floodtunnel) == null));
		if (val2 == deepDrill.Position)
		{
			return false;
		}
		TunnelHiveSpawner tunnelHiveSpawner = (TunnelHiveSpawner)(object)ThingMaker.MakeThing(val, (ThingDef)null);
		tunnelHiveSpawner.spawnHive = false;
		Rand.PushState();
		tunnelHiveSpawner.initialPoints = Mathf.Clamp(parms.points * Rand.Range(0.3f, 0.6f), 200f, 1000f);
		Rand.PopState();
		tunnelHiveSpawner.spawnedByInfestationThingComp = true;
		GenSpawn.Spawn((Thing)(object)tunnelHiveSpawner, val2, map, (WipeMode)1);
		ThingCompUtility.TryGetComp<CompCreatesInfestations>(deepDrill).Notify_CreatedInfestation();
		this.SendStandardLetter(parms, new TargetInfo(((Thing)tunnelHiveSpawner).Position, map, false), Array.Empty<NamedArgument>());
		return true;
	}
}
