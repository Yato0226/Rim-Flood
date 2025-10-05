using RimWorld;
using Verse;

namespace ExtraHives;

public class CompProperties_SpawnerHives : CompProperties
{
	public ThingDef hiveDef;

	public ThingDef tunnelDef;

	public bool requireRoofed = true;

	public SimpleCurve radiusPerDayCurve;

	public float HiveSpawnRadius = 10f;

	public float HiveSpawnPreferredMinDist = 3.5f;

	public SimpleCurve ReproduceRateFactorFromNearbyHiveCountCurve;

	public FloatRange HiveSpawnIntervalDays = new FloatRange(1f, 2f);

	public CompProperties_SpawnerHives()
	{
		((CompProperties)this).compClass = typeof(CompSpawnerHives);
	}
}
