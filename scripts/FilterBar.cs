using Godot;
using System;

public partial class FilterBar : Control
{
	[Signal]
	public delegate void OnStatusChangedEventHandler();
	
	MarginContainer mContain;
	VBoxContainer vBoxContainer;
	HBoxContainer hBoxContainer;
	LineEdit lEditName;
	OptionButton OBtnType;
	OptionButton OBtnDate;
	OptionButton OBtnStatus;
	Filter filter = new();

	public override void _Ready()
	{
		mContain = GetNode<MarginContainer>("MCon");
		vBoxContainer = mContain.GetNode<VBoxContainer>("VBoxC");
		hBoxContainer = vBoxContainer.GetNode<HBoxContainer>("HBoxC");
		lEditName = hBoxContainer.GetNode<LineEdit>("LEditName");
		OBtnType = hBoxContainer.GetNode<OptionButton>("OBtnType");
		OBtnDate = hBoxContainer.GetNode<OptionButton>("OBtnDate");
		OBtnStatus = hBoxContainer.GetNode<OptionButton>("OBtnStatus");

		SetOptionsStatus();
		SetOptionsType();
	}

	// public override void _Process(double delta)
	// {
	// }

	public Filter GetFilter()
	{
		return filter;
	}

	private void SetOptionsStatus()
	{
		OBtnStatus.Clear();
		OBtnStatus.AddItem("Filter by status");
		foreach (var item in Enum.GetValues(typeof(Status)))
		{
			OBtnStatus.AddItem(item.ToString());
		}

		// TODO : Maybe select index of last time it was closed
		OBtnStatus.Select(0);
	}

	private void SetOptionsType()
	{
		OBtnType.Clear();
		OBtnType.AddItem("Filter by status");
		foreach (var item in Enum.GetValues(typeof(SerialType)))
		{
			OBtnType.AddItem(item.ToString());
		}

		// TODO : Maybe select index of last time it was closed
		OBtnType.Select(0);
	}

	private void _on_o_btn_type_item_selected(int i)
	{
		if (i != 0)
			filter.SerialTypeFilter = (SerialType)Enum.Parse(typeof(SerialType), OBtnType.GetItemText(i), true);
		else
			filter.SerialTypeFilter = null;

		EmitSignal(SignalName.OnStatusChanged);
	}

	private void _on_o_btn_status_item_selected(int i)
	{
		if (i != 0)
			filter.StatusFilter = (Status)Enum.Parse(typeof(Status), OBtnStatus.GetItemText(i), true);
		else
		 	filter.StatusFilter= null;

		EmitSignal(SignalName.OnStatusChanged);
	}

	private void _on_o_btn_date_item_selected(int i)
	{
		if (i == 0)
			filter.DateFilter = null;
		if (i == 1)
			filter.DateFilter = "asc";
		if (i == 2)
			filter.DateFilter = "desc";

		EmitSignal(SignalName.OnStatusChanged);
	}

	private void _on_l_edit_name_text_submitted(string s)
	{
		if (string.IsNullOrEmpty(s))
			filter.NameFilter = null;
		else
			filter.NameFilter = s;

		EmitSignal(SignalName.OnStatusChanged);
	}
}
