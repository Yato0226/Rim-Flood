using RimWorld;
using Verse;

namespace BuildingDefaultFaction;

public class CompBuildingDefaultFaction : ThingComp
{
	public CompProperties_BuildingDefaultFaction Props => base.props as CompProperties_BuildingDefaultFaction;

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		((ThingComp)this).PostSpawnSetup(respawningAfterLoad);
		if (Props.debug)
		{
			Log.Message("Debug: CompBuildingDefaultFaction spawned on " + (object)base.parent);
		}
		if (!(base.parent is Building) || respawningAfterLoad)
		{
			return;
		}
		if (Props.debug)
		{
			Log.Message("Debug: " + ((object)base.parent)?.ToString() + " is Building");
		}
		Faction val = Find.FactionManager.FirstFactionOfDef(Props.faction);
		if (val != null)
		{
			if (Props.debug)
			{
				Log.Message("Debug: " + ((object)base.parent)?.ToString() + " Faction should be " + val.Name);
			}
			((Thing)base.parent).SetFaction(val, (Pawn)null);
			if (Props.debug)
			{
				Log.Message("Debug: " + ((object)base.parent)?.ToString() + " Faction is " + ((Thing)base.parent).Faction.Name);
			}
		}
	}
}
