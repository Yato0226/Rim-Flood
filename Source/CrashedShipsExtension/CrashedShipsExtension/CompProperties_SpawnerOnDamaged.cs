using System.Collections.Generic;
using RimWorld;
using Verse;

namespace CrashedShipsExtension;

public class CompProperties_SpawnerOnDamaged : CompProperties
{
	public FactionDef Faction;

	public Faction faction;

	public List<PawnGenOption> allowedKinddefs = new List<PawnGenOption>();

	public List<PawnKindDef> disallowedKinddefs = new List<PawnKindDef>();

	public List<FactionDef> Factions = new List<FactionDef>();

	public List<FactionDef> disallowedFactions = new List<FactionDef>();

	public string techLevel;

	public bool allowHidden = true;

	public bool allowNonHumanlike;

	public bool allowDefeated = true;

	public ThingDef skyFaller;

	public float defaultPoints = 550f;

	public float minPoints = 300f;

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
			factionGroupKindDef = PawnGroupKindDefOf.Combat;
		}
	}
}
