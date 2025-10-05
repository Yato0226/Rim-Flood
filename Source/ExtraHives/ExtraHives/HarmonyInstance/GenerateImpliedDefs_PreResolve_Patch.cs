using System.Collections.Generic;
using ExtraHives.GenStuff;
using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using Verse;

namespace ExtraHives.HarmonyInstance;

[HarmonyPatch(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve", null)]
public class GenerateImpliedDefs_PreResolve_Patch
{
	public static void Postfix()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected O, but got Unknown
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Expected O, but got Unknown
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Expected O, but got Unknown
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Expected O, but got Unknown
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Expected O, but got Unknown
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Expected O, but got Unknown
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Expected O, but got Unknown
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Expected O, but got Unknown
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Expected O, but got Unknown
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Expected O, but got Unknown
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Expected O, but got Unknown
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Expected O, but got Unknown
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Expected O, but got Unknown
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Expected O, but got Unknown
		PawnGroupKindDef val = new PawnGroupKindDef();
		((Def)val).defName = "Hive_ExtraHives";
		val.workerClass = typeof(PawnGroupKindWorker_Normal);
		DefGenerator.AddImpliedDef<PawnGroupKindDef>(val, false);
		val = new PawnGroupKindDef();
		((Def)val).defName = "Tunneler_ExtraHives";
		val.workerClass = typeof(PawnGroupKindWorker_Normal);
		DefGenerator.AddImpliedDef<PawnGroupKindDef>(val, false);
		PawnsArrivalModeDef val2 = new PawnsArrivalModeDef();
		((Def)val2).defName = "EdgeTunnelIn_ExtraHives";
		val2.textEnemy = "A group of {0} from {1} have tunneled in nearby.";
		val2.textFriendly = "A group of friendly {0} from {1} have tunneled in nearby.";
		val2.textWillArrive = "{0_pawnsPluralDef} will tunnel in.";
		val2.workerClass = typeof(PawnsArrivalModeWorker_EdgeTunnel);
		DefGenerator.AddImpliedDef<PawnsArrivalModeDef>(val2, false);
		val2 = new PawnsArrivalModeDef();
		((Def)val2).defName = "EdgeTunnelInGroups_ExtraHives";
		val2.textEnemy = "Several separate groups of {0} from {1} have tunneled in nearby.";
		val2.textFriendly = "Several separate groups of friendly {0} from {1} have tunneled in nearby.";
		val2.textWillArrive = "Several separate groups of {0_pawnsPluralDef} will tunnel in.";
		val2.workerClass = typeof(PawnsArrivalModeWorker_EdgeTunnelGroups);
		DefGenerator.AddImpliedDef<PawnsArrivalModeDef>(val2, false);
		val2 = new PawnsArrivalModeDef();
		((Def)val2).defName = "CenterTunnelIn_ExtraHives";
		val2.textEnemy = "A group of {0} from {1} have tunneled in right on top of you!";
		val2.textFriendly = "A group of friendly {0} from {1} have tunneled in right on top of you!";
		val2.textWillArrive = "{0_pawnsPluralDef} will tunnel in right on top of you.";
		val2.workerClass = typeof(PawnsArrivalModeWorker_CenterTunnel);
		DefGenerator.AddImpliedDef<PawnsArrivalModeDef>(val2, false);
		val2 = new PawnsArrivalModeDef();
		((Def)val2).defName = "RandomTunnelIn_ExtraHives";
		val2.textEnemy = "A group of {0} from {1} have tunneled in. They are scattered all over the area!";
		val2.textFriendly = "A group of friendly {0} from {1} have tunneled in. They are scattered all over the area!";
		val2.textWillArrive = "{0_pawnsPluralDef} will tunnel in.";
		val2.workerClass = typeof(PawnsArrivalModeWorker_RandomTunnel);
		DefGenerator.AddImpliedDef<PawnsArrivalModeDef>(val2, false);
		ThingDef val3 = new ThingDef();
		((Def)val3).defName = "InfestedMeteoriteIncoming_ExtraHives";
		val3.category = (ThingCategory)10;
		val3.thingClass = typeof(Skyfaller);
		val3.useHitPoints = false;
		val3.drawOffscreen = true;
		val3.tickerType = (TickerType)1;
		((BuildableDef)val3).altitudeLayer = (AltitudeLayer)28;
		val3.drawerType = (DrawerType)1;
		val3.skyfaller = new SkyfallerProperties();
		((Def)val3).label = "meteorite (incoming)";
		val3.size = new IntVec2(3, 3);
		val3.graphicData = new GraphicData();
		val3.graphicData.texPath = "Things/Skyfaller/Meteorite";
		val3.graphicData.graphicClass = typeof(Graphic_Single);
		val3.graphicData.shaderType = ShaderTypeDefOf.Transparent;
		val3.graphicData.drawSize = new Vector2(10f, 10f);
		val3.skyfaller.shadowSize = new Vector2(3f, 3f);
		val3.skyfaller.explosionRadius = 4f;
		val3.skyfaller.explosionDamage = DamageDefOf.Bomb;
		val3.skyfaller.rotateGraphicTowardsDirection = true;
		val3.skyfaller.speed = 1.2f;
		DefGenerator.AddImpliedDef<ThingDef>(val3, false);
		val3 = new ThingDef();
		((Def)val3).defName = "Tunneler_ExtraHives";
		val3.category = (ThingCategory)10;
		val3.thingClass = typeof(TunnelRaidSpawner);
		val3.useHitPoints = false;
		val3.drawOffscreen = true;
		val3.alwaysFlee = true;
		val3.tickerType = (TickerType)1;
		((BuildableDef)val3).altitudeLayer = (AltitudeLayer)28;
		val3.drawerType = (DrawerType)1;
		((Def)val3).label = "tunnel (incoming)";
		val3.size = new IntVec2(1, 1);
		DefGenerator.AddImpliedDef<ThingDef>(val3, false);
		RuleDef val4 = new RuleDef();
		((Def)val4).defName = "ExtraHives_HiveBaseMaker";
		val4.symbol = "ExtraHives_HiveBaseMaker";
		val4.resolvers = new List<SymbolResolver>();
		val4.resolvers.Add((SymbolResolver)(object)new SymbolResolver_Hivebase());
		DefGenerator.AddImpliedDef<RuleDef>(val4, false);
		val4 = new RuleDef();
		((Def)val4).defName = "ExtraHives_HiveMoundMaker";
		val4.symbol = "ExtraHives_HiveMoundMaker";
		val4.resolvers = new List<SymbolResolver>();
		val4.resolvers.Add((SymbolResolver)(object)new SymbolResolver_HiveBaseMoundMaker());
		DefGenerator.AddImpliedDef<RuleDef>(val4, false);
		val4 = new RuleDef();
		((Def)val4).defName = "ExtraHives_HiveClearChamber";
		val4.symbol = "ExtraHives_HiveClearChamber";
		val4.resolvers = new List<SymbolResolver>();
		val4.resolvers.Add((SymbolResolver)(object)new SymbolResolver_ClearChamber());
		DefGenerator.AddImpliedDef<RuleDef>(val4, false);
		val4 = new RuleDef();
		((Def)val4).defName = "ExtraHives_HiveInterals";
		val4.symbol = "ExtraHives_HiveInterals";
		val4.resolvers = new List<SymbolResolver>();
		val4.resolvers.Add((SymbolResolver)(object)new SymbolResolver_HiveInternals());
		DefGenerator.AddImpliedDef<RuleDef>(val4, false);
		val4 = new RuleDef();
		((Def)val4).defName = "ExtraHives_HiveOutdoorLighting";
		val4.symbol = "ExtraHives_HiveOutdoorLighting";
		val4.resolvers = new List<SymbolResolver>();
		val4.resolvers.Add((SymbolResolver)(object)new SymbolResolver_OutdoorLightingHivebase());
		DefGenerator.AddImpliedDef<RuleDef>(val4, false);
		val4 = new RuleDef();
		((Def)val4).defName = "ExtraHives_HiveRandomCorpse";
		val4.symbol = "ExtraHives_HiveRandomCorpse";
		val4.resolvers = new List<SymbolResolver>();
		val4.resolvers.Add((SymbolResolver)(object)new SymbolResolver_RandomCorpse());
		DefGenerator.AddImpliedDef<RuleDef>(val4, false);
		val4 = new RuleDef();
		((Def)val4).defName = "ExtraHives_HiveRandomDamage";
		val4.symbol = "ExtraHives_HiveRandomDamage";
		val4.resolvers = new List<SymbolResolver>();
		val4.resolvers.Add((SymbolResolver)(object)new SymbolResolver_RandomDamage());
		DefGenerator.AddImpliedDef<RuleDef>(val4, false);
		val4 = new RuleDef();
		((Def)val4).defName = "ExtraHives_HiveRandomHives";
		val4.symbol = "ExtraHives_HiveRandomHives";
		val4.resolvers = new List<SymbolResolver>();
		val4.resolvers.Add((SymbolResolver)(object)new SymbolResolver_RandomHives());
		DefGenerator.AddImpliedDef<RuleDef>(val4, false);
		val4 = new RuleDef();
		((Def)val4).defName = "ExtraHives_PawnGroup";
		val4.symbol = "ExtraHives_PawnGroup";
		val4.resolvers = new List<SymbolResolver>();
		val4.resolvers.Add((SymbolResolver)(object)new SymbolResolver_PawnHiveGroup());
		DefGenerator.AddImpliedDef<RuleDef>(val4, false);
		val4 = new RuleDef();
		((Def)val4).defName = "ExtraHives_Pawn";
		val4.symbol = "ExtraHives_Pawn";
		val4.resolvers = new List<SymbolResolver>();
		val4.resolvers.Add((SymbolResolver)(object)new SymbolResolver_SingleHivePawn());
		DefGenerator.AddImpliedDef<RuleDef>(val4, false);
	}
}
