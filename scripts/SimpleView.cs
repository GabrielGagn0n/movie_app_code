using Godot;
using System;

public partial class SimpleView : Control
{
	private MarginContainer mContainer;
	private HBoxContainer hContainer;
	private HBoxContainer hLblContainer;
	private HBoxContainer hBtnContainer;
	
	private bool validNbr = true;
	private int nbrToAdd = 1;
	private Serial serial;

	public override void _Ready()
	{
		mContainer = GetNode<MarginContainer>("MContain");
		hContainer = mContainer.GetNode<HBoxContainer>("HContain");
		hLblContainer = hContainer.GetNode<HBoxContainer>("HLblContain");
		hBtnContainer = hContainer.GetNode<HBoxContainer>("HBtnContain");
	}

	public void LoadDataIntoView(Serial serial)
	{
		this.serial = serial;
		ChangeLabels();
	}

	private void ChangeLabels()
	{

	}

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
}
