using RimWorld.Planet;
using Verse;

namespace ExtraHives.Planet;

public class HiveBaseComp : WorldObjectComp
{
	public int HivePhase = 1;

	public override void PostExposeData()
	{
		((WorldObjectComp)this).PostExposeData();
		Scribe_Values.Look<int>(ref HivePhase, "HivePhase", 1, false);
	}
}
