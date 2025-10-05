using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ExtraHives;

public class HiveStage
{
	public int DaysPassed = -1;

	public float pointMultipler = 1f;

	public IntRange sizeRange = new IntRange(44, 60);

	public SimpleCurve maxPawnCostPerTotalPointsCurve;

	public List<PawnGroupMaker> pawnGroupMakers;

	public ThingDef smallCaveHive = null;

	public ThingDef largeCaveHive = null;

	public ThingDef centerCaveHive = null;
}
