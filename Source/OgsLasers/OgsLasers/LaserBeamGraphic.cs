using System;
using UnityEngine;
using Verse;

namespace OgsLasers;

internal class LaserBeamGraphic : Thing
{
	public LaserBeam laserBeam;

	private int ticks;

	private int colorIndex = 2;

	private Vector3 a;

	private Vector3 b;

	public Matrix4x4 drawingMatrix = default(Matrix4x4);

	private Material materialBeam;

	private Mesh mesh;

	public LaserBeamDef LaserBeamDef => ((Thing)(laserBeam?)).def as LaserBeamDef;

	public float Opacity => (float)Math.Sin(Math.Pow(1.0 - 1.0 * (double)ticks / (double)LaserBeamDef.lifetime, LaserBeamDef.impulse) * Math.PI);

	public override void ExposeData()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		((Thing)this).ExposeData();
		Scribe_References.Look<LaserBeam>(ref laserBeam, "laserBeam", false);
		Scribe_Values.Look<int>(ref ticks, "ticks", 0, false);
		Scribe_Values.Look<int>(ref colorIndex, "colorIndex", 0, false);
		Scribe_Values.Look<Vector3>(ref a, "a", default(Vector3), false);
		Scribe_Values.Look<Vector3>(ref b, "b", default(Vector3), false);
	}

	public override void Tick()
	{
		if (LaserBeamDef == null || ticks++ > LaserBeamDef.lifetime)
		{
			((Thing)this).Destroy((DestroyMode)0);
		}
	}

	private void SetColor(Thing launcher)
	{
		IBeamColorThing beamColorThing = null;
		Pawn val = (Pawn)(object)((launcher is Pawn) ? launcher : null);
		if (val != null && val.equipment != null)
		{
			beamColorThing = val.equipment.Primary as IBeamColorThing;
		}
		if (beamColorThing == null)
		{
			beamColorThing = launcher as IBeamColorThing;
		}
		if (beamColorThing != null && beamColorThing.BeamColor != -1)
		{
			colorIndex = beamColorThing.BeamColor;
		}
	}

	public void Setup(Thing launcher, Vector3 origin, Vector3 destination)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		SetColor(launcher);
		a = origin;
		b = destination;
	}

	public void SetupDrawing()
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)mesh != (Object)null))
		{
			materialBeam = LaserBeamDef.GetBeamMaterial(colorIndex) ?? ((ThingDef)LaserBeamDef).graphicData.Graphic.MatSingle;
			if (((ThingDef)LaserBeamDef).graphicData.graphicClass == typeof(Graphic_Random))
			{
				materialBeam = LaserBeamDef.GetBeamMaterial(Rand.RangeInclusive(0, LaserBeamDef.materials.Count)) ?? ((ThingDef)LaserBeamDef).graphicData.Graphic.MatSingle;
			}
			float beamWidth = LaserBeamDef.beamWidth;
			Quaternion val = Quaternion.LookRotation(b - a);
			Vector3 val2 = b - a;
			Vector3 normalized = ((Vector3)(ref val2)).normalized;
			val2 = b - a;
			float magnitude = ((Vector3)(ref val2)).magnitude;
			Vector3 val3 = default(Vector3);
			((Vector3)(ref val3))._002Ector(beamWidth, 1f, magnitude);
			Vector3 val4 = (a + b) / 2f;
			((Matrix4x4)(ref drawingMatrix)).SetTRS(val4, val, val3);
			float num = 1f * (float)materialBeam.mainTexture.width / (float)materialBeam.mainTexture.height;
			float num2 = ((LaserBeamDef.seam < 0f) ? num : LaserBeamDef.seam);
			float num3 = beamWidth / num / 2f * num2;
			float sv = ((magnitude <= num3 * 2f) ? 0.5f : (num3 * 2f / magnitude));
			mesh = MeshMakerLaser.Mesh(num2, sv);
		}
	}

	public override void SpawnSetup(Map map, bool respawningAfterLoad)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		((Thing)this).SpawnSetup(map, respawningAfterLoad);
		if (LaserBeamDef == null || LaserBeamDef.decorations == null || respawningAfterLoad)
		{
			return;
		}
		foreach (LaserBeamDecoration decoration in LaserBeamDef.decorations)
		{
			float num = decoration.spacing * LaserBeamDef.beamWidth;
			float num2 = decoration.initialOffset * LaserBeamDef.beamWidth;
			Vector3 val = b - a;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			float num3 = Vector3Utility.AngleFlat(b - a);
			Vector3 val2 = normalized * num;
			Vector3 val3 = a + val2 * 0.5f + normalized * num2;
			val = b - a;
			float num4 = ((Vector3)(ref val)).magnitude - num;
			int num5 = 0;
			while (num4 > 0f && ThingMaker.MakeThing(decoration.mote, (ThingDef)null) is MoteLaserDectoration moteLaserDectoration)
			{
				moteLaserDectoration.beam = this;
				((MoteThrown)moteLaserDectoration).airTimeLeft = LaserBeamDef.lifetime;
				((Mote)moteLaserDectoration).Scale = LaserBeamDef.beamWidth;
				((Mote)moteLaserDectoration).exactRotation = num3;
				((Mote)moteLaserDectoration).exactPosition = val3;
				((MoteThrown)moteLaserDectoration).SetVelocity(num3, decoration.speed);
				moteLaserDectoration.baseSpeed = decoration.speed;
				moteLaserDectoration.speedJitter = decoration.speedJitter;
				moteLaserDectoration.speedJitterOffset = decoration.speedJitterOffset * (float)num5;
				GenSpawn.Spawn((Thing)(object)moteLaserDectoration, IntVec3Utility.ToIntVec3(a), map, (WipeMode)0);
				val3 += val2;
				num4 -= num;
				num5++;
			}
		}
	}

	protected override void DrawAt(Vector3 drawLoc, bool flip = false)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		SetupDrawing();
		float opacity = Opacity;
		if (((ThingDef)LaserBeamDef).graphicData.graphicClass == typeof(Graphic_Flicker) && !Find.TickManager.Paused && Find.TickManager.TicksGame % LaserBeamDef.flickerFrameTime == 0)
		{
			materialBeam = LaserBeamDef.GetBeamMaterial(Rand.RangeInclusive(0, LaserBeamDef.materials.Count)) ?? ((ThingDef)LaserBeamDef).graphicData.Graphic.MatSingle;
		}
		Graphics.DrawMesh(mesh, drawingMatrix, FadedMaterialPool.FadedVersionOf(materialBeam, opacity), 0);
	}
}
