using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ExtraHives;

public class CompProperties_SpawnerPawn : CompProperties
{
	public List<PawnGenOption> spawnablePawnKinds = new List<PawnGenOption>();

	public List<PawnKindDef> AlwaysSpawnWith = new List<PawnKindDef>();

	public SoundDef spawnSound;

	public string spawnMessageKey;

	public string noPawnsLeftToSpawnKey;

	public string pawnsLeftToSpawnKey;

	public bool showNextSpawnInInspect;

	public bool assaultOnSpawn;

	public bool shouldJoinParentLord;

	public Type lordJob;

	public float defendRadius = 21f;

	public int initialSpawnDelay = 0;

	public int initialPawnsCount = 10;

	public int initialPawnsPoints;

	public float maxSpawnedPawnsPoints = -1f;

	public FloatRange pawnSpawnIntervalDays = new FloatRange(0.85f, 1.15f);

	public int pawnSpawnRadius = 2;

	public IntRange maxPawnsToSpawn = IntRange.Zero;

	public bool chooseSingleTypeToSpawn;

	public string nextSpawnInspectStringKey;

	public string nextSpawnInspectStringKeyDormant;

	public PawnGroupKindDef factionGroupKindDef;

	public CompProperties_SpawnerPawn()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		base.compClass = typeof(CompSpawnerPawn);
	}

	public override void ResolveReferences(ThingDef parentDef)
	{
		((CompProperties)this).ResolveReferences(parentDef);
		if (factionGroupKindDef == null)
		{
			factionGroupKindDef = PawnGroupKindDefOf.Hive_ExtraHives;
		}
	}
}
