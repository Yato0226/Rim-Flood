using RimWorld;
using Verse;

namespace BuildingDefaultFaction;

public class CompProperties_BuildingDefaultFaction : CompProperties
{
	public FactionDef faction;

	public bool debug = false;

	public CompProperties_BuildingDefaultFaction()
	{
		base.compClass = typeof(CompBuildingDefaultFaction);
	}
}
