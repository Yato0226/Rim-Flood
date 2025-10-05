using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ExtraHives;

public class LordJob_DefendAndExpandHive : LordJob
{
	private bool aggressive;

	public override bool CanBlockHostileVisitors => false;

	public override bool AddFleeToil => false;

	public LordJob_DefendAndExpandHive()
	{
	}

	public LordJob_DefendAndExpandHive(SpawnedPawnParams parms)
	{
		aggressive = parms.aggressive;
	}

	public override StateGraph CreateGraph()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected O, but got Unknown
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected O, but got Unknown
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected O, but got Unknown
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Expected O, but got Unknown
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Expected O, but got Unknown
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Expected O, but got Unknown
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Expected O, but got Unknown
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Expected O, but got Unknown
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Expected O, but got Unknown
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Expected O, but got Unknown
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Expected O, but got Unknown
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Expected O, but got Unknown
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Expected O, but got Unknown
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Expected O, but got Unknown
		StateGraph val = new StateGraph();
		LordToil_DefendAndExpandHive lordToil_DefendAndExpandHive = new LordToil_DefendAndExpandHive();
		lordToil_DefendAndExpandHive.distToHiveToAttack = (aggressive ? 10f : 30f);
		val.StartingToil = (LordToil)(object)lordToil_DefendAndExpandHive;
		LordToil_DefendHiveAggressively lordToil_DefendHiveAggressively = new LordToil_DefendHiveAggressively();
		lordToil_DefendHiveAggressively.distToHiveToAttack = (aggressive ? 80f : 40f);
		val.AddToil((LordToil)(object)lordToil_DefendHiveAggressively);
		LordToil_AssaultColony lordToil_AssaultColony = new LordToil_AssaultColony();
		val.AddToil((LordToil)(object)lordToil_AssaultColony);
		Transition val2 = new Transition((LordToil)(object)lordToil_DefendAndExpandHive, (LordToil)(aggressive ? ((object)lordToil_AssaultColony) : ((object)lordToil_DefendHiveAggressively)), false, true);
		val2.AddTrigger((Trigger)new Trigger_Memo(Hive.MemoAssaultOnSpawn));
		val2.AddTrigger((Trigger)new Trigger_PawnHarmed(0.5f, true, aggressive ? null : ((LordJob)this).Map.ParentFaction, (DutyDef)null, (int?)null));
		val2.AddTrigger((Trigger)new Trigger_PawnLostViolently(false));
		val2.AddTrigger((Trigger)new Trigger_Memo(Hive.MemoAttackedByEnemy));
		val2.AddTrigger((Trigger)new Trigger_Memo(Hive.MemoAttackedByEnemy));
		val2.AddTrigger((Trigger)new Trigger_Memo("HiveDestroyedNonRoofCollapse"));
		val2.AddTrigger((Trigger)new Trigger_Memo("HiveDestroyedNonRoofCollapse"));
		val2.AddTrigger((Trigger)new Trigger_Memo(HediffGiver_Heat.MemoPawnBurnedByAir));
		val2.AddPostAction((TransitionAction)new TransitionAction_EndAllJobs());
		val.AddTransition(val2, false);
		Transition val3 = new Transition((LordToil)(object)lordToil_DefendAndExpandHive, (LordToil)(object)lordToil_AssaultColony, false, true);
		val3.AddTrigger((Trigger)new Trigger_PawnHarmed(0.5f, false, ((LordJob)this).Map.ParentFaction, (DutyDef)null, (int?)null));
		val3.AddPostAction((TransitionAction)new TransitionAction_EndAllJobs());
		val.AddTransition(val3, false);
		Transition val4 = new Transition((LordToil)(object)lordToil_DefendHiveAggressively, (LordToil)(object)lordToil_AssaultColony, false, true);
		val4.AddTrigger((Trigger)new Trigger_PawnHarmed(0.5f, false, ((LordJob)this).Map.ParentFaction, (DutyDef)null, (int?)null));
		val4.AddPostAction((TransitionAction)new TransitionAction_EndAllJobs());
		val.AddTransition(val4, false);
		Transition val5 = new Transition((LordToil)(object)lordToil_DefendAndExpandHive, (LordToil)(object)lordToil_DefendAndExpandHive, true, true);
		val5.AddTrigger((Trigger)new Trigger_Memo(Hive.MemoDeSpawned));
		val5.AddTrigger((Trigger)new Trigger_Memo(Hive.MemoDeSpawned));
		val.AddTransition(val5, false);
		Transition val6 = new Transition((LordToil)(object)lordToil_DefendHiveAggressively, (LordToil)(object)lordToil_DefendHiveAggressively, true, true);
		val6.AddTrigger((Trigger)new Trigger_Memo(Hive.MemoDeSpawned));
		val6.AddTrigger((Trigger)new Trigger_Memo(Hive.MemoDeSpawned));
		val.AddTransition(val6, false);
		Transition val7 = new Transition((LordToil)(object)lordToil_AssaultColony, (LordToil)(object)lordToil_DefendAndExpandHive, false, true);
		val7.AddSource((LordToil)(object)lordToil_DefendHiveAggressively);
		val7.AddTrigger((Trigger)new Trigger_TicksPassedWithoutHarmOrMemos(1200, new string[9]
		{
			Hive.MemoAttackedByEnemy,
			Hive.MemoAttackedByEnemy,
			Hive.MemoBurnedBadly,
			Hive.MemoBurnedBadly,
			"HiveDestroyedNonRoofCollapse",
			"HiveDestroyedNonRoofCollapse",
			Hive.MemoDeSpawned,
			Hive.MemoDeSpawned,
			HediffGiver_Heat.MemoPawnBurnedByAir
		}));
		val7.AddPostAction((TransitionAction)new TransitionAction_EndAttackBuildingJobs());
		val.AddTransition(val7, false);
		return val;
	}

	public override void ExposeData()
	{
		Scribe_Values.Look<bool>(ref aggressive, "aggressive", false, false);
	}
}
