using RimWorld;
using Verse;

namespace ExtraHives;

[DefOf]
public static class ThingDefOf
{
	public static ThingDef Tunneler_ExtraHives;

	public static ThingDef InfestedMeteoriteIncoming_ExtraHives;

	public static ThingDef Hive;

	static ThingDefOf()
	{
		DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
	}
}
