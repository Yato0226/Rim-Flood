using System.Reflection;
using HarmonyLib;
using Verse;

namespace ExtraHives.HarmonyInstance;

[StaticConstructorOnStartup]
public class MainHarmonyInstance : Mod
{
	public MainHarmonyInstance(ModContentPack content)
		: base(content)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		Harmony val = new Harmony("com.ogliss.rimworld.mod.ExtraHives");
		val.PatchAll(Assembly.GetExecutingAssembly());
	}
}
