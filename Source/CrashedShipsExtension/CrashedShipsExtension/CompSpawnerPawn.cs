using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace CrashedShipsExtension;

public class CompSpawnerPawn : ThingComp
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<Thing, List<Pawn>> _003C_003E9__13_0;

		public static RegionEntryPredicate _003C_003E9__13_2;

		public static Func<PawnGroupMaker, float> _003C_003E9__21_5;

		public static Func<PawnGroupMaker, bool> _003C_003E9__21_6;

		public static Func<PawnGroupMaker, float> _003C_003E9__21_7;

		public static Func<PawnGenOption, float> _003C_003E9__21_1;

		public static Predicate<Pawn> _003C_003E9__28_0;

		public static Predicate<Pawn> _003C_003E9__30_0;

		internal List<Pawn> _003CFindLordToJoin_003Eb__13_0(Thing s)
		{
			return ThingCompUtility.TryGetComp<CompSpawnerPawn>(s)?.spawnedPawns;
		}

		internal bool _003CFindLordToJoin_003Eb__13_2(Region from, Region to)
		{
			return true;
		}

		internal float _003CRandomPawnKindDef_003Eb__21_5(PawnGroupMaker x)
		{
			return x.commonality;
		}

		internal bool _003CRandomPawnKindDef_003Eb__21_6(PawnGroupMaker x)
		{
			if (x.kindDef != PawnGroupKindDefOf.Combat)
			{
				return x.kindDef == PawnGroupKindDefOf.Settlement;
			}
			return true;
		}

		internal float _003CRandomPawnKindDef_003Eb__21_7(PawnGroupMaker x)
		{
			return x.commonality;
		}

		internal float _003CRandomPawnKindDef_003Eb__21_1(PawnGenOption x)
		{
			return x.selectionWeight;
		}

		internal bool _003CCompGetGizmosExtra_003Eb__28_0(Pawn x)
		{
			return ((Thing)x).def.race.Humanlike;
		}

		internal bool _003CPostExposeData_003Eb__30_0(Pawn x)
		{
			return x == null;
		}
	}

	private Lord lord;

	public int nextPawnSpawnTick = -1;

	public int initialSpawnDelay = -1;

	public int pawnsLeftToSpawn = -1;

	public List<Pawn> spawnedPawns = new List<Pawn>();

	public List<PawnGenOption> spawnablePawnKinds = new List<PawnGenOption>();

	public bool aggressive = true;

	public bool canSpawnPawns = true;

	private PawnKindDef chosenKind;

	private CompCanBeDormant dormancyCompCached;

	private CompProperties_SpawnerPawn Props => (CompProperties_SpawnerPawn)(object)base.props;

	public Lord Lord
	{
		get
		{
			if (lord == null)
			{
				lord = FindLordToJoin(parent, Props.lordJob, Props.shouldJoinParentLord);
			}
			return lord;
		}
	}

	private float SpawnedPawnsPoints
	{
		get
		{
			FilterOutUnspawnedPawns();
			float num = 0f;
			for (int i = 0; i < spawnedPawns.Count; i++)
			{
				num += spawnedPawns[i].kindDef.combatPower;
			}
			return num;
		}
	}

	public bool Active
	{
		get
		{
			if (pawnsLeftToSpawn != 0)
			{
				return !Dormant;
			}
			return false;
		}
	}

	public CompCanBeDormant DormancyComp
	{
		get
		{
			CompCanBeDormant result;
			if ((result = dormancyCompCached) == null)
			{
				result = (dormancyCompCached = parent.GetComp<CompCanBeDormant>());
			}
			return result;
		}
	}

	public bool Dormant
	{
		get
		{
			if (DormancyComp != null)
			{
				return !DormancyComp.Awake;
			}
			return false;
		}
	}

	public override void Initialize(CompProperties props)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(props);
		if (chosenKind == null)
		{
			chosenKind = RandomPawnKindDef();
		}
		if (Props.maxPawnsToSpawn != IntRange.Zero)
		{
			pawnsLeftToSpawn = Props.maxPawnsToSpawn.RandomInRange;
		}
	}

	public static Lord FindLordToJoin(Thing spawner, Type lordJobType, bool shouldTryJoinParentLord, Func<Thing, List<Pawn>> spawnedPawnSelector = null)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		if (spawner.Spawned)
		{
			if (shouldTryJoinParentLord)
			{
				Thing obj = spawner;
				Building val = (Building)(object)((obj is Building) ? obj : null);
				Lord val2 = ((val != null) ? LordUtility.GetLord(val) : null);
				if (val2 != null)
				{
					return val2;
				}
			}
			if (spawnedPawnSelector == null)
			{
				spawnedPawnSelector = (Thing s) => ThingCompUtility.TryGetComp<CompSpawnerPawn>(s)?.spawnedPawns;
			}
			Predicate<Pawn> hasJob = delegate(Pawn x)
			{
				Lord val5 = LordUtility.GetLord(x);
				return val5 != null && ((object)val5.LordJob).GetType() == lordJobType;
			};
			Pawn foundPawn = null;
			CellRect val3 = GenAdj.OccupiedRect(spawner);
			Region region = GridsUtility.GetRegion(val3.AdjacentCells.RandomElement(), spawner.Map, (RegionType)14);
			object obj2 = _003C_003Ec._003C_003E9__13_2;
			if (obj2 == null)
			{
				RegionEntryPredicate val4 = (Region from, Region to) => true;
				_003C_003Ec._003C_003E9__13_2 = val4;
				obj2 = (object)val4;
			}
			RegionTraverser.BreadthFirstTraverse(region, (RegionEntryPredicate)obj2, (RegionProcessor)delegate(Region r)
			{
				List<Thing> list = r.ListerThings.ThingsOfDef(spawner.def);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].Faction == spawner.Faction)
					{
						List<Pawn> list2 = spawnedPawnSelector(list[i]);
						if (list2 != null)
						{
							foundPawn = list2.Find(hasJob);
						}
						if (foundPawn != null)
						{
							return true;
						}
					}
				}
				return false;
			}, 40, (RegionType)14);
			if (foundPawn != null)
			{
				return LordUtility.GetLord(foundPawn);
			}
		}
		return null;
	}

	public static Lord CreateNewLord(Thing byThing, bool aggressive, float defendRadius, Type lordJobType)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 invalid = default(IntVec3);
		if (!CellFinder.TryFindRandomCellNear(byThing.Position, byThing.Map, 5, (Predicate<IntVec3>)((IntVec3 c) => GenGrid.Standable(c, byThing.Map) && byThing.Map.reachability.CanReach(c, new LocalTargetInfo(byThing), PathEndMode.OnCell, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false, false, false))), out invalid, -1))
		{
			Log.Error("Found no place for pawns to defend " + (object)byThing);
			invalid = IntVec3.Invalid;
		}
		Faction faction = byThing.Faction;
		object obj = Activator.CreateInstance(lordJobType, (object)new SpawnedPawnParams
		{
			aggressive = aggressive,
			defendRadius = defendRadius,
			defSpot = invalid,
			spawnerThing = byThing
		});
		return LordMaker.MakeNewLord(faction, (LordJob)((obj is LordJob) ? obj : null), byThing.Map, (IEnumerable<Pawn>)null);
	}

	private void SpawnInitialPawns()
	{
		Pawn pawn;
		for (int i = 0; i < Props.initialPawnsCount; i++)
		{
			if (!TrySpawnPawn(out pawn))
			{
				break;
			}
		}
		SpawnPawnsUntilPoints(Props.initialPawnsPoints);
		if (!GenList.NullOrEmpty<PawnKindDef>((IList<PawnKindDef>)Props.AlwaysSpawnWith))
		{
			foreach (PawnKindDef item in Props.AlwaysSpawnWith)
			{
				_ = item;
				TrySpawnPawn(out pawn);
			}
		}
		CalculateNextPawnSpawnTick();
	}

	public void SpawnPawnsUntilPoints(float points)
	{
		int num = 0;
		while (SpawnedPawnsPoints < points)
		{
			num++;
			if (num > 1000)
			{
				Log.Error("Too many iterations.");
				break;
			}
			if (!TrySpawnPawn(out var _))
			{
				break;
			}
		}
		CalculateNextPawnSpawnTick();
	}

	private void CalculateNextPawnSpawnTick()
	{
		CalculateNextPawnSpawnTick(Props.pawnSpawnIntervalDays.RandomInRange * 60000f);
	}

	public void CalculateNextPawnSpawnTick(float delayTicks)
	{
		float num = GenMath.LerpDouble(0f, 5f, 1f, 0.5f, (float)spawnedPawns.Count);
		nextPawnSpawnTick = Find.TickManager.TicksGame + (int)(delayTicks / (num * Find.Storyteller.difficulty.enemyReproductionRateFactor));
	}

	private void FilterOutUnspawnedPawns()
	{
		for (int num = spawnedPawns.Count - 1; num >= 0; num--)
		{
			if (!spawnedPawns[num].Spawned)
			{
				spawnedPawns.RemoveAt(num);
			}
		}
	}

	public static float Inverse(float val)
	{
		return 1f / val;
	}

	private PawnKindDef RandomPawnKindDef()
	{
		float curPoints = SpawnedPawnsPoints;
		if (spawnablePawnKinds.NullOrEmpty())
		{
			if (!Props.spawnablePawnKinds.NullOrEmpty())
			{
				spawnablePawnKinds = Props.spawnablePawnKinds;
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
				}
			}
		}
		IEnumerable<PawnGenOption> enumerable = spawnablePawnKinds;
		if (Props.maxSpawnedPawnsPoints > -1f)
		{
			enumerable = enumerable.Where((PawnGenOption x) => curPoints + x.kind.combatPower <= Props.maxSpawnedPawnsPoints);
		}
		PawnGenOption pawnGenOption = default(PawnGenOption);
		if (enumerable.TryRandomElementByWeight((PawnGenOption x) => x.selectionWeight, out pawnGenOption))
		{
			return pawnGenOption.kind;
		}
		return null;
	}

	private bool TrySpawnPawn(out Pawn pawn)
	{
		if (!canSpawnPawns)
		{
			pawn = null;
			return false;
		}
		if (!Props.chooseSingleTypeToSpawn)
		{
			chosenKind = RandomPawnKindDef();
		}
		if (chosenKind == null)
		{
			pawn = null;
			return false;
		}
		PawnGenerationRequest request = new PawnGenerationRequest(chosenKind, parent.Faction, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, 1f, false, true, true, true, true, fixedBiologicalAge: chosenKind.race.race.lifeStageAges.Last().minAge);
		pawn = PawnGenerator.GeneratePawn(request);
		spawnedPawns.Add(pawn);
		GenSpawn.Spawn(pawn, parent.OccupiedRect().AdjacentCells.RandomElement(), parent.Map, WipeMode.Vanish);
		Lord lord = Lord;
		if (lord == null)
		{
			lord = CreateNewLord(parent, aggressive, Props.defendRadius, Props.lordJob);
		}
		lord.AddPawn(pawn);
		if (Props.spawnSound != null)
		{
			SoundStarter.PlayOneShot(Props.spawnSound, SoundInfo.InMap(parent));
		}
		if (pawnsLeftToSpawn > 0)
		{
			pawnsLeftToSpawn--;
		}
		SendMessage();
		return true;
	}

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		((ThingComp)this).PostSpawnSetup(respawningAfterLoad);
		if (!respawningAfterLoad && Active)
		{
			_ = nextPawnSpawnTick;
			_ = -1;
		}
	}

	public override void PostPostMake()
	{
		((ThingComp)this).PostPostMake();
		initialSpawnDelay = Props.initialSpawnDelay;
	}

	private void FeedSpawnedPawns()
	{
		FilterOutUnspawnedPawns();
		for (int i = 0; i < spawnedPawns.Count; i++)
		{
			Pawn val = spawnedPawns[i];
			if (val.def.race.EatsFood && val.RaceProps.Humanlike && val.needs.food.Starving)
			{
				Thing val2 = ThingMaker.MakeThing(ThingDefOf.MealNutrientPaste, (ThingDef)null);
				val2.stackCount = 3;
				val.inventory.TryAddItemNotForSale(val2);
			}
		}
	}

	public override void CompTick()
	{
		if (parent.Map == null)
		{
			return;
		}
		if (initialSpawnDelay > -1)
		{
			initialSpawnDelay--;
		}
		if (Active && parent.Spawned && initialSpawnDelay == 0)
		{
			SpawnInitialPawns();
		}
		if (!parent.Spawned || initialSpawnDelay != -1)
		{
			return;
		}
		FilterOutUnspawnedPawns();
		if (Find.TickManager.TicksGame % 30000 == 0)
		{
			FeedSpawnedPawns();
		}
		if (Active && Find.TickManager.TicksGame >= nextPawnSpawnTick)
		{
			if ((Props.maxSpawnedPawnsPoints < 0f || SpawnedPawnsPoints < Props.maxSpawnedPawnsPoints) && TrySpawnPawn(out var pawn) && pawn.caller != null)
			{
				pawn.caller.DoCall();
			}
			CalculateNextPawnSpawnTick();
		}
	}

	public void SendMessage()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (!GenText.NullOrEmpty(Props.spawnMessageKey) && MessagesRepeatAvoider.MessageShowAllowed(Props.spawnMessageKey, 0.1f))
		{
			Messages.Message(Translator.Translate(Props.spawnMessageKey), parent, MessageTypeDefOf.NegativeEvent, true);
		}
	}

	public override IEnumerable<Gizmo> CompGetGizmosExtra()
	{
		if (!Prefs.DevMode)
		{
			yield break;
		}
		yield return (Gizmo)new Command_Action
		{
			defaultLabel = "DEBUG: Spawn pawn",
			icon = TexCommand.ReleaseAnimals,
			action = delegate
			{
				TrySpawnPawn(out var _);
			}
		};
		if (GenCollection.Any<Pawn>(spawnedPawns, (Predicate<Pawn>)((Pawn x) => x.def.race.Humanlike)))
		{
			yield return (Gizmo)new Command_Action
			{
				defaultLabel = "DEBUG: Feed pawns",
				icon = TexCommand.ReleaseAnimals,
				action = delegate
				{
					FeedSpawnedPawns();
				}
			};
		}
	}

	    public override string CompInspectStringExtra()
	    {
	        if (!Props.showNextSpawnInInspect || nextPawnSpawnTick <= 0 || chosenKind == null)
	        {
	            return null;
	        }
	        if (pawnsLeftToSpawn == 0 && !Props.noPawnsLeftToSpawnKey.NullOrEmpty())
	        {
	            return Props.noPawnsLeftToSpawnKey.Translate();
	        }
	        string text;
	        if (!Dormant)
	        {
	            text = (Props.nextSpawnInspectStringKey ?? "SpawningNextPawnIn").Translate(((Def)chosenKind).LabelCap, (nextPawnSpawnTick - Find.TickManager.TicksGame).ToStringTicksToDays("F1"));
	        }
	        else
	        {
	            if (Props.nextSpawnInspectStringKeyDormant == null)
	            {
	                return null;
	            }
	            text = Props.nextSpawnInspectStringKeyDormant.Translate() + ": " + ((Def)chosenKind).LabelCap;
	        }
	        if (pawnsLeftToSpawn > 0 && !Props.pawnsLeftToSpawnKey.NullOrEmpty())
	        {
	            text = text + "\n" + Props.pawnsLeftToSpawnKey.Translate() + ": " + pawnsLeftToSpawn;
	        }
	        return text;
	    }
	public override void PostExposeData()
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Invalid comparison between Unknown and I4
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		base.PostExposeData();
		Scribe_Values.Look<int>(ref nextPawnSpawnTick, "nextPawnSpawnTick", 0, false);
		Scribe_Values.Look<int>(ref initialSpawnDelay, "initialSpawnDelay", 0, false);
		Scribe_Values.Look<int>(ref pawnsLeftToSpawn, "pawnsLeftToSpawn", -1, false);
		Scribe_Collections.Look<Pawn>(ref spawnedPawns, "spawnedPawns", (LookMode)3, Array.Empty<object>());
		Scribe_Values.Look<bool>(ref aggressive, "aggressive", false, false);
		Scribe_Values.Look<bool>(ref canSpawnPawns, "canSpawnPawns", true, false);
		Scribe_Defs.Look<PawnKindDef>(ref chosenKind, "chosenKind");
		Scribe_References.Look<Lord>(ref lord, "lord", false);
		if ((int)Scribe.mode == 4)
		{
			spawnedPawns.RemoveAll((Pawn x) => x == null);
			if (pawnsLeftToSpawn == -1 && Props.maxPawnsToSpawn != IntRange.Zero)
			{
				pawnsLeftToSpawn = Props.maxPawnsToSpawn.RandomInRange;
			}
		}
	}
}
