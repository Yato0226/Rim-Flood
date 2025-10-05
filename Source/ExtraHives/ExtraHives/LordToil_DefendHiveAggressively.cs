using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ExtraHives;

public class LordToil_DefendHiveAggressively : LordToil_HiveRelated
{
	public float distToHiveToAttack = 40f;

	public override void UpdateAllDuties()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		FilterOutUnspawnedHives();
		for (int i = 0; i < ((LordToil)this).lord.ownedPawns.Count; i++)
		{
			Hive hiveFor = GetHiveFor(((LordToil)this).lord.ownedPawns[i]);
			PawnDuty duty = new PawnDuty(DutyDefOf.DefendHiveAggressively, hiveFor, distToHiveToAttack);
			((LordToil)this).lord.ownedPawns[i].mindState.duty = duty;
		}
	}
}
