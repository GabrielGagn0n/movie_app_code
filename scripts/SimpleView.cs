using Godot;
using System;

public partial class SimpleView : Control
{
	[Signal]
	public delegate void OnBtnAddEpPressedEventHandler(int id);
	[Signal]
	public delegate void OnBtnRmvEpPressedEventHandler(int id);
	private MarginContainer mContainer;
	private HBoxContainer hContainer;
	private HBoxContainer hLblContainer;
	private HBoxContainer hBtnContainer;
	
	private bool validNbr = true;
	private int nbrToAdd = 1;
	private Serial serial;
	private string link;

	private const int EMPTY_SPACE = 40;
	private const int MAX_LENGTH_ALIAS = 20;

	public override void _Ready()
	{
		mContainer = GetNode<MarginContainer>("MContain");
		hContainer = mContainer.GetNode<HBoxContainer>("HContain");
		hLblContainer = hContainer.GetNode<HBoxContainer>("HLblContain");
		hBtnContainer = hContainer.GetNode<HBoxContainer>("HBtnContain");
	}

	// Set the serial for the view, will call methods to change the labels and the link
	public void LoadDataIntoView(Serial serial)
	{
		this.serial = serial;
		ChangeLabels();
		ChangeLink();
	}

	public Serial GetSerial()
	{
		return serial;
	}

	private void ChangeLink()
	{
		if(!string.IsNullOrEmpty(serial.Link))
		{
			this.link = serial.Link;
		}
	}

	// take the alias if it exist, else the name.
	// find the latest season and the episode with the list in serial
	private void ChangeLabels()
	{
		if (!string.IsNullOrEmpty(serial.Alias))
		{
			ChangeAliasLbl(serial.Alias);
		}
		else
		{
			ChangeAliasLbl(serial.Name);
		}

		int latestEpisode = serial.GetIndexLatestWatchedEpisode();
		int episodePerSeason = 0;
		int season = -1;
		int episode = -1;
		for (int i = 0; i <= serial.EpisodeSeasons.Length - 1; i++)
		{
			int nbreEpisode = serial.EpisodeSeasons[i];
			if (episodePerSeason + nbreEpisode > latestEpisode)
			{
				season = i + 1;
				episode =  latestEpisode - episodePerSeason + 1;
				break;
			}
			episodePerSeason += nbreEpisode;
		}

		ChangeSeasonLbl(season, episode);
		ChangeModifiedDateLbl(serial.LatestUpdate);
	}

	// Try to always have the same length
	private void ChangeAliasLbl(string toChange)
	{
	    var aliasLbl = hLblContainer.GetNode<Label>("AliasLbl");
	
	    if (toChange.Length > MAX_LENGTH_ALIAS)
	    {
	        string truncatedText = toChange.Substring(0, MAX_LENGTH_ALIAS - 3) + "...";
	        aliasLbl.Text = truncatedText.PadRight(MAX_LENGTH_ALIAS + EMPTY_SPACE);
	    }
	    else
	    {
	        aliasLbl.Text = toChange.PadRight(MAX_LENGTH_ALIAS + EMPTY_SPACE);
	    }
	}
	

	private void ChangeSeasonLbl(int season, int episode)
	{
	    var seasonLbl = hLblContainer.GetNode<Label>("SeasonLbl");
	    string text;

	    if (season >= 0 && episode >= 0)
	    {
	        text = string.Format("Season {0} - Episode {1}", season, episode);
	    }
	    else
	    {
	        text = "Completed";
	    }
	    seasonLbl.Text = text.PadRight(MAX_LENGTH_ALIAS + EMPTY_SPACE).Substring(0, MAX_LENGTH_ALIAS + EMPTY_SPACE);
	}


	private void ChangeModifiedDateLbl(DateTime dateTime)
	{
		var modifiedDate = hLblContainer.GetNode<Label>("ModifiedDateLbl");
		modifiedDate.Text = string.Format("Modified last : {0}", dateTime.ToString());
	}

	// if the value is not a number, change the box red
	private void _on_nbr_text_box_focus_exited()
	{
		var nbrTextBox = hBtnContainer.GetNode<TextEdit>("NbrTextBox");
		if(int.TryParse(nbrTextBox.Text, out nbrToAdd))
		{
			nbrTextBox.RemoveThemeColorOverride("background_color");
			validNbr = true;
		}
		else
		{
			nbrTextBox.AddThemeColorOverride("background_color", Color.FromHtml("#620000"));
			validNbr = false;
		}
	}

	private void _on_open_btn_pressed()
	{
		OS.ShellOpen(link);
	}

	private void _on_add_ep_btn_pressed()
	{
		EmitSignal(SignalName.OnBtnAddEpPressed, serial.Id);
	}

	private void _on_rmv_ep_btn_pressed()
	{
		EmitSignal(SignalName.OnBtnRmvEpPressed, serial.Id);
	}
}
