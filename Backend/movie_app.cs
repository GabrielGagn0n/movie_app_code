using System;
using System.Linq;
using Godot;

class movie_app 
{
    private Serials[] serials_list = Array.Empty<Serials>();

    public movie_app()
    {
        GD.Print("BACKEND ON!");
        //load_data();
    }

    public void AddSerial(Serials serial)
    {
        int id = GenerateID();
        serial.set_Id(id);
        serials_list.Append(serial);

        SaveData();
    }

    public void UpdateSerialsAddWatched(int id, string name)
    {
        Serials serial;
        for (int i = 0; i < serials_list.Length - 1; i++)
        {
            if (serials_list[i].get_Id() == id || serials_list[i].get_name() == name)
            {
                serial = serials_list[i];
                serial.AddWatchedEpisode();
                serial.set_Status(Status.Watching);
                Data_Saver.SingleSaveData(serial);
                break;
            }
        }
    }

    public void UpdateSerialsRemovedWatched(int id, string name)
    {
        Serials serial;
        for (int i = 0; i < serials_list.Length - 1; i++)
        {
            if (serials_list[i].get_Id() == id || serials_list[i].get_name() == name)
            {
                serial = serials_list[i];
                serial.RemovedWatchedEpisode();
                Data_Saver.SingleSaveData(serial);
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
        Data_Saver.SaveData(serials_list);
    }

    // TODO : Find a better way to generate ids
    private int GenerateID()
    {
        return serials_list.Count();
    } 
}