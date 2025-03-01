using System;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Godot;
using System.Text.RegularExpressions;

class Data_Loader
{
    static string DIRECTORY = OS.GetDataDir() + "/movie_app/SavedData";

    internal static void SetDirectory(string directory)
    {
        DIRECTORY = directory;
    }

    internal static Serial[] GetAllData()
    {
        return GetData(new Filter());
    }

    internal static Serial[] GetData(Filter filter)
    {
        if (!Directory.Exists(DIRECTORY))
        {
            try
            {
                Directory.CreateDirectory(DIRECTORY);
            }
            catch (System.Exception)
            {
                throw;
            }
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

            if (serialFromFile != null && !string.IsNullOrEmpty(filter.NameFilter) && filter.SearchOption == "contain")
            {
                toReturn.AddRange(serialFromFile.Where(x => statuses.Contains(x.Status) && 
                    (x.Alias.ToLower().Contains(filter.NameFilter.ToLower()) || x.Name.ToLower().Contains(filter.NameFilter.ToLower()))));
            }
            else if (serialFromFile != null && !string.IsNullOrEmpty(filter.NameFilter) && filter.SearchOption == "strict")
            {
                var regex = new Regex($"^{Regex.Escape(filter.NameFilter)}", RegexOptions.IgnoreCase);
                toReturn.AddRange(serialFromFile.Where(x => statuses.Contains(x.Status) && 
                    (regex.IsMatch(x.Alias) || regex.IsMatch(x.Name))));
            }
            else if (serialFromFile != null)
            {
                toReturn.AddRange(serialFromFile.Where(x => statuses.Contains(x.Status)));
            }
        }

        return toReturn.ToArray();
    }

    internal static Settings LoadSettings()
    {
        if (!Directory.Exists(DIRECTORY))
        {
            throw new DirectoryNotFoundException($"The directory '{DIRECTORY}' does not exist.");
        }

        var jsonFiles = Directory.GetFiles(DIRECTORY, "Settings.json");
        if (jsonFiles.Length == 0)
        {
            return null;
        }

        try
        {
            string jsonContent = File.ReadAllText(jsonFiles[0]);
            return JsonSerializer.Deserialize<Settings>(jsonContent);
        }
        catch (Exception ex)
        {
            GD.Print($"Error loading settings: {ex.Message}");
            return null;
        }
    }

    internal static Filter LoadFilter()
    {
        if (!Directory.Exists(DIRECTORY))
        {
            throw new DirectoryNotFoundException($"The directory '{DIRECTORY}' does not exist.");
        }

        var jsonFiles = Directory.GetFiles(DIRECTORY, "SavedFilter.json");
        if (jsonFiles.Length == 0)
        {
            return null;
        }

        try
        {
            string jsonContent = File.ReadAllText(jsonFiles[0]);
            return JsonSerializer.Deserialize<Filter>(jsonContent);
        }
        catch (Exception ex)
        {
            GD.Print($"Error loading settings: {ex.Message}");
            return null;
        }
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