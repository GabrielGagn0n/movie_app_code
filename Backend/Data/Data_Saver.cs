using System;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Godot;

class Data_Saver
{
    const string DIRECTORY = "./Backend/Data/SavedData";
    internal static void SaveAllData(Serial[] serials_list)
    {
        foreach (SerialType type in Enum.GetValues(typeof(SerialType)))
        {
            Serial[] serialToSave = serials_list.Where(serial => serial.Type == type).ToArray();
            if (serialToSave.Length > 0)
            {
                
            }
        }
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

            File.WriteAllText(filePath, JsonSerializer.Serialize(existingSerials, new JsonSerializerOptions { WriteIndented = true }));
        }
        else
        {
            GD.Print($"Serial with Name '{serial.Name}' and Id '{serial.Id}' already exists.");
        }
    }

    internal static void SaveSingleData(Serial serial)
    {
        throw new NotImplementedException();
    }
}