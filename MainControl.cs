using Godot;
using System;

public partial class MainControl : Control
{
	Vector2 windowSize = default(Vector2);
	movie_app backend = new();
	MarginContainer mainContainer;
	MarginContainer addContainer;
	AddSingle addSingle;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// windowSize = GetViewport().GetVisibleRect().Size;
		mainContainer = GetNode<MarginContainer>("MMainContain");
		addContainer = GetNode<MarginContainer>("MAddContain");
		addSingle = addContainer.GetNode<AddSingle>("AddSingle");

		addSingle.Connect("OnBtnAddPressed", new Callable(this, MethodName.OnBtnAddPressedSignalReceived));
		addSingle.Connect("OnBtnCancelPressed", new Callable(this, MethodName.OnBtnCancelPressedSignalReceived));
	}

	public void _on_add_new_btn_pressed()
	{
		ChangeScreen();
	}

	public void OnBtnAddPressedSignalReceived()
	{
		Serials toAdd = addSingle.GetSerial();
		if (!string.IsNullOrEmpty(toAdd.get_name()))
		{
        	toAdd.set_Status(Status.NotStarted);
			toAdd.set_episode_seasons(addSingle.GetNbrEpiSeason());
			
			backend.AddSerial(toAdd);
			addSingle.ClearData();
			ChangeScreen();
		}
		else
		{
			addSingle.ChangeColorRequired();
		}
	}

	public void OnBtnCancelPressedSignalReceived()
	{
		addSingle.ClearData();
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
}
