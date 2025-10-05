using RimWorld;
using Verse;

namespace ExtraHives;

public class CompProperties_SpawnerOnDamaged : CompProperties
{
	public float defaultPoints = 550f;

	public float minPoints = 300f;

	public float minTimeBetween = -1f;

	public PawnGroupKindDef factionGroupKindDef;

	public CompProperties_SpawnerOnDamaged()
	{
		base.compClass = typeof(CompSpawnerOnDamaged);
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
