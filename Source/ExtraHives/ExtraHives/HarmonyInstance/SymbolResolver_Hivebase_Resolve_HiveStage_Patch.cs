using ExtraHives.GenStuff;
using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ExtraHives.HarmonyInstance;

[HarmonyPatch(typeof(SymbolResolver_Hivebase), "Resolve")]
public static class SymbolResolver_Hivebase_Resolve_HiveStage_Patch
{
	public static void Postfix(SymbolResolver_Hivebase __instance, ref ResolveParams rp)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		Map map = BaseGen.globalSettings.map;
		if (!((Def)rp.faction.def).HasModExtension<HiveFactionExtension>())
		{
			return;
		}
		float num = 1f;
		HiveFactionEvolutionTracker component = Find.World.GetComponent<HiveFactionEvolutionTracker>();
		HiveFactionExtension modExtension = ((Def)rp.faction.def).GetModExtension<HiveFactionExtension>();
		if (component != null && component.HiveFactionStages.TryGetValue(((object)rp.faction).ToString(), out var _))
		{
			num = modExtension.CurStage.pointMultipler;
		}
		FloatRange defaultPawnsPoints;
		float? settlementPawnGroupPoints;
		if (rp.pawnGroupMakerParams != null)
		{
			PawnGroupMakerParms pawnGroupMakerParams = rp.pawnGroupMakerParams;
			settlementPawnGroupPoints = rp.settlementPawnGroupPoints;
			float num2;
			if (!settlementPawnGroupPoints.HasValue)
			{
				defaultPawnsPoints = SymbolResolver_Settlement.DefaultPawnsPoints;
				num2 = defaultPawnsPoints.RandomInRange;
			}
			else
			{
				num2 = settlementPawnGroupPoints.GetValueOrDefault();
			}
			pawnGroupMakerParams.points = num2 * num;
			return;
		}
		rp.pawnGroupMakerParams = new PawnGroupMakerParms();
		rp.pawnGroupMakerParams.tile = map.Tile;
		rp.pawnGroupMakerParams.faction = rp.faction;
		PawnGroupMakerParms pawnGroupMakerParams2 = rp.pawnGroupMakerParams;
		settlementPawnGroupPoints = rp.settlementPawnGroupPoints;
		float num3;
		if (!settlementPawnGroupPoints.HasValue)
		{
			defaultPawnsPoints = SymbolResolver_Settlement.DefaultPawnsPoints;
			num3 = defaultPawnsPoints.RandomInRange;
		}
		else
		{
			num3 = settlementPawnGroupPoints.GetValueOrDefault();
		}
		pawnGroupMakerParams2.points = num3 * num;
		rp.pawnGroupMakerParams.inhabitants = true;
		rp.pawnGroupMakerParams.seed = rp.settlementPawnGroupSeed;
	}
}
