using RimWorld;

namespace ExtraHives;

[DefOf]
public static class PawnGroupKindDefOf
{
	public static PawnGroupKindDef Hive_ExtraHives;

	public static PawnGroupKindDef Tunneler_ExtraHives;

	static PawnGroupKindDefOf()
	{
		DefOfHelper.EnsureInitializedInCtor(typeof(PawnGroupKindDefOf));
	}
}
