using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CrashedShipsExtension;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ExtraHives;

public class Hive : ThingWithComps
{
	private CompSpawnerPawn pawnSpawner;

	public static readonly string MemoAssaultOnSpawn = "AssaultOnSpawn";

	public static readonly string MemoDeSpawned = "MemoDeSpawned";

	public static readonly string MemoAttackedByEnemy = "MemoAttackedByEnemy";

	public static readonly string MemoBurnedBadly = "MemoBurnedBadly";

	public HiveDefExtension Ext => ((Def)((Thing)this).def).HasModExtension<HiveDefExtension>() ? ((Def)((Thing)this).def).GetModExtension<HiveDefExtension>() : null;

	public CompSpawnerPawn PawnSpawner => pawnSpawner ?? (pawnSpawner = ((ThingWithComps)this).GetComp<CompSpawnerPawn>());

	public CompCanBeDormant CompDormant => GetComp<CompCanBeDormant>();

	public override void SpawnSetup(Map map, bool respawningAfterLoad)
	{
		if (Ext != null)
		{
			FactionDef faction = Ext.Faction;
			if (faction != null)
			{
				Faction val = Find.FactionManager.FirstFactionOfDef(faction);
				if (val != null && ((Thing)this).Faction != val)
				{
					((Thing)this).SetFaction(val, (Pawn)null);
				}
			}
		}
		base.SpawnSetup(map, respawningAfterLoad);
	}

	protected override void Tick()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		base.Tick();
		if (Spawned && !CompDormant.Awake && !GridsUtility.Fogged(Position, Map))
		{
			CompDormant.WakeUp();
		}
	}

	public override void DeSpawn(DestroyMode mode = (DestroyMode)0)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Map map = ((Thing)this).Map;
		base.DeSpawn(mode);
		if (HiveUtility.TotalSpawnedHivesCount(map, filterFogged: true, ((Thing)this).def) == 0)
		{
			foreach (Pawn item in map.mapPawns.FreeColonistsSpawned)
			{
				Pawn_NeedsTracker needs = item.needs;
				if (needs == null)
				{
					continue;
				}
				Need_Mood mood = needs.mood;
				if (mood == null)
				{
					continue;
				}
				ThoughtHandler thoughts = mood.thoughts;
				if (thoughts != null)
				{
					MemoryThoughtHandler memories = thoughts.memories;
					if (memories != null)
					{
						memories.TryGainMemory(Ext.defeatedThought ?? ThoughtDefOf.DefeatedInsectHive, (Pawn)null, (Precept)null);
					}
				}
			}
		}
		List<Lord> lords = map.lordManager.lords;
		for (int i = 0; i < lords.Count; i++)
		{
			lords[i].ReceiveMemo(Hive.MemoDeSpawned);
		}
		HiveUtility.Notify_HiveDespawned(this, map);
	}

	public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		if (((DamageInfo)dinfo).Def.ExternalViolenceFor((Thing)(object)this) && ((DamageInfo)dinfo).Instigator != null && ((DamageInfo)dinfo).Instigator.Faction != null)
		{
			Lord val = PawnSpawner?.Lord;
			if (val != null)
			{
				val.ReceiveMemo(Hive.MemoAttackedByEnemy);
			}
			if (Main.CrashedShipsExtension)
			{
				CrashedShipsExtensionMemoAttackedByEnemy();
			}
		}
		if (((DamageInfo)dinfo).Def == DamageDefOf.Flame && (float)((Thing)this).HitPoints < (float)((Thing)this).MaxHitPoints * 0.3f)
		{
			Lord val2 = PawnSpawner?.Lord;
			if (val2 != null)
			{
				val2.ReceiveMemo(Hive.MemoBurnedBadly);
			}
			if (Main.CrashedShipsExtension)
			{
				CrashedShipsExtensionMemoBurnedBadly();
			}
		}
		if (this.AllComps != null)
		{
			for (int i = 0; i < this.AllComps.Count; i++)
			{
				this.AllComps[i].PostPostApplyDamage(dinfo, totalDamageDealt);
			}
		}
	}

	public void CrashedShipsExtensionMemoAttackedByEnemy()
	{
		Lord val = ((ThingWithComps)this).GetComp<CrashedShipsExtension.CompSpawnerOnDamaged>()?.Lord;
		if (val != null)
		{
			val.ReceiveMemo(Hive.MemoAttackedByEnemy);
		}
	}

	public void CrashedShipsExtensionMemoBurnedBadly()
	{
		Lord val = ((ThingWithComps)this).GetComp<CrashedShipsExtension.CompSpawnerOnDamaged>()?.Lord;
		if (val != null)
		{
			val.ReceiveMemo(Hive.MemoBurnedBadly);
		}
	}

	public override IEnumerable<Gizmo> GetGizmos()
	{
		foreach (Gizmo item in _003C_003En__0())
		{
			yield return item;
		}
		foreach (Gizmo questRelatedGizmo in QuestUtility.GetQuestRelatedGizmos((Thing)(object)this))
		{
			yield return questRelatedGizmo;
		}
	}

	public override void ExposeData()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		base.ExposeData();
		if ((int)Scribe.mode != 1)
		{
			bool flag = false;
			Scribe_Values.Look<bool>(ref flag, "active", false, false);
			if (flag)
			{
				CompDormant.WakeUp();
			}
		}
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerable<Gizmo> _003C_003En__0()
	{
		return base.GetGizmos();
	}
}
