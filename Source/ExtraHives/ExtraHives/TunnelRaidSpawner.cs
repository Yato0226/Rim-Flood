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

[StaticConstructorOnStartup]
public class TunnelRaidSpawner : ThingWithComps, IThingHolder
{
	protected ThingOwner innerContainer;

	public FactionDef factiondef = null;

	private Faction faction = null;

	private int secondarySpawnTick;

	public bool spawnHive = true;

	public float initialPoints;

	public bool spawnedByInfestationThingComp;

	private Sustainer sustainer;

	private static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();

	public FloatRange ResultSpawnDelay = new FloatRange(3f, 6f);

	[TweakValue("Gameplay", 0f, 1f)]
	private static float DustMoteSpawnMTB = 0.2f;

	[TweakValue("Gameplay", 0f, 1f)]
	private static float FilthSpawnMTB = 0.3f;

	[TweakValue("Gameplay", 0f, 10f)]
	private static float FilthSpawnRadius = 3f;

	public Type lordJobType = typeof(LordJob_AssaultColony);

	private static readonly Material TunnelMaterial = MaterialPool.MatFrom("Things/Filth/Grainy/GrainyA", ShaderDatabase.Transparent);

	public List<PawnGenOption> spawnablePawnKinds = new List<PawnGenOption>();

	private static List<ThingDef> filthTypes = new List<ThingDef>();

	public int SpawnTick
	{
		get
		{
			return secondarySpawnTick;
		}
		set
		{
			secondarySpawnTick = value;
		}
	}

	public Faction SpawnedFaction
	{
		get
		{
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

	public TunnelRaidSpawner()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		innerContainer = (ThingOwner)(object)new ThingOwner<Thing>((IThingHolder)(object)this, false, (LookMode)2);
		ResetStaticData();
	}

	public ThingOwner GetDirectlyHeldThings()
	{
		return innerContainer;
	}

	public void GetChildHolders(List<IThingHolder> outChildren)
	{
		ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, (IList<Thing>)GetDirectlyHeldThings());
	}

	public static void ResetStaticData()
	{
		filthTypes.Clear();
		        filthTypes.Add(DefDatabase<ThingDef>.GetNamed("Filth_Dirt"));
		        filthTypes.Add(DefDatabase<ThingDef>.GetNamed("Filth_Dirt"));		filthTypes.Add(DefDatabase<ThingDef>.GetNamed("Filth_Dirt"));
		filthTypes.Add(DefDatabase<ThingDef>.GetNamed("Filth_RubbleRock"));
	}

	public override void ExposeData()
	{
		((ThingWithComps)this).ExposeData();
		Scribe_Values.Look<int>(ref secondarySpawnTick, "secondarySpawnTick", 0, false);
		Scribe_Values.Look<bool>(ref spawnHive, "spawnHive", true, false);
		Scribe_Values.Look<float>(ref initialPoints, "insectsPoints", 0f, false);
		Scribe_Values.Look<bool>(ref spawnedByInfestationThingComp, "spawnedByInfestationThingComp", false, false);
		Scribe_Defs.Look<FactionDef>(ref factiondef, "factiondef");
		Scribe_References.Look<Faction>(ref faction, "faction", false);
		Scribe_Collections.Look<PawnGenOption>(ref spawnablePawnKinds, "spawnablePawnKinds", (LookMode)0, Array.Empty<object>());
		Scribe_Deep.Look<ThingOwner>(ref innerContainer, "innerContainer", new object[1] { this });
	}

	public override void SpawnSetup(Map map, bool respawningAfterLoad)
	{
		((ThingWithComps)this).SpawnSetup(map, respawningAfterLoad);
		if (!respawningAfterLoad)
		{
			secondarySpawnTick = Find.TickManager.TicksGame + GenTicks.SecondsToTicks(ResultSpawnDelay.RandomInRange);
		}
		CreateSustainer();
	}

	protected override void Tick()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		if (((Thing)this).Spawned)
		{
			sustainer.Maintain();
			IntVec3 position = ((Thing)this).Position;
			Vector3 val = position.ToVector3Shifted();
			Rand.PushState();
			IntVec3 val2 = default(IntVec3);
			if (Rand.MTBEventOccurs(FilthSpawnMTB, 1f, GenTicks.TicksToSeconds(1)) && CellFinder.TryFindRandomReachableCellNearPosition(((Thing)this).Position, ((Thing)this).Position, ((Thing)this).Map, FilthSpawnRadius, TraverseParms.For((TraverseMode)2, (Danger)3, false, false, false), (Predicate<IntVec3>)null, (Predicate<Region>)null, out val2, 999999))
			{
				FilthMaker.TryMakeFilth(val2, ((Thing)this).Map, GenCollection.RandomElement<ThingDef>((IEnumerable<ThingDef>)filthTypes), 1, (FilthSourceFlags)0, true);
			}
			if (Rand.MTBEventOccurs(DustMoteSpawnMTB, 1f, GenTicks.TicksToSeconds(1)))
			{
				Vector3 val3 = new Vector3(val.x, 0f, val.z);
				val3.y = Altitudes.AltitudeFor((AltitudeLayer)26);
				FleckMaker.ThrowDustPuffThick(val3, ((Thing)this).Map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));
			}
			Rand.PopState();
			if (secondarySpawnTick <= Find.TickManager.TicksGame)
			{
				sustainer.End();
				List<Pawn> list = new List<Pawn>();
				SpawnThings(out list);
				((Thing)this).Destroy((DestroyMode)0);
			}
		}
	}

	public virtual void SpawnThings(out List<Pawn> list)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		list = new List<Pawn>();
		Map map = ((Thing)this).Map;
		IntVec3 position = ((Thing)this).Position;
		if (initialPoints > 0f && !GenList.NullOrEmpty<PawnGenOption>((IList<PawnGenOption>)spawnablePawnKinds))
		{
			initialPoints = Mathf.Max(initialPoints, spawnablePawnKinds.Min((PawnGenOption x) => x.Cost));
			float pointsLeft = initialPoints;
			int num = 0;
			PawnGenOption val = default(PawnGenOption);
			for (; pointsLeft > 0f; pointsLeft -= val.Cost)
			{
				num++;
				if (num > 1000)
				{
					Log.Error("Too many iterations.");
					break;
				}
				if (!GenCollection.TryRandomElementByWeight<PawnGenOption>(spawnablePawnKinds.Where((PawnGenOption x) => x.Cost <= pointsLeft), (Func<PawnGenOption, float>)((PawnGenOption x) => x.selectionWeight), out val))
				{
					break;
				}
				Pawn val2 = PawnGenerator.GeneratePawn(val.kind, SpawnedFaction);
				GenSpawn.Spawn((Thing)(object)val2, CellFinder.RandomClosewalkCellNear(position, map, 2, (Predicate<IntVec3>)null), map, (WipeMode)0);
				val2.mindState.spawnedByInfestationThingComp = spawnedByInfestationThingComp;
				list.Add(val2);
			}
		}
		if (!GenList.NullOrEmpty<Thing>((IList<Thing>)innerContainer))
		{
			innerContainer.TryDropAll(position, map, (ThingPlaceMode)1, (Action<Thing, int>)null, (Predicate<IntVec3>)((IntVec3 x) => GenGrid.Walkable(x, map) && IntVec3Utility.DistanceTo(position, x) > 2f), true);
		}
		if (GenCollection.Any<Pawn>(list))
		{
			MakeLord(lordJobType, list);
		}
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
		val.y += 3f / 64f * Rand.Range(0f, 1f);
		Rand.PopState();
		Color val2 = new Color(0.47058824f, 0.38431373f, 0.3254902f, 0.7f);
		matPropertyBlock.SetColor(ShaderPropertyIDs.Color, val2);
		Matrix4x4 val3 = Matrix4x4.TRS(val, Quaternion.Euler(0f, initialAngle + speedMultiplier * num, 0f), Vector3.one * scale);
		Graphics.DrawMesh(MeshPool.plane10, val3, TunnelMaterial, 0, (Camera)null, 0, matPropertyBlock);
	}

	private void CreateSustainer()
	{
		LongEventHandler.ExecuteWhenFinished((Action)delegate
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			SoundDef tunnel = SoundDefOf.Tunnel;
			sustainer = SoundStarter.TrySpawnSustainer(tunnel, SoundInfo.InMap((Thing)this, (MaintenanceType)1));
		});
	}
}
