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
		SetOptionsType();
		SetOptionsStatus();
	}

	// public override void _Process(double delta)
	// {
	// }

	public Filter GetFilter()
	{
		return filter;
	}
	
    internal void SetFilter(Filter filter)
    {
        SetOptionsStatus(filter.StatusFilter);
		SetOptionsType(filter.SerialTypeFilter);
    }

	private void SetOptionsStatus(Status[] existingFilter = null)
	{
		IListStatus.Clear();
	    Status[] values = (Status[])Enum.GetValues(typeof(Status));

		for (int i = 0; i < values.Length; i++)
	    {
	        Status item = values[i];
	        IListStatus.AddItem(item.ToString());
	
	        if (existingFilter != null && Array.IndexOf(existingFilter, item) >= 0)
	        {
	            IListStatus.Select(i, false);
	        }
	    }
	}

	private void SetOptionsType(SerialType[] existingFilter = null)
	{
	    IListType.Clear();
	    SerialType[] values = (SerialType[])Enum.GetValues(typeof(SerialType));
	
	    for (int i = 0; i < values.Length; i++)
	    {
	        SerialType item = values[i];
	        IListType.AddItem(item.ToString());
	
	        if (existingFilter != null && Array.IndexOf(existingFilter, item) >= 0)
	        {
	            IListType.Select(i, false);
	        }
	    }
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
