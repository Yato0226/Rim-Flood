using RimWorld;

namespace CrashedShipsExtension;

[DefOf]
public static class FactionDefOf
{
	public static FactionDef Pirate;

	static FactionDefOf()
	{
		DefOfHelper.EnsureInitializedInCtor(typeof(FactionDefOf));
	}
}
