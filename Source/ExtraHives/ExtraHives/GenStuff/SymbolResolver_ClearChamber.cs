using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.BaseGen;
using Verse;

namespace ExtraHives.GenStuff;

public class SymbolResolver_ClearChamber : SymbolResolver
{
	private static List<Thing> tmpThingsToDestroy = new List<Thing>();

	public override void Resolve(ResolveParams rp)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Invalid comparison between Unknown and I4
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		Map map = BaseGen.globalSettings.map;
		float num = Math.Min(IntVec3Utility.DistanceTo(rp.rect.Corners.ToList()[0], rp.rect.Corners.ToList()[2]), GenRadial.MaxRadialPatternRadius);
		List<IntVec3> list = GenRadial.RadialCellsAround(rp.rect.CenterCell, num, true).ToList();
		foreach (IntVec3 item in list)
		{
			if (rp.clearEdificeOnly.HasValue && rp.clearEdificeOnly.Value)
			{
				Building edifice = GridsUtility.GetEdifice(item, BaseGen.globalSettings.map);
				if (edifice != null && ((Thing)edifice).def.destroyable)
				{
					((Thing)edifice).Destroy((DestroyMode)0);
				}
			}
			else if (rp.clearFillageOnly.HasValue && rp.clearFillageOnly.Value)
			{
				tmpThingsToDestroy.Clear();
				tmpThingsToDestroy.AddRange(GridsUtility.GetThingList(item, BaseGen.globalSettings.map));
				for (int i = 0; i < tmpThingsToDestroy.Count; i++)
				{
					if (tmpThingsToDestroy[i].def.destroyable && (int)tmpThingsToDestroy[i].def.Fillage > 0)
					{
						tmpThingsToDestroy[i].Destroy((DestroyMode)0);
					}
				}
			}
			else
			{
				tmpThingsToDestroy.Clear();
				tmpThingsToDestroy.AddRange(GridsUtility.GetThingList(item, BaseGen.globalSettings.map));
				for (int j = 0; j < tmpThingsToDestroy.Count; j++)
				{
					if (tmpThingsToDestroy[j].def.destroyable)
					{
						tmpThingsToDestroy[j].Destroy((DestroyMode)0);
					}
				}
			}
			if (rp.clearRoof.HasValue && rp.clearRoof.Value)
			{
				BaseGen.globalSettings.map.roofGrid.SetRoof(item, (RoofDef)null);
			}
		}
	}
}
