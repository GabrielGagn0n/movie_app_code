using Godot;
using System;

public partial class InfoSeasonButtons : Control
{
	private int season;
	private int nbrEpisodes;
	private Label lblSeason;
	private LineEdit LineEditEpisode;
	public override void _Ready()
	{
		lblSeason = GetNode<MarginContainer>("MCont").GetNode<HBoxContainer>("HBoxCont").GetNode<Label>("LblSeason");
		LineEditEpisode = GetNode<MarginContainer>("MCont").GetNode<HBoxContainer>("HBoxCont").GetNode<LineEdit>("LineEdit");
	}

	public void Setup(int season, int nbrEpisodes)
	{
		this.season = season;
		this.nbrEpisodes = nbrEpisodes;

		lblSeason.Text = "Season " + season.ToString() + " : ";
		ChangeLabelNbrEpisode();
	}

	public int GetNbrEpi()
	{
		return nbrEpisodes;
	}

	public int GetSeason()
	{
		return season;
	}

    private void ChangeLabelNbrEpisode()
    {
		if (nbrEpisodes <= 0)
		{
			nbrEpisodes = 1;
		}
        LineEditEpisode.Text = nbrEpisodes.ToString();
    }

	public void _on_btn_add_10_pressed()
	{
		nbrEpisodes += 10;
		ChangeLabelNbrEpisode();
	}

	public void _on_btn_add_5_pressed()
	{
		nbrEpisodes += 5;
		ChangeLabelNbrEpisode();
	}

	public void _on_btn_add_1_pressed()
	{
		nbrEpisodes += 1;
		ChangeLabelNbrEpisode();
	}

	public void _on_btn_remove_1_pressed()
	{
		nbrEpisodes -= 1;
		ChangeLabelNbrEpisode();
	}

    public void _on_btn_remove_5_pressed()
	{
		nbrEpisodes -= 5;
		ChangeLabelNbrEpisode();
	}

	public void _on_btn_remove_10_pressed()
	{
		nbrEpisodes -= 10;
		ChangeLabelNbrEpisode();
	}
}
