using Godot;
using System;

public partial class AddSingle : Control
{
	[Signal]
	public delegate void OnBtnAddPressedEventHandler();
	[Signal]
	public delegate void OnBtnCancelPressedEventHandler();
	ScrollContainer scrollContainer;
	VBoxContainer vBoxContainer;
	ItemList serialList;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scrollContainer = GetNode<ScrollContainer>("adding-scrolling-container");
		vBoxContainer = scrollContainer.GetNode<VBoxContainer>("MContain/adding-vcontainer");
		serialList = vBoxContainer.GetNode<ItemList>("ItemList");
		serialList.Clear();

		foreach (var item in Enum.GetValues(typeof(TypeSerial)))
		{
			serialList.AddItem(item.ToString());
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_check_button_toggled(bool toggle_on)
	{
		var more_container = vBoxContainer.GetNode<Control>("more-controls");
		more_container.Visible = toggle_on;
	}

	private void _on_btnaddadd_pressed()
	{
		EmitSignal(SignalName.OnBtnAddPressed);
	}

	private void _on_btnaddcancel_pressed()
	{
		EmitSignal(SignalName.OnBtnCancelPressed);
	}
}
