using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ExtraHives;

public class PlaceWorker_ShowHiveRadius : PlaceWorker
{
	public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (thing == null)
		{
			return;
		}
		CompSpawnerHives compSpawnerHives = ThingCompUtility.TryGetComp<CompSpawnerHives>(thing);
		if (compSpawnerHives != null && !GenCollection.EnumerableNullOrEmpty<CurvePoint>((IEnumerable<CurvePoint>)compSpawnerHives.Props.radiusPerDayCurve))
		{
			float currentRadius = compSpawnerHives.CurrentRadius;
			if (currentRadius < 50f)
			{
				GenDraw.DrawRadiusRing(center, currentRadius);
			}
		}
	}
}
