using RimWorld.BaseGen;
using Verse;

namespace ExtraHives.GenStuff;

internal class SymbolResolver_RandomDamage : SymbolResolver
{
	public override void Resolve(ResolveParams rp)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Map map = BaseGen.globalSettings.map;
		map.listerThings.AllThings.FindAll((Thing t1) => t1.Faction != rp.faction).ForEach(delegate(Thing t)
		{
			Rand.PushState();
			t.HitPoints -= t.HitPoints / Rand.RangeInclusive(3, 10);
			Rand.PopState();
		});
		map.listerThings.AllThings.FindAll((Thing t2) => t2.def.IsMeat || ((Def)t2.def).defName == "Pemmican").ForEach(delegate(Thing t)
		{
			((Entity)t).DeSpawn((DestroyMode)0);
		});
	}
}
