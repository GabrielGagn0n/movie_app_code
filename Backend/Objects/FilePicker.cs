using Godot;

public partial class FilePicker : Node
{
    private FileDialog _dialog;

    public override void _Ready()
    {
        _dialog = new FileDialog();
        _dialog.FileMode = FileDialog.FileModeEnum.OpenDir;
        _dialog.Access = FileDialog.AccessEnum.Filesystem;
        _dialog.UseNativeDialog = true;
        _dialog.DirSelected += OnDirSelected;
        AddChild(_dialog);
    }

    private void OnFilePickerPressed()
    {
        _dialog.PopupCenteredRatio();
    }

    private void OnDirSelected(string path)
    {
        string selectedPath = path;
        GD.Print("Selected Directory: " + selectedPath);
    }
}
