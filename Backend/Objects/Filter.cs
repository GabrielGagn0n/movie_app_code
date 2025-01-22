using System;

public class Filter
{
    public string NameFilter { get; set; } = null;
    public SerialType[] SerialTypeFilter { get; set; } = Array.Empty<SerialType>();
    public Status[] StatusFilter { get; set; } = Array.Empty<Status>();
    public string DateFilter { get; set; } = null;
}