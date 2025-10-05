using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace OgsLasers;

[HarmonyPatch(typeof(TurretTop), "DrawTurret")]
[StaticConstructorOnStartup]
internal class CYA_TuretTop_DrawTurret_LaserTurret_Patch
{
	private static FieldInfo parentTurretField;

	private static FieldInfo curRotationIntField;

	static CYA_TuretTop_DrawTurret_LaserTurret_Patch()
	{
		parentTurretField = typeof(TurretTop).GetField("parentTurret", BindingFlags.Instance | BindingFlags.NonPublic);
		curRotationIntField = typeof(TurretTop).GetField("curRotationInt", BindingFlags.Instance | BindingFlags.NonPublic);
	}

	private static bool Prefix(TurretTop __instance)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		if (!(parentTurretField.GetValue(__instance) is Building_LaserGun building_LaserGun))
		{
			return true;
		}
		float num = (float)curRotationIntField.GetValue(__instance);
		LocalTargetInfo targetCurrentlyAimingAt = ((Building_Turret)building_LaserGun).TargetCurrentlyAimingAt;
		if (((LocalTargetInfo)(ref targetCurrentlyAimingAt)).HasThing)
		{
			targetCurrentlyAimingAt = ((Building_Turret)building_LaserGun).TargetCurrentlyAimingAt;
			num = Vector3Utility.AngleFlat(((LocalTargetInfo)(ref targetCurrentlyAimingAt)).CenterVector3 - GenThing.TrueCenter((Thing)(object)building_LaserGun));
		}
		if (((Building_TurretGun)building_LaserGun).gun is IDrawnWeaponWithRotation drawnWeaponWithRotation)
		{
			num += drawnWeaponWithRotation.RotationOffset;
		}
		Material val = ((ThingDef)building_LaserGun.def).building.turretTopMat;
		if (((Building_TurretGun)building_LaserGun).gun is SpinningLaserGunTurret spinningLaserGunTurret)
		{
			spinningLaserGunTurret.turret = building_LaserGun;
			val = ((Thing)spinningLaserGunTurret).Graphic.MatSingle;
		}
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(((ThingDef)building_LaserGun.def).building.turretTopOffset.x, 0f, ((ThingDef)building_LaserGun.def).building.turretTopOffset.y);
		float turretTopDrawSize = ((ThingDef)building_LaserGun.def).building.turretTopDrawSize;
		Matrix4x4 val3 = default(Matrix4x4);
		((Matrix4x4)(ref val3)).SetTRS(((Thing)building_LaserGun).DrawPos + Altitudes.AltIncVect + val2, GenMath.ToQuat(num), new Vector3(turretTopDrawSize, 1f, turretTopDrawSize));
		Graphics.DrawMesh(MeshPool.plane10, val3, val, 0);
		return false;
	}
}
