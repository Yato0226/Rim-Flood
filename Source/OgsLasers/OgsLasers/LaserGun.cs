using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Verse;

namespace OgsLasers;

public class LaserGun : ThingWithComps, IBeamColorThing, IDrawnWeaponWithRotation
{
	private int ticksPreviously = 0;

	private int beamColorIndex = -1;

	private float rotationSpeed = 0f;

	private float rotationOffset = 0f;

	public LaserGunDef def => (((Thing)this).def as LaserGunDef) ?? LaserGunDef.defaultObj;

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

	public float RotationOffset
	{
		get
		{
			int ticksGame = Find.TickManager.TicksGame;
			UpdateRotationOffset(ticksGame - ticksPreviously);
			ticksPreviously = ticksGame;
			return rotationOffset;
		}
		set
		{
			rotationOffset = value;
			rotationSpeed = 0f;
		}
	}

	public override void ExposeData()
	{
		((ThingWithComps)this).ExposeData();
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

	private void UpdateRotationOffset(int ticks)
	{
		if (rotationOffset == 0f || ticks <= 0)
		{
			return;
		}
		if (ticks > 30)
		{
			ticks = 30;
		}
		if (rotationOffset > 0f)
		{
			rotationOffset -= rotationSpeed;
			if (rotationOffset < 0f)
			{
				rotationOffset = 0f;
			}
		}
		else if (rotationOffset < 0f)
		{
			rotationOffset += rotationSpeed;
			if (rotationOffset > 0f)
			{
				rotationOffset = 0f;
			}
		}
		rotationSpeed += (float)ticks * 0.01f;
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerable<FloatMenuOption> _003C_003En__0(Pawn selPawn)
	{
		return ((ThingWithComps)this).GetFloatMenuOptions(selPawn);
	}
}
