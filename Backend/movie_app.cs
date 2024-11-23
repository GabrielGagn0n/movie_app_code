using System;
using Godot;

class movie_app 
{
    private Serials[] serials_list;

    public movie_app()
    {
        GD.Print("BACKEND ON!");
        load_data();
    }

    public void update_serials_add_watched(int id, string name)
    {
        Serials serial;
        for (int i = 0; i < serials_list.Length - 1; i++)
        {
            if (serials_list[i].get_Id() == id || serials_list[i].get_name() == name)
            {
                serial = serials_list[i];
                serial.add_watched_episode();
                Data_Saver.Single_Save_Data(serial);
                break;
            }
        }
    }

    public void update_serials_removed_watched(int id, string name)
    {
        Serials serial;
        for (int i = 0; i < serials_list.Length - 1; i++)
        {
            if (serials_list[i].get_Id() == id || serials_list[i].get_name() == name)
            {
                serial = serials_list[i];
                serial.removed_watched_episode();
                Data_Saver.Single_Save_Data(serial);
                break;
            }
        }
    }

    private void load_data()
    {
        serials_list = Data_Loader.Get_Data();
    }

    private void save_data()
    {
        Data_Saver.Save_Data(serials_list);
    }
}