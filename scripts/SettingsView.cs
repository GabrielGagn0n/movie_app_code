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
	[Signal]
	public delegate void CloseAppEventHandler();
	
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

	private void _on_btn_export_pressed()
	{
		var dialog = new FileDialog();
		dialog.FileMode = FileDialog.FileModeEnum.OpenDir;
		dialog.Access = FileDialog.AccessEnum.Filesystem;
		dialog.UseNativeDialog = true;
		dialog.Connect("dir_selected", new Callable(this, MethodName.OnDirSelected));
		AddChild(dialog);
		dialog.PopupCenteredRatio();
	}

	private void OnDirSelected(string path)
	{
		Data_Exporter.Export(path);
		EmitSignal(SignalName.OnBtnSaveSettings);
	}

	private void _on_btn_import_pressed()
	{
		var dialog = new FileDialog();
		dialog.FileMode = FileDialog.FileModeEnum.OpenFile;
		dialog.Access = FileDialog.AccessEnum.Filesystem;
		dialog.UseNativeDialog = true;
		dialog.Connect("file_selected", new Callable(this, MethodName.OnFileSelected));
		AddChild(dialog);
		dialog.PopupCenteredRatio();
	}

	private void OnFileSelected(string path)
	{
		Data_Importer.Import(path);
		EmitSignal(SignalName.OnBtnSaveSettings);
	}

	private void _on_btn_delete_pressed()
	{
		var label = vBoxContainer.GetNode<Label>("HBCDeleteData/LblDeleteData");
		var buttonConfirm = vBoxContainer.GetNode<Button>("HBCDeleteData/BtnConfirm");
		var buttonCancel = vBoxContainer.GetNode<Button>("HBCDeleteData/BtnCancel");
		var buttonDelete = vBoxContainer.GetNode<Button>("HBCDeleteData/BtnDelete");

		label.Text = "Are you sure? The app will close after...";
		buttonConfirm.Visible = true;
		buttonCancel.Visible = true;
		buttonDelete.Visible = false;
	}

	private void _on_btn_delete_cancel_pressed()
	{
		var label = vBoxContainer.GetNode<Label>("HBCDeleteData/LblDeleteData");
		var buttonConfirm = vBoxContainer.GetNode<Button>("HBCDeleteData/BtnConfirm");
		var buttonCancel = vBoxContainer.GetNode<Button>("HBCDeleteData/BtnCancel");
		var buttonDelete = vBoxContainer.GetNode<Button>("HBCDeleteData/BtnDelete");

		label.Text = "Delete all saved data : ";
		buttonConfirm.Visible = false;
		buttonCancel.Visible = false;
		buttonDelete.Visible = true;
	}

	private void _on_btn_delete_confirm_pressed()
	{
		var label = vBoxContainer.GetNode<Label>("HBCDeleteData/LblDeleteData");
		var buttonConfirm = vBoxContainer.GetNode<Button>("HBCDeleteData/BtnConfirm");
		var buttonCancel = vBoxContainer.GetNode<Button>("HBCDeleteData/BtnCancel");
		var buttonDelete = vBoxContainer.GetNode<Button>("HBCDeleteData/BtnDelete");

		label.Text = "Alright... done";
		buttonConfirm.Visible = false;
		buttonCancel.Visible = false;
		buttonDelete.Visible = true;

		Data_Deleter.DeleteAll();
		EmitSignal(SignalName.CloseApp);
	}
}
