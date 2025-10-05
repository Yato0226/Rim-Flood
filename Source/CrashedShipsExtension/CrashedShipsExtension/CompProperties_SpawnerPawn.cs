using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace CrashedShipsExtension;

public class CompProperties_SpawnerPawn : CompProperties
{
	public List<PawnGenOption> spawnablePawnKinds = new List<PawnGenOption>();

	public List<PawnKindDef> AlwaysSpawnWith = new List<PawnKindDef>();

	public SoundDef spawnSound;

	public string spawnMessageKey;

	public string noPawnsLeftToSpawnKey;

	public string pawnsLeftToSpawnKey;

	public bool showNextSpawnInInspect;

	public bool shouldJoinParentLord;

	public Type lordJob;

	public float defendRadius = 21f;

	public int initialSpawnDelay = 120;

	public int initialPawnsCount;

	public float initialPawnsPoints;

	public float maxSpawnedPawnsPoints = -1f;

	public FloatRange pawnSpawnIntervalDays = new FloatRange(0.85f, 1.15f);

	public int pawnSpawnRadius = 2;

	public IntRange maxPawnsToSpawn = IntRange.zero;

	public bool chooseSingleTypeToSpawn;

	public string nextSpawnInspectStringKey;

	public string nextSpawnInspectStringKeyDormant;

	public PawnGroupKindDef factionGroupKindDef;

	public CompProperties_SpawnerPawn()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		base.compClass = typeof(CompSpawnerPawn);
	}

	public override void ResolveReferences(ThingDef parentDef)
	{
		((CompProperties)this).ResolveReferences(parentDef);
		if (factionGroupKindDef == null)
		{
			factionGroupKindDef = PawnGroupKindDefOf.Combat;
		}
	}
}
