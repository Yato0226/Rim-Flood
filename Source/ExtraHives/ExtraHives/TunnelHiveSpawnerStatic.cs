using UnityEngine;
using Verse;

namespace ExtraHives;

[StaticConstructorOnStartup]
public static class TunnelHiveSpawnerStatic
{
	public static readonly Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt", -1);

	public static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();

	public static readonly Material TunnelMaterial = MaterialPool.MatFrom("Things/Filth/Grainy/GrainyA", ShaderDatabase.Transparent);
}
