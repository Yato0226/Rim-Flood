using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExtraHives;

public class IncidentWorker_Infestation : IncidentWorker
{
	public const float HivePoints = 220f;

	private Faction Faction = null;

	public Faction GetFaction
	{
		get
		{
			if (Faction == null)
			{
				FactionDef val = null;
				if (base.def is HivelikeIncidentDef hivelikeIncidentDef)
				{
					val = hivelikeIncidentDef.Faction;
				}
				else if (((Def)base.def.mechClusterBuilding).HasModExtension<HiveDefExtension>())
				{
					HiveDefExtension modExtension = ((Def)base.def.mechClusterBuilding).GetModExtension<HiveDefExtension>();
					val = modExtension.Faction;
				}
				if (val == null)
				{
					Log.Error("ExtraHives Infestation factionDef Not found for: " + ((Def)base.def).defName);
					return null;
				}
				if (val != null)
				{
					Faction = Find.FactionManager.FirstFactionOfDef(val);
				}
			}
			else if (Prefs.DevMode)
			{
				Log.Warning("ExtraHives Infestation IncidentDef: " + ((Def)base.def).defName + " using cached faction");
			}
			return Faction;
		}
	}

	protected override bool CanFireNowSub(IncidentParms parms)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		Map map = (Map)parms.target;
		if (base.def.mechClusterBuilding == null)
		{
			Log.Warning("ExtraHives Infestation tried CanFireNowSub " + ((Def)base.def).defName + " with no mechClusterBuilding");
			return false;
		}
		if (Faction == null && GetFactionFromParms(parms) == null)
		{
			Log.Warning("ExtraHives Infestation tried GetFactionFromParms " + ((Def)base.def).defName + " but found not matching faction");
			return false;
		}
		FactionDef val = null;
		//InfestationCellFinder.InfestationParms parms2;
		if (base.def is HivelikeIncidentDef hivelikeIncidentDef)
		{
			//parms2 = new InfestationCellFinder.InfestationParms(hivelikeIncidentDef);
			val = hivelikeIncidentDef.Faction;
		}
		else
		{
			if (!((Def)base.def.mechClusterBuilding).HasModExtension<HiveDefExtension>())
			{
				Log.Warning("ExtraHives.IncidentWorker_Infestation tried CanFireNowSub " + ((Def)base.def).defName + " with a mechClusterBuilding with no HiveDefExtension");
				return false;
			}
			HiveDefExtension modExtension = ((Def)base.def.mechClusterBuilding).GetModExtension<HiveDefExtension>();
			val = modExtension.Faction;
			//parms2 = new InfestationCellFinder.InfestationParms(modExtension);
		}
		if (val == null)
		{
			return false;
		}
		if (val.earliestRaidDays > (float)GenDate.DaysPassedSinceSettle)
		{
			return false;
		}
		Faction val2 = Find.FactionManager.FirstFactionOfDef(val);
		if (val2 == null)
		{
			return false;
		}
		if (HiveUtility.TotalSpawnedHivesCount(map, base.def.mechClusterBuilding) < 30)
		{
			if (InfestationCellFinder.TryFindCell(out var _, map))
			{
				return base.CanFireNowSub(parms);
			}
			Log.Warning("ExtraHives.IncidentWorker_Infestation tried CanFireNowSub " + ((Def)base.def).defName + " with HivelikeIncidentDef but TryFindCell failed");
		}
		return false;
	}

	protected override bool TryExecuteWorker(IncidentParms parms)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if (base.def.mechClusterBuilding == null && GetFactionFromParms(parms, CanFireNow: false) == null)
		{
			Log.Warning("ExtraHives Infestation tried CanFireNowSub " + ((Def)base.def).defName + " with no mechClusterBuilding");
			return false;
		}
		if (GetFactionFromParms(parms, CanFireNow: false) == null)
		{
			Log.Warning("ExtraHives TryExecuteWorker Infestation tried GetFactionFromParms " + ((Def)base.def).defName + " but found not matching faction");
			return false;
		}
		Map map = (Map)parms.target;
		float num = ((float?)base.def.mechClusterBuilding.GetCompProperties<CompProperties_SpawnerPawn>()?.initialPawnsPoints) ?? 250f;
		int hiveCount = Mathf.Max(GenMath.RoundRandom(parms.points / num), 1);
		if (Prefs.DevMode)
		{
			Log.Message($"ExtraHives TryExecuteWorker trying {((Def)base.def).LabelCap} with {parms.points} Points");
		}
		Thing val = InfestationUtility.SpawnTunnels(base.def.mechClusterBuilding, hiveCount, map, spawnAnywhereIfNoGoodCell: true, ignoreRoofedRequirement: true, null, parms.faction);
		base.SendStandardLetter(parms, val, Array.Empty<NamedArgument>());
		Find.TickManager.slower.SignalForceNormalSpeedShort();
		return true;
	}

	public Faction GetFactionFromParms(IncidentParms parms, bool CanFireNow = true)
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (Faction == null)
		{
			if (parms.faction != null)
			{
				Faction = parms.faction;
				if (Prefs.DevMode)
				{
					Log.Warning($"{((Def)base.def).LabelCap} using parms.faction {parms.faction.Name} CanFireNow: {CanFireNow}");
				}
			}
			else
			{
				parms.faction = GetFaction;
				Faction = parms.faction;
				if (Prefs.DevMode)
				{
					Log.Warning($"{((Def)base.def).LabelCap} using this.GetFaction {GetFaction} CanFireNow: {CanFireNow}");
				}
			}
		}
		if (Faction == null)
		{
			Log.Warning($"{((Def)base.def).LabelCap} Failed to find viable faction");
		}
		return Faction;
	}
}
