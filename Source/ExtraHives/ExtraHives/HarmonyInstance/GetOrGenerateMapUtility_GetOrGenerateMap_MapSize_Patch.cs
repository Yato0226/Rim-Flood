using System;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using System.Collections.Generic;

namespace ExtraHives.HarmonyInstance;

[HarmonyPatch(typeof(GetOrGenerateMapUtility), "GetOrGenerateMap", new Type[]
{
	typeof(PlanetTile),
	typeof(IntVec3),
	typeof(WorldObjectDef),
	typeof(IEnumerable<GenStepWithParams>),
	typeof(bool)
})]
public static class GetOrGenerateMapUtility_GetOrGenerateMap_MapSize_Patch
{
	public static void Prefix(PlanetTile tile, IntVec3 size, WorldObjectDef suggestedMapParentDef, IEnumerable<GenStepWithParams> extraGenStepDefs, bool stepDebugger)
	{
		Map val = Current.Game.FindMap(tile);
		Faction val2 = null;
		if (val != null)
		{
			if (val.ParentFaction != null)
			{
				val2 = val.ParentFaction;
			}
		}
		else
		{
			MapParent val3 = Find.WorldObjects.MapParentAt(tile);
			if (((val3 != null) ? ((WorldObject)val3).Faction : null) != null)
			{
				val2 = ((WorldObject)val3).Faction;
			}
		}
		if (val2 == null)
		{
			return;
		}
		HiveFactionEvolutionTracker component = Find.World.GetComponent<HiveFactionEvolutionTracker>();
		HiveFactionExtension modExtension = ((Def)val2.def).GetModExtension<HiveFactionExtension>();
		if (component == null || modExtension == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < 10; i++)
		{
			if (!((float)modExtension.CurStage.sizeRange.max >= (float)((size.x + num) / 2) * 0.75f))
			{
				break;
			}
			num = (int)((float)size.x * 0.25f);
		}
		if (num > 0)
		{
			size.x += num;
			size.z += num;
		}
	}
}
