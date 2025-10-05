using System.Reflection;
using HarmonyLib;
using Verse;

namespace OgsLasers.HarmonyInstance;

[StaticConstructorOnStartup]
public static class HarmonyInstance
{
	static HarmonyInstance()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		Harmony val = new Harmony("com.ogliss.rimworld.mod.OgsLasers");
		val.PatchAll(Assembly.GetExecutingAssembly());
	}
}
