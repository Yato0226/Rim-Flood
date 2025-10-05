using RimWorld;
using Verse;

namespace ExtraHives
{
    [DefOf]
    public static class FloodDefOf
    {
        public static ThingDef Floodtunnel;

        static FloodDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FloodDefOf));
        }
    }
}