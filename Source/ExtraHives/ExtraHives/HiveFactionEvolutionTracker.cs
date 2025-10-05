using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ExtraHives;

public class HiveFactionEvolutionTracker : WorldComponent
{
	public Dictionary<string, int> HiveFactionStages = new Dictionary<string, int>();

	private List<FactionDef> factionDefs;

	private List<Faction> hiveFactions = new List<Faction>();

	private List<string> factions;

	private List<int> stages;

	public int CurrentPhase = 1;

	private int ticks = 0;

	public bool startMsg = false;

	public bool activetMsg = false;

	private int tickInterval = 30000;

	public List<FactionDef> FactionDefs
	{
		get
		{
			if (GenList.NullOrEmpty<FactionDef>((IList<FactionDef>)factionDefs))
			{
				factionDefs = new List<FactionDef>();
				factionDefs = DefDatabase<FactionDef>.AllDefs.Where((FactionDef x) => ((Def)x).HasModExtension<HiveFactionExtension>()).ToList();
			}
			return factionDefs;
		}
	}

	public int DaysPassed => GenDate.DaysPassed;

	public HiveFactionEvolutionTracker(World world)
		: base(world)
	{
		ticks = 0;
	}

	public List<Settlement> Settlements(Faction faction)
	{
		List<Settlement> list = new List<Settlement>();
		return base.world.worldObjects.Settlements.FindAll((Settlement x) => ((WorldObject)x).Faction == faction);
	}

	public override void WorldComponentTick()
	{
		if (ticks == 0)
		{
			for (int i = 0; i < Find.FactionManager.AllFactionsListForReading.Count; i++)
			{
				Faction val = Find.FactionManager.AllFactionsListForReading[i];
				if (val != null && ((Def)val.def).HasModExtension<HiveFactionExtension>() && !val.defeated)
				{
					HiveFactionExtension modExtension = ((Def)val.def).GetModExtension<HiveFactionExtension>();
					if (!GenList.NullOrEmpty<HiveStage>((IList<HiveStage>)modExtension.stages) && (CurrentPhase < modExtension.ActiveStage || !HiveFactionStages.ContainsKey(((object)val).ToString())))
					{
						UpdatePhase(val, modExtension.ActiveStage);
					}
					if (!GenText.NullOrEmpty(modExtension.hiveStartMessageKey) && !startMsg)
					{
						startMsg = true;
						NewGameDialogMessage(val, modExtension.hiveStartMessageKey);
					}
					if (!GenText.NullOrEmpty(modExtension.hiveActiveMessageKey) && !activetMsg && (float)DaysPassed > val.def.earliestRaidDays)
					{
						activetMsg = true;
						HiveActiveDialogMessage(val, modExtension.ActiveStage, modExtension.hiveActiveMessageKey, modExtension.stages[modExtension.ActiveStage].DaysPassed - DaysPassed);
					}
				}
			}
			ticks = tickInterval;
		}
		ticks--;
	}

	public void UpdatePhase(Faction f, int phase)
	{
		HiveFactionExtension modExtension = ((Def)f.def).GetModExtension<HiveFactionExtension>();
		if (!GenText.NullOrEmpty(modExtension.hiveStageProgressionKey) && HiveFactionStages.ContainsKey(((object)f).ToString()))
		{
			UpdatePhaseDialogMessage(f, phase, modExtension.hiveStageProgressionKey);
		}
		CurrentPhase = phase;
		GenCollection.SetOrAdd<string, int>(HiveFactionStages, ((object)f).ToString(), phase);
	}

	public void UpdatePhaseDialogMessage(Faction f, int phase, string msg)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		DiaNode val = new DiaNode(TranslatorFormattedStringExtensions.Translate(msg, f, phase));
		DiaOption val2 = new DiaOption("OK");
		val2.resolveTree = true;
		val.options.Add(val2);
		Dialog_NodeTree val3 = new Dialog_NodeTree(val, true, false, (string)null);
		Find.WindowStack.Add((Window)(object)val3);
		Find.Archive.Add((IArchivable)new ArchivedDialog(val.text, (string)null, (Faction)null));
	}

	public void NewGameDialogMessage(Faction f, string msg)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		DiaNode val = new DiaNode(TranslatorFormattedStringExtensions.Translate(msg, f));
		DiaOption val2 = new DiaOption("OK");
		val2.resolveTree = true;
		val.options.Add(val2);
		Dialog_NodeTree val3 = new Dialog_NodeTree(val, true, false, (string)null);
		Find.WindowStack.Add((Window)(object)val3);
		Find.Archive.Add((IArchivable)new ArchivedDialog(val.text, (string)null, (Faction)null));
	}

	public void HiveActiveDialogMessage(Faction f, int phase, string msg, int daysLeft)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		DiaNode val = new DiaNode(TranslatorFormattedStringExtensions.Translate(msg, f, phase, daysLeft));
		DiaOption val2 = new DiaOption("OK");
		val2.resolveTree = true;
		val.options.Add(val2);
		Dialog_NodeTree val3 = new Dialog_NodeTree(val, true, false, (string)null);
		Find.WindowStack.Add((Window)(object)val3);
		Find.Archive.Add((IArchivable)new ArchivedDialog(val.text, (string)null, (Faction)null));
	}

	public override void ExposeData()
	{
		((WorldComponent)this).ExposeData();
		Scribe_Values.Look<int>(ref CurrentPhase, "CurrentPhase", 0, false);
		Scribe_Values.Look<bool>(ref startMsg, "startMsg", false, false);
		Scribe_Values.Look<bool>(ref activetMsg, "activetMsg", false, false);
		Scribe_Collections.Look<string, int>(ref HiveFactionStages, "HiveFactionStages", (LookMode)1, (LookMode)1, ref factions, ref stages, true, false, false);
		Scribe_Collections.Look<string>(ref factions, "Hivefactions", LookMode.Deep);
		Scribe_Collections.Look<int>(ref stages, "Hivestages", (LookMode)1, new object[1]
		{
			new List<int>()
		});
	}
}
