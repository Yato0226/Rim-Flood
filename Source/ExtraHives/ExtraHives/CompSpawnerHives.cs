using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExtraHives;

public class CompSpawnerHives : ThingComp
{
	private int plantHarmAge;

	private int ticksToPlantHarm;

	protected int nextHiveSpawnTick;

	protected bool canSpawnHives = true;

	private bool wasActivated;

	protected CompInitiatable initiatableComp;

	public CompProperties_SpawnerHives Props => (CompProperties_SpawnerHives)(object)((ThingComp)this).props;

	public bool CanSpawnChildHive => canSpawnHives && HiveUtility.TotalSpawnedHivesCount(((Thing)((ThingComp)this).parent).Map, Props.hiveDef) < 30;

	public float AgeDays => (float)plantHarmAge / 60000f;

	public float CurrentRadius
	{
		get
		{
			SimpleCurve radiusPerDayCurve = Props.radiusPerDayCurve;
			return Mathf.Max((radiusPerDayCurve != null) ? radiusPerDayCurve.Evaluate(AgeDays) : Props.HiveSpawnRadius, Props.HiveSpawnPreferredMinDist);
		}
	}

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		if (!respawningAfterLoad)
		{
			CalculateNextHiveSpawnTick();
		}
		initiatableComp = ((ThingComp)this).parent.GetComp<CompInitiatable>();
	}

	public override void CompTick()
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (((Thing)((ThingComp)this).parent).Map == null)
		{
			return;
		}
		plantHarmAge++;
		CompCanBeDormant comp = ((ThingComp)this).parent.GetComp<CompCanBeDormant>();
		if ((comp == null || comp.Awake) && !wasActivated)
		{
			CalculateNextHiveSpawnTick();
			wasActivated = true;
		}
		if ((comp == null || comp.Awake) && Find.TickManager.TicksGame >= nextHiveSpawnTick)
		{
			if (TrySpawnChildHive(!Props.requireRoofed, out var newHive))
			{
				Messages.Message("MessageHiveReproduced".Translate(), newHive, MessageTypeDefOf.NegativeEvent, true);
			}
			else
			{
				CalculateNextHiveSpawnTick();
			}
		}
	}

	public override string CompInspectStringExtra()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		string text = !GenCollection.EnumerableNullOrEmpty(Props.radiusPerDayCurve) ? ($"{"FoliageKillRadius".Translate()}: {CurrentRadius:0.0}\n{"RadiusExpandRate".Translate()}: {Math.Round(Props.radiusPerDayCurve.Evaluate(AgeDays + 1f) - Props.radiusPerDayCurve.Evaluate(AgeDays))}/{"day".Translate()}\n") : "";
		if (!canSpawnHives)
		{
			return text + "DormantHiveNotReproducing".Translate();
		}
		if (CanSpawnChildHive)
		{
			return text + "HiveReproducesIn".Translate() + ": " + GenDate.ToStringTicksToPeriod(nextHiveSpawnTick - Find.TickManager.TicksGame);
		}
		return text;
	}

	public void CalculateNextHiveSpawnTick()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		Room room = RegionAndRoomQuery.GetRoom((Thing)(object)((ThingComp)this).parent, (RegionType)14);
		int num = 0;
		int num2 = GenRadial.NumCellsInRadius(CurrentRadius);
		for (int i = 0; i < num2; i++)
		{
			IntVec3 val = ((Thing)((ThingComp)this).parent).Position + GenRadial.RadialPattern[i];
			if (GenGrid.InBounds(val, ((Thing)((ThingComp)this).parent).Map) && GridsUtility.GetRoom(val, ((Thing)((ThingComp)this).parent).Map) == room && GenCollection.Any<Thing>(GridsUtility.GetThingList(val, ((Thing)((ThingComp)this).parent).Map), (Predicate<Thing>)((Thing t) => t is Hive && t.def == ((Thing)((ThingComp)this).parent).def)))
			{
				num++;
			}
		}
		float num3 = Props.ReproduceRateFactorFromNearbyHiveCountCurve.Evaluate((float)num);
nextHiveSpawnTick = Find.TickManager.TicksGame + (int)(Props.HiveSpawnIntervalDays.RandomInRange * 60000f / (num3 * Find.Storyteller.difficulty.enemyReproductionRateFactor));
	}

	public bool TrySpawnChildHive(bool ignoreRoofedRequirement, out Hive newHive)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		if (!CanSpawnChildHive)
		{
			newHive = null;
			return false;
		}
		HiveDefExtension hiveDefExtension = null;
		if (((Def)((Thing)((ThingComp)this).parent).def).HasModExtension<HiveDefExtension>())
		{
			hiveDefExtension = ((Def)((Thing)((ThingComp)this).parent).def).GetModExtension<HiveDefExtension>();
		}
		ThingDef val = Props.hiveDef ?? ((Thing)((ThingComp)this).parent).def;
		CellRect val2 = GenAdj.OccupiedRect((Thing)(object)((ThingComp)this).parent);
		IntVec3 val3 = FindChildHiveLocation(val2.AdjacentCells.RandomElement(), parent.Map, val, Props, ignoreRoofedRequirement, allowUnreachable: false, CurrentRadius);
		if (!val3.IsValid)
		{
			newHive = null;
			Log.Warning("this !loc.IsValid");
			return false;
		}
		newHive = (Hive)(object)ThingMaker.MakeThing(val, (ThingDef)null);
		if (((Thing)newHive).Faction != ((Thing)((ThingComp)this).parent).Faction)
		{
			((Thing)newHive).SetFaction(((Thing)((ThingComp)this).parent).Faction, (Pawn)null);
		}
		if (((ThingComp)this).parent is Hive hive)
		{
			if (((Hive)hive).CompDormant.Awake)
			{
				((Hive)newHive).CompDormant.WakeUp();
			}
			((Thing)newHive).questTags = ((Thing)hive).questTags;
		}
		if (newHive.Ext?.TunnelDef != null)
		{
			TunnelHiveSpawner tunnelHiveSpawner = (TunnelHiveSpawner)(object)ThingMaker.MakeThing(newHive.Ext.TunnelDef, (ThingDef)null);
			tunnelHiveSpawner.hive = newHive;
			GenSpawn.Spawn((Thing)(object)tunnelHiveSpawner, val3, ((Thing)((ThingComp)this).parent).Map, (WipeMode)1);
			CalculateNextHiveSpawnTick();
		}
		else
		{
			GenSpawn.Spawn((Thing)(object)newHive, val3, ((Thing)((ThingComp)this).parent).Map, (WipeMode)1);
			CalculateNextHiveSpawnTick();
		}
		return true;
	}

	public static IntVec3 FindChildHiveLocation(IntVec3 pos, Map map, ThingDef parentDef, CompProperties_SpawnerHives props, bool ignoreRoofedRequirement, bool allowUnreachable, float radius = 0f)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 val = IntVec3.Invalid;
		float num = ((radius > 0f) ? radius : props.HiveSpawnRadius);
		for (int i = 0; i < 3; i++)
		{
			float minDist = props.HiveSpawnPreferredMinDist;
			bool flag;
			if (i < 2)
			{
				if (i == 1)
				{
					minDist = 0f;
				}
				flag = CellFinder.TryFindRandomReachableCellNearPosition(pos, pos, map, num, TraverseParms.For((TraverseMode)2, (Danger)3, false, false, false), (Predicate<IntVec3>)((IntVec3 c) => CanSpawnHiveAt(c, map, pos, parentDef, minDist, ignoreRoofedRequirement)), (Predicate<Region>)null, out val, 999999);
			}
			else
			{
				flag = allowUnreachable && CellFinder.TryFindRandomCellNear(pos, map, (int)num, (Predicate<IntVec3>)((IntVec3 c) => CanSpawnHiveAt(c, map, pos, parentDef, minDist, ignoreRoofedRequirement)), out val, -1);
			}
			if (flag)
			{
				val = CellFinder.FindNoWipeSpawnLocNear(val, map, parentDef, Rot4.North, 2, (Predicate<IntVec3>)((IntVec3 c) => CanSpawnHiveAt(c, map, pos, parentDef, minDist, ignoreRoofedRequirement)));
				break;
			}
		}
		return val;
	}

	private static bool CanSpawnHiveAt(IntVec3 c, Map map, IntVec3 parentPos, ThingDef parentDef, float minDist, bool ignoreRoofedRequirement)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Invalid comparison between Unknown and I4
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Invalid comparison between Unknown and I4
		if ((!ignoreRoofedRequirement && !GridsUtility.Roofed(c, map)) || !GenGrid.Walkable(c, map) || (minDist != 0f && (float)IntVec3Utility.DistanceToSquared(c, parentPos) < minDist * minDist) || GridsUtility.GetFirstThing(c, map, RimWorld.ThingDefOf.InsectJelly) != null || GridsUtility.GetFirstThing(c, map, RimWorld.ThingDefOf.GlowPod) != null)
		{
			return false;
		}
		for (int i = 0; i < 9; i++)
		{
			IntVec3 val = c + GenAdj.AdjacentCellsAndInside[i];
			if (!GenGrid.InBounds(val, map))
			{
				continue;
			}
			List<Thing> thingList = GridsUtility.GetThingList(val, map);
			for (int j = 0; j < thingList.Count; j++)
			{
				if (thingList[j] is Hive || thingList[j] is TunnelHiveSpawner)
				{
					return false;
				}
			}
		}
		List<Thing> thingList2 = GridsUtility.GetThingList(c, map);
		for (int k = 0; k < thingList2.Count; k++)
		{
			Thing val2 = thingList2[k];
			if ((int)val2.def.category == 3 && (int)((BuildableDef)val2.def).passability == 2 && GenSpawn.SpawningWipes((BuildableDef)(object)parentDef, (BuildableDef)(object)val2.def))
			{
				return true;
			}
		}
		return true;
	}

	public override IEnumerable<Gizmo> CompGetGizmosExtra()
	{
		if (!Prefs.DevMode)
		{
			yield break;
		}
		yield return (Gizmo)new Command_Action
		{
			defaultLabel = "Dev: Reproduce",
			icon = (Texture)(object)TexCommand.GatherSpotActive,
			action = delegate
			{
				if (!TrySpawnChildHive(ignoreRoofedRequirement: true, out var _))
				{
					Log.Warning("Failed Spawning hive");
				}
			}
		};
	}

	public override void PostExposeData()
	{
		Scribe_Values.Look<int>(ref plantHarmAge, "plantHarmAge", 0, false);
		Scribe_Values.Look<int>(ref ticksToPlantHarm, "ticksToPlantHarm", 0, false);
	}
}
