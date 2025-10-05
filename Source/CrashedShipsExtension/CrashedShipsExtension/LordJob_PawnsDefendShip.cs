using RimWorld;
using Verse;
using Verse.AI.Group;

namespace CrashedShipsExtension;

public class LordJob_PawnsDefendShip : LordJob
{
	private Thing shipPart;

	private Faction faction;

	private float defendRadius;

	private IntVec3 defSpot;

	public override bool CanBlockHostileVisitors => false;

	public override bool AddFleeToil => false;

	public LordJob_PawnsDefendShip()
	{
	}

	public LordJob_PawnsDefendShip(SpawnedPawnParams parms)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		defSpot = parms.defSpot;
		defendRadius = parms.defendRadius;
		shipPart = parms.spawnerThing;
		faction = parms.spawnerThing.Faction;
	}

	public LordJob_PawnsDefendShip(Thing shipPart, Faction faction, float defendRadius, IntVec3 defSpot)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		this.shipPart = shipPart;
		this.faction = faction;
		this.defendRadius = defendRadius;
		this.defSpot = defSpot;
	}

	public override StateGraph CreateGraph()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		StateGraph val = new StateGraph();
		if (!((IntVec3)(ref defSpot)).IsValid)
		{
			Log.Warning("LordJob_PawnsDefendShip defSpot is invalid. Returning graph for LordJob_AssaultColony.", false);
			val.AttachSubgraph(((LordJob)new LordJob_AssaultColony(faction, true, true, false, false, true, false, false)).CreateGraph());
			return val;
		}
		LordToil_DefendPoint val2 = (LordToil_DefendPoint)(object)(val.StartingToil = (LordToil)new LordToil_DefendPoint(defSpot, defendRadius, (float?)null));
		LordToil_AssaultColony val4 = new LordToil_AssaultColony(false, false);
		val.AddToil((LordToil)(object)val4);
		LordToil_AssaultColony val5 = new LordToil_AssaultColony(false, false);
		val.AddToil((LordToil)(object)val5);
		Transition val6 = new Transition((LordToil)(object)val2, (LordToil)(object)val5, false, true);
		val6.AddSource((LordToil)(object)val4);
		val6.AddTrigger((Trigger)new Trigger_PawnCannotReachMapEdge());
		val.AddTransition(val6, false);
		Transition val7 = new Transition((LordToil)(object)val2, (LordToil)(object)val4, false, true);
		val7.AddTrigger((Trigger)new Trigger_PawnHarmed(0.5f, true, (Faction)null));
		val7.AddTrigger((Trigger)new Trigger_PawnLostViolently(true));
		val7.AddTrigger((Trigger)new Trigger_Memo(CompSpawnerOnDamaged.MemoDamaged));
		val7.AddPostAction((TransitionAction)new TransitionAction_EndAllJobs());
		val.AddTransition(val7, false);
		Transition val8 = new Transition((LordToil)(object)val4, (LordToil)(object)val2, false, true);
		val8.AddTrigger((Trigger)new Trigger_TicksPassedWithoutHarmOrMemos(1380, new string[1] { CompSpawnerOnDamaged.MemoDamaged }));
		val8.AddPostAction((TransitionAction)new TransitionAction_EndAttackBuildingJobs());
		val.AddTransition(val8, false);
		Transition val9 = new Transition((LordToil)(object)val2, (LordToil)(object)val5, false, true);
		val9.AddSource((LordToil)(object)val4);
		val9.AddTrigger((Trigger)new Trigger_ThingDamageTaken(shipPart, 0.5f));
		val9.AddTrigger((Trigger)new Trigger_Memo(HediffGiver_Heat.MemoPawnBurnedByAir));
		val.AddTransition(val9, false);
		return val;
	}

	public override void ExposeData()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Scribe_References.Look<Thing>(ref shipPart, "shipPart", false);
		Scribe_References.Look<Faction>(ref faction, "faction", false);
		Scribe_Values.Look<float>(ref defendRadius, "defendRadius", 0f, false);
		Scribe_Values.Look<IntVec3>(ref defSpot, "defSpot", default(IntVec3), false);
	}
}
