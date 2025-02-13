using System;
using System.IO;
using System.Text.Json;
using Godot;

class Data_Exporter
{
    public static void Export(string path)
    {
        Serial[] serials = Data_Loader.GetAllData();
        try
        {
            if (Directory.Exists(path))
            {
                path = Path.Combine(path, "exported_data.json");
            }
    
            string json = JsonSerializer.Serialize(serials, new JsonSerializerOptions { WriteIndented = false });
    
            File.WriteAllText(path, json);
            GD.Print("Exported " + serials.Length + " serial to " + path);
        }
        catch (Exception ex)
        {
            OS.Alert(ex.ToString(), "Error!");
        }
    }
}