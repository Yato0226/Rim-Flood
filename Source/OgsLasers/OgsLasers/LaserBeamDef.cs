using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace OgsLasers;

public class LaserBeamDef : ThingDef
{
	public float capSize = 1f;

	public float capOverlap = 0.0171875f;

	public int lifetime = 30;

	public int flickerFrameTime = 5;

	public float impulse = 4f;

	public float beamWidth = 1f;

	public float shieldDamageMultiplier = 0.5f;

	public float seam = -1f;

	public float causefireChance = -1f;

	public bool canExplode = false;

	public List<LaserBeamDecoration> decorations;

	public EffecterDef explosionEffect;

	public EffecterDef hitLivingEffect;

	public ThingDef beamGraphic;

	public List<string> textures;

	public List<Material> materials = new List<Material>();

	public bool IsWeakToShields => shieldDamageMultiplier < 1f;

	private void CreateGraphics()
	{
		if (base.graphicData.graphicClass == typeof(Graphic_Random) || base.graphicData.graphicClass == typeof(Graphic_Flicker))
		{
			for (int i = 0; i < textures.Count; i++)
			{
				List<Texture2D> list = (from x in ContentFinder<Texture2D>.GetAllInFolder(textures[i])
					where !((Object)x).name.EndsWith(Graphic_Single.MaskSuffix)
					orderby ((Object)x).name
					select x).ToList();
				if (GenList.NullOrEmpty<Texture2D>((IList<Texture2D>)list))
				{
					Log.Error("Collection cannot init: No textures found at path " + textures[i]);
				}
				for (int num = 0; num < list.Count; num++)
				{
					materials.Add(MaterialPool.MatFrom(textures[i] + "/" + ((Object)list[num]).name, ShaderDatabase.TransparentPostLight));
				}
			}
		}
		else
		{
			for (int num2 = 0; num2 < textures.Count; num2++)
			{
				materials.Add(MaterialPool.MatFrom(textures[num2], ShaderDatabase.TransparentPostLight));
			}
		}
	}

	public Material GetBeamMaterial(int index)
	{
		if (materials.Count == 0 && textures.Count != 0)
		{
			CreateGraphics();
		}
		if (materials.Count == 0)
		{
			return null;
		}
		if (index >= materials.Count || index < 0)
		{
			index = 0;
		}
		return materials[index];
	}
}
