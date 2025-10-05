using RimWorld;

namespace ExtraHives;

[DefOf]
public static class PawnsArrivalModeDefOf
{
	public static PawnsArrivalModeDef EdgeWalkInGroups;

	public static PawnsArrivalModeDef EdgeDropGroups;

	public static PawnsArrivalModeDef EdgeTunnelIn_ExtraHives;

	public static PawnsArrivalModeDef EdgeTunnelInGroups_ExtraHives;

	public static PawnsArrivalModeDef CenterTunnelIn_ExtraHives;

	public static PawnsArrivalModeDef RandomTunnelIn_ExtraHives;

	static PawnsArrivalModeDefOf()
	{
		DefOfHelper.EnsureInitializedInCtor(typeof(PawnsArrivalModeDefOf));
	}
}
