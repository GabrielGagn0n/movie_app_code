using System;
using System.Linq;
using Godot;

class movie_app 
{
    private Serials[] serials_list = Array.Empty<Serials>();

    public movie_app()
    {
        GD.Print("BACKEND ON!");
        //LoadData();
    }

    public void AddSerial(Serials serial)
    {
        int id = GenerateID();
        serial.Id = id;

        // TODO : If it exist already, show something
        AddData(serial);
        //LoadData();
    }

    public void UpdateSerialsAddWatched(int id, string name)
    {
        Serials serial;
        for (int i = 0; i < serials_list.Length - 1; i++)
        {
            if (serials_list[i].Id == id || serials_list[i].Name == name)
            {
                serial = serials_list[i];
                serial.AddWatchedEpisode();
                serial.Status = Status.Watching;
                Data_Saver.SaveSingleData(serial);
                break;
            }
        }
    }

    public void UpdateSerialsRemovedWatched(int id, string name)
    {
        Serials serial;
        for (int i = 0; i < serials_list.Length - 1; i++)
        {
            if (serials_list[i].Id == id || serials_list[i].Name == name)
            {
                serial = serials_list[i];
                serial.RemoveWatchedEpisode();
                Data_Saver.SaveSingleData(serial);
                break;
            }
        }
    }

    private void LoadData()
    {
        serials_list = Data_Loader.GetData();
    }

    private void SaveData()
    {
        Data_Saver.SaveAllData(serials_list);
    }

    private void AddData(Serials serial)
    {
        Data_Saver.AddData(serial);
    }

    // TODO : Find a better way to generate ids
    private int GenerateID()
    {
        return serials_list.Count();
    } 
}