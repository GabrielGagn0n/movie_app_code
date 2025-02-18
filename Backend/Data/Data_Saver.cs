using System;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Godot;

class Data_Saver
{
    static string DIRECTORY = OS.GetDataDir() + "/movie_app/SavedData";

    internal static void SetDirectory(string directory)
    {
        DIRECTORY = directory;
    }

    internal static void AddData(Serial serial)
    {
        if (!Directory.Exists(DIRECTORY))
        {
            Directory.CreateDirectory(DIRECTORY);
        }

        string filePath = Path.Combine(DIRECTORY, $"{serial.Type}.json");

        List<Serial> existingSerials = File.Exists(filePath)
            ? JsonSerializer.Deserialize<List<Serial>>(File.ReadAllText(filePath))
            : new List<Serial>();
        
        if (!existingSerials.Any(s => s.Name == serial.Name))
        {
            existingSerials.Add(serial);

            File.WriteAllText(filePath, JsonSerializer.Serialize(existingSerials, new JsonSerializerOptions { WriteIndented = false }));
        }
        else
        {
            GD.Print(serial.Name + " already exist");
        }
    }

    internal static void SaveSingleData(Serial serial)
    {
        if (!Directory.Exists(DIRECTORY))
        {
            Directory.CreateDirectory(DIRECTORY);
        }

        string originalFilePath = Path.Combine(DIRECTORY, $"{serial.Type}.json");
        List<Serial> existingSerials = File.Exists(originalFilePath)
            ? JsonSerializer.Deserialize<List<Serial>>(File.ReadAllText(originalFilePath))
            : new List<Serial>();

        var toModify = existingSerials.FirstOrDefault(s => s.Id == serial.Id);
        if (toModify != null)
        {
            // Update the existing entry
            toModify.Name = serial.Name;
            toModify.Alias = serial.Alias;
            toModify.Link = serial.Link;
            toModify.Type = serial.Type;
            toModify.Status = serial.Status;
            toModify.EpisodeSeasons = serial.EpisodeSeasons;
            toModify.DidWatch = serial.DidWatch;
            toModify.LatestUpdate = serial.LatestUpdate;

            File.WriteAllText(originalFilePath, JsonSerializer.Serialize(existingSerials, new JsonSerializerOptions { WriteIndented = false }));
            return;
        }

        string realOriginalFilePath = "";
        foreach (var type in Enum.GetValues(typeof(SerialType)))
        {
            if (type.ToString() == serial.Type.ToString())
            {
                continue;
            }
            
            realOriginalFilePath = Path.Combine(DIRECTORY, $"{type}.json");
            existingSerials = File.Exists(realOriginalFilePath)
                ? JsonSerializer.Deserialize<List<Serial>>(File.ReadAllText(realOriginalFilePath))
                : new List<Serial>();
            toModify = existingSerials.FirstOrDefault(s => s.Id == serial.Id);
            if (toModify != null)
            {
                break;
            }
        }

        if (toModify != null)
            ChangeFile(realOriginalFilePath, originalFilePath, toModify, serial);
        else
            throw new Exception(string.Format("The serial with the id:{0} doesn't exist.", serial.Id.ToString()));
    }

    private static void ChangeFile(string originalFilePath, string newFilePath, Serial toRemove, Serial toAdd)
    {
        List<Serial> toRemoveSerials = File.Exists(originalFilePath)
            ? JsonSerializer.Deserialize<List<Serial>>(File.ReadAllText(originalFilePath))
            : new List<Serial>();
        List<Serial> toAddSerials = File.Exists(newFilePath)
            ? JsonSerializer.Deserialize<List<Serial>>(File.ReadAllText(newFilePath))
            : new List<Serial>();

        toRemoveSerials.RemoveAll(s => s.Id == toRemove.Id);
        File.WriteAllText(originalFilePath, JsonSerializer.Serialize(toRemoveSerials, new JsonSerializerOptions { WriteIndented = false }));

        toAddSerials.Add(toAdd);
        File.WriteAllText(newFilePath, JsonSerializer.Serialize(toAddSerials, new JsonSerializerOptions { WriteIndented = false }));
    }

    internal static void SaveSettings(Settings settings)
    {
        if (!Directory.Exists(DIRECTORY))
        {
            Directory.CreateDirectory(DIRECTORY); 
        }

        try
        {
            string jsonContent = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = false });
            File.WriteAllText(Path.Combine(DIRECTORY, "Settings.json"), jsonContent);
        }
        catch (Exception ex)
        {
            GD.Print($"Error saving settings: {ex.Message}");
        }
    }

    internal static void SaveFilter(Filter filter)
    {
        if (!Directory.Exists(DIRECTORY))
        {
            Directory.CreateDirectory(DIRECTORY);
        }

        try
        {
            string jsonContent = JsonSerializer.Serialize(filter, new JsonSerializerOptions { WriteIndented = false });
            File.WriteAllText(Path.Combine(DIRECTORY, "SavedFilter.json"), jsonContent);
        }
        catch (Exception ex)
        {
            GD.Print($"Error saving settings: {ex.Message}");
        }
    }
}