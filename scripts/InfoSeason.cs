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
		infoSeasonButtonsTemplate = vBoxInfoSeasons.GetNode<InfoSeasonButtons>("InfoSeasonButtonsTemplate");
		titleLEdit = GetNode<LineEdit>("MCont/VBoxCont/HBCTitle/LEditNbrSeason");

		AddNewSeason(1, 1);
	}	

    internal void Setup(Serial serial)
    {
	    infoSeasonButtonsList = Array.Empty<InfoSeasonButtons>();
        RemoveAllSeason();
		int[] seasons = serial.EpisodeSeasons;
		
		for (int i = 0; i <= seasons.Length - 1; i++)
		{
			AddNewSeason(i + 1, seasons[i]);
		}
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
		
		titleLEdit.Text = infoSeasonButtonsList.Length.ToString();
	}

	private void RemoveLastSeason()
	{
    	if (infoSeasonButtonsList.Length > 1)
    	{
    	    InfoSeasonButtons lastButton = infoSeasonButtonsList[^1];

    	    Node nodeToRemove = vBoxInfoSeasons.GetChildren()
            	.FirstOrDefault(node => node.Name == lastButton.GetSeason() + "infoSeason");

			vBoxInfoSeasons.RemoveChild(nodeToRemove);
    	    nodeToRemove?.QueueFree();
    	    infoSeasonButtonsList = infoSeasonButtonsList.Take(infoSeasonButtonsList.Length - 1).ToArray();
    	}
		
		titleLEdit.Text = infoSeasonButtonsList.Length.ToString();
	}

	private void RemoveAllSeason()
	{
	    foreach (Node node in vBoxInfoSeasons.GetChildren().ToList()) 
	    {
			if (node.Name != "InfoSeasonButtonsTemplate")
			{
	        	node.QueueFree();
				vBoxInfoSeasons.RemoveChild(node);
			}
	    }

	    infoSeasonButtonsList = Array.Empty<InfoSeasonButtons>();
	}


	private void _on_btn_add_pressed()
	{
		AddNewSeason(infoSeasonButtonsList.Length + 1, 1);
	}

	private void _on_btn_remove_pressed()
	{
		RemoveLastSeason();
	}
}

