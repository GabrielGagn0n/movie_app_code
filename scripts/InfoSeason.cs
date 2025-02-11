using Godot;
using System;
using System.Linq;

public partial class InfoSeason : Control
{
	InfoSeasonButtons infoSeasonButtonsTemplate;
	InfoSeasonButtons[] infoSeasonButtonsList = Array.Empty<InfoSeasonButtons>();
	LineEdit titleLEdit;
	VBoxContainer vBoxInfoSeasons;

	public override void _Ready()
	{
		vBoxInfoSeasons = GetNode<VBoxContainer>("MCont/VBoxCont/SCont/VBCInfoSeason");
		infoSeasonButtonsTemplate = vBoxInfoSeasons.GetNode<InfoSeasonButtons>("InfoSeasonButtons");
		titleLEdit = GetNode<LineEdit>("MCont/VBoxCont/HBCTitle/LEditNbrSeason");

		AddNewSeason(1, 1);
	}

	public int[] GetNbrEpiSeason()
	{
		int[] toReturn = Array.Empty<int>();
		
		foreach (InfoSeasonButtons infoSeasonButton in infoSeasonButtonsList)
		{
			toReturn = toReturn.Append(infoSeasonButton.GetNbrEpi()).ToArray();
		}

		return toReturn;
	}

	private void AddNewSeason(int season, int episodes)
	{
		InfoSeasonButtons newInfo = (InfoSeasonButtons)infoSeasonButtonsTemplate.Duplicate();
		newInfo._Ready();
		newInfo.Setup(season, episodes);
		newInfo.Name = season + "infoSeason";
		infoSeasonButtonsList = infoSeasonButtonsList.Append(newInfo).ToArray();
		vBoxInfoSeasons.AddChild(newInfo);
		newInfo.Visible = true;
	}

	private void RemoveLastSeason()
	{
    	if (infoSeasonButtonsList.Length > 1)
    	{
    	    string lastSeasonName = infoSeasonButtonsList[^1].GetSeason() + "infoSeason";

    	    Node nodeToRemove = vBoxInfoSeasons.GetChildren()
    	        .FirstOrDefault(node => node.Name == lastSeasonName);

    	    if (nodeToRemove != null)
    	    {
    	        nodeToRemove.QueueFree();
    	    }
    	    infoSeasonButtonsList = infoSeasonButtonsList.Take(infoSeasonButtonsList.Length - 1).ToArray();
    	}
	}

	private void _on_btn_add_pressed()
	{
		AddNewSeason(infoSeasonButtonsList.Length + 1, 1);
		titleLEdit.Text = infoSeasonButtonsList.Length.ToString();
	}

	private void _on_btn_remove_pressed()
	{
		RemoveLastSeason();
		titleLEdit.Text = infoSeasonButtonsList.Length.ToString();
	}
}

