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
			List<Faction> allFactionsListForReading = Find.FactionManager.AllFactionsListForReading;
			List<Faction> allFactionsListForReading2 = Find.FactionManager.AllFactionsListForReading;
			if (Props.disallowedFactions != null)
			{
				foreach (Faction item in allFactionsListForReading)
				{
					if (!Props.disallowedFactions.Contains(item.def))
					{
						allFactionsListForReading2.Remove(item);
					}
				}
			}
			return allFactionsListForReading2;
		}
	}

	public TechLevel techLevel
	{
		get
		{
			if (Props.techLevel != null)
			{
				if (!(Props.techLevel == "Animal"))
				{
					if (!(Props.techLevel == "Archotech"))
					{
						if (!(Props.techLevel == "Industrial"))
						{
							if (!(Props.techLevel == "Medieval"))
							{
								if (!(Props.techLevel == "Neolithic"))
								{
									if (!(Props.techLevel == "Spacer"))
									{
										if (!(Props.techLevel == "Ultra"))
										{
											return (TechLevel)0;
										}
										return (TechLevel)6;
									}
									return (TechLevel)5;
								}
								return (TechLevel)2;
							}
							return (TechLevel)3;
						}
						return (TechLevel)4;
					}
					return (TechLevel)7;
				}
				return (TechLevel)1;
			}
			return (TechLevel)0;
		}
	}

	public Faction OfFaction
	{
		get
		{
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			if (faction == null)
			{
				if (((Thing)base.parent).Faction != null)
				{
					faction = ((Thing)base.parent).Faction;
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
					factionDef = faction.def;
					Props.faction = faction;
				}
			}
			return faction;
		}
		set
		{
			faction = value;
			((Thing)base.parent).SetFactionDirect(value);
		}
	}

	public Faction RandomEnemyFaction(bool allowHidden = false, bool allowDefeated = false, bool allowNonHumanlike = true, TechLevel minTechLevel = (TechLevel)0)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Faction result = default(Faction);
		if (GenCollection.TryRandomElement<Faction>(from x in Find.FactionManager.GetFactions(allowHidden, allowDefeated, allowNonHumanlike, minTechLevel, false)
			where FactionUtility.HostileTo(x, Faction.OfPlayer) && x.RandomPawnKind() != null
			select x, ref result))
		{
			return result;
		}
		return null;
	}

	public override void PostExposeData()
	{
		((ThingComp)this).PostExposeData();
		Scribe_References.Look<Lord>(ref lord, "defenseLord", false);
		Scribe_References.Look<Faction>(ref faction, "defenseFaction", false);
		Scribe_Values.Look<float>(ref pointsLeft, "PawnPointsLeft", 0f, false);
	}

	public static float Inverse(float val)
	{
		return 1f / val;
	}

	public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((ThingComp)this).PostPreApplyDamage(dinfo, ref absorbed);
		if (absorbed)
		{
			return;
		}
		if (((DamageInfo)(ref dinfo)).Def.harmsHealth)
		{
			if (((Thing)base.parent).Faction == null)
			{
				((Thing)base.parent).SetFactionDirect(OfFaction);
				if (GenList.NullOrEmpty<PawnGenOption>((IList<PawnGenOption>)spawnablePawnKinds))
				{
					if (!GenList.NullOrEmpty<PawnGenOption>((IList<PawnGenOption>)Props.allowedKinddefs))
					{
						spawnablePawnKinds = Props.allowedKinddefs;
					}
					else if (((Thing)base.parent).Faction != null)
					{
						if (GenList.NullOrEmpty<PawnGroupMaker>((IList<PawnGroupMaker>)((Thing)base.parent).Faction.def.pawnGroupMakers))
						{
							List<PawnKindDef> list = DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef x) => x.isFighter && x.defaultFactionType != null && x.defaultFactionType == ((Thing)base.parent).Faction.def).ToList();
							for (int num = 0; num < list.Count(); num++)
							{
								spawnablePawnKinds.Add(new PawnGenOption(list[num], Inverse(list[num].combatPower)));
							}
						}
						else
						{
							List<PawnGenOption> list2 = new List<PawnGenOption>();
							list2 = ((!GenCollection.Any<PawnGroupMaker>(((Thing)base.parent).Faction.def.pawnGroupMakers, (Predicate<PawnGroupMaker>)((PawnGroupMaker x) => x.kindDef == Props.factionGroupKindDef))) ? GenCollection.RandomElementByWeight<PawnGroupMaker>(((Thing)base.parent).Faction.def.pawnGroupMakers.Where((PawnGroupMaker x) => x.kindDef == PawnGroupKindDefOf.Combat || x.kindDef == PawnGroupKindDefOf.Settlement), (Func<PawnGroupMaker, float>)((PawnGroupMaker x) => x.commonality)).options : GenCollection.RandomElementByWeight<PawnGroupMaker>(((Thing)base.parent).Faction.def.pawnGroupMakers.Where((PawnGroupMaker x) => x.kindDef == Props.factionGroupKindDef), (Func<PawnGroupMaker, float>)((PawnGroupMaker x) => x.commonality)).options);
							for (int num2 = 0; num2 < list2.Count(); num2++)
							{
								spawnablePawnKinds.Add(new PawnGenOption(list2[num2]));
							}
						}
					}
				}
			}
			if (lord != null)
			{
				lord.ReceiveMemo(MemoDamaged);
			}
			float num3 = (float)((Thing)base.parent).HitPoints - ((DamageInfo)(ref dinfo)).Amount;
			if ((num3 < (float)((Thing)base.parent).MaxHitPoints * 0.98f && ((DamageInfo)(ref dinfo)).Instigator != null && ((DamageInfo)(ref dinfo)).Instigator.Faction != null) || num3 < (float)((Thing)base.parent).MaxHitPoints * 0.9f)
			{
				TrySpawnPawns();
			}
		}
		absorbed = false;
	}

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		((ThingComp)this).PostSpawnSetup(respawningAfterLoad);
		if (!respawningAfterLoad && pointsLeft == 0f)
		{
			pointsLeft = Mathf.Max(Props.defaultPoints * 0.9f, Props.minPoints);
		}
	}

	private void TrySpawnPawns()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<PawnGenOption> source = spawnablePawnKinds;
		if (pointsLeft <= 0f || !((Thing)base.parent).Spawned)
		{
			return;
		}
		if (lord == null)
		{
			IntVec3 invalid = default(IntVec3);
			if (!CellFinder.TryFindRandomCellNear(((Thing)base.parent).Position, ((Thing)base.parent).Map, 5, (Predicate<IntVec3>)((IntVec3 c) => GenGrid.Standable(c, ((Thing)base.parent).Map) && ((Thing)base.parent).Map.reachability.CanReach(c, LocalTargetInfo.op_Implicit((Thing)(object)base.parent), (PathEndMode)2, TraverseParms.For((TraverseMode)1, (Danger)3, false, false, false))), ref invalid, -1))
			{
				Log.Error("Found no place for Pawns to defend " + (object)this);
				invalid = IntVec3.Invalid;
			}
			LordJob_PawnsDefendShip lordJob_PawnsDefendShip = new LordJob_PawnsDefendShip((Thing)(object)base.parent, ((Thing)base.parent).Faction, 21f, invalid);
			lord = LordMaker.MakeNewLord(OfFaction, (LordJob)(object)lordJob_PawnsDefendShip, ((Thing)base.parent).Map, (IEnumerable<Pawn>)null);
		}
		try
		{
			PawnGenOption pawnGenOption = default(PawnGenOption);
			IntVec3 val = default(IntVec3);
			while (pointsLeft > 0f && GenCollection.TryRandomElementByWeight<PawnGenOption>(source.Select((PawnGenOption def) => def), (Func<PawnGenOption, float>)((PawnGenOption x) => x.selectionWeight), ref pawnGenOption) && GenCollection.TryRandomElement<IntVec3>(from cell in GenAdj.CellsAdjacent8Way((Thing)(object)base.parent)
				where CanSpawnPawnAt(cell)
				select cell, ref val))
			{
				Pawn val2 = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnGenOption.kind, faction, (PawnGenerationContext)2, -1, true, false, false, false, true, false, 1f, false, true, true, false, false, false, false, false, 0f, 0f, (Pawn)null, 1f, (Predicate<Pawn>)null, (Predicate<Pawn>)null, (IEnumerable<TraitDef>)null, (IEnumerable<TraitDef>)null, (float?)null, (float?)null, (float?)null, (Gender?)null, (float?)null, (string)null, (string)null, (RoyalTitleDef)null, (Ideo)null, false, false, false));
				if (!GenPlace.TryPlaceThing((Thing)(object)val2, val, ((Thing)base.parent).Map, (ThingPlaceMode)1, (Action<Thing, int>)null, (Predicate<IntVec3>)null, default(Rot4)))
				{
					Find.WorldPawns.PassToWorld(val2, (PawnDiscardDecideMode)2);
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
		SoundStarter.PlayOneShotOnCamera(SoundDefOf.PsychicPulseGlobal, ((Thing)base.parent).Map);
	}

	private bool CanSpawnPawnAt(IntVec3 c)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return GenGrid.Walkable(c, ((Thing)base.parent).Map);
	}
}
