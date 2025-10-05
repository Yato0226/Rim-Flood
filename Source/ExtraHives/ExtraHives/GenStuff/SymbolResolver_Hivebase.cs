using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using Verse;
using Verse.AI.Group;

namespace ExtraHives.GenStuff;

public class SymbolResolver_Hivebase : SymbolResolver
{
	public override void Resolve(ResolveParams rp)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Invalid comparison between Unknown and I4
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Expected O, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		Map map = BaseGen.globalSettings.map;
		Faction parentFaction = map.ParentFaction;
		int num = 0;
		if (rp.edgeDefenseWidth.HasValue)
		{
			num = rp.edgeDefenseWidth.Value;
		}
		else
		{
			Rand.PushState();
			bool flag = rp.rect.Width >= 20 && rp.rect.Height >= 20 && ((int)parentFaction.def.techLevel >= 4 || Rand.Bool);
			Rand.PopState();
			if (flag)
			{
				Rand.PushState();
				num = (Rand.Bool ? 2 : 4);
				Rand.PopState();
			}
		}
		float num2 = (float)rp.rect.Area / 144f * 0.17f;
		BaseGen.globalSettings.minEmptyNodes = ((!(num2 < 1f)) ? GenMath.RoundRandom(num2) : 0);
		Lord singlePawnLord = rp.singlePawnLord ?? LordMaker.MakeNewLord(parentFaction, (LordJob)(object)new LordJob_DefendHiveBase(parentFaction, GenCollection.RandomElement<IntVec3>(from x in GenRadial.RadialCellsAround(rp.rect.CenterCell, 5f, true)
			where GenGrid.Walkable(x, map)
			select x)), map, (IEnumerable<Pawn>)null);
		TraverseParms traverseParms = TraverseParms.For((TraverseMode)1, (Danger)3, false, false, false);
		ResolveParams val = rp;
		val.rect = rp.rect;
		val.faction = parentFaction;
		val.singlePawnLord = singlePawnLord;
		val.pawnGroupKindDef = rp.pawnGroupKindDef ?? RimWorld.PawnGroupKindDefOf.Settlement;
		val.singlePawnSpawnCellExtraPredicate = rp.singlePawnSpawnCellExtraPredicate ?? ((Predicate<IntVec3>)((IntVec3 x) => map.reachability.CanReachMapEdge(x, traverseParms)));
		if (val.pawnGroupMakerParams == null)
		{
			val.pawnGroupMakerParams = new PawnGroupMakerParms();
			val.pawnGroupMakerParams.tile = map.Tile;
			val.pawnGroupMakerParams.faction = parentFaction;
			PawnGroupMakerParms pawnGroupMakerParams = val.pawnGroupMakerParams;
			float? settlementPawnGroupPoints = rp.settlementPawnGroupPoints;
			float points;
			if (!settlementPawnGroupPoints.HasValue)
			{
				FloatRange defaultPawnsPoints = SymbolResolver_Settlement.DefaultPawnsPoints;
				points = defaultPawnsPoints.RandomInRange;
			}
			else
			{
				points = settlementPawnGroupPoints.GetValueOrDefault();
			}
			pawnGroupMakerParams.points = points;
			val.pawnGroupMakerParams.inhabitants = true;
			val.pawnGroupMakerParams.groupKind = RimWorld.PawnGroupKindDefOf.Settlement;
			val.pawnGroupMakerParams.seed = rp.settlementPawnGroupSeed;
		}
		BaseGen.symbolStack.Push("ExtraHives_PawnGroup", val, (string)null);
		PawnGenerationRequest value = default(PawnGenerationRequest);
		value = new PawnGenerationRequest(GenCollection.RandomElementByWeight<PawnGenOption>((IEnumerable<PawnGenOption>)GenCollection.RandomElement<PawnGroupMaker>(parentFaction.def.pawnGroupMakers.Where((PawnGroupMaker x) => x.kindDef == PawnGroupKindDefOf.Hive_ExtraHives || x.kindDef == RimWorld.PawnGroupKindDefOf.Combat)).options, (Func<PawnGenOption, float>)((PawnGenOption x) => x.Cost)).kind, parentFaction, (PawnGenerationContext)2, -1, false, false, false, false, true, 1f, false, true, true, true, false, false, false, false, false, 0f, 0f, (Pawn)null, 1f, (Predicate<Pawn>)null, (Predicate<Pawn>)null, (IEnumerable<TraitDef>)null, (IEnumerable<TraitDef>)null, (float?)null, (float?)null, (float?)null, (Gender?)null, (string)null, (string)null, (RoyalTitleDef)null, (Ideo)null, false, false, false, false, (List<GeneDef>)null, (List<GeneDef>)null, (XenotypeDef)null, (CustomXenotype)null, (List<XenotypeDef>)null, 0f, (DevelopmentalStage)8, (Func<XenotypeDef, PawnKindDef>)null, (FloatRange?)null, (FloatRange?)null, false, false, false, -1, 0, false);
		ResolveParams val2 = rp;
		val2.faction = parentFaction;
		val2.singlePawnGenerationRequest = value;
		val2.rect = rp.rect;
		val2.singlePawnLord = singlePawnLord;
		BaseGen.symbolStack.Push("ExtraHives_Pawn", val2, (string)null);
		ResolveParams val3 = rp;
		val3.rect = rp.rect.ContractedBy(num);
		val3.faction = parentFaction;
		BaseGen.symbolStack.Push("ensureCanReachMapEdge", val3, (string)null);
		ResolveParams val4 = rp;
		BaseGen.symbolStack.Push("ExtraHives_HiveRandomCorpse", rp, (string)null);
		ResolveParams val5 = rp;
		val5.rect = rp.rect.ContractedBy(num);
		val5.faction = parentFaction;
		val5.floorOnlyIfTerrainSupports = rp.floorOnlyIfTerrainSupports ?? true;
		val5.wallStuff = rp.wallStuff ?? BaseGenUtility.RandomCheapWallStuff(rp.faction, true);
		val5.chanceToSkipWallBlock = rp.chanceToSkipWallBlock ?? 0.1f;
		val5.clearEdificeOnly = rp.clearEdificeOnly ?? true;
		val5.noRoof = rp.noRoof ?? true;
		val5.chanceToSkipFloor = rp.chanceToSkipFloor ?? 0.1f;
		val5.filthDef = DefDatabase<ThingDef>.GetNamed("Filth_Slime");
		val5.filthDensity = new FloatRange(0.5f, 1f);
		val5.cultivatedPlantDef = null;
		BaseGen.symbolStack.Push("ExtraHives_HiveInterals", val5, (string)null);
		BaseGen.symbolStack.Push("ExtraHives_HiveMoundMaker", val5, (string)null);
	}
}
