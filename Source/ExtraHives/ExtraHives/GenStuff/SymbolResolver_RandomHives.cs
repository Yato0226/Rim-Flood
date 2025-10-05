using System.Collections.Generic;
using ExtraHives.ExtensionMethods;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ExtraHives.GenStuff;

internal class SymbolResolver_RandomHives : SymbolResolver
{
	public override void Resolve(ResolveParams rp)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		Map map = BaseGen.globalSettings.map;
		Rand.PushState();
		for (int i = 0; i < Rand.RangeInclusive(5, 20); i++)
		{
			Faction faction = rp.faction;
			IntVec3 randomCell = rp.rect.RandomCell;
			if (!GenGrid.Standable(randomCell, map) || GridsUtility.GetFirstItem(randomCell, map) != null || GridsUtility.GetFirstPawn(randomCell, map) != null || GridsUtility.GetFirstBuilding(randomCell, map) != null)
			{
				continue;
			}
			if (Rand.RangeInclusive(1, 4) < 3)
			{
				ThingDef val = GenCollection.RandomElement<ThingDef>((IEnumerable<ThingDef>)faction.HivedefsFor()) ?? ThingDefOf.Hive;
				Hive hive = (Hive)(object)GenSpawn.Spawn(ThingMaker.MakeThing(val, (ThingDef)null), randomCell, map, (WipeMode)0);
				((Thing)hive).SetFaction(faction, (Pawn)null);
				foreach (CompSpawner comp in ((ThingWithComps)hive).GetComps<CompSpawner>())
				{
					if (comp.PropsSpawner.thingToSpawn == DefDatabase<ThingDef>.GetNamed("InsectJelly"))
					{
						comp.TryDoSpawn();
						break;
					}
				}
				continue;
			}
			ThingDef val2 = GenCollection.RandomElement<ThingDef>((IEnumerable<ThingDef>)faction.HivedefsFor()) ?? ThingDefOf.Hive;
			Hive hive2 = (Hive)(object)GenSpawn.Spawn(ThingMaker.MakeThing(val2, (ThingDef)null), randomCell, map, (WipeMode)0);
			((Thing)hive2).SetFaction(faction, (Pawn)null);
			foreach (CompSpawner comp2 in ((ThingWithComps)hive2).GetComps<CompSpawner>())
			{
				if (comp2.PropsSpawner.thingToSpawn == DefDatabase<ThingDef>.GetNamed("InsectJelly"))
				{
					comp2.TryDoSpawn();
					break;
				}
			}
		}
		Rand.PopState();
	}
}
