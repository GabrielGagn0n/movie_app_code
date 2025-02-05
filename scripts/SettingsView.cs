using Godot;
using System;

public partial class SettingsView : Control
{
	private Settings settings;
	private bool allValid = true;

	private VBoxContainer vBoxContainer;
	private CheckBox cBoxAutoSwitch;
	private LineEdit lEditAutoSwitchTimer;
	private CheckBox cBoxSaveFilter;

	[Signal]
	public delegate void OnBtnSaveSettingsEventHandler();
	[Signal]
	public delegate void OnBtnCancelSettingsEventHandler();
	
	public override void _Ready()
	{
		vBoxContainer = GetNode<MarginContainer>("MCont").GetNode<VBoxContainer>("VBCont");
		cBoxAutoSwitch = vBoxContainer.GetNode<HBoxContainer>("HBDelayConfirmation").GetNode<CheckBox>("CBoxAutoSwitch");
		lEditAutoSwitchTimer = vBoxContainer.GetNode<HBoxContainer>("HBDelayTimer").GetNode<LineEdit>("LEditAutoSwitchTimer");
		cBoxSaveFilter = vBoxContainer.GetNode<HBoxContainer>("HBSaveFilters").GetNode<CheckBox>("CBoxSaveFilters");
		
		LoadInfo();
	}

	public void CreateNewSettings()
	{
		this.settings = new Settings
        {
            saveFilters = true,
            autoSwitch = true,
            autoSwitchTime = 30
        };
		SaveInfo();
	}

	public void LoadInfo()
	{
		// Load the info saved / create new
		this.settings = Data_Loader.LoadSettings();
		if (settings == null)
		{
		 	CreateNewSettings();
		}
		LoadValuesIntoInterface();
	}

	public Settings GetSettings()
	{
		return this.settings;
	}

	private void SaveInfo()
	{
		if (allValid)
		{
			Data_Saver.SaveSettings(this.settings);
			EmitSignal(SignalName.OnBtnSaveSettings);
		}
	}

	private void LoadValuesIntoInterface()
	{
		cBoxAutoSwitch.ButtonPressed = settings.autoSwitch;
		cBoxSaveFilter.ButtonPressed = settings.saveFilters;
		lEditAutoSwitchTimer.Text = settings.autoSwitchTime.ToString();
		lEditAutoSwitchTimer.Editable = settings.autoSwitch;
	}

	private void _on_c_box_auto_switch_pressed()
	{
		settings.autoSwitch = cBoxAutoSwitch.ButtonPressed;
		lEditAutoSwitchTimer.Editable = settings.autoSwitch;
	}

	private void _on_c_box_save_filters_pressed()
	{
	    settings.saveFilters = cBoxSaveFilter.ButtonPressed;
	}

	private void _on_btn_cancel_pressed()
	{
		LoadInfo();
		EmitSignal(SignalName.OnBtnCancelSettings);
	}

	private void _on_btn_save_pressed()
	{
		SaveInfo();
	}

	private void _on_l_edit_auto_switch_timer_text_submitted(string newText)
	{
	    if (int.TryParse(newText, out int value) && value > 0)
	    {
	        settings.autoSwitchTime = value;
	        allValid = true;
	        lEditAutoSwitchTimer.RemoveThemeColorOverride("font_color");
	    }
	    else
	    {
	        allValid = false;
	        lEditAutoSwitchTimer.AddThemeColorOverride("font_color", Color.FromHtml("#ff0000"));
	    }
	}

}
