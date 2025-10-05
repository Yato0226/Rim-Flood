using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace ExtraHives;

public class CompSpawnerPawn : ThingComp
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<Thing, List<Pawn>> _003C_003E9__23_0;

		public static RegionEntryPredicate _003C_003E9__23_3;

		public static Func<PawnGroupMaker, float> _003C_003E9__28_1;

		public static Func<PawnGroupMaker, bool> _003C_003E9__28_2;

		public static Func<PawnGroupMaker, float> _003C_003E9__28_3;

		public static Func<PawnGenOption, float> _003C_003E9__28_7;

		public static Predicate<Pawn> _003C_003E9__37_2;

		public static Predicate<Pawn> _003C_003E9__39_0;

		internal List<Pawn> _003CFindLordToJoin_003Eb__23_0(Thing s)
		{
			return ThingCompUtility.TryGetComp<CompSpawnerPawn>(s)?.spawnedPawns;
		}

		internal bool _003CFindLordToJoin_003Eb__23_3(Region from, Region to)
		{
			return true;
		}

		internal float _003CRandomPawnKindDef_003Eb__28_1(PawnGroupMaker x)
		{
			return x.commonality;
		}

		internal bool _003CRandomPawnKindDef_003Eb__28_2(PawnGroupMaker x)
		{
			return x.kindDef == DefDatabase<PawnGroupKindDef>.GetNamed("Combat") || x.kindDef == DefDatabase<PawnGroupKindDef>.GetNamed("Settlement");
		}

		internal float _003CRandomPawnKindDef_003Eb__28_3(PawnGroupMaker x)
		{
			return x.commonality;
		}

		internal float _003CRandomPawnKindDef_003Eb__28_7(PawnGenOption x)
		{
			return x.selectionWeight;
		}

		internal bool _003CCompGetGizmosExtra_003Eb__37_2(Pawn x)
		{
			return ((Thing)x).def.race.EatsFood;
		}

		internal bool _003CPostExposeData_003Eb__39_0(Pawn x)
		{
			return x == null;
		}
	}

	public float maxSpawnedPointsOverride = 0f;

	public int nextPawnSpawnTick = -1;

	public int initialSpawnDelay = -1;

	public int pawnsLeftToSpawn = -1;

	public int initialPawnsPoints = -1;

	public List<Pawn> spawnedPawns = new List<Pawn>();

	public List<PawnGenOption> spawnablePawnKinds = new List<PawnGenOption>();

	public bool aggressive = true;

	public bool canSpawnPawns = true;

	public PawnKindDef chosenKind;

	private HiveDefExtension cachedHiveExtension;

	private Lord cachedlord;

	private CompCanBeDormant dormancyCompCached;

	public CompProperties_SpawnerPawn Props => (CompProperties_SpawnerPawn)(object)base.props;

	public bool Active => pawnsLeftToSpawn != 0 && !Dormant;

	public bool Dormant => DormancyComp != null && !DormancyComp.Awake;

	public HiveDefExtension HiveExtension => cachedHiveExtension ?? (cachedHiveExtension = ((Def)((Thing)base.parent).def).GetModExtension<HiveDefExtension>());

	public Lord Lord => cachedlord ?? (cachedlord = FindLordToJoin((Thing)(object)base.parent, Props.lordJob, Props.shouldJoinParentLord));

	public virtual float MaxSpawnedPoints => Props.maxSpawnedPawnsPoints + maxSpawnedPointsOverride;

	public float SpawnedPawnsPoints
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

	public float PointsForSpawning => MaxSpawnedPoints - SpawnedPawnsPoints;

	public int SpawnedPawnsCount
	{
		get
		{
			FilterOutUnspawnedPawns();
			return spawnedPawns.Count;
		}
	}

	public CompCanBeDormant DormancyComp
	{
		get
		{
			CompCanBeDormant result;
			if ((result = dormancyCompCached) == null)
			{
				result = (dormancyCompCached = ThingCompUtility.TryGetComp<CompCanBeDormant>((Thing)(object)base.parent));
			}
			return result;
		}
	}

	public void FeedSpawnedPawns()
	{
		FilterOutUnspawnedPawns();
		for (int i = 0; i < spawnedPawns.Count; i++)
		{
			Pawn val = spawnedPawns[i];
			if (((Thing)val).def.race.EatsFood && val.RaceProps.Humanlike && val.needs.food.Starving)
			{
				Thing val2 = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("MealNutrientPaste"), (ThingDef)null);
				val2.stackCount = 3;
				val.inventory.TryAddItemNotForSale(val2);
			}
		}
	}

	public override void Initialize(CompProperties props)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		((ThingComp)this).Initialize(props);
		if (chosenKind == null)
		{
			chosenKind = RandomPawnKindDef();
		}
		            pawnsLeftToSpawn = Props.maxPawnsToSpawn.RandomInRange;	}

	public Lord FindLordToJoin(Thing spawner, Type lordJobType, bool shouldTryJoinParentLord, Func<Thing, List<Pawn>> spawnedPawnSelector = null)
	{
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Expected O, but got Unknown
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
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
				Lord lord = LordUtility.GetLord(x);
				return lord != null && ((object)lord.LordJob).GetType() == lordJobType;
			};
			Pawn foundPawn = null;
			CellRect val3 = GenAdj.OccupiedRect(spawner);
			Region region = GridsUtility.GetRegion(GenCollection.RandomElement<IntVec3>(val3.AdjacentCells.Where((IntVec3 x) => GenGrid.Walkable(x, spawner.Map))), spawner.Map, (RegionType)14);
			object obj2 = _003C_003Ec._003C_003E9__23_3;
			if (obj2 == null)
			{
				RegionEntryPredicate val4 = (Region from, Region to) => true;
				_003C_003Ec._003C_003E9__23_3 = val4;
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

	public Lord CreateNewLord(Thing byThing, bool aggressive, float defendRadius, Type lordJobType)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 invalid = default(IntVec3);
		if (!CellFinder.TryFindRandomCellNear(byThing.Position, byThing.Map, 5, (Predicate<IntVec3>)((IntVec3 c) => GenGrid.Standable(c, byThing.Map) && byThing.Map.reachability.CanReach(c, byThing, (PathEndMode)2, TraverseParms.For((TraverseMode)1, (Danger)3, false, false, false))), out invalid, -1))
		{
			Log.Error("Found no place for mechanoids to defend " + (object)byThing);
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

	public virtual void SpawnInitialPawns()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		string text = $"{((Entity)base.parent).LabelCap}({((Thing)base.parent).Position.x},{((Thing)base.parent).Position.z}) SpawnInitialPawns\n    initialPawnsCount: {Props.initialPawnsCount}";
		int num = ((initialPawnsPoints > -1) ? initialPawnsPoints : Props.initialPawnsPoints);
		if (Prefs.DevMode)
		{
			text += $"\n    SpawnInitialPawns points: {num}";
		}
		SpawnPawnsUntilPoints(num, spawningInital: true);
		if (!GenList.NullOrEmpty<PawnKindDef>((IList<PawnKindDef>)Props.AlwaysSpawnWith))
		{
			foreach (PawnKindDef item in Props.AlwaysSpawnWith)
			{
				if (TrySpawnPawn(out var pawn, item) && Prefs.DevMode)
				{
					text += $"\n    AlwaysSpawnWith: {pawn.NameShortColored}";
				}
			}
		}
		CalculateNextPawnSpawnTick();
	}

	public virtual void SpawnPawnsUntilPoints(float points, bool spawningInital = false, bool spawningWave = false, int extraPoints = 0)
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int num2 = 0;
		string text = $"SpawnPawnsUntilXPoints {points + (float)extraPoints - (spawningWave ? ((float)num2) : SpawnedPawnsPoints)}";
		while ((spawningWave ? ((float)num2) : SpawnedPawnsPoints) < points + (float)extraPoints)
		{
			num++;
			if (num > 1000)
			{
				Log.Error("Too many iterations.");
				break;
			}
			float num3 = points + (float)extraPoints - (spawningWave ? ((float)num2) : SpawnedPawnsPoints);
			if (!TrySpawnPawn(out var pawn, null, (int)num3))
			{
				break;
			}
			num2 += (int)pawn.kindDef.combatPower;
			if (!spawningInital)
			{
				continue;
			}
			if (Prefs.DevMode)
			{
				text += $"\n    spawningInital: {pawn.NameShortColored}:({pawn.kindDef.combatPower})";
			}
			if (SpawnedPawnsCount >= Props.initialPawnsCount)
			{
				if (Prefs.DevMode)
				{
					text += $"\n    spawningInital Finished: {SpawnedPawnsCount}";
				}
				break;
			}
		}
		if (Prefs.DevMode)
		{
			Log.Message(text);
		}
		CalculateNextPawnSpawnTick();
	}

	public bool TrySpawnPawn(out Pawn pawn, PawnKindDef kindDef = null, int? maxPower = null)
	{
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		if (!canSpawnPawns)
		{
			pawn = null;
			return false;
		}
		if (!Props.chooseSingleTypeToSpawn)
		{
			chosenKind = ((kindDef != null) ? kindDef : RandomPawnKindDef(maxPower));
		}
		if (chosenKind == null)
		{
			pawn = null;
			return false;
		}
		Faction val = null;
		if (((Thing)base.parent).Faction != null)
		{
			val = ((Thing)base.parent).Faction;
		}
		else
		{
			Log.Warning("Warning faction not found");
			if (HiveExtension?.Faction != null)
			{
				val = Find.FactionManager.FirstFactionOfDef(HiveExtension.Faction);
			}
		}
		if (((Thing)base.parent).Faction == null)
		{
			((Thing)base.parent).SetFaction(val, (Pawn)null);
		}
		pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(chosenKind, val, (PawnGenerationContext)2, -1, false, false, false, false, true, 1f, false, true, true, true, false, false, false, false, false, 0f, 0f, (Pawn)null, 1f, (Predicate<Pawn>)null, (Predicate<Pawn>)null, (IEnumerable<TraitDef>)null, (IEnumerable<TraitDef>)null, (float?)null, (float?)chosenKind.race.race.lifeStageAges.Last().minAge, (float?)null, (Gender?)null, (string)null, (string)null, (RoyalTitleDef)null, (Ideo)null, false, false, false, false, (List<GeneDef>)null, (List<GeneDef>)null, (XenotypeDef)null, (CustomXenotype)null, (List<XenotypeDef>)null, 0f, (DevelopmentalStage)8, (Func<XenotypeDef, PawnKindDef>)null, (FloatRange?)null, (FloatRange?)null, false, false, false, -1, 0, false));
		spawnedPawns.Add(pawn);
		Pawn obj = pawn;
		CellRect val2 = GenAdj.OccupiedRect((Thing)(object)base.parent);
		GenSpawn.Spawn((Thing)(object)obj, CellFinder.RandomClosewalkCellNear(GenCollection.RandomElement<IntVec3>(val2.AdjacentCells), ((Thing)base.parent).Map, Props.pawnSpawnRadius, (Predicate<IntVec3>)null), ((Thing)base.parent).Map, (WipeMode)0);
		Lord val3 = Lord;
		if (val3 == null)
		{
			val3 = CreateNewLord((Thing)(object)base.parent, aggressive, Props.defendRadius, Props.lordJob);
		}
		val3.AddPawn(pawn);
		if (Props.spawnSound != null)
		{
			SoundStarter.PlayOneShot(Props.spawnSound, (Thing)(object)base.parent);
		}
		if (pawnsLeftToSpawn > 0)
		{
			pawnsLeftToSpawn--;
		}
		SendMessage();
		return true;
	}

	public PawnKindDef RandomPawnKindDef(int? maxPower = null)
	{
		float spawnedPawnsPoints = SpawnedPawnsPoints;
		if (GenList.NullOrEmpty<PawnGenOption>((IList<PawnGenOption>)spawnablePawnKinds))
		{
			if (!GenList.NullOrEmpty<PawnGenOption>((IList<PawnGenOption>)Props.spawnablePawnKinds))
			{
				spawnablePawnKinds = Props.spawnablePawnKinds;
			}
			else if (((Thing)base.parent).Faction != null)
			{
				if (GenCollection.Any<PawnGroupMaker>(((Thing)base.parent).Faction.def.pawnGroupMakers, (Predicate<PawnGroupMaker>)((PawnGroupMaker x) => x.kindDef == Props.factionGroupKindDef)))
				{
					spawnablePawnKinds = GenCollection.RandomElementByWeight<PawnGroupMaker>(((Thing)base.parent).Faction.def.pawnGroupMakers.Where((PawnGroupMaker x) => x.kindDef == Props.factionGroupKindDef), (Func<PawnGroupMaker, float>)((PawnGroupMaker x) => x.commonality)).options;
				}
				else
				{
					spawnablePawnKinds = GenCollection.RandomElementByWeight<PawnGroupMaker>(((Thing)base.parent).Faction.def.pawnGroupMakers.Where((PawnGroupMaker x) => x.kindDef == DefDatabase<PawnGroupKindDef>.GetNamed("Combat") || x.kindDef == DefDatabase<PawnGroupKindDef>.GetNamed("Settlement")), (Func<PawnGroupMaker, float>)((PawnGroupMaker x) => x.commonality)).options;
				}
			}
		}
		IEnumerable<PawnGenOption> enumerable = spawnablePawnKinds;
		if (maxPower.HasValue)
		{
			enumerable = enumerable.Where((PawnGenOption x) => x.kind.combatPower <= (float)maxPower.Value);
		}
		else if (MaxSpawnedPoints > -1f)
		{
			enumerable = enumerable.Where((PawnGenOption x) => x.kind.combatPower <= PointsForSpawning);
		}
		PawnGenOption val = default(PawnGenOption);
		if (GenCollection.TryRandomElementByWeight<PawnGenOption>(enumerable, (Func<PawnGenOption, float>)((PawnGenOption x) => x.selectionWeight), out val))
		{
			return val.kind;
		}
		return null;
	}

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		((ThingComp)this).PostSpawnSetup(respawningAfterLoad);
		if (!respawningAfterLoad && Active && nextPawnSpawnTick == -1)
		{
			initialSpawnDelay = Props.initialSpawnDelay;
		}
	}

	public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((ThingComp)this).PostPostApplyDamage(dinfo, totalDamageDealt);
	}

	public override void PostPostMake()
	{
		((ThingComp)this).PostPostMake();
	}

	public override void CompTick()
	{
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		if (((Thing)base.parent).Map == null)
		{
			if (initialSpawnDelay == -1)
			{
				initialSpawnDelay = Props.initialSpawnDelay;
			}
			return;
		}
		if (initialSpawnDelay > -1)
		{
			initialSpawnDelay--;
		}
		if (Active && ((Thing)base.parent).Spawned && initialSpawnDelay == 0)
		{
			SpawnInitialPawns();
		}
		if (!((Thing)base.parent).Spawned || initialSpawnDelay != -1)
		{
			return;
		}
		FilterOutUnspawnedPawns();
		if (Find.TickManager.TicksGame % 30000 == 0)
		{
			FeedSpawnedPawns();
		}
		if (!Active || Find.TickManager.TicksGame < nextPawnSpawnTick)
		{
			return;
		}
		Pawn pawn;
		if (PointsForSpawning < 0f && TrySpawnPawn(out pawn, null, null))
		{
			Log.Message("PawnSpawner CompTick Spawned " + pawn.NameShortColored);
			if (pawn.caller != null)
			{
				pawn.caller.DoCall();
			}
		}
		CalculateNextPawnSpawnTick();
	}

	public void CalculateNextPawnSpawnTick()
	{
		CalculateNextPawnSpawnTick(Props.pawnSpawnIntervalDays.RandomInRange * 60000f);
	}

	public void CalculateNextPawnSpawnTick(float delayTicks)
	{
		float num = GenMath.LerpDouble(0f, 5f, 1f, 0.5f, (float)SpawnedPawnsCount);
		num = delayTicks / (num * Find.Storyteller.difficulty.enemyReproductionRateFactor);
		nextPawnSpawnTick = Find.TickManager.TicksGame + (int)num;
	}

	public void FilterOutUnspawnedPawns()
	{
		for (int num = spawnedPawns.Count - 1; num >= 0; num--)
		{
			if (!((Thing)spawnedPawns[num]).Spawned)
			{
				spawnedPawns.RemoveAt(num);
			}
		}
	}

	public void SendMessage()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!GenText.NullOrEmpty(Props.spawnMessageKey) && MessagesRepeatAvoider.MessageShowAllowed(Props.spawnMessageKey, 0.1f))
		{
			Messages.Message(Translator.Translate(Props.spawnMessageKey), (Thing)(object)base.parent, MessageTypeDefOf.NegativeEvent, true);
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
			defaultLabel = "DEBUG: Spawn random pawn",
			icon = (Texture)(object)TexCommand.ReleaseAnimals,
			action = delegate
			{
				TrySpawnPawn(out var _);
			}
		};
		foreach (PawnGenOption item in spawnablePawnKinds)
		{
			yield return (Gizmo)new Command_Action
			{
				defaultLabel = "DEBUG: Spawn " + ((Def)item.kind).label,
				icon = (Texture)(object)TexCommand.ReleaseAnimals,
				action = delegate
				{
					TrySpawnPawn(out var _, item.kind);
				}
			};
		}
		if (GenCollection.Any<Pawn>(spawnedPawns, (Predicate<Pawn>)((Pawn x) => ((Thing)x).def.race.EatsFood)))
		{
			yield return (Gizmo)new Command_Action
			{
				defaultLabel = "DEBUG: Feed pawns",
				icon = (Texture)(object)TexCommand.DesirePower,
				action = delegate
				{
					FeedSpawnedPawns();
				}
			};
		}
	}

	public override string CompInspectStringExtra()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		if (!Props.showNextSpawnInInspect || nextPawnSpawnTick <= 0 || chosenKind == null)
		{
			return null;
		}
		if (pawnsLeftToSpawn == 0 && !GenText.NullOrEmpty(Props.noPawnsLeftToSpawnKey))
		{
			return Translator.Translate(Props.noPawnsLeftToSpawnKey);
		}
		string text;
		if (!Dormant)
		{
			text = TranslatorFormattedStringExtensions.Translate(Props.nextSpawnInspectStringKey ?? "SpawningNextPawnIn", ((Def)chosenKind).LabelCap, GenDate.ToStringTicksToDays(nextPawnSpawnTick - Find.TickManager.TicksGame, "F1"));
		}
		else
		{
			if (Props.nextSpawnInspectStringKeyDormant == null)
			{
				return null;
			}
			text = Translator.Translate(Props.nextSpawnInspectStringKeyDormant) + ": " + ((Def)chosenKind).LabelCap;
		}
		if (pawnsLeftToSpawn > 0 && !GenText.NullOrEmpty(Props.pawnsLeftToSpawnKey))
		{
			text = text + ("\n" + Translator.Translate(Props.pawnsLeftToSpawnKey) + ": ") + pawnsLeftToSpawn;
		}
		return text;
	}

	public override void PostExposeData()
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Invalid comparison between Unknown and I4
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		((ThingComp)this).PostExposeData();
		Scribe_Values.Look<int>(ref nextPawnSpawnTick, "nextPawnSpawnTick", 0, false);
		Scribe_Values.Look<int>(ref initialSpawnDelay, "initialSpawnDelay", 0, false);
		Scribe_Values.Look<int>(ref initialPawnsPoints, "initialPawnsPoints", -1, false);
		Scribe_Values.Look<int>(ref pawnsLeftToSpawn, "pawnsLeftToSpawn", -1, false);
		Scribe_Collections.Look<Pawn>(ref spawnedPawns, "spawnedPawns", (LookMode)3, Array.Empty<object>());
		Scribe_Values.Look<bool>(ref aggressive, "aggressive", false, false);
		Scribe_Values.Look<bool>(ref canSpawnPawns, "canSpawnPawns", true, false);
		Scribe_Defs.Look<PawnKindDef>(ref chosenKind, "chosenKind");
		Scribe_References.Look<Lord>(ref cachedlord, "lord", false);
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
