using System;
using System.Linq;
using Godot;

class movie_app 
{
    private Serial[] serials_list = Array.Empty<Serial>();

    public movie_app()
    {
        GD.Print("BACKEND ON!");

        try
        {
            LoadData();
        }
        catch (System.Exception)
        {
            
        }
    }

    public void AddSerial(Serial serial)
    {
        int id = GenerateID();
        serial.Id = id;

        // TODO : If it exist already, show something
        AddData(serial);
        LoadData();
    }

    public void UpdateSerials(ButtonViewActions action, int id = -1, string name = "")
    {
        for (int i = 0; i <= serials_list.Length - 1; i++)
        {
            if (serials_list[i].Id == id || serials_list[i].Name == name)
            {
                Serial serial = serials_list[i];

                switch (action)
                {
                    case ButtonViewActions.AddEpisode:
                        serial.AddWatchedEpisode();
                        break;
                    case ButtonViewActions.RemoveEpisode:
                        serial.RemoveWatchedEpisode();
                        break;
                    case ButtonViewActions.AddSeason:
                        serial.AddWatchedSeason();
                        break;
                    case ButtonViewActions.RemoveSeason:
                        serial.RemoveWatchedSeason();
                        break;
                }

                serial.Status = Status.Watching;
                serial.LatestUpdate = DateTime.Now;
                Data_Saver.SaveSingleData(serial);
                break;
            }
        }
    }


    public Serial[] GetSerials()
    {
        return this.serials_list;
    }

    private void LoadData()
    {
        serials_list = Data_Loader.GetAllData();
    }

    private void AddData(Serial serial)
    {
        Data_Saver.AddData(serial);
    }

    // TODO : Find a better way to generate ids
    private int GenerateID()
    {
        return serials_list.Count();
    } 
}