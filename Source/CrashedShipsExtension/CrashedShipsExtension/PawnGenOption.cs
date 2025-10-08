using System;
using System.Xml;
using RimWorld;
using Verse;

namespace CrashedShipsExtension;

public class PawnGenOption
{
	public PawnKindDef kind;

	public float selectionWeight;

	public float Cost => kind.combatPower;

	public PawnGenOption(PawnGenOption opt)
	{
		kind = opt.kind;
		selectionWeight = opt.selectionWeight;
	}

	public PawnGenOption(PawnKindDef kind, float weight)
	{
		this.kind = kind;
		selectionWeight = weight;
	}

	public override string ToString()
	{
		return "(" + ((kind != null) ? ((object)kind).ToString() : "null") + " w=" + selectionWeight.ToString("F2") + " c=" + ((kind != null) ? Cost.ToString("F2") : "null") + ")";
	}

	public void LoadDataFromXmlCustom(XmlNode xmlRoot)
	{
		DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, typeof(PawnGenOption).GetField("kind"), xmlRoot.Name, null, null, null);
		selectionWeight = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
	}
}
