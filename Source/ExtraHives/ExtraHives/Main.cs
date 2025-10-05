using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace ExtraHives;

[StaticConstructorOnStartup]
internal class Main
{
	public static bool CrashedShipsExtension;

	public static List<ThingDef> HiveDefs;

	public static List<ThingDef> TunnelDefs;

	static Main()
	{
		CrashedShipsExtension = false;
		HiveDefs = new List<ThingDef>();
		TunnelDefs = new List<ThingDef>();
		foreach (ModContentPack runningMod in LoadedModManager.RunningMods)
		{
			foreach (Assembly loadedAssembly in runningMod.assemblies.loadedAssemblies)
			{
				Type type = loadedAssembly.GetType("CrashedShipsExtension.IncidentWorker_CrashedShip");
				if (type != null)
				{
					CrashedShipsExtension = true;
					break;
				}
			}
			if (CrashedShipsExtension)
			{
				break;
			}
		}
		HiveDefs = DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => ((Def)x).HasModExtension<HiveDefExtension>()).ToList();
		TunnelDefs = DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => ((Def)x).HasModExtension<TunnelExtension>()).ToList();
	}
}
