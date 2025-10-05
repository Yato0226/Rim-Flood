using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ExtraHives.GenStuff;

public class SymbolResolver_PawnHiveGroup : SymbolResolver
{
	private const float DefaultPoints = 250f;

	public override bool CanResolve(ResolveParams rp)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (!((SymbolResolver)this).CanResolve(rp))
		{
			return false;
		}
		return rp.rect.Cells.Where((IntVec3 x) => GenGrid.Standable(x, BaseGen.globalSettings.map)).Any();
	}

	public override void Resolve(ResolveParams rp)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		Map map = BaseGen.globalSettings.map;
		PawnGroupMakerParms val = rp.pawnGroupMakerParams;
		if (val == null)
		{
			val = new PawnGroupMakerParms();
			val.tile = map.Tile;
			val.faction = Find.FactionManager.RandomEnemyFaction(false, false, true, (TechLevel)0);
			val.points = 250f;
		}
		val.groupKind = rp.pawnGroupKindDef ?? RimWorld.PawnGroupKindDefOf.Combat;
		List<PawnKindDef> list = new List<PawnKindDef>();
		foreach (Pawn item in PawnGroupMakerUtility.GeneratePawns(val, true))
		{
			list.Add(item.kindDef);
			ResolveParams val2 = rp;
			val2.singlePawnToSpawn = item;
			BaseGen.symbolStack.Push("ExtraHives_Pawn", val2, (string)null);
		}
	}
}
