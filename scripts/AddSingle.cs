using Godot;
using System;

public partial class AddSingle : Control
{
	ScrollContainer scrollContainer;
	VBoxContainer vBoxContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scrollContainer = GetNode<ScrollContainer>("adding-scrolling-container");
		vBoxContainer = scrollContainer.GetNode<VBoxContainer>("adding-vcontainer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_check_button_toggled(bool toggle_on)
	{
		var more_container = vBoxContainer.GetNode<Control>("more-controls");
		more_container.Visible = toggle_on;
	}
}
