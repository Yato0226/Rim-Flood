using System.Collections.Generic;
using UnityEngine;

namespace OgsLasers;

public static class MeshMakerLaser
{
	private static int textureSeamPrecision = 256;

	private static int geometrySeamPrecision = 512;

	private static Dictionary<int, Mesh> cachedMeshes = new Dictionary<int, Mesh>();

	public static Mesh Mesh(float st, float sv)
	{
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Expected O, but got Unknown
		if (st < 0f)
		{
			st = 0f;
		}
		if (st > 0.5f)
		{
			st = 0.5f;
		}
		if (sv < 0f)
		{
			sv = 0f;
		}
		if (sv > 0.5f)
		{
			sv = 0.5f;
		}
		int num = (int)(st / 0.5f * (float)textureSeamPrecision);
		int num2 = (int)(sv / 0.5f * (float)geometrySeamPrecision);
		int key = num2 + (textureSeamPrecision + 1) * geometrySeamPrecision;
		if (cachedMeshes.TryGetValue(key, out var value))
		{
			return value;
		}
		st = 0.5f * (float)num / (float)textureSeamPrecision;
		sv = 0.5f * (float)num2 / (float)geometrySeamPrecision;
		float num3 = 1f - st;
		float num4 = 0.5f - sv;
		Vector3[] vertices = (Vector3[])(object)new Vector3[8]
		{
			new Vector3(-0.5f, 0f, -0.5f),
			new Vector3(-0.5f, 0f, 0f - num4),
			new Vector3(0.5f, 0f, 0f - num4),
			new Vector3(0.5f, 0f, -0.5f),
			new Vector3(-0.5f, 0f, num4),
			new Vector3(0.5f, 0f, num4),
			new Vector3(-0.5f, 0f, 0.5f),
			new Vector3(0.5f, 0f, 0.5f)
		};
		Vector2[] uv = (Vector2[])(object)new Vector2[8]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, st),
			new Vector2(1f, st),
			new Vector2(1f, 0f),
			new Vector2(0f, num3),
			new Vector2(1f, num3),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		int[] array = new int[18]
		{
			0, 1, 2, 0, 2, 3, 1, 4, 5, 1,
			5, 2, 4, 6, 7, 4, 7, 5
		};
		value = new Mesh();
		((Object)value).name = "NewLaserMesh()";
		value.vertices = vertices;
		value.uv = uv;
		value.SetTriangles(array, 0);
		value.RecalculateNormals();
		value.RecalculateBounds();
		cachedMeshes[key] = value;
		return value;
	}
}
