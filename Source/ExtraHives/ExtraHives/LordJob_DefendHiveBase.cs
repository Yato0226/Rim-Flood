using System;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ExtraHives;

public class LordJob_DefendHiveBase : LordJob
{
	private Faction faction;

	private IntVec3 baseCenter;

	public LordJob_DefendHiveBase()
	{
	}

	public LordJob_DefendHiveBase(Faction faction, IntVec3 baseCenter)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		this.faction = faction;
		this.baseCenter = baseCenter;
	}

	public override StateGraph CreateGraph()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Expected O, but got Unknown
		StateGraph val = new StateGraph();
		LordToil_DefendBase val2 = (LordToil_DefendBase)(object)(val.StartingToil = (LordToil)new LordToil_DefendBase(baseCenter));
		LordToil_DefendBase val4 = new LordToil_DefendBase(baseCenter);
		val.AddToil((LordToil)(object)val4);
		LordToil_AssaultColony lordToil_AssaultColony = new LordToil_AssaultColony(attackDownedIfStarving: true);
		((LordToil)lordToil_AssaultColony).useAvoidGrid = true;
		val.AddToil((LordToil)(object)lordToil_AssaultColony);
		Transition val5 = new Transition((LordToil)(object)val2, (LordToil)(object)val4, false, true);
		val5.AddSource((LordToil)(object)lordToil_AssaultColony);
		val5.AddTrigger((Trigger)new Trigger_BecameNonHostileToPlayer());
		val.AddTransition(val5, false);
		Transition val6 = new Transition((LordToil)(object)val4, (LordToil)(object)val2, false, true);
		val6.AddTrigger((Trigger)new Trigger_BecamePlayerEnemy());
		val.AddTransition(val6, false);
		Transition val7 = new Transition((LordToil)(object)val2, (LordToil)(object)lordToil_AssaultColony, false, true);
		val7.AddTrigger((Trigger)new Trigger_FractionPawnsLost(0.2f));
		val7.AddTrigger((Trigger)new Trigger_PawnHarmed(0.4f, false, (Faction)null, (DutyDef)null, (int?)null));
		val7.AddTrigger((Trigger)new Trigger_ChanceOnTickInterval(2500, 0.03f));
		val7.AddTrigger((Trigger)new Trigger_TicksPassed(251999));
		val7.AddTrigger((Trigger)new Trigger_ChanceOnPlayerHarmNPCBuilding(0.4f));
		val7.AddTrigger((Trigger)new Trigger_OnClamor(ClamorDefOf.Ability));
		val7.AddPostAction((TransitionAction)new TransitionAction_WakeAll());
		TaggedString val8 = TranslatorFormattedStringExtensions.Translate("MessageDefendersAttacking", faction.def.pawnsPlural, faction.Name, Faction.OfPlayer.def.pawnsPlural);
		TaggedString val9 = val8.CapitalizeFirst();
		val7.AddPreAction((TransitionAction)new TransitionAction_Message(val9, MessageTypeDefOf.ThreatBig, (string)null, 1f, (Func<bool>)null));
		val.AddTransition(val7, false);
		return val;
	}

	public override void ExposeData()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		((LordJob)this).ExposeData();
		Scribe_References.Look<Faction>(ref faction, "faction", false);
		Scribe_Values.Look<IntVec3>(ref baseCenter, "baseCenter", default(IntVec3), false);
	}
}
