using System;
using System.Linq;
using Godot;

class movie_app 
{
    private Settings settings;
    private Filter filter = null;
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
            throw;
        }
    }

    public void SetSettings(Settings settings)
    {
        this.settings = settings;

        if (settings.autoSwitch)
            UpdateStatusOnHold();
        
        if (settings.saveFilters)
        {
            SetFilter(Data_Loader.LoadFilter());
        }
        else
        {
            SetFilter(null);
        }
    }

    public Settings GetSettings()
    {
        return this.settings;
    }

    public void SetFilter(Filter filter)
    {
        this.filter = filter;
        if (settings.saveFilters && filter != null)
            Data_Saver.SaveFilter(this.filter);
    }

    public Filter GetFilter()
    {
        return this.filter;
    }

    public void AddSerial(Serial serial)
    {
        Guid id = GenerateID();
        serial.Id = id;

        // TODO : If it exist already, show something
        AddData(serial);
        LoadData();
    }

    public void UpdateSerials(ButtonViewActions action, Guid id, string name = "")
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
                        serial.UpdateStatus();
                        serial.LatestUpdate = DateTime.Now;
                        break;
                    case ButtonViewActions.RemoveEpisode:
                        serial.RemoveWatchedEpisode();
                        serial.LatestUpdate = DateTime.Now;
                        break;
                    case ButtonViewActions.AddSeason:
                        serial.AddWatchedSeason();
                        serial.UpdateStatus();
                        serial.LatestUpdate = DateTime.Now;
                        break;
                    case ButtonViewActions.RemoveSeason:
                        serial.RemoveWatchedSeason();
                        break;
                    case ButtonViewActions.UpdateSerial:
                        serial.LatestUpdate = DateTime.Now;
                        break;
                    case ButtonViewActions.Rewatch:
                        serial.Rewatch();
                        serial.LatestUpdate = DateTime.Now;
                        break;
                }
                Data_Saver.SaveSingleData(serial);
                break;
            }
        }
    }


    public Serial[] GetSerials()
    {
        SortData(filter, serials_list);
        return this.serials_list;
    }

    public Serial[] GetFilteredSerial(Filter filter)
    {
        LoadDataFiltered(filter);
        SortData(filter, serials_list);
        return this.serials_list;
    }

    internal void DeleteSerial(Guid id)
    {
        for (int i = 0; i <= serials_list.Length - 1; i++)
        {
            if (serials_list[i].Id == id)
            {
                Serial serial = serials_list[i];
                Data_Deleter.DeleteSerial(serial);
            }
        }
    }

    private void LoadData()
    {
        serials_list = Data_Loader.GetAllData();
    }

    private void LoadDataFiltered(Filter filter)
    {
        serials_list = Data_Loader.GetData(filter);
    }

    private void UpdateStatusOnHold()
    {
        foreach (var serial in serials_list)
        {
            if (serial.Status == Status.Watching && serial.LatestUpdate <= DateTime.Now.AddDays(-settings.autoSwitchTime))
            {
                serial.UpdateStatus(Status.OnHold);
                Data_Saver.SaveSingleData(serial);
            }
        }
    }

    private void SortData(Filter filter, Serial[] serial)
    {
        if (filter == null || (int) filter.SortOption == 0)
        {
            serials_list = serial.OrderBy(serial => serial.Name).ToArray();
        }
        else if ((int) filter.SortOption == 1)
        {
            serials_list = serial.OrderByDescending(serial => serial.Name).ToArray();
        }
        else if ((int) filter.SortOption == 2)
        {
            serials_list = serial.OrderBy(serial => serial.LatestUpdate).ToArray();
        }
        else if ((int)filter.SortOption == 3)
        {
            serials_list = serial.OrderByDescending(serial => serial.LatestUpdate).ToArray();
        }
    }

    private void AddData(Serial serial)
    {
        Data_Saver.AddData(serial);
    }

    // TODO : Find a better way to generate ids
    private Guid GenerateID()
    {
        return Guid.NewGuid();
    }
}