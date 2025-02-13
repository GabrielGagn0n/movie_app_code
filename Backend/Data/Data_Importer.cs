using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;

class Data_Importer
{
    public static void Import(string path)
    {
        if (!File.Exists(path))
        {
            OS.Alert("File doesn't exist", "Error!");
        }
        try
        {
            List<Serial> existingSerials = File.Exists(path)
                ? JsonSerializer.Deserialize<List<Serial>>(File.ReadAllText(path))
                : new List<Serial>();

            foreach (Serial serial in existingSerials)
            {
                Data_Saver.AddData(serial);
            }

            GD.Print(existingSerials.Count + " serial was imported");
        }
        catch (Exception ex)
        {
            OS.Alert(ex.ToString(), "Error happened while importing");
        }
    }
}