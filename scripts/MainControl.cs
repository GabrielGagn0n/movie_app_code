using Godot;
using System;
using System.Linq;

public partial class MainControl : Control
{
	movie_app backend = new();
	MarginContainer mainContainer;
	MarginContainer addContainer;
	MarginContainer settingsContainer;
	MarginContainer[] containerList;
	ScrollContainer scrollContainer;
	AddSingle addSingle;
	VBoxContainer vContain;
	VBoxContainer vContainSimpleView;
	SimpleView simpleViewTemplate;
	FilterBar filterBar;
	SettingsView settingsView;

	SimpleView[] simpleViews = Array.Empty<SimpleView>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Setup of the differtent nodes
		mainContainer = GetNode<MarginContainer>("MMainContain");
		addContainer = GetNode<MarginContainer>("MAddContain");
		settingsContainer = GetNode<MarginContainer>("MSettings");
		addSingle = addContainer.GetNode<AddSingle>("AddSingle");
		vContain = mainContainer.GetNode<VBoxContainer>("VContain");
		scrollContainer = vContain.GetNode<ScrollContainer>("ScrollContainer");
		vContainSimpleView = vContain.GetNode<VBoxContainer>("ScrollContainer/VContainSimpleView");
		simpleViewTemplate = vContainSimpleView.GetNode<SimpleView>("SimpleView");
		filterBar = vContain.GetNode<FilterBar>("FilterBar");
		settingsView = GetNode<SettingsView>("MSettings/SettingsView");
		
		// Small list of container
		containerList = new MarginContainer[]{mainContainer, addContainer, settingsContainer};

		// Set settings in the backend
		backend.SetSettings(settingsView.GetSettings());
		// Load the different simpleView
		LoadSimpleView();

		addSingle.Connect("OnBtnAddPressed", new Callable(this, MethodName.OnBtnAddPressedSignalReceived));
		addSingle.Connect("OnBtnCancelPressed", new Callable(this, MethodName.OnBtnCancelPressedSignalReceived));
		filterBar.Connect("OnStatusChanged", new Callable(this, MethodName.OnStatusChangedSignalReceived));
		filterBar.Connect("OnMoreOptBtnClicked", new Callable(this, MethodName.OnMoreOptBtnClickedSignalReceived));
		settingsView.Connect("OnBtnSaveSettings", new Callable(this, MethodName.OnBtnSaveSettingsSignalReceived));
		settingsView.Connect("OnBtnCancelSettings", new Callable(this, MethodName.OnBtnCancelSettingsSignalReceived));		
	}

	public void _on_add_new_btn_pressed()
	{
		ChangeScreen(1);
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
			ChangeScreen(0);
		}
		else
		{
			addSingle.ChangeColorRequired();
		}
	}

	public void OnBtnCancelPressedSignalReceived()
	{
		addSingle.ClearData();
		ChangeScreen(0);
	}

	private void ChangeScreen(int index)
	{
		for (int i = 0; i < containerList.Length; i++)
		{
			if (i == index)
				containerList[i].Visible = true;
			else
				containerList[i].Visible = false;
		}
	}

	private void LoadSimpleView()
	{
		var serials = Array.Empty<Serial>();
		if (backend.GetSettings().saveFilters)
		{
			filterBar.SetFilter(backend.GetFilter());
		}

		if (backend.GetFilter() == null)
		{
			serials = backend.GetSerials();
		}
		else 
		{
			serials = backend.GetFilteredSerial(backend.GetFilter());
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
				newSimpleView.Connect("OnBtnSaveBtnClicked", new Callable(this, MethodName.OnBtnSaveBtnClickedSignalReceived));
				newSimpleView.Connect("OnBtnDeleteConfirmBtnClicked", new Callable(this, MethodName.OnBtnDeleteConfirmBtnClickedSignalReceived));

				newSimpleView.LoadDataIntoView(serial);
				vContainSimpleView.AddChild(newSimpleView);
				simpleViews = simpleViews.Append(newSimpleView).ToArray();
			}
			else
			{
				SimpleView toEnable = simpleViews.FirstOrDefault(view => view.Name == serial.Id + "SimpleView");
				toEnable.LoadDataIntoView(serial);
				toEnable.Visible = true;
			}
		}
	}

	private void OnBtnAddEpPressedSignalReceived(string id)
	{
		SimpleView simpleView = simpleViews.FirstOrDefault(view => view.GetSerial().Id == Guid.Parse(id));
		if (simpleView != null)
		{
			Serial serial = simpleView.GetSerial();
			backend.UpdateSerials(ButtonViewActions.AddEpisode, serial.Id);
			simpleView.LoadDataIntoView(serial);
		}
	}

	private void OnBtnRmvEpPressedSignalReceived(string id)
	{
		SimpleView simpleView = simpleViews.FirstOrDefault(view => view.GetSerial().Id == Guid.Parse(id));
		if (simpleView != null)
		{
			Serial serial = simpleView.GetSerial();
			backend.UpdateSerials(ButtonViewActions.RemoveEpisode, serial.Id);
			simpleView.LoadDataIntoView(serial);
		}
	}

	private void OnBtnAddSeasonPressedSignalReceived(string id)
	{
		SimpleView simpleView = simpleViews.FirstOrDefault(view => view.GetSerial().Id == Guid.Parse(id));
		if (simpleView != null)
		{
			Serial serial = simpleView.GetSerial();
			backend.UpdateSerials(ButtonViewActions.AddSeason, serial.Id);
			simpleView.LoadDataIntoView(serial);
		}
	}

	private void OnBtnRmvSeasonPressedSignalReceived(string id)
	{
		SimpleView simpleView = simpleViews.FirstOrDefault(view => view.GetSerial().Id == Guid.Parse(id));
		if (simpleView != null)
		{
			Serial serial = simpleView.GetSerial();
			backend.UpdateSerials(ButtonViewActions.RemoveSeason, serial.Id);
			simpleView.LoadDataIntoView(serial);
		}
	}

	private void OnStatusChangedSignalReceived()
	{
		backend.SetFilter(filterBar.GetFilter());
		LoadSimpleView();
	}

	private void OnMoreOptBtnClickedSignalReceived(int size_y)
	{
		scrollContainer.CustomMinimumSize = new Vector2(1550, 800 - size_y);
		filterBar.CustomMinimumSize = new Vector2(1550, size_y);
		filterBar.Position = new Vector2(0,0);
	}

	private void OnMoreInfoBtnClickedSignalReceived(int size_y, string Id)
	{
		//vContainSimpleView.CustomMinimumSize = new Vector2(1550, 800 - size_y);
		SimpleView simpleView = simpleViews.FirstOrDefault(view => view.Name == Id + "SimpleView");
		if (simpleView != null)
			simpleView.CustomMinimumSize = new Vector2(1550, size_y);
	}

	private void OnBtnSaveBtnClickedSignalReceived(string id)
	{
		SimpleView simpleView = simpleViews.FirstOrDefault(view => view.GetSerial().Id == Guid.Parse(id));
		if (simpleView != null)
		{
			Serial serial = simpleView.GetSerial();
			backend.UpdateSerials(ButtonViewActions.UpdateSerial, serial.Id);
			simpleView.LoadDataIntoView(serial);
		}
	}

	private void _on_btn_settings_pressed()
	{
		ChangeScreen(2);
	}

	private void OnBtnSaveSettingsSignalReceived()
	{
		backend.SetSettings(settingsView.GetSettings());

		if (backend.GetSettings().saveFilters)
			backend.SetFilter(filterBar.GetFilter());
		else
			Data_Deleter.DeleteFilter();

		ChangeScreen(0);
	}

	private void OnBtnCancelSettingsSignalReceived()
	{
		ChangeScreen(0);
	}

	private void OnBtnDeleteConfirmBtnClickedSignalReceived(string id)
	{
		SimpleView simpleView = simpleViews.FirstOrDefault(view => view.GetSerial().Id == Guid.Parse(id));
		if (simpleView != null)
		{
			Serial serial = simpleView.GetSerial();
			backend.DeleteSerial(serial.Id);
			simpleView.Destroy();
		}
	}
}
