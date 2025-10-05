using System;
using ExtraHives.ExtensionMethods;
using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ExtraHives.HarmonyInstance;

[HarmonyPatch(typeof(GenStep_Settlement), "ScatterAt")]
public static class GenStep_Settlement_ScatterAt_ExtraHives_Patch
{
	[HarmonyPrefix]
	[HarmonyPriority(0)]
	public static bool Prefix(IntVec3 c, Map map, ref GenStepParams parms, int stackCount = 1)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (map.ParentFaction != null && map.ParentFaction.HiveExt() != null)
			{
				HiveFactionExtension hiveFactionExtension = map.ParentFaction.HiveExt();
				if (hiveFactionExtension.overrideBaseGen)
				{
					IntRange sizeRange = hiveFactionExtension.CurStage.sizeRange;
					int randomInRange = sizeRange.RandomInRange;
					int randomInRange2 = sizeRange.RandomInRange;
					CellRect rect = new CellRect(c.x - randomInRange / 2, c.z - randomInRange2 / 2, randomInRange, randomInRange2);
					rect.ClipInsideMap(map);
					ResolveParams val = new ResolveParams
					{
						rect = rect,
						faction = map.ParentFaction,
						cultivatedPlantDef = (hiveFactionExtension.cultivatedPlantDef ?? DefDatabase<ThingDef>.GetNamed("Plant_Grass"))
					};
					BaseGen.globalSettings.map = map;
					BaseGen.globalSettings.minBuildings = 8;
					BaseGen.globalSettings.minBarracks = 2;
					BaseGen.symbolStack.Push(GenText.NullOrEmpty(hiveFactionExtension.baseGenOverride) ? "ExtraHives_HiveBaseMaker" : hiveFactionExtension.baseGenOverride, val, (string)null);
					BaseGen.Generate();
					if (hiveFactionExtension.baseDamage)
					{
						BaseGen.globalSettings.map = map;
						BaseGen.symbolStack.Push("ExtraHives_HiveRandomDamage", val, (string)null);
						BaseGen.Generate();
					}
					if (hiveFactionExtension.randomHives)
					{
						BaseGen.globalSettings.map = map;
						BaseGen.symbolStack.Push("ExtraHives_HiveRandomHives", val, (string)null);
						BaseGen.Generate();
					}
					return false;
				}
				return true;
			}
			return true;
		}
		catch (Exception ex)
		{
			Log.Error(ex.Message);
			return true;
		}
	}
}
