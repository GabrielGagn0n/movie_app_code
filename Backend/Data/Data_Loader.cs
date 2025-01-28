using System;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Godot;

class Data_Loader
{
    const string DIRECTORY = "./Backend/Data/SavedData";

    internal static Serial[] GetAllData()
    {
        if (!Directory.Exists(DIRECTORY))
        {
            throw new DirectoryNotFoundException($"The directory '{DIRECTORY}' does not exist.");
        }

        var toReturn = new List<Serial>();

        var jsonFile = Directory.GetFiles(DIRECTORY, "*.json");

        foreach (var file in jsonFile)
        {
            var serialFromFile = JsonSerializer.Deserialize<List<Serial>>(File.ReadAllText(file));

            if (serialFromFile != null)
            {
                toReturn.AddRange(serialFromFile);
            }
        }

        return toReturn.ToArray();
    }

    internal static Serial[] GetData(Filter filter)
    {
        if (!Directory.Exists(DIRECTORY))
        {
            throw new DirectoryNotFoundException($"The directory '{DIRECTORY}' does not exist.");
        }


        SerialType[] serialList = GetSerialList(filter);
        Status[] statuses = GetStatusList(filter);
        var toReturn = new List<Serial>();
        var jsonFile = new List<string>();

        foreach (var serial in serialList)
        {
            jsonFile.AddRange(Directory.GetFiles(DIRECTORY, serial.ToString() + ".json"));
        }

        foreach (var file in jsonFile)
        {
            var serialFromFile = JsonSerializer.Deserialize<List<Serial>>(File.ReadAllText(file));

            if (serialFromFile != null && !string.IsNullOrEmpty(filter.NameFilter))
            {
                toReturn.AddRange(serialFromFile.Where(x => statuses.Contains(x.Status) && 
                    (x.Alias.ToLower().Contains(filter.NameFilter.ToLower()) || x.Name.ToLower().Contains(filter.NameFilter.ToLower()))));
            }
            else if (serialFromFile != null)
            {
                toReturn.AddRange(serialFromFile.Where(x => statuses.Contains(x.Status)));
            }
        }

        return toReturn.ToArray();
    }

    private static SerialType[] GetSerialList(Filter filter)
    {
        SerialType[] serialList = Array.Empty<SerialType>();
        if (filter.SerialTypeFilter.Length <= 0)
        {
            serialList = Enum.GetValues(typeof(SerialType)).Cast<SerialType>().ToArray();
        }
        else
        {
            serialList = filter.SerialTypeFilter;
        }
        return serialList;
    }

    private static Status[] GetStatusList(Filter filter)
    {
        Status[] statusList = Array.Empty<Status>();
        if (filter.StatusFilter.Length <= 0)
        {
            statusList = Enum.GetValues(typeof(Status)).Cast<Status>().ToArray();
        }
        else
        {
            statusList = filter.StatusFilter;
        }
        return statusList;
    }
}