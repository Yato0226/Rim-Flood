using System;
using System.Collections.Generic;
using System.Linq;
using LudeonTK;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace ExtraHives;

public class TunnelHiveSpawner : ThingWithComps
{
	private TunnelExtension ext;

	public Type lordJobType = typeof(LordJob_AssaultColony);

	public Faction faction = null;

	public FactionDef factiondef = null;

	public FloatRange ResultSpawnDelay = new FloatRange(26f, 30f);

	public bool spawnHive = true;

	public Hive hive = null;

	public float initialPoints;

	public bool spawnedByInfestationThingComp;

	private Sustainer sustainer;

	private Effecter Effecter;

	private int secondarySpawnTick;

	private int spawnTick;

	private static List<ThingDef> filthTypes = new List<ThingDef>();

	[TweakValue("Gameplay", 0f, 1f)]
	private static float DustMoteSpawnMTB = 0.2f;

	[TweakValue("Gameplay", 0f, 1f)]
	private static float EMPMoteSpawnMTB = 1f;

	[TweakValue("Gameplay", 0f, 1f)]
	private static float FilthSpawnMTB = 0.3f;

	[TweakValue("Gameplay", 0f, 10f)]
	private static float FilthSpawnRadius = 3f;

	public TunnelExtension Ext => ext ?? (ext = (((Def)((Thing)this).def).HasModExtension<TunnelExtension>() ? ((Def)((Thing)this).def).GetModExtension<TunnelExtension>() : null));

	public Faction SpawnedFaction
	{
		get
		{
			if (faction == null && Ext != null && Ext.Faction != null)
			{
				faction = Find.FactionManager.FirstFactionOfDef(Ext.Faction);
			}
			if (faction == null && factiondef != null)
			{
				faction = Find.FactionManager.FirstFactionOfDef(factiondef);
			}
			return faction;
		}
		set
		{
			faction = value;
		}
	}

	public float TimeRemaining => Math.Max(Mathf.InverseLerp((float)secondarySpawnTick, (float)spawnTick, (float)Find.TickManager.TicksGame), 0.0001f);

	protected override void Tick()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected O, but got Unknown
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		if (!((Thing)this).Spawned)
		{
			return;
		}
		sustainer.Maintain();
		IntVec3 position = ((Thing)this).Position;
		Vector3 val = position.ToVector3Shifted();
		TargetInfo val2 = new TargetInfo((Thing)this);
		Rand.PushState();
		IntVec3 val3 = default(IntVec3);
		if (Rand.MTBEventOccurs(FilthSpawnMTB, 1f, GenTicks.TicksToSeconds(1)) && CellFinder.TryFindRandomReachableCellNearPosition(((Thing)this).Position, ((Thing)this).Position, ((Thing)this).Map, FilthSpawnRadius, TraverseParms.For((TraverseMode)2, (Danger)3, false, false, false), (Predicate<IntVec3>)null, (Predicate<Region>)null, out val3, 999999) && !GenList.NullOrEmpty<ThingDef>((IList<ThingDef>)filthTypes))
		{
			FilthMaker.TryMakeFilth(val3, ((Thing)this).Map, GenCollection.RandomElement<ThingDef>((IEnumerable<ThingDef>)filthTypes), 1, (FilthSourceFlags)0, true);
		}
		if (Rand.MTBEventOccurs(DustMoteSpawnMTB, 1f, GenTicks.TicksToSeconds(1)))
		{
			Vector3 val4 = new Vector3(val.x, 0f, val.z);
			val4.y = Altitudes.AltitudeFor((AltitudeLayer)26);
			FleckMaker.ThrowDustPuffThick(val4, ((Thing)this).Map, Rand.Range(1.5f, 3f), Ext.dustColor ?? new Color(1f, 1f, 1f, 2.5f));
			if (Ext.thowSparksinDust && Rand.MTBEventOccurs(EMPMoteSpawnMTB * TimeRemaining, 1f, 0.25f))
			{
				FleckMaker.ThrowMicroSparks(val4, ((Thing)this).Map);
			}
		}
		if (Ext.effecter != null && Rand.MTBEventOccurs(EMPMoteSpawnMTB * TimeRemaining, 0.5f, 0.25f))
		{
			if (Effecter == null && Ext.effecter != null)
			{
				Effecter = new Effecter(Ext.effecter);
			}
			if (Effecter != null)
			{
				Effecter.EffectTick(val2, val2);
			}
			else
			{
				Effecter.EffectTick(val2, val2);
			}
		}
		Rand.PopState();
		if (secondarySpawnTick > Find.TickManager.TicksGame)
		{
			return;
		}
		if (Effecter != null)
		{
			Effecter.Cleanup();
		}
		sustainer.End();
		Map map = ((Thing)this).Map;
		IntVec3 position2 = ((Thing)this).Position;
		((Thing)this).Destroy((DestroyMode)0);
		if (Ext.strikespreexplode)
		{
			FireEvent(map, position2);
		}
		if (Ext.explodesprespawn)
		{
			GenExplosion.DoExplosion(position2, map, Ext.blastradius, Ext.damageDef, null, -1, -1f, null, null, null, null, null, 0f, 1, null, null, 255, false, null, 0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f, null, null, null, null);
		}
		Hive hive = null;
		if (spawnHive)
		{
			hive = (Hive)(object)GenSpawn.Spawn(ThingMaker.MakeThing(Ext.HiveDef, (ThingDef)null), position2, map, (WipeMode)0);
			if (hive != null)
			{
				((Thing)hive).SetFaction(SpawnedFaction, (Pawn)null);
				((Thing)hive).questTags = ((Thing)this).questTags;
				foreach (CompSpawner comp in ((ThingWithComps)hive).GetComps<CompSpawner>())
				{
					if (comp.PropsSpawner.thingToSpawn == DefDatabase<ThingDef>.GetNamed("InsectJelly"))
					{
						comp.TryDoSpawn();
						break;
					}
				}
				CompSpawnerPawn compSpawnerPawn = ThingCompUtility.TryGetComp<CompSpawnerPawn>((Thing)(object)hive);
				if (compSpawnerPawn != null)
				{
					compSpawnerPawn.initialPawnsPoints = compSpawnerPawn.Props.initialPawnsPoints;
					compSpawnerPawn.SpawnInitialPawns();
				}
			}
		}
		List<Pawn> list = new List<Pawn>();
		if (initialPoints > 0f)
		{
			initialPoints = Mathf.Max(initialPoints, Ext.HiveDef.GetCompProperties<CompProperties_SpawnerPawn>().spawnablePawnKinds.Min((PawnGenOption x) => x.Cost));
			float pointsLeft = initialPoints;
			int num = 0;
			PawnGenOption val5 = default(PawnGenOption);
			for (; pointsLeft > 0f; pointsLeft -= val5.Cost)
			{
				num++;
				if (num > 1000)
				{
					Log.Error("Too many iterations.");
					break;
				}
				if (!GenCollection.TryRandomElementByWeight<PawnGenOption>(Ext.HiveDef.GetCompProperties<CompProperties_SpawnerPawn>().spawnablePawnKinds.Where((PawnGenOption x) => x.Cost <= pointsLeft), (Func<PawnGenOption, float>)((PawnGenOption x) => x.selectionWeight), out val5))
				{
					break;
				}
				Pawn val6 = PawnGenerator.GeneratePawn(val5.kind, SpawnedFaction);
				GenSpawn.Spawn((Thing)(object)val6, CellFinder.RandomClosewalkCellNear(position2, map, 2, (Predicate<IntVec3>)null), map, (WipeMode)0);
				val6.mindState.spawnedByInfestationThingComp = spawnedByInfestationThingComp;
				list.Add(val6);
			}
		}
		if (GenCollection.Any<Pawn>(list))
		{
			MakeLord(lordJobType, list);
		}
	}

	protected override void DrawAt(Vector3 drawLoc, bool flip = false)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		base.DrawAt(drawLoc, flip);
		Rand.PushState();
		Rand.Seed = ((Thing)this).thingIDNumber;
		for (int i = 0; i < 6; i++)
		{
			DrawDustPart(Rand.Range(0f, 360f), Rand.Range(0.9f, 1.1f) * (float)Rand.Sign * 4f, Rand.Range(1f, 1.5f));
		}
		Rand.PopState();
	}

	public void FireEvent(Map map, IntVec3 strikeLoc)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (!strikeLoc.IsValid)
		{
			strikeLoc = CellFinderLoose.RandomCellWith((Predicate<IntVec3>)((IntVec3 sq) => GenGrid.Standable(sq, map) && !map.roofGrid.Roofed(sq)), map, 1000);
		}
		Mesh randomBoltMesh = LightningBoltMeshPool.RandomBoltMesh;
		if (!GridsUtility.Fogged(strikeLoc, map))
		{
			Vector3 val = strikeLoc.ToVector3Shifted();
			for (int num = 0; num < 4; num++)
			{
				FleckMaker.ThrowSmoke(val, map, 1.5f);
				FleckMaker.ThrowMicroSparks(val, map);
				FleckMaker.ThrowLightningGlow(val, map, 1.5f);
			}
		}
		SoundInfo val2 = SoundInfo.InMap(new TargetInfo(strikeLoc, map, false), (MaintenanceType)0);
		SoundStarter.PlayOneShot(SoundDefOf.Thunder_OnMap, val2);
		EventDraw(map, strikeLoc, randomBoltMesh);
	}

	public void EventDraw(Map map, IntVec3 strikeLoc, Mesh boltMesh)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Graphics.DrawMesh(boltMesh, strikeLoc.ToVector3ShiftedWithAltitude((AltitudeLayer)29), Quaternion.identity, FadedMaterialPool.FadedVersionOf(TunnelHiveSpawnerStatic.LightningMat, 3f), 0);
	}

	private void DrawDustPart(float initialAngle, float speedMultiplier, float scale)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		float num = GenTicks.TicksToSeconds(Find.TickManager.TicksGame - secondarySpawnTick);
		IntVec3 position = ((Thing)this).Position;
		Vector3 val = position.ToVector3ShiftedWithAltitude((AltitudeLayer)6);
		Rand.PushState();
		val.y += 1f / 22f * Rand.Range(0f, 1f);
		Rand.PopState();
		Color val2 = new Color(0.47058824f, 0.38431373f, 0.3254902f, 0.7f);
		TunnelHiveSpawnerStatic.matPropertyBlock.SetColor(ShaderPropertyIDs.Color, val2);
		Matrix4x4 val3 = Matrix4x4.TRS(val, Quaternion.Euler(0f, initialAngle + speedMultiplier * num, 0f), Vector3.one * scale);
		Graphics.DrawMesh(MeshPool.plane10, val3, TunnelHiveSpawnerStatic.TunnelMaterial, 0, (Camera)null, 0, TunnelHiveSpawnerStatic.matPropertyBlock);
	}

	public virtual void MakeLord(Type lordJobType, List<Pawn> list)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Map map = ((Thing)this).Map;
		IntVec3 position = ((Thing)this).Position;
		if (GenCollection.Any<Pawn>(list))
		{
			Faction spawnedFaction = SpawnedFaction;
			object obj = Activator.CreateInstance(lordJobType, SpawnedFaction, false, false, false, false, false);
			LordMaker.MakeNewLord(spawnedFaction, (LordJob)((obj is LordJob) ? obj : null), map, (IEnumerable<Pawn>)null);
		}
	}

	private void CreateSustainer()
	{
		LongEventHandler.ExecuteWhenFinished((Action)delegate
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			SoundDef val = Ext.soundSustainer ?? SoundDefOf.Tunnel;
			sustainer = SoundStarter.TrySpawnSustainer(val, SoundInfo.InMap((Thing)this, (MaintenanceType)1));
		});
	}

	public override void SpawnSetup(Map map, bool respawningAfterLoad)
	{
		((ThingWithComps)this).SpawnSetup(map, respawningAfterLoad);
		ResetStaticData();
		if (!respawningAfterLoad)
		{
			if (Ext != null && Ext.spawnWavePoints > 0f)
			{
				initialPoints = Ext.spawnWavePoints;
			}
			Rand.PushState();
			secondarySpawnTick = Find.TickManager.TicksGame + GenTicks.SecondsToTicks(ResultSpawnDelay.RandomInRange);
			Rand.PopState();
			spawnTick = Find.TickManager.TicksGame;
		}
		CreateSustainer();
	}

	public static void ResetStaticData()
	{
		filthTypes.Clear();
		filthTypes.Add(DefDatabase<ThingDef>.GetNamed("Filth_Dirt"));
		filthTypes.Add(DefDatabase<ThingDef>.GetNamed("Filth_Dirt"));
		filthTypes.Add(DefDatabase<ThingDef>.GetNamed("Filth_Dirt"));
		filthTypes.Add(DefDatabase<ThingDef>.GetNamed("Filth_RubbleRock"));
	}

	public override void ExposeData()
	{
		((ThingWithComps)this).ExposeData();
		Scribe_Values.Look<int>(ref secondarySpawnTick, "secondarySpawnTick", 0, false);
		Scribe_Values.Look<int>(ref spawnTick, "spawnTick", 0, false);
		Scribe_Values.Look<bool>(ref spawnHive, "spawnHive", true, false);
		Scribe_Values.Look<float>(ref initialPoints, "initialPoints", 0f, false);
		Scribe_Values.Look<bool>(ref spawnedByInfestationThingComp, "spawnedByInfestationThingComp", false, false);
		Scribe_Defs.Look<FactionDef>(ref factiondef, "factiondef");
		Scribe_References.Look<Faction>(ref faction, "faction", false);
		Scribe_Deep.Look<Hive>(ref hive, "hive", (object[])null);
	}
}
