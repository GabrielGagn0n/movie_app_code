using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;

class Data_Deleter
{
    static string DIRECTORY = OS.GetDataDir() + "/movie_app/SavedData";

    internal static void DeleteAll()
    {
        string folderPath = Path.Combine(OS.GetDataDir(), "movie_app");

        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException($"The directory '{folderPath}' does not exist.");
        }

        Directory.Delete(folderPath, true);
    }


    internal static void DeleteFilter()
    {
        if (!Directory.Exists(DIRECTORY))
        {
            throw new DirectoryNotFoundException($"The directory '{DIRECTORY}' does not exist.");
        }
        string originalFilePath = Path.Combine(DIRECTORY, $"SavedFilter.json");

        if (!File.Exists(originalFilePath))
        {
            return;
        }
        try
        {
            string json = File.ReadAllText(originalFilePath);
        }
        catch (Exception ex)
        {
            GD.Print($"Error reading or deserializing JSON: {ex.Message}");
            return;
        }

        try
        {
            File.WriteAllText(originalFilePath, JsonSerializer.Serialize(new Filter(), new JsonSerializerOptions { WriteIndented = false }));
        }
        catch (Exception ex)
        {
            GD.Print($"Error writing to file: {ex.Message}");
        }
    }

    internal static void DeleteSerial(Serial serial)
    {
        if (!Directory.Exists(DIRECTORY))
        {
            throw new DirectoryNotFoundException($"The directory '{DIRECTORY}' does not exist.");
        }

        string originalFilePath = Path.Combine(DIRECTORY, $"{serial.Type}.json");

        if (!File.Exists(originalFilePath))
        {
            return;
        }

        List<Serial> existingSerials;
        try
        {
            string json = File.ReadAllText(originalFilePath);
            existingSerials = JsonSerializer.Deserialize<List<Serial>>(json) ?? new List<Serial>();
        }
        catch (Exception ex)
        {
            GD.Print($"Error reading or deserializing JSON: {ex.Message}");
            return;
        }

        int removedCount = existingSerials.RemoveAll(s => s.Id == serial.Id);
        if (removedCount > 0)
        {
            try
            {
                File.WriteAllText(originalFilePath, JsonSerializer.Serialize(existingSerials, new JsonSerializerOptions { WriteIndented = false }));
            }
            catch (Exception ex)
            {
                GD.Print($"Error writing to file: {ex.Message}");
            }
        }
    }
}
