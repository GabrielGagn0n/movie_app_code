using Godot;
using System;
using System.Linq;

public partial class FilterBar : Control
{
	[Signal]
	public delegate void OnStatusChangedEventHandler();

	[Signal]
	public delegate void OnMoreOptBtnClickedEventHandler(int size_y);
	
	MarginContainer mContain;
	VBoxContainer vBoxContainer;
	HBoxContainer hBoxContainer;
	HBoxContainer hBoxMoreFilter;
	LineEdit lEditName;
	Button filterBtn;
	ItemList IListType;
	ItemList IListDate;
	ItemList IListStatus;
	Filter filter = new();
	bool MoreFilter = false;

	public override void _Ready()
	{
		mContain = GetNode<MarginContainer>("MCon");
		vBoxContainer = mContain.GetNode<VBoxContainer>("VBoxC");
		hBoxContainer = vBoxContainer.GetNode<HBoxContainer>("HBoxC");
		hBoxMoreFilter = vBoxContainer.GetNode<HBoxContainer>("HBoxMoreFilter");
		lEditName = hBoxContainer.GetNode<LineEdit>("LEditName");
		filterBtn = hBoxContainer.GetNode<Button>("FilterBtn");
		IListType = hBoxMoreFilter.GetNode<ItemList>("HBoxType/TypeList");
		IListDate = hBoxMoreFilter.GetNode<ItemList>("HBoxDate/DateList");
		IListStatus = hBoxMoreFilter.GetNode<ItemList>("HBoxStatus/StatusList");

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
		IListStatus.Clear();
		foreach (var item in Enum.GetValues(typeof(Status)))
		{
			IListStatus.AddItem(item.ToString());
		}

		// TODO : Maybe select index of last time it was closed
	}

	private void SetOptionsType()
	{
		IListType.Clear();
		foreach (var item in Enum.GetValues(typeof(SerialType)))
		{
			IListType.AddItem(item.ToString());
		}

		// TODO : Maybe select index of last time it was closed
	}

	private void _on_l_edit_name_text_submitted(string s)
	{
		if (string.IsNullOrEmpty(s))
			filter.NameFilter = null;
		else
			filter.NameFilter = s;

		EmitSignal(SignalName.OnStatusChanged);
	}

	private void _on_type_btn_pressed()
	{
		if (!MoreFilter)
		{
			EmitSignal(SignalName.OnMoreOptBtnClicked, 150);
			SetSize(new Vector2(1550, 150));
			MoreFilter = true;
			filterBtn.Text = "Close filters";
		}
		else
		{
			EmitSignal(SignalName.OnMoreOptBtnClicked, 40);
			SetSize(new Vector2(1550, 40));
			MoreFilter = false;
			filterBtn.Text = "Open filters";

		}
		hBoxMoreFilter.Visible = MoreFilter;
	}

	private void _on_type_list_multi_selected(int i, bool b)
	{
		SerialType[] toChange = Array.Empty<SerialType>();
		foreach (int selected in IListType.GetSelectedItems())
		{
			toChange = toChange.Append((SerialType)Enum.Parse(typeof(SerialType), IListType.GetItemText(selected), true)).ToArray();
		}

		filter.SerialTypeFilter = toChange;
		EmitSignal(SignalName.OnStatusChanged);
	}

	private void _on_status_list_multi_selected(int i, bool b)
	{
		Status[] toChange = Array.Empty<Status>();
		foreach (int selected in IListStatus.GetSelectedItems())
		{
			toChange = toChange.Append((Status)Enum.Parse(typeof(Status), IListStatus.GetItemText(selected), true)).ToArray();
		}

		filter.StatusFilter = toChange;
		EmitSignal(SignalName.OnStatusChanged);
	}

	private void _on_date_list_item_selected(int i)
	{
		if (i == 0)
		{
			filter.DateFilter = "asc";
		}
		else
		{
			filter.DateFilter = "desc";
		}
		EmitSignal(SignalName.OnStatusChanged);
	}

	private void _on_clear_btn_pressed()
	{
		IListDate.DeselectAll();
		IListStatus.DeselectAll();
		IListType.DeselectAll();
		lEditName.Text = "";
		
		filter.StatusFilter = Array.Empty<Status>();
		filter.SerialTypeFilter = Array.Empty<SerialType>();
		filter.NameFilter = null;

		EmitSignal(SignalName.OnStatusChanged);
	}
}
