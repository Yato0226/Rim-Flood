using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ExtraHives;

public class HiveFactionExtension : DefModExtension
{
	public bool overrideBaseGen = false;

	public string baseGenOverride = string.Empty;

	public bool baseDamage = true;

	public bool randomHives = false;

	public bool noPawnPointsCurve = false;

	public IntRange sizeRange = new IntRange(44, 60);

	public ThingDef smallCaveHive = null;

	public ThingDef largeCaveHive = null;

	public ThingDef centerCaveHive = null;

	public ThingDef cultivatedPlantDef;

	public string hiveStartMessageKey = string.Empty;

	public string hiveActiveMessageKey = string.Empty;

	public string hiveStageProgressionKey = string.Empty;

	public List<HiveStage> stages = new List<HiveStage>();

	public bool showStageInName = false;

	public string stageKey = ": Stage {0}";

	public bool HasStages => !GenList.NullOrEmpty<HiveStage>((IList<HiveStage>)stages);

	public HiveStage CurStage
	{
		get
		{
			HiveStage result = stages[0];
			if (HasStages)
			{
				List<HiveStage> list = stages;
				for (int i = 0; i < list.Count; i++)
				{
					HiveStage hiveStage = list[i];
					if (hiveStage.DaysPassed > GenDate.DaysPassed)
					{
						break;
					}
					result = hiveStage;
				}
			}
			return result;
		}
	}

	public int ActiveStage
	{
		get
		{
			int result = 0;
			if (HasStages)
			{
				result = stages.IndexOf(CurStage) + 1;
			}
			return result;
		}
	}
}
