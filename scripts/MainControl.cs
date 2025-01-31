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
	FilterBar filterBar;

	SimpleView[] simpleViews = Array.Empty<SimpleView>();
	Filter filter = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// windowSize = GetViewport().GetVisibleRect().Size;
		mainContainer = GetNode<MarginContainer>("MMainContain");
		addContainer = GetNode<MarginContainer>("MAddContain");
		addSingle = addContainer.GetNode<AddSingle>("AddSingle");
		vContain = mainContainer.GetNode<VBoxContainer>("VContain");
		vContainSimpleView = vContain.GetNode<VBoxContainer>("ScrollContainer/VContainSimpleView");
		simpleViewTemplate = vContainSimpleView.GetNode<SimpleView>("SimpleView");
		filterBar = vContain.GetNode<FilterBar>("FilterBar");

		LoadSimpleView();

		addSingle.Connect("OnBtnAddPressed", new Callable(this, MethodName.OnBtnAddPressedSignalReceived));
		addSingle.Connect("OnBtnCancelPressed", new Callable(this, MethodName.OnBtnCancelPressedSignalReceived));
		filterBar.Connect("OnStatusChanged", new Callable(this, MethodName.OnStatusChangedSignalReceived));
		filterBar.Connect("OnMoreOptBtnClicked", new Callable(this, MethodName.OnMoreOptBtnClickedSignalReceived));
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
		var serials = Array.Empty<Serial>();
		if (filter == null)
		{
			serials = backend.GetSerials();
		}
		else 
		{
			serials = backend.GetFilteredSerial(filter);
		}

		foreach (SimpleView sv in simpleViews)
		{
			sv.Visible = false;
		}

		foreach (var serial in serials)
		{
			bool exists = simpleViews.Any(view => view.Name == serial.Id + "SimpleView");

			if (!exists)
			{
				SimpleView newSimpleView = (SimpleView)simpleViewTemplate.Duplicate();
				newSimpleView.Name = serial.Id + "SimpleView";
				newSimpleView.Visible = true;
				newSimpleView._Ready();

				newSimpleView.Connect("OnBtnAddEpPressed", new Callable(this, MethodName.OnBtnAddEpPressedSignalReceived));
				newSimpleView.Connect("OnBtnRmvEpPressed", new Callable(this, MethodName.OnBtnRmvEpPressedSignalReceived));
				newSimpleView.Connect("OnBtnAddSeasonPressed", new Callable(this, MethodName.OnBtnAddSeasonPressedSignalReceived));
				newSimpleView.Connect("OnBtnRmvSeasonPressed", new Callable(this, MethodName.OnBtnRmvSeasonPressedSignalReceived));
				newSimpleView.Connect("OnMoreInfoBtnClicked", new Callable(this, MethodName.OnMoreInfoBtnClickedSignalReceived));

				newSimpleView.LoadDataIntoView(serial);
				vContainSimpleView.AddChild(newSimpleView);
				simpleViews = simpleViews.Append(newSimpleView).ToArray();
			}
			else
			{
				SimpleView toEnable = simpleViews.FirstOrDefault(view => view.Name == serial.Id + "SimpleView");
				toEnable.Visible = true;
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

	private void OnStatusChangedSignalReceived()
	{
		filter = filterBar.GetFilter();
		LoadSimpleView();
	}

	private void OnMoreOptBtnClickedSignalReceived(int size_y)
	{
		vContainSimpleView.CustomMinimumSize = new Vector2(1550, 800 - size_y);
		filterBar.CustomMinimumSize = new Vector2(1550, size_y);
	}

	private void OnMoreInfoBtnClickedSignalReceived(int size_y, string Id)
	{
		//vContainSimpleView.CustomMinimumSize = new Vector2(1550, 800 - size_y);
		SimpleView simpleView = simpleViews.FirstOrDefault(view => view.Name == Id + "SimpleView");
		if (simpleView != null)
			simpleView.CustomMinimumSize = new Vector2(1550, size_y);
	}
}
