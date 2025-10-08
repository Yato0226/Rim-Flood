using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace OgsLasers;

public class LaserBeam : Bullet
{
	private new LaserBeamDef def => ((Thing)this).def as LaserBeamDef;

	public override int DamageAmount
	{
		get
		{
			return (int)((float)base.DamageAmount * _shieldDamageMultiplier);
		}
	}

	private float _shieldDamageMultiplier = 1f;

	private void TriggerEffect(EffecterDef effect, Vector3 position)
	{
		TriggerEffect(effect, IntVec3.FromVector3(position));
	}

	private void TriggerEffect(EffecterDef effect, IntVec3 dest)
	{
		if (effect != null)
		{
			TargetInfo val = new TargetInfo(dest, ((Thing)this).Map, false);
			Effecter val2 = effect.Spawn();
			val2.Trigger(val, val, -1);
			val2.Cleanup();
		}
	}

	private void SpawnBeam(Vector3 a, Vector3 b)
	{
		LaserBeamGraphic laserBeamGraphic = ThingMaker.MakeThing(def.beamGraphic, (ThingDef)null) as LaserBeamGraphic;
		laserBeamGraphic.laserBeam = this;
		if (laserBeamGraphic != null)
		{
			laserBeamGraphic.Setup(base.launcher, a, b);
			GenSpawn.Spawn((Thing)(object)laserBeamGraphic, IntVec3Utility.ToIntVec3(base.origin), ((Thing)this).Map, (WipeMode)0);
		}
	}

	private void SpawnBeamReflections(Vector3 a, Vector3 b, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Vector3 val = b - a;
			Vector3 normalized = val.normalized;
			Vector3 b2 = b - Vector3Utility.RotatedBy(normalized, Rand.Range(-22.5f, 22.5f)) * Rand.Range(1f, 4f);
			SpawnBeam(b, b2);
		}
	}

	protected override void Impact(Thing hitThing, bool blockedByShield = false)
	{
		bool flag = (hitThing.IsShielded() && def.IsWeakToShields) || blockedByShield;
		_shieldDamageMultiplier = 1f;
		LaserGunDef laserGunDef = base.equipmentDef as LaserGunDef;
		Vector3 val = base.destination - base.origin;
		Vector3 normalized = val.normalized;
		normalized.y = 0f;
		Vector3 val2 = base.origin + normalized * (laserGunDef?.barrelLength ?? 0.9f);
		Vector3 val3 = ((flag && hitThing != null) ? (GenThing.TrueCenter(hitThing) - Vector3Utility.RotatedBy(normalized, Rand.Range(-22.5f, 22.5f)) * 0.8f) : base.destination);
		val2.y = (val3.y = ((BuildableDef)def).Altitude);
		SpawnBeam(val2, val3);
		if (def.canExplode && ((ThingDef)def).projectile.explosionRadius > 0f)
		{
			Explode(hitThing);
			GenExplosion.NotifyNearbyPawnsOfDangerousExplosive((Thing)(object)this, ((ThingDef)def).projectile.damageDef, base.launcher.Faction, (Thing)null);
			Thing launcher = base.launcher; Pawn val4 = (Pawn)(object)((launcher is Pawn) ? launcher : null);
			IDrawnWeaponWithRotation drawnWeaponWithRotation = null;
			if (val4 != null && val4.equipment != null)
			{
				drawnWeaponWithRotation = val4.equipment.Primary as IDrawnWeaponWithRotation;
			}
			if (drawnWeaponWithRotation == null && base.launcher is Building_LaserGun building_LaserGun)
			{
				drawnWeaponWithRotation = ((Building_TurretGun)building_LaserGun).gun as IDrawnWeaponWithRotation;
			}
			if (drawnWeaponWithRotation != null)
			{
				float num = Vector3Utility.AngleFlat(val3 - val2) - Vector3Utility.AngleFlat(base.intendedTarget.CenterVector3 - val2);
				drawnWeaponWithRotation.RotationOffset = (num + 180f) % 360f - 180f;
			}
			if (hitThing == null)
			{
				TriggerEffect(def.explosionEffect, base.destination);
				if (def.causefireChance > 0f && Rand.Chance(def.causefireChance))
				{
					FireUtility.TryStartFireIn(IntVec3Utility.ToIntVec3(base.destination), base.launcher.Map, 0.05f, (Thing)null, (SimpleCurve)null);
				}
			}
			else
			{
				if (hitThing is Pawn)
				{
					Pawn val5 = (Pawn)(object)((hitThing is Pawn) ? hitThing : null);
					if (flag)
					{
						                        _shieldDamageMultiplier = def.shieldDamageMultiplier;						SpawnBeamReflections(val2, val3, 5);
					}
				}
				if (def.causefireChance > 0f && Rand.Range(0f, 1f) > def.causefireChance)
				{
					FireUtility.TryAttachFire(hitThing, 0.05f, (Thing)null);
				}
				TriggerEffect(def.explosionEffect, ((Projectile)this).ExactPosition);
			}
			base.Impact(hitThing, blockedByShield);
		}
	}

	protected virtual void Explode(Thing hitThing, bool destroy = false)
	{
		Map map = ((Thing)this).Map;
		IntVec3 val = ((hitThing != null) ? hitThing.PositionHeld : IntVec3Utility.ToIntVec3(base.destination));
		if (destroy)
		{
			((Thing)this).Destroy((DestroyMode)0);
		}
		if (((ThingDef)def).projectile.explosionEffect != null)
		{
			Effecter val2 = ((ThingDef)def).projectile.explosionEffect.Spawn();
			val2.Trigger(new TargetInfo(val, map, false), new TargetInfo(val, map, false), -1);
			val2.Cleanup();
		}
		IntVec3 val3 = val;
		Map val4 = map;
		float explosionRadius = ((ThingDef)def).projectile.explosionRadius;
		DamageDef damageDef = ((ThingDef)def).projectile.damageDef;
		Thing launcher = base.launcher;
		int damageAmount = ((ThingDef)def).projectile.GetDamageAmount(null, (StringBuilder)null);
		SoundDef soundExplode = ((ThingDef)def).projectile.soundExplode;
		ThingDef equipmentDef = base.equipmentDef;
		ThingDef val5 = (ThingDef)(object)def;
		ThingDef postExplosionSpawnThingDef = ((ThingDef)def).projectile.postExplosionSpawnThingDef;
		float postExplosionSpawnChance = ((ThingDef)def).projectile.postExplosionSpawnChance;
		int postExplosionSpawnThingCount = ((ThingDef)def).projectile.postExplosionSpawnThingCount;
		ThingDef preExplosionSpawnThingDef = ((ThingDef)def).projectile.preExplosionSpawnThingDef;
		GenExplosion.DoExplosion(val3, val4, explosionRadius, damageDef, launcher, damageAmount, 0f, soundExplode, equipmentDef, val5, null, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, ((ThingDef)def).projectile.postExplosionGasType, null, 255, ((ThingDef)def).projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, ((ThingDef)def).projectile.preExplosionSpawnChance, ((ThingDef)def).projectile.preExplosionSpawnThingCount, ((ThingDef)def).projectile.explosionChanceToStartFire, ((ThingDef)def).projectile.explosionDamageFalloff, null, null, null, true, 1f, 0f, true, null, 1f, null, null, null, null);
	}

	public override float ArmorPenetration
	{
		get
		{
			return base.ArmorPenetration * _shieldDamageMultiplier;
		}
	}
}
