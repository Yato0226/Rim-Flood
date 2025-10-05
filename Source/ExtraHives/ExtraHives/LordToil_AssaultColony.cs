using RimWorld;
using Verse.AI;
using Verse.AI.Group;

namespace ExtraHives;

public class LordToil_AssaultColony : LordToil
{
	private bool attackDownedIfStarving;

	public override bool ForceHighStoryDanger => true;

	public override bool AllowSatisfyLongNeeds => false;

	public LordToil_AssaultColony(bool attackDownedIfStarving = false)
	{
		this.attackDownedIfStarving = attackDownedIfStarving;
	}

	public override void Init()
	{
		((LordToil)this).Init();
		LessonAutoActivator.TeachOpportunity(ConceptDefOf.Drafting, (OpportunityType)2);
	}

	public override void UpdateAllDuties()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		for (int i = 0; i < base.lord.ownedPawns.Count; i++)
		{
			base.lord.ownedPawns[i].mindState.duty = new PawnDuty(DutyDefOf.AssaultColony);
			base.lord.ownedPawns[i].mindState.duty.attackDownedIfStarving = attackDownedIfStarving;
		}
	}
}
