using System;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace ExtraHives;

public class LordToil_HiveRelatedData : LordToilData
{
	public Dictionary<Pawn, Hive> assignedHives = new Dictionary<Pawn, Hive>();

	public override void ExposeData()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		if ((int)Scribe.mode == 1)
		{
			GenCollection.RemoveAll<Pawn, Hive>(assignedHives, (Predicate<KeyValuePair<Pawn, Hive>>)((KeyValuePair<Pawn, Hive> x) => ((Thing)x.Key).Destroyed));
		}
		Scribe_Collections.Look<Pawn, Hive>(ref assignedHives, "assignedHives", (LookMode)3, (LookMode)3);
		if ((int)Scribe.mode == 4)
		{
			GenCollection.RemoveAll<Pawn, Hive>(assignedHives, (Predicate<KeyValuePair<Pawn, Hive>>)((KeyValuePair<Pawn, Hive> x) => x.Value == null));
		}
	}
}
