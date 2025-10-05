using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace OgsLasers;

[HarmonyPatch(typeof(PawnRenderUtility), "DrawEquipmentAiming", new Type[]
{
	typeof(Thing),
	typeof(Vector3),
	typeof(float)
})]
[StaticConstructorOnStartup]
public static class CYA_PawnRenderUtility_Draw_EquipmentAiming_GunDrawing_Patch
{
	private static FieldInfo pawnField;

	static CYA_PawnRenderUtility_Draw_EquipmentAiming_GunDrawing_Patch()
	{
		pawnField = typeof(PawnRenderer).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic);
	}

	private static void Prefix(ref Thing eq, ref Vector3 drawLoc, ref float aimAngle, PawnRenderer __instance)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if (__instance == null)
		{
			return;
		}
		object value = pawnField.GetValue(__instance);
		Pawn val = (Pawn)((value is Pawn) ? value : null);
		if (val != null && eq is IDrawnWeaponWithRotation drawnWeaponWithRotation)
		{
			Stance curStance = val.stances.curStance;
			Stance_Busy val2 = (Stance_Busy)(object)((curStance is Stance_Busy) ? curStance : null);
			if (val2 != null && !val2.neverAimWeapon && ((LocalTargetInfo)(ref val2.focusTarg)).IsValid)
			{
				drawLoc -= Vector3Utility.RotatedBy(new Vector3(0f, 0f, 0.4f), aimAngle);
				aimAngle = (aimAngle + drawnWeaponWithRotation.RotationOffset) % 360f;
				drawLoc += Vector3Utility.RotatedBy(new Vector3(0f, 0f, 0.4f), aimAngle);
			}
		}
	}
}
