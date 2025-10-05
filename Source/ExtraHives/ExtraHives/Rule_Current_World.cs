using Verse;
using Verse.Grammar;

namespace ExtraHives;

public class Rule_Current_World : Rule
{
	public override float BaseSelectionWeight => 1f;

	public override Rule DeepCopy()
	{
		return (Rule)(object)(Rule_Current_World)(object)((Rule)this).DeepCopy();
	}

	public override string Generate()
	{
		return Find.World.info.name;
	}

	public override string ToString()
	{
		return base.keyword + "->(WorldName: " + Find.World.info.name + ")";
	}
}
