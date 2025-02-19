using System;

public class Filter
{
    public string NameFilter { get; set; } = "";
    public SerialType[] SerialTypeFilter { get; set; } = Array.Empty<SerialType>();
    public Status[] StatusFilter { get; set; } = Array.Empty<Status>();
    public SortOptions SortOption { get; set; } = (SortOptions)0;
    public string SearchOption { get; set; } = null;
}