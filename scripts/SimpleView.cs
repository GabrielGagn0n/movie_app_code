using Godot;
using System;

public partial class SimpleView : Control
{
	[Signal]
	public delegate void OnBtnAddEpPressedEventHandler(string id);
	[Signal]
	public delegate void OnBtnRmvEpPressedEventHandler(string id);
	[Signal]
	public delegate void OnBtnAddSeasonPressedEventHandler(string id);
	[Signal]
	public delegate void OnBtnRmvSeasonPressedEventHandler(string id);
	[Signal]
	public delegate void OnMoreInfoBtnClickedEventHandler(int size_y, string Id);
	[Signal]
	public delegate void OnBtnDeleteConfirmBtnClickedEventHandler(string id);
	[Signal]
	public delegate void OnBtnSaveBtnClickedEventHandler(string id);
	[Signal]
	public delegate void OnBtnEditClickedEventHandler(string id);
	[Signal]
	public delegate void OnBtnRewatchClickedEventHandler(string id);
	
	private MarginContainer mContainer;
	private HBoxContainer hContainer;
	private HBoxContainer hLblContainer;
	private HBoxContainer hBtnContainer;
	private VBoxContainer vBCMoreOptions;
	private LineEdit lEditName;
	private LineEdit lEditId;
	private LineEdit lEditAlias;
	private LineEdit lEditLink;
	private CheckBox cBoxDepricated;
	private OptionButton oBtnStatus;
	private OptionButton oBtnType;
	private Label lblSeasonEpi;
	private Button btnDelete;
	private Button btnConfirm;
	private Button btnCancel;
	private Button btnModify;
	private Button btnSave;
	private Button btnSaveCancel;
	private Button btnOpen;
	private Button btnEdit;
	
	private bool validNbr = true;
	private int nbrToAdd = 1;
	private Serial serial;
	private string link;

	private Texture2D downArrow = (Texture2D)ResourceLoader.Load("./ressource/down-arrow.png");
	private Texture2D upArrow = (Texture2D)ResourceLoader.Load("./ressource/up-arrow.png");

	private const int MAX_LENGTH_ALIAS = 20;

	public override void _Ready()
	{
		mContainer = GetNode<MarginContainer>("MContain");
		hContainer = mContainer.GetNode<HBoxContainer>("VContain/HContain");
		hLblContainer = hContainer.GetNode<HBoxContainer>("HLblContain");
		hBtnContainer = hContainer.GetNode<HBoxContainer>("HBtnContain");
		vBCMoreOptions = mContainer.GetNode<VBoxContainer>("VContain/VBCMoreOptions");
		lEditName = vBCMoreOptions.GetNode<LineEdit>("HBCTop/HBCName/LEditName");
		lEditAlias = vBCMoreOptions.GetNode<LineEdit>("HBCAlias/HBCAliasInner/LEditAlias");
		lEditLink = vBCMoreOptions.GetNode<LineEdit>("HBCLink/HBCLinkInner/LEditLink");
		lEditId = vBCMoreOptions.GetNode<LineEdit>("HBCTop/HBCId/LEditId");
		cBoxDepricated = vBCMoreOptions.GetNode<CheckBox>("HBCLink/HBCLinkInner/CBoxDepricated");
		oBtnStatus = vBCMoreOptions.GetNode<OptionButton>("HBCStatusType/HBCStatus/OBtnStatus");
		oBtnType = vBCMoreOptions.GetNode<OptionButton>("HBCStatusType/HBCType/OBtnType");
		btnDelete = vBCMoreOptions.GetNode<Button>("HBCBottom/HBCBottomInner/MCBtn/HBCBtn/BtnDelete");
		btnConfirm = vBCMoreOptions.GetNode<Button>("HBCBottom/HBCBottomInner/MCBtn/HBCBtn/BtnConfirm");
		btnCancel = vBCMoreOptions.GetNode<Button>("HBCBottom/HBCBottomInner/MCBtn/HBCBtn/BtnCancel");
		btnModify = vBCMoreOptions.GetNode<Button>("HBCBottom/HBCBottomInner/MCBtn/HBCBtn/BtnModify");
		btnSave = vBCMoreOptions.GetNode<Button>("HBCBottom/HBCBottomInner/MCBtn/HBCBtn/BtnSave");
		btnSaveCancel = vBCMoreOptions.GetNode<Button>("HBCBottom/HBCBottomInner/MCBtn/HBCBtn/BtnSaveCancel");
		lblSeasonEpi = vBCMoreOptions.GetNode<Label>("HBCBottom/HBCBottomInner/LblSeasonEpi");
		btnEdit = vBCMoreOptions.GetNode<Button>("HBCSeason/BtnEdit");

		btnOpen = hContainer.GetNode<Button>("OpenBtn");
	}

	// Set the serial for the view, will call methods to change the labels and the link
	public void LoadDataIntoView(Serial serial)
	{
		this.serial = serial;
		ChangeLabels();
		ChangeLink();
	}

	public void Destroy()
	{
		QueueFree();
	}

	public Serial GetSerial()
	{
		return serial;
	}

	private void ChangeLink()
	{
		if(!string.IsNullOrEmpty(serial.Link))
		{
			this.link = serial.Link;
			btnOpen.Disabled = false;
			btnOpen.TooltipText = this.link;
		}
		else
		{
			btnOpen.Disabled = true;
			btnOpen.TooltipText = "You don't have a link saved";
		}
	}

	// take the alias if it exist, else the name.
	// find the latest season and the episode with the list in serial
	private void ChangeLabels()
	{
		if (!string.IsNullOrEmpty(serial.Alias))
		{
			ChangeAliasLbl(serial.Alias);
		}
		else
		{
			ChangeAliasLbl(serial.Name);
		}

		int latestEpisode = serial.GetIndexLatestWatchedEpisode();
		int episodePerSeason = 0;
		int season = -1;
		int episode = -1;
		for (int i = 0; i <= serial.EpisodeSeasons.Length - 1; i++)
		{
			int nbreEpisode = serial.EpisodeSeasons[i];
			if (episodePerSeason + nbreEpisode > latestEpisode)
			{
				season = i + 1;
				episode =  latestEpisode - episodePerSeason + 1;
				break;
			}
			episodePerSeason += nbreEpisode;
		}

		ChangeSeasonLbl(season, episode, serial.Type);
		ChangeModifiedDateLbl(serial.LatestUpdate);
	}

	// Try to always have the same length
	private void ChangeAliasLbl(string toChange)
	{
	    var aliasLbl = hLblContainer.GetNode<Label>("AliasC/AliasLbl");
	
	    if (toChange.Length > MAX_LENGTH_ALIAS)
	    {
	        string truncatedText = toChange.Substring(0, MAX_LENGTH_ALIAS - 3) + "...";
	        aliasLbl.Text = truncatedText;
	    }
	    else
	    {
	        aliasLbl.Text = toChange;
	    }
	}
	

	private void ChangeSeasonLbl(int season, int episode, SerialType type)
	{
	    var seasonLbl = hLblContainer.GetNode<Label>("SeasonC/SeasonLbl");
	    string text;

	    if (season >= 0 && episode >= 0)
	    {
			if (type == SerialType.Movie)
				text = string.Format("Movie {0}", season);
			else
	        	text = string.Format("Season {0} - Episode {1}", season, episode);
	    }
	    else
	    {
			if (type == SerialType.Movie)
				text = string.Format("Movie {0}", season + " (Completed)");
			else
	        	text = string.Format("Season {0} - Episode {1}", serial.EpisodeSeasons.Length, serial.EpisodeSeasons[^1]) + " (Completed)";
	    }
	    seasonLbl.Text = text;
	}

	private void ChangeModifiedDateLbl(DateTime dateTime)
	{
		var modifiedDate = hLblContainer.GetNode<Label>("DateC/ModifiedDateLbl");
		modifiedDate.Text = string.Format("Modified last : {0}", dateTime.ToString());
	}

	private void LoadInfoIntoMoreOption()
	{
		lEditName.Text = serial.Name;
		lEditAlias.Text = serial.Alias;
		lEditId.Text = serial.Id.ToString();
		lEditLink.Text = serial.Link;

		oBtnStatus.Clear();
		foreach (var item in Enum.GetValues(typeof(Status)))
		{
			oBtnStatus.AddItem(item.ToString());
		}
		oBtnStatus.Select((int)serial.Status);

		oBtnType.Clear();
		foreach (var item in Enum.GetValues(typeof(SerialType)))
		{
			oBtnType.AddItem(item.ToString());
		}
		oBtnType.Select((int)serial.Type);
		var seasonLbl = hLblContainer.GetNode<Label>("SeasonC/SeasonLbl");

		if (serial.AlreadyWatched)
			lblSeasonEpi.Text = serial.Name + " *Prestiged*";
		else
			lblSeasonEpi.Text = serial.Name;
	}

	private void SaveModifiedSerial()
	{
		serial.Name = lEditName.Text;
		serial.Alias = lEditAlias.Text;
		serial.Link = lEditLink.Text;
		serial.Status = (Status) oBtnStatus.GetSelectedId();
		serial.Type = (SerialType) oBtnType.GetSelectedId();
	}

	private void ChangeEditable(bool toChange)
	{
		lEditName.Editable = toChange;
		lEditAlias.Editable = toChange;
		lEditLink.Editable = toChange;
		oBtnStatus.Disabled = !toChange;
		oBtnType.Disabled = !toChange;
		btnEdit.Disabled = !toChange;
	}

	// if the value is not a number, change the box red
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

	private void _on_open_btn_pressed()
	{
		if (!String.IsNullOrEmpty(link))
			OS.ShellOpen(link);
	}

	private void _on_add_ep_btn_pressed()
	{
		EmitSignal(SignalName.OnBtnAddEpPressed, serial.Id.ToString());
	}

	private void _on_rmv_ep_btn_pressed()
	{
		EmitSignal(SignalName.OnBtnRmvEpPressed, serial.Id.ToString());
	}

	private void _on_add_season_btn_pressed()
	{
		EmitSignal(SignalName.OnBtnAddSeasonPressed, serial.Id.ToString());
	}

	private void _on_rmv_season_btn_pressed()
	{
		EmitSignal(SignalName.OnBtnRmvSeasonPressed, serial.Id.ToString());
	}

	private void _on_more_info_btn_pressed()
	{
		Button button = hBtnContainer.GetNode<Button>("MoreInfoBtn");
		ChangeEditable(false);
		SetVisibility(btnModify: true, btnDelete: false, btnSave: false, btnSaveCancel: false);
		LoadInfoIntoMoreOption();

		if (vBCMoreOptions.Visible)
		{
			EmitSignal(SignalName.OnMoreInfoBtnClicked, 50, serial.Id.ToString());
			vBCMoreOptions.Visible = false;
			SetSize(new Vector2(1550, 50));
			button.Icon = downArrow; 
		}
		else
		{
			EmitSignal(SignalName.OnMoreInfoBtnClicked, 250, serial.Id.ToString());
			vBCMoreOptions.Visible = true;
			SetSize(new Vector2(1550, 250));
			button.Icon = upArrow;
		}
	}

	private void _on_btn_delete_pressed()
	{
	    SetVisibility(btnDelete: false, btnCancel: true, btnConfirm: true);
	}

	private void _on_btn_confirm_pressed()
	{
	    SetVisibility(false, false, false);
	    EmitSignal(SignalName.OnBtnDeleteConfirmBtnClicked, serial.Id.ToString());
	}

	private void _on_btn_cancel_pressed()
	{
	    SetVisibility(false, false, false);
	}

	private void _on_btn_modify_pressed()
	{
	    SetVisibility(btnModify: false, btnDelete: true, btnSave: true, btnSaveCancel: true);
	    ChangeEditable(true);
	}

	private void _on_btn_save_cancel_pressed()
	{
	    SetVisibility(btnModify: true, btnDelete: false, btnSave: false, btnSaveCancel: false);
	    ChangeEditable(false);
	    LoadInfoIntoMoreOption();
	}

	private void _on_btn_save_pressed()
	{
	    SetVisibility(btnModify: true, btnDelete: false, btnSave: false, btnSaveCancel: false);
	    SaveModifiedSerial();
	    ChangeEditable(false);
	    EmitSignal(SignalName.OnBtnSaveBtnClicked, serial.Id.ToString());
	}

	private void SetVisibility(bool btnDelete = false, bool btnCancel = false, bool btnConfirm = false, 
	                           bool btnModify = true, bool btnSave = false, bool btnSaveCancel = false)
	{
	    this.btnDelete.Visible = btnDelete;
	    this.btnCancel.Visible = btnCancel;
	    this.btnConfirm.Visible = btnConfirm;
	    this.btnModify.Visible = btnModify;
	    this.btnSave.Visible = btnSave;
	    this.btnSaveCancel.Visible = btnSaveCancel;
	}

	private void _on_btn_edit_pressed()
	{
		EmitSignal(SignalName.OnBtnEditClicked, serial.Id.ToString());
	}

	private void _on_btn_rewatch_pressed()
	{
		lblSeasonEpi.Text = serial.Name + " *Prestiged*";
		EmitSignal(SignalName.OnBtnRewatchClicked, serial.Id.ToString());
	}
}
