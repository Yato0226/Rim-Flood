using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace ExtraHives;

public class CompSpawnerOnDamaged : ThingComp
{
	public FactionDef factionDef;

	public int lastSpawnTick = -1;

	public Faction faction = null;

	public float pointsLeft;

	private Lord lord;

	public static readonly string MemoDamaged = "ShipPartDamaged";

	private List<Faction> allFactions = new List<Faction>();

	public List<PawnGenOption> spawnablePawnKinds = new List<PawnGenOption>();

	public CompProperties_SpawnerOnDamaged Props => (CompProperties_SpawnerOnDamaged)(object)base.props;

	public Lord Lord => lord;

	public Faction OfFaction
	{
		get
		{
			if (faction == null && ((Thing)base.parent).Faction != null)
			{
				faction = ((Thing)base.parent).Faction;
			}
			return faction;
		}
		set
		{
			faction = value;
		}
	}

	public override void PostExposeData()
	{
		((ThingComp)this).PostExposeData();
		Scribe_References.Look<Lord>(ref lord, "defenseLord", false);
		Scribe_References.Look<Faction>(ref faction, "defenseFaction", false);
		Scribe_Values.Look<float>(ref pointsLeft, "PawnPointsLeft", 0f, false);
		Scribe_Values.Look<int>(ref lastSpawnTick, "lastSpawnTick", 0, false);
	}

	public override void CompTick()
	{
		((ThingComp)this).CompTick();
		if (lastSpawnTick > 0)
		{
			lastSpawnTick--;
		}
	}

	public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
	{
		((ThingComp)this).PostPreApplyDamage(ref dinfo, out absorbed);
		if (absorbed)
		{
			return;
		}
		if (dinfo.Def.harmsHealth)
		{
			if (lord != null)
			{
				lord.ReceiveMemo(MemoDamaged);
			}
			float num = (float)((Thing)base.parent).HitPoints - dinfo.Amount;
			if ((num < (float)((Thing)base.parent).MaxHitPoints * 0.98f && dinfo.Instigator != null && dinfo.Instigator.Faction != null) || num < (float)((Thing)base.parent).MaxHitPoints * 0.9f)
			{
				TrySpawnPawns();
			}
		}
		absorbed = false;
	}

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		((ThingComp)this).PostSpawnSetup(respawningAfterLoad);
		if (respawningAfterLoad)
		{
			return;
		}
		if (((Thing)base.parent).Faction == null)
		{
			((Thing)base.parent).SetFaction(OfFaction, (Pawn)null);
		}
		if (pointsLeft == 0f)
		{
			pointsLeft = Mathf.Max(Props.defaultPoints * 0.9f, Props.minPoints);
		}
		if (!GenList.NullOrEmpty<PawnGenOption>((IList<PawnGenOption>)spawnablePawnKinds) || ((Thing)base.parent).Faction == null)
		{
			return;
		}
		if (GenCollection.Any<PawnGroupMaker>(((Thing)base.parent).Faction.def.pawnGroupMakers, (Predicate<PawnGroupMaker>)((PawnGroupMaker x) => x.kindDef == Props.factionGroupKindDef)))
		{
			spawnablePawnKinds = GenCollection.RandomElementByWeight<PawnGroupMaker>(((Thing)base.parent).Faction.def.pawnGroupMakers.Where((PawnGroupMaker x) => x.kindDef == Props.factionGroupKindDef), (Func<PawnGroupMaker, float>)((PawnGroupMaker x) => x.commonality)).options;
		}
		else
		{
			spawnablePawnKinds = GenCollection.RandomElementByWeight<PawnGroupMaker>(((Thing)base.parent).Faction.def.pawnGroupMakers.Where((PawnGroupMaker x) => x.kindDef == PawnGroupKindDefOf.Hive_ExtraHives), (Func<PawnGroupMaker, float>)((PawnGroupMaker x) => x.commonality)).options;
		}
	}

	private void TrySpawnPawns()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		if (lastSpawnTick > 0 || pointsLeft <= 0f || !((Thing)base.parent).Spawned)
		{
			return;
		}
		if (lord == null)
		{
			LordJob_AssaultColony val = new LordJob_AssaultColony(((Thing)base.parent).Faction, false, false, false, false, false, false, false);
			lord = LordMaker.MakeNewLord(OfFaction, (LordJob)(object)val, ((Thing)base.parent).Map, (IEnumerable<Pawn>)null);
		}
		try
		{
			PawnGenOption val2 = default(PawnGenOption);
			IntVec3 val3 = default(IntVec3);
			PawnGenerationRequest val4 = default(PawnGenerationRequest);
			while (true)
			{
				if (!(pointsLeft > 0f))
				{
					break;
				}
				if (!GenCollection.TryRandomElementByWeight<PawnGenOption>(spawnablePawnKinds.Select((PawnGenOption def) => def), (Func<PawnGenOption, float>)((PawnGenOption x) => x.selectionWeight), out val2))
				{
					break;
				}
				if (!GenCollection.TryRandomElement<IntVec3>((from cell in GenAdj.CellsAdjacent8Way((Thing)(object)base.parent) where CanSpawnPawnAt(cell) select cell).ToList(), out val3))
				{
					break;
				}
				{
					val4 = new PawnGenerationRequest(val2.kind, faction, PawnGenerationContext.NonPlayer, fixedBiologicalAge: -1, forceGenerateNewPawn: true, canGeneratePawnRelations: true, allowAddictions: true, allowFood: true);
					Pawn val5 = PawnGenerator.GeneratePawn(val4);
					if (!GenPlace.TryPlaceThing((Thing)(object)val5, val3, ((Thing)base.parent).Map, (ThingPlaceMode)1, (Action<Thing, int>)null, (Predicate<IntVec3>)null, default(Rot4)))
					{
						Find.WorldPawns.PassToWorld(val5, (PawnDiscardDecideMode)2);
						break;
					}
					lord.AddPawn(val5);
					pointsLeft -= val5.kindDef.combatPower;
				}
			}
		}
		finally
		{
			pointsLeft = 0f;
		}
		SoundStarter.PlayOneShotOnCamera(SoundDefOf.PsychicPulseGlobal, ((Thing)base.parent).Map);
		if (Props.minTimeBetween > 0f)
		{
			lastSpawnTick = (int)(Props.minTimeBetween * 60000f / Find.Storyteller.difficulty.enemyReproductionRateFactor);
		}
	}

	private bool CanSpawnPawnAt(IntVec3 c)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GenGrid.Walkable(c, ((Thing)base.parent).Map);
	}
}
