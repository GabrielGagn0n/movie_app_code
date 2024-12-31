using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AddMoreOptions : Control
{
	Button btnRemove;
	Button btnAdd;
	ScrollContainer scrollContainer;
	VBoxContainer vBoxContainer;
	HBoxContainer infoSeasonContainer;
	List<Node> infoSeasonList = new List<Node>();
	TextEdit nbrSeason;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scrollContainer = GetNode<ScrollContainer>("adding-more-options-scrolling-container");
		vBoxContainer = scrollContainer.GetNode<VBoxContainer>("more-opt-vcontainer");
		infoSeasonContainer = vBoxContainer.GetNode<HBoxContainer>("info-season-1");
		btnRemove = vBoxContainer.GetNode<Button>("button-container/btn-remove");
		btnAdd = vBoxContainer.GetNode<Button>("button-container/btn-add");
		nbrSeason = vBoxContainer.GetNode<HBoxContainer>("HContainer").GetNode<TextEdit>("NbrSeasonTextBox");

		infoSeasonList.Add(infoSeasonContainer);
		btnRemove.Disabled = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_btnremove_pressed()
	{
		if (infoSeasonList.Count > 1)
		{
			var toRemove = infoSeasonList[infoSeasonList.Count - 1];
			foreach (var item in toRemove.GetChildren())
			{
				toRemove.RemoveChild(item);
				item.QueueFree();
			}
			toRemove.QueueFree();
			infoSeasonList.RemoveAt(infoSeasonList.Count - 1);
			nbrSeason.Text = "" + infoSeasonList.Count;
		}

		if (infoSeasonList.Count <= 1)
		{
			btnRemove.Disabled = true;
		}
	}

	public void _on_btnadd_pressed()
	{
		btnRemove.Disabled = false;
		var newContainer = infoSeasonContainer.Duplicate();
		infoSeasonList.Add(newContainer);

		var label = newContainer.GetNode<Label>("NbrSeasonLbl");
		nbrSeason.Text = "" + infoSeasonList.Count;
		label.Text = "Season " + infoSeasonList.Count;

		vBoxContainer.AddChild(newContainer);
	}
}
