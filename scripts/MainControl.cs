using Godot;
using System;
using System.Linq;

public partial class MainControl : Control
{
	Vector2 windowSize = default(Vector2);
	movie_app backend = new();
	MarginContainer mainContainer;
	MarginContainer addContainer;
	AddSingle addSingle;
	VBoxContainer vContain;
	VBoxContainer vContainSimpleView;
	SimpleView simpleViewTemplate;
	SimpleView[] simpleViews = Array.Empty<SimpleView>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// windowSize = GetViewport().GetVisibleRect().Size;
		mainContainer = GetNode<MarginContainer>("MMainContain");
		addContainer = GetNode<MarginContainer>("MAddContain");
		addSingle = addContainer.GetNode<AddSingle>("AddSingle");
		vContain = mainContainer.GetNode<VBoxContainer>("VContain");
		vContainSimpleView = vContain.GetNode<VBoxContainer>("VContainSimpleView");
		simpleViewTemplate = vContainSimpleView.GetNode<SimpleView>("SimpleView");

		LoadSimpleView();

		addSingle.Connect("OnBtnAddPressed", new Callable(this, MethodName.OnBtnAddPressedSignalReceived));
		addSingle.Connect("OnBtnCancelPressed", new Callable(this, MethodName.OnBtnCancelPressedSignalReceived));
	}

	public void _on_add_new_btn_pressed()
	{
		ChangeScreen();
	}

	public void OnBtnAddPressedSignalReceived()
	{
		Serial toAdd = addSingle.GetSerial();
		if (!string.IsNullOrEmpty(toAdd.Name))
		{
        	toAdd.Status = Status.NotStarted;
			toAdd.LatestUpdate = DateTime.Now;
			toAdd.EpisodeSeasons = addSingle.GetNbrEpiSeason();

			backend.AddSerial(toAdd);
			addSingle.ClearData();
			LoadSimpleView();
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

	private void LoadSimpleView()
	{
		var serials = backend.GetSerials();
		foreach (var serial in serials)
		{
			bool exists = simpleViews.Any(view => view.Name == serial.Alias + "SimpleView");

			if (!exists)
			{
				SimpleView newSimpleView = (SimpleView)simpleViewTemplate.Duplicate();
				newSimpleView.Name = serial.Alias + "SimpleView";
				newSimpleView.Visible = true;
				newSimpleView._Ready();
				newSimpleView.LoadDataIntoView(serial);
				vContainSimpleView.AddChild(newSimpleView);
				simpleViews.Append(newSimpleView);
			}
		}
	}
}
