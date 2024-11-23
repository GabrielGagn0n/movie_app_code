using Godot;
using System;

public partial class MainControl : Control
{
	Vector2 windowSize = default(Vector2);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		windowSize = GetViewport().GetVisibleRect().Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_button_pressed()
	{
		var TextContainer = GetNode<BoxContainer>("TextBox");
		var message = TextContainer.GetNode<Label>("TestLabel");
		message.Text = windowSize.X + " " + windowSize.Y;
	}

	private void _on_resized()
	{
		windowSize = GetViewport().GetVisibleRect().Size;
	}
}
