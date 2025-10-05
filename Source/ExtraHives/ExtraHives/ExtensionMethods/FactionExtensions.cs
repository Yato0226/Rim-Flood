using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ExtraHives.ExtensionMethods;

public static class FactionExtensions
{
	public static HiveFactionExtension HiveExt(this Faction faction)
	{
		return faction.def.HiveExt();
	}

	public static HiveFactionExtension HiveExt(this FactionDef faction)
	{
		if (((Def)faction).HasModExtension<HiveFactionExtension>())
		{
			return ((Def)faction).GetModExtension<HiveFactionExtension>();
		}
		return null;
	}

	public static List<ThingDef> HivedefsFor(this Faction faction)
	{
		return faction.def.HivedefsFor();
	}

	public static List<ThingDef> HivedefsFor(this FactionDef factionDef)
	{
		List<ThingDef> result = new List<ThingDef>();
		if (GenCollection.Any<ThingDef>(Main.HiveDefs, (Predicate<ThingDef>)((ThingDef x) => ((Def)x).GetModExtension<HiveDefExtension>().Faction == factionDef && x.thingClass == typeof(Hive))))
		{
			result = Main.HiveDefs.FindAll((ThingDef x) => ((Def)x).GetModExtension<HiveDefExtension>().Faction == factionDef);
		}
		return result;
	}
}
