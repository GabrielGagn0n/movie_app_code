using Godot;
using System;

public partial class MainControl : Control
{
	Vector2 windowSize = default(Vector2);
	movie_app backend = new();
	MarginContainer mainContainer;
	MarginContainer addContainer;
	Node addSingle;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		windowSize = GetViewport().GetVisibleRect().Size;
		mainContainer = GetNode<MarginContainer>("MMainContain");
		addContainer = GetNode<MarginContainer>("MAddContain");
		addSingle = addContainer.GetNode<Node>("AddSingle");

		addSingle.Connect("OnBtnAddPressed", new Callable(this, MethodName.OnBtnAddPressedSignalReceived));
		addSingle.Connect("OnBtnCancelPressed", new Callable(this, MethodName.OnBtnCancelPressedSignalReceived));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_add_new_btn_pressed()
	{
		ChangeScreen();
	}

	public void OnBtnAddPressedSignalReceived()
	{
		ChangeScreen();
	}

	public void OnBtnCancelPressedSignalReceived()
	{
		ChangeScreen();
	}

	private void ChangeScreen()
	{
		if (mainContainer.Visible)
		{
			mainContainer.Visible = false;
			addContainer.Visible = true;
		}
		else
		{
			mainContainer.Visible = true;
			addContainer.Visible = false;
		}
	}
	// private void _on_button_pressed()
	// {
	// 	var TextContainer = GetNode<BoxContainer>("TextBox");
	// 	var message = TextContainer.GetNode<Label>("TestLabel");
	// 	message.Text = windowSize.X + " " + windowSize.Y;
	// }

	// private void _on_resized()
	// {
	// 	windowSize = GetViewport().GetVisibleRect().Size;
	// }
}
