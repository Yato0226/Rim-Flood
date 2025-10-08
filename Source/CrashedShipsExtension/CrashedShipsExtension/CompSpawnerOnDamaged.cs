using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace CrashedShipsExtension;

public class CompSpawnerOnDamaged : ThingComp
{
	public FactionDef factionDef;

	public Faction faction;

	public float pointsLeft;

	private Lord lord;

	private const float PawnsDefendRadius = 21f;

	public static readonly string MemoDamaged = "ShipPartDamaged";

	private List<Faction> allFactions = new List<Faction>();

	public List<PawnGenOption> spawnablePawnKinds = new List<PawnGenOption>();

	public CompProperties_SpawnerOnDamaged Props => (CompProperties_SpawnerOnDamaged)(object)base.props;

	public Lord Lord => lord;

	public List<Faction> AllFactions
	{
		get
		{
			if (Props.disallowedFactions == null)
			{
				return Find.FactionManager.AllFactions.ToList();
			}
			return Find.FactionManager.AllFactions.Where(fac => !Props.disallowedFactions.Contains(fac.def)).ToList();
		}
	}

	public TechLevel techLevel
	{
		get
		{
			if (Props.techLevel != null && Enum.TryParse<TechLevel>(Props.techLevel, out TechLevel result))
			{
				return result;
			}
			return TechLevel.Undefined;
		}
	}

	public Faction OfFaction
	{
		get
		{
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			if (faction == null)
			{
				if (parent.Faction != null)
				{
					faction = parent.Faction;
					return faction;
				}
				if (Props.Faction != null)
				{
					factionDef = Props.Faction;
					if (Find.FactionManager.FirstFactionOfDef(factionDef) != null)
					{
						faction = Find.FactionManager.FirstFactionOfDef(factionDef);
						return faction;
					}
				}
				if (Props.Factions.Count > 0)
				{
					factionDef = GenCollection.RandomElement<FactionDef>(Props.Factions.Where((FactionDef x) => Find.FactionManager.FirstFactionOfDef(x) != null));
					if (factionDef != null)
					{
						faction = Find.FactionManager.FirstFactionOfDef(factionDef);
					}
				}
				if (faction == null)
				{
					faction = RandomEnemyFaction(Props.allowHidden, Props.allowDefeated, Props.allowNonHumanlike, techLevel);
					if (faction != null)
					{
						factionDef = faction.def;
						//Props.faction = faction;
					}
				}
			}
			return faction;
		}
		set
		{
			faction = value;
			parent.SetFactionDirect(value);
		}
	}

	public Faction RandomEnemyFaction(bool allowHidden = false, bool allowDefeated = false, bool allowNonHumanlike = true, TechLevel minTechLevel = (TechLevel)0)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Faction result = default(Faction);
		if (GenCollection.TryRandomElement(from x in Find.FactionManager.GetFactions(allowHidden, allowDefeated, allowNonHumanlike, minTechLevel, false)
			where FactionUtility.HostileTo(x, Faction.OfPlayerSilentFail) && x.RandomPawnKind() != null
			select x, out result))
		{
			return result;
		}
		return null;
	}

	public override void PostExposeData()
	{
		base.PostExposeData();
		Scribe_References.Look<Lord>(ref lord, "defenseLord", false);
		Scribe_References.Look<Faction>(ref faction, "defenseFaction", false);
		Scribe_Values.Look<float>(ref pointsLeft, "PawnPointsLeft", 0f, false);
	}

	public static float Inverse(float val)
	{
		return 1f / val;
	}

	public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
	{
		base.PostPreApplyDamage(ref dinfo, out absorbed);
		if (absorbed)
		{
			return;
		}
		if (dinfo.Def.harmsHealth)
		{
			if (parent.Faction == null)
			{
				parent.SetFactionDirect(OfFaction);
				if (spawnablePawnKinds.NullOrEmpty())
				{
					if (!Props.allowedKinddefs.NullOrEmpty())
					{
						spawnablePawnKinds = Props.allowedKinddefs;
					}
					else if (parent.Faction != null)
					{
						if (parent.Faction.def.pawnGroupMakers.NullOrEmpty())
						{
							List<PawnKindDef> list = DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef x) => x.isFighter && x.defaultFactionDef != null && x.defaultFactionDef == parent.Faction.def).ToList();
							for (int num = 0; num < list.Count(); num++)
							{
								spawnablePawnKinds.Add(new PawnGenOption(list[num], Inverse(list[num].combatPower)));
							}
						}
						                        else
						                        {
						                            List<RimWorld.PawnGenOption> list2 = new List<RimWorld.PawnGenOption>();
						                            list2 = ((!parent.Faction.def.pawnGroupMakers.Any((PawnGroupMaker x) => x.kindDef == Props.factionGroupKindDef))) ? GenCollection.RandomElementByWeight<PawnGroupMaker>(parent.Faction.def.pawnGroupMakers.Where((PawnGroupMaker x) => x.kindDef == PawnGroupKindDefOf.Combat || x.kindDef == PawnGroupKindDefOf.Settlement), (PawnGroupMaker x) => x.commonality).options : GenCollection.RandomElementByWeight<PawnGroupMaker>(parent.Faction.def.pawnGroupMakers.Where((PawnGroupMaker x) => x.kindDef == Props.factionGroupKindDef), (PawnGroupMaker x) => x.commonality).options;
						                            for (int num2 = 0; num2 < list2.Count(); num2++)
						                            {
						                                spawnablePawnKinds.Add(new PawnGenOption(list2[num2].kind, list2[num2].selectionWeight));
						                            }
						                        }					}
				}
			}
			if (lord != null)
			{
				lord.ReceiveMemo(MemoDamaged);
			}
			float num3 = parent.HitPoints - dinfo.Amount;
			if ((num3 < (float)parent.MaxHitPoints * 0.98f && dinfo.Instigator != null && dinfo.Instigator.Faction != null) || num3 < (float)parent.MaxHitPoints * 0.9f)
			{
				TrySpawnPawns();
			}
		}
		absorbed = false;
	}

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		base.PostSpawnSetup(respawningAfterLoad);
		if (!respawningAfterLoad && pointsLeft == 0f)
		{
			pointsLeft = Mathf.Max(Props.defaultPoints * 0.9f, Props.minPoints);
		}
	}

	private void TrySpawnPawns()
	{
		IEnumerable<PawnGenOption> source = spawnablePawnKinds;
		if (pointsLeft <= 0f || !parent.Spawned)
		{
			return;
		}
		if (lord == null)
		{
			IntVec3 invalid = default(IntVec3);
			if (!CellFinder.TryFindRandomCellNear(parent.Position, parent.Map, 5, (Predicate<IntVec3>)((IntVec3 c) => c.Standable(parent.Map) && parent.Map.reachability.CanReach(c, parent, PathEndMode.Touch, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false, false, false))), out invalid, -1))
			{
				Log.Error("Found no place for Pawns to defend " + (object)this);
				invalid = IntVec3.Invalid;
			}
			LordJob_PawnsDefendShip lordJob_PawnsDefendShip = new LordJob_PawnsDefendShip(parent, parent.Faction, 21f, invalid);
			lord = LordMaker.MakeNewLord(OfFaction, lordJob_PawnsDefendShip, parent.Map, null);
		}
		try
		{
			PawnGenOption pawnGenOption = default(PawnGenOption);
			IntVec3 val = default(IntVec3);
			while (pointsLeft > 0f && source.TryRandomElementByWeight((PawnGenOption x) => x.selectionWeight, out pawnGenOption) && (from cell in GenAdj.CellsAdjacent8Way(parent)
				where CanSpawnPawnAt(cell)
				select cell).TryRandomElement(out val))
			{
				PawnGenerationRequest request = new PawnGenerationRequest(pawnGenOption.kind, faction, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, 1f, false, true, true, true, true);
				Pawn val2 = PawnGenerator.GeneratePawn(request);
				if (!GenPlace.TryPlaceThing(val2, val, parent.Map, ThingPlaceMode.Direct, null, null, default(Rot4)))
				{
					Find.WorldPawns.PassToWorld(val2, PawnDiscardDecideMode.Discard);
					break;
				}
				lord.AddPawn(val2);
				pointsLeft -= val2.kindDef.combatPower;
			}
		}
		finally
		{
			pointsLeft = 0f;
		}
		SoundStarter.PlayOneShotOnCamera(SoundDefOf.PsychicPulseGlobal, parent.Map);
	}

	private bool CanSpawnPawnAt(IntVec3 c)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return GenGrid.Walkable(c, parent.Map);
	}
}
