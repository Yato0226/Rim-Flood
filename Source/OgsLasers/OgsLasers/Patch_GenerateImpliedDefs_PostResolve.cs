using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace OgsLasers;

[HarmonyPatch(typeof(DefGenerator), "GenerateImpliedDefs_PostResolve", null)]
public class Patch_GenerateImpliedDefs_PostResolve
{
	public static void Postfix()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Expected O, but got Unknown
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Expected O, but got Unknown
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Expected O, but got Unknown
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Expected O, but got Unknown
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Expected O, but got Unknown
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Expected O, but got Unknown
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		if (DefDatabase<ThingDef>.GetNamedSilentFail("Mote_BloodPuff") == null)
		{
			ThingDef val = new ThingDef();
			((Def)val).defName = "Mote_BloodPuff";
			((Def)val).label = "blood";
			val.category = (ThingCategory)9;
			val.thingClass = typeof(Graphic_Mote);
			val.graphicData = new GraphicData();
			val.graphicData.texPath = "Things/Mote/BodyImpact";
			val.graphicData.shaderType = DefDatabase<ShaderTypeDef>.GetNamed("Mote", true);
			((BuildableDef)val).altitudeLayer = (AltitudeLayer)26;
			val.tickerType = (TickerType)1;
			val.useHitPoints = false;
			val.isSaveable = false;
			val.rotatable = false;
			val.mote = new MoteProperties();
			val.mote.fadeInTime = 0.04f;
			val.mote.solidTime = 0.25f;
			val.mote.fadeOutTime = 0.1f;
			DefGenerator.AddImpliedDef<ThingDef>(val, false);
		}
		if (DefDatabase<ThingDef>.GetNamedSilentFail("Mote_Blood_Puff") == null)
		{
			ThingDef val = new ThingDef();
			((Def)val).defName = "Mote_Blood_Puff";
			((Def)val).label = "blood";
			val.category = (ThingCategory)9;
			val.thingClass = typeof(Graphic_Mote);
			val.graphicData = new GraphicData();
			val.graphicData.texPath = "Things/Mote/BodyImpact";
			val.graphicData.shaderType = DefDatabase<ShaderTypeDef>.GetNamed("Mote", true);
			((BuildableDef)val).altitudeLayer = (AltitudeLayer)26;
			val.tickerType = (TickerType)1;
			val.useHitPoints = false;
			val.isSaveable = false;
			val.rotatable = false;
			val.mote = new MoteProperties();
			val.mote.fadeInTime = 0.04f;
			val.mote.solidTime = 0.25f;
			val.mote.fadeOutTime = 0.1f;
			DefGenerator.AddImpliedDef<ThingDef>(val, false);
		}
		if (DefDatabase<EffecterDef>.GetNamedSilentFail("LaserImpact") == null)
		{
			EffecterDef val2 = new EffecterDef();
			((Def)val2).defName = "LaserImpact";
			((Def)val2).label = "laser impact";
			val2.children = new List<SubEffecterDef>();
			List<SubEffecterDef> children = val2.children;
			SubEffecterDef val3 = new SubEffecterDef();
			val3.subEffecterClass = typeof(SubEffecter_SprayerTriggered);
			val3.moteDef = ThingDef.Named("Mote_SparkFlash");
			val3.positionLerpFactor = 0.6f;
			val3.chancePerTick = 0.2f;
			val3.scale = new FloatRange(2.5f, 4.5f);
			val3.spawnLocType = (MoteSpawnLocType)0;
			children.Add(val3);
			List<SubEffecterDef> children2 = val2.children;
			val3 = new SubEffecterDef();
			val3.subEffecterClass = typeof(SubEffecter_SprayerTriggered);
			val3.positionRadius = 0.2f;
			val3.moteDef = ThingDef.Named("Mote_AirPuff");
			val3.burstCount = new IntRange(4, 5);
			val3.speed = new FloatRange(0.4f, 0.8f);
			val3.scale = new FloatRange(0.5f, 0.8f);
			val3.spawnLocType = (MoteSpawnLocType)0;
			children2.Add(val3);
			List<SubEffecterDef> children3 = val2.children;
			val3 = new SubEffecterDef();
			val3.subEffecterClass = typeof(SubEffecter_SprayerTriggered);
			val3.positionRadius = 0.2f;
			val3.moteDef = ThingDef.Named("Mote_SparkThrownFast");
			val3.burstCount = new IntRange(4, 5);
			val3.speed = new FloatRange(3.3f, 5f);
			val3.scale = new FloatRange(0.1f, 0.2f);
			val3.spawnLocType = (MoteSpawnLocType)0;
			children3.Add(val3);
			List<SubEffecterDef> children4 = val2.children;
			val3 = new SubEffecterDef();
			val3.subEffecterClass = typeof(SubEffecter_SprayerTriggered);
			val3.positionRadius = 0.2f;
			val3.moteDef = ThingDef.Named("Mote_MicroSparksFast");
			val3.burstCount = new IntRange(1, 1);
			val3.speed = new FloatRange(0.3f, 0.4f);
			val3.rotationRate = new FloatRange(5f, 10f);
			val3.scale = new FloatRange(0.3f, 0.5f);
			val3.spawnLocType = (MoteSpawnLocType)0;
			children4.Add(val3);
			List<SubEffecterDef> children5 = val2.children;
			val3 = new SubEffecterDef();
			val3.subEffecterClass = typeof(SubEffecter_SprayerTriggered);
			val3.positionRadius = 0.1f;
			val3.moteDef = ThingDef.Named("Mote_SparkFlash");
			val3.burstCount = new IntRange(1, 1);
			val3.scale = new FloatRange(0.9f, 1.3f);
			val3.spawnLocType = (MoteSpawnLocType)0;
			children5.Add(val3);
			val2.offsetTowardsTarget = default(FloatRange);
			val2.positionRadius = 0.01f;
			DefGenerator.AddImpliedDef<EffecterDef>(val2, false);
		}
	}
}
