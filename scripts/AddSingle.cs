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
	CheckButton cBtnMoreOpt;
	TextEdit textBoxName;
	TextEdit textBoxAlias;
	TextEdit textBoxLink;
	int selectedID;

	const string NAME_TEXTBOX = "name-textbox";
	const string ALIAS_TEXTBOX = "alias-textbox";
	const string LINK_TEXTBOX = "link-textbox";
	const string GENRE_ITEMLIST = "ItemList";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scrollContainer = GetNode<ScrollContainer>("adding-scrolling-container");
		vBoxContainer = scrollContainer.GetNode<VBoxContainer>("MContain/adding-vcontainer");
		cBtnMoreOpt = vBoxContainer.GetNode<CheckButton>("cBtnMoreOpt");

		textBoxName = vBoxContainer.GetNode<TextEdit>(NAME_TEXTBOX);
		textBoxAlias = vBoxContainer.GetNode<TextEdit>(ALIAS_TEXTBOX);
		textBoxLink = vBoxContainer.GetNode<TextEdit>(LINK_TEXTBOX);
		serialList = vBoxContainer.GetNode<ItemList>(GENRE_ITEMLIST);

		serialList.Clear();

		foreach (var item in Enum.GetValues(typeof(SerialType)))
		{
			serialList.AddItem(item.ToString());
		}
		serialList.Select(0);
	}

	public void ClearData()
	{
		textBoxName.Clear();
		textBoxAlias.Clear();
		textBoxLink.Clear();
		serialList.Select(0);
		cBtnMoreOpt.ButtonPressed = false;
		clearMoreOption();
	}

	// Get the data of every text box and other
	public Serial GetSerial()
	{
		string name = textBoxName.Text;
		string alias = textBoxAlias.Text;
		string link = textBoxLink.Text;
		string genre = serialList.GetItemText(selectedID);

		Serial serial = new Serial(name, alias, link, Enum.Parse<SerialType>(genre));
		LastMinuteFix(serial);
		return serial;
	}

	public int[] GetNbrEpiSeason()
	{
		// TODO
		if (cBtnMoreOpt.ButtonPressed)
		{
			return new int[] {1};
		}
		return new int[] {1};
	}

	// Change the color of the required textbox
	public void ChangeColorRequired()
	{
		textBoxName.AddThemeColorOverride("background_color", Color.FromHtml("#620000"));
	}

	private void _on_check_button_toggled(bool toggle_on)
	{
		var more_container = vBoxContainer.GetNode<Control>("more-controls");
		more_container.Visible = toggle_on;
	}

	//Change some things if need be
	private void LastMinuteFix(Serial serial)
	{
		// Check if the link is valid
		serial.Link = EnsureHyperlink(serial.Link);
	}

	// Taken from ChatGPT.
	private string EnsureHyperlink(string input)
	{
    	if (string.IsNullOrWhiteSpace(input))
    	    return input;

    	// Check if the string is already a valid hyperlink
    	if (Uri.TryCreate(input, UriKind.Absolute, out Uri uriResult) &&
    	    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
    	{
    	    return input;
    	}

    	// Prepend "https://" if it's not a hyperlink
    	return "https://" + input.TrimStart('/');
	}

	private void _on_btnaddadd_pressed()
	{
		EmitSignal(SignalName.OnBtnAddPressed);
	}

	private void _on_btnaddcancel_pressed()
	{
		EmitSignal(SignalName.OnBtnCancelPressed);
	}

	private void _on_item_list_item_selected(int index)
	{
		selectedID = index;
	}

	// Change the color of the textbox if its empty
	private void _on_nametextbox_focus_exited()
	{
		if (string.IsNullOrEmpty(textBoxName.Text))
		{
			textBoxName.AddThemeColorOverride("background_color", Color.FromHtml("#620000"));
		}
		else
		{
			textBoxName.RemoveThemeColorOverride("background_color");
		}
	}

	// TODO
	private void clearMoreOption()
	{

	}
}
