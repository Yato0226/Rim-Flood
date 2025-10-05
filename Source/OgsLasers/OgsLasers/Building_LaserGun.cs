using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;
using Verse.Sound;

namespace OgsLasers;

public class Building_LaserGun : Building_TurretGun, IBeamColorThing
{
	private CompPowerTrader power;

	public bool isCharged = false;

	public int previousBurstCooldownTicksLeft = 0;

	private int beamColorIndex = -1;

	public Building_LaserGunDef def => ((Thing)this).def as Building_LaserGunDef;

	public int BurstCooldownTicksLeft => base.burstCooldownTicksLeft;

	public int BurstWarmupTicksLeft => base.burstWarmupTicksLeft;

	public int BeamColor
	{
		get
		{
			return LaserColor.IndexBasedOnThingQuality(beamColorIndex, (Thing)(object)this);
		}
		set
		{
			beamColorIndex = value;
		}
	}

	public override void ExposeData()
	{
		((Building_TurretGun)this).ExposeData();
		Scribe_Values.Look<bool>(ref isCharged, "isCharged", false, false);
		Scribe_Values.Look<int>(ref previousBurstCooldownTicksLeft, "previousBurstCooldownTicksLeft", 0, false);
		Scribe_Values.Look<int>(ref beamColorIndex, "beamColorIndex", -1, false);
	}

	public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn pawn)
	{
		foreach (FloatMenuOption o in _003C_003En__0(pawn))
		{
			if (o != null)
			{
				yield return o;
			}
		}
		if (!def.supportsColors)
		{
		}
	}

	public override void SpawnSetup(Map map, bool respawningAfterLoad)
	{
		((Building_TurretGun)this).SpawnSetup(map, respawningAfterLoad);
		power = ((ThingWithComps)this).GetComp<CompPowerTrader>();
	}

	public override void Tick()
	{
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		if (base.burstCooldownTicksLeft > previousBurstCooldownTicksLeft)
		{
			isCharged = false;
		}
		previousBurstCooldownTicksLeft = base.burstCooldownTicksLeft;
		if (!isCharged && Drain(def.beamPowerConsumption))
		{
			isCharged = true;
		}
		if (isCharged || base.burstCooldownTicksLeft > 1)
		{
			int burstWarmupTicksLeft = base.burstWarmupTicksLeft;
			((Building_TurretGun)this).Tick();
			if (base.burstWarmupTicksLeft == GenTicks.SecondsToTicks(((ThingDef)def).building.turretBurstWarmupTime.max) - 1 && burstWarmupTicksLeft == base.burstWarmupTicksLeft + 1 && ((Building_Turret)this).AttackVerb.verbProps.soundAiming != null)
			{
				SoundStarter.PlayOneShot(((Building_Turret)this).AttackVerb.verbProps.soundAiming, SoundInfo.op_Implicit(new TargetInfo(((Thing)this).Position, ((Thing)this).Map, false)));
			}
		}
	}

	public float AvailablePower()
	{
		if (((CompPower)power).PowerNet == null)
		{
			return 0f;
		}
		float num = 0f;
		foreach (CompPowerBattery batteryComp in ((CompPower)power).PowerNet.batteryComps)
		{
			num += batteryComp.StoredEnergy;
		}
		return num;
	}

	public bool Drain(float amount)
	{
		if (amount <= 0f)
		{
			return true;
		}
		if (AvailablePower() < amount)
		{
			return false;
		}
		foreach (CompPowerBattery batteryComp in ((CompPower)power).PowerNet.batteryComps)
		{
			float num = ((batteryComp.StoredEnergy > amount) ? amount : batteryComp.StoredEnergy);
			batteryComp.DrawPower(num);
			amount -= num;
			if (amount <= 0f)
			{
				break;
			}
		}
		return true;
	}

	public override string GetInspectString()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		string text = ((Building_TurretGun)this).GetInspectString();
		if (!isCharged)
		{
			text += "\n";
			text = TaggedString.op_Implicit(text + Translator.Translate("LaserTurretNotCharged"));
		}
		return text;
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerable<FloatMenuOption> _003C_003En__0(Pawn selPawn)
	{
		return ((ThingWithComps)this).GetFloatMenuOptions(selPawn);
	}
}
