using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CrashedShipsExtension;

public class IncidentWorker_CrashedShip : IncidentWorker
{
	private const float ShipPointsFactor = 0.9f;

	private const int IncidentMinimumPoints = 300;

	protected virtual int CountToSpawn(IncidentParms parms)
	{
		return 1;
	}

	public override bool CanFireNowSub(IncidentParms parms)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Map)parms.target).listerThings.ThingsOfDef(base.def.mechClusterBuilding).Count <= 0;
	}

	public override bool TryExecuteWorker(IncidentParms parms)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		Map val = (Map)parms.target;
		int num = 0;
		int num2 = CountToSpawn(parms);
		List<TargetInfo> list = new List<TargetInfo>();
		Rand.PushState();
		float shrapnelDirection = Rand.Range(0f, 360f);
		Rand.PopState();
		ThingDef mechClusterBuilding = base.def.mechClusterBuilding;
		if (((mechClusterBuilding != null) ? mechClusterBuilding.GetCompProperties<CompProperties_SpawnerOnDamaged>() : null) == null)
		{
			return false;
		}
		IntVec3 val4 = default(IntVec3);
		for (int i = 0; i < num2; i++)
		{
			Building val2 = (Building)ThingMaker.MakeThing(base.def.mechClusterBuilding, (ThingDef)null);
			CompSpawnerOnDamaged compSpawnerOnDamaged = ThingCompUtility.TryGetComp<CompSpawnerOnDamaged>((Thing)(object)val2);
			ThingDef val3 = compSpawnerOnDamaged?.Props.skyFaller ?? ThingDefOf.CrashedShipPartIncoming;
			if (!CellFinderLoose.TryFindSkyfallerCell(val3, val, ref val4, 14, default(IntVec3), -1, false, true, true, true, true, false, (Predicate<IntVec3>)null))
			{
				break;
			}
			if (compSpawnerOnDamaged != null)
			{
				compSpawnerOnDamaged.pointsLeft = Mathf.Max(parms.points * 0.9f, 300f);
			}
			Skyfaller obj = SkyfallerMaker.MakeSkyfaller(val3, (Thing)(object)val2);
			obj.shrapnelDirection = shrapnelDirection;
			GenSpawn.Spawn((Thing)(object)obj, val4, val, (WipeMode)0);
			num++;
			list.Add(TargetInfo.op_Implicit((Thing)(object)val2));
		}
		if (num > 0)
		{
			((IncidentWorker)this).SendStandardLetter(parms, LookTargets.op_Implicit(list), Array.Empty<NamedArgument>());
		}
		return num > 0;
	}
}
