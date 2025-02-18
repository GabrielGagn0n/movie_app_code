using Godot;
using System;
using System.Collections.Generic;
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
	HBoxContainer searchFilter;
	LineEdit lEditName;
	CheckBox cBoxStrict;
	CheckBox cBoxContain;
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
		cBoxStrict = hBoxContainer.GetNode<CheckBox>("HBCSearch/CBoxStrict");
		cBoxContain = hBoxContainer.GetNode<CheckBox>("HBCSearch/CBoxContain");
		filterBtn = hBoxContainer.GetNode<Button>("FilterBtn");
		IListType = hBoxMoreFilter.GetNode<ItemList>("HBoxType/TypeList");
		IListDate = hBoxMoreFilter.GetNode<ItemList>("HBoxDate/DateList");
		IListStatus = hBoxMoreFilter.GetNode<ItemList>("HBoxStatus/StatusList");
		searchFilter = hBoxContainer.GetNode<HBoxContainer>("HBCSearch");
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
		if (filter != null)
		{
			this.filter = filter;
		}
		else
		{
			filter = new();
		}
        SetOptionsStatus(filter.StatusFilter);
		SetOptionsType(filter.SerialTypeFilter);
		SetOptionsSearchOption(filter.SearchOption);
    }

	private void SetOptionsSearchOption(string searchOption)
	{
		if (searchOption == null || searchOption == "contain")
		{
			cBoxContain.ButtonPressed = true;
			cBoxStrict.ButtonPressed = false;
		}
		else
		{
			cBoxContain.ButtonPressed = false;
			cBoxStrict.ButtonPressed = true;
		}
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
			searchFilter.Visible = true;
		}
		else
		{
			EmitSignal(SignalName.OnMoreOptBtnClicked, 40);
			SetSize(new Vector2(1550, 40));
			MoreFilter = false;
			filterBtn.Text = "Open filters";
			searchFilter.Visible = false;
		}
		hBoxMoreFilter.Visible = MoreFilter;
	}

	private void _on_type_list_multi_selected(int i, bool b)
	{
	    string itemText = IListType.GetItemText(i);
	    SerialType selectedType = (SerialType)Enum.Parse(typeof(SerialType), itemText, true);
	    List<SerialType> toChange = filter.SerialTypeFilter.ToList();

	    if (toChange.Contains(selectedType))
	    {
	        toChange.Remove(selectedType);
	        IListType.Deselect(i);
	    }
	    else
	    {
	        toChange.Add(selectedType);
	    }

	    filter.SerialTypeFilter = toChange.ToArray();
		SetOptionsType(filter.SerialTypeFilter);
	    EmitSignal(SignalName.OnStatusChanged);
	}

	private void _on_status_list_multi_selected(int i, bool b)
	{
	    string itemText = IListStatus.GetItemText(i);
	    Status selectedStatus = (Status)Enum.Parse(typeof(Status), itemText, true);
	    List<Status> toChange = filter.StatusFilter.ToList();

	    if (toChange.Contains(selectedStatus))
	    {
	        toChange.Remove(selectedStatus);
	        IListStatus.Deselect(i); 
	    }
	    else
	    {
	        toChange.Add(selectedStatus);
	    }
	
	    filter.StatusFilter = toChange.ToArray();
		SetOptionsStatus(filter.StatusFilter);
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
		cBoxContain.ButtonPressed = true;
		cBoxStrict.ButtonPressed = false;

		EmitSignal(SignalName.OnStatusChanged);
	}

	private void _on_c_box_contain_pressed()
	{
		if (cBoxContain.ButtonPressed)
			filter.SearchOption = "contain";
		else
			filter.SearchOption = "strict";
			
		cBoxStrict.ButtonPressed = !cBoxContain.ButtonPressed;
		EmitSignal(SignalName.OnStatusChanged);
	}

	private void _on_c_box_strict_pressed()
	{
		if (cBoxStrict.ButtonPressed)
			filter.SearchOption = "strict";
		else
			filter.SearchOption = "contain";

		cBoxContain.ButtonPressed = !cBoxStrict.ButtonPressed;
		EmitSignal(SignalName.OnStatusChanged);
	}

}
