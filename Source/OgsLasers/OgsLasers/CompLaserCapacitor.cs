using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace OgsLasers;

public class CompLaserCapacitor : ThingComp
{
	public LocalTargetInfo lastFiringLocation = (Thing)null;

	public int shotstack = 0;

	public float originalwarmupTime;

	public bool hotshot;

	public bool initalized;

	public CompProperties_LaserCapacitor Props => (CompProperties_LaserCapacitor)(object)base.props;

	public CompEquippable equippable => ThingCompUtility.TryGetComp<CompEquippable>((Thing)(object)base.parent);

	protected virtual bool IsWorn => GetWearer != null;

	protected virtual Pawn GetWearer
	{
		get
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			if (((ThingComp)this).ParentHolder != null && ((ThingComp)this).ParentHolder is Pawn_EquipmentTracker)
			{
				return (Pawn)((ThingComp)this).ParentHolder.ParentHolder;
			}
			return null;
		}
	}

	private Texture2D CommandTex => GenText.NullOrEmpty(Props.UiIconPath) ? ((BuildableDef)((Thing)base.parent).def).uiIcon : ContentFinder<Texture2D>.Get(Props.UiIconPath, true);

	public override IEnumerable<Gizmo> CompGetGizmosExtra()
	{
		if (!IsWorn)
		{
			_ = base.parent;
		}
		else
		{
			_ = GetWearer;
		}
		if ((object)Find.Selector.SingleSelectedThing == GetWearer && GetWearer.Drafted && (GetWearer.IsColonist || GetWearer.IsColonyMech))
		{
			int num = 700000101;
			yield return (Gizmo)new Command_Toggle
			{
				icon = (Texture)(object)CommandTex,
				defaultLabel = Translator.Translate("VWEL_ToggleHotshotLabel"),
				defaultDesc = Translator.Translate("VWEL_ToggleHotshotDesc"),
				isActive = () => hotshot,
				toggleAction = delegate
				{
					hotshot = !hotshot;
				},
				activateSound = SoundDef.Named("Click"),
				groupKey = num,
				hotKey = KeyBindingDefOf.Misc2
			};
		}
	}

	public override void PostExposeData()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((ThingComp)this).PostExposeData();
		Scribe_TargetInfo.Look(ref lastFiringLocation, "lastFiringLocation", LocalTargetInfo.Invalid);
		Scribe_Values.Look<int>(ref shotstack, "shotstack", 0, false);
		Scribe_Values.Look<float>(ref originalwarmupTime, "originalwarmupTime", 0f, false);
		Scribe_Values.Look<bool>(ref hotshot, "hotshot", false, false);
		Scribe_Values.Look<bool>(ref initalized, "initalized", false, false);
	}

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		((ThingComp)this).PostSpawnSetup(respawningAfterLoad);
		if (!respawningAfterLoad && initalized)
		{
			originalwarmupTime = ((Thing)base.parent).def.Verbs[0].warmupTime;
		}
	}

	public void CriticalOverheatExplosion(Verb_Shoot __instance)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		Map map = ((Verb)__instance).caster.Map;
		if (((Verb_LaunchProjectile)__instance).Projectile.projectile.explosionEffect != null)
		{
			Effecter val = ((Verb_LaunchProjectile)__instance).Projectile.projectile.explosionEffect.Spawn();
			val.Trigger(new TargetInfo(((Thing)((Verb)__instance).EquipmentSource).Position, map, false), new TargetInfo(((Thing)((Verb)__instance).EquipmentSource).Position, map, false), -1);
			val.Cleanup();
		}
		IntVec3 position = ((Verb)__instance).caster.Position;
		Map val2 = map;
		float overheatBlastRadius = Props.OverheatBlastRadius;
		DamageDef named = DefDatabase<DamageDef>.GetNamed(Props.OverheatBlastDamageDef, true);
		Thing equipmentSource = (Thing)(object)((Verb)__instance).EquipmentSource;
		int overheatBlastExtraDamage = Props.OverheatBlastExtraDamage;
		float armorPenetration = ((Verb_LaunchProjectile)__instance).Projectile.projectile.GetArmorPenetration((Thing)(object)((Verb)__instance).EquipmentSource, (StringBuilder)null);
		SoundDef val3 = ((((Verb_LaunchProjectile)__instance).Projectile.projectile.soundExplode == null) ? named.soundExplosion : ((Verb_LaunchProjectile)__instance).Projectile.projectile.soundExplode);
		ThingDef def = ((Thing)((Verb)__instance).EquipmentSource).def;
		ThingDef def2 = ((Thing)((Verb)__instance).EquipmentSource).def;
		Thing equipmentSource2 = (Thing)(object)((Verb)__instance).EquipmentSource;
		ThingDef postExplosionSpawnThingDef = ((Verb_LaunchProjectile)__instance).Projectile.projectile.postExplosionSpawnThingDef;
		float postExplosionSpawnChance = ((Verb_LaunchProjectile)__instance).Projectile.projectile.postExplosionSpawnChance;
		int postExplosionSpawnThingCount = ((Verb_LaunchProjectile)__instance).Projectile.projectile.postExplosionSpawnThingCount;
		ThingDef preExplosionSpawnThingDef = ((Verb_LaunchProjectile)__instance).Projectile.projectile.preExplosionSpawnThingDef;
		float preExplosionSpawnChance = ((Verb_LaunchProjectile)__instance).Projectile.projectile.preExplosionSpawnChance;
		int preExplosionSpawnThingCount = ((Verb_LaunchProjectile)__instance).Projectile.projectile.preExplosionSpawnThingCount;
		GenExplosion.DoExplosion(position, val2, overheatBlastRadius, named, equipmentSource, overheatBlastExtraDamage, armorPenetration, val3, null, null, null, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, null, null, 255, false, preExplosionSpawnThingDef, preExplosionSpawnChance, preExplosionSpawnThingCount, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f, null, null, null, null);
	}
}
