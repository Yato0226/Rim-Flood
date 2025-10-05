using System.Runtime.CompilerServices;
using Verse;
using Verse.Grammar;

namespace ExtraHives;

public class Rule_Ordinal_Number : Rule
{
	private IntRange range = IntRange.Zero;

	public int selectionWeight = 1;

	public override float BaseSelectionWeight => selectionWeight;

	public override Rule DeepCopy()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Rule_Ordinal_Number rule_Ordinal_Number = (Rule_Ordinal_Number)(object)((Rule)this).DeepCopy();
		rule_Ordinal_Number.range = range;
		rule_Ordinal_Number.selectionWeight = selectionWeight;
		return (Rule)(object)rule_Ordinal_Number;
	}

	public override string Generate()
	{
		return AddOrdinal(range.RandomInRange);
	}

	public static string AddOrdinal(int num)
	{
		if (num <= 0)
		{
			return num.ToString();
		}
		int num2 = num % 100;
		int num3 = num2;
		if ((uint)(num3 - 11) <= 2u)
		{
			return num + "th";
		}
		return (num % 10) switch
		{
			1 => num + "st", 
			2 => num + "nd", 
			3 => num + "rd", 
			_ => num + "th", 
		};
	}

	public override string ToString()
	{
		return base.keyword + "->(number: " + range.ToString() + ")";
	}
}
