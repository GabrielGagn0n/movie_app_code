using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;

class Data_Deleter
{
    static string DIRECTORY = "./Backend/Data/SavedData";

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
