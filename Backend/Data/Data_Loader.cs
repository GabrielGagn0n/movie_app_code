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
}