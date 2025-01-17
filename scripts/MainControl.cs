using Godot;
using System;
using System.Linq;

public partial class MainControl : Control
{
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
			// TODO : DidWatched from EpisodeSeasons

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

				newSimpleView.Connect("OnBtnAddEpPressed", new Callable(this, MethodName.OnBtnAddEpPressedSignalReceived));
				newSimpleView.Connect("OnBtnRmvEpPressed", new Callable(this, MethodName.OnBtnRmvEpPressedSignalReceived));
				newSimpleView.Connect("OnBtnAddSeasonPressed", new Callable(this, MethodName.OnBtnAddSeasonPressedSignalReceived));
				newSimpleView.Connect("OnBtnRmvSeasonPressed", new Callable(this, MethodName.OnBtnRmvSeasonPressedSignalReceived));

				newSimpleView.LoadDataIntoView(serial);
				vContainSimpleView.AddChild(newSimpleView);
				simpleViews = simpleViews.Append(newSimpleView).ToArray();
			}
		}
	}

	private void OnBtnAddEpPressedSignalReceived(int id)
	{
		SimpleView simpleView = simpleViews.FirstOrDefault(view => view.GetSerial().Id == id);
		if (simpleView != null)
		{
			Serial serial = simpleView.GetSerial();
			backend.UpdateSerials(ButtonViewActions.AddEpisode, id = serial.Id);
			simpleView.LoadDataIntoView(serial);
		}
	}

	private void OnBtnRmvEpPressedSignalReceived(int id)
	{
		SimpleView simpleView = simpleViews.FirstOrDefault(view => view.GetSerial().Id == id);
		if (simpleView != null)
		{
			Serial serial = simpleView.GetSerial();
			backend.UpdateSerials(ButtonViewActions.RemoveEpisode, id = serial.Id);
			simpleView.LoadDataIntoView(serial);
		}
	}

	private void OnBtnAddSeasonPressedSignalReceived(int id)
	{
		SimpleView simpleView = simpleViews.FirstOrDefault(view => view.GetSerial().Id == id);
		if (simpleView != null)
		{
			Serial serial = simpleView.GetSerial();
			backend.UpdateSerials(ButtonViewActions.AddSeason, id = serial.Id);
			simpleView.LoadDataIntoView(serial);
		}
	}

	private void OnBtnRmvSeasonPressedSignalReceived(int id)
	{
		SimpleView simpleView = simpleViews.FirstOrDefault(view => view.GetSerial().Id == id);
		if (simpleView != null)
		{
			Serial serial = simpleView.GetSerial();
			backend.UpdateSerials(ButtonViewActions.RemoveSeason, id = serial.Id);
			simpleView.LoadDataIntoView(serial);
		}
	}
}
