using System;
using System.Linq;
using System.Text.Json;

// TODO : Update the DidWatch from the EpisodeSeason, if I add more should add the difference
public class Serials
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public int[] EpisodeSeasons { get; set; } = Array.Empty<int>();
    public bool[] DidWatch { get; set; } = Array.Empty<bool>();
    public string Link { get; set; }
    public DateTime LatestUpdate { get; set; }
    public int Id { get; set; }
    public SerialType Type { get; set; }
    public Status Status { get; set; } = Status.NotStarted;

    public Serials(string name, string alias = null, string link = null, SerialType type = SerialType.None)
    {
        Name = name;
        Alias = alias;
        Link = link;
        Type = type;
    }

    // Add a watched episode
    public void AddWatchedEpisode()
    {
        int index = GetIndexLatestWatchedEpisode();
        if (index + 1 < DidWatch.Length)
        {
            DidWatch[index + 1] = true;
        }
        else
        {
            throw new IndexOutOfRangeException("No more episodes to mark as watched.");
        }
    }

    // Remove the last watched episode
    public void RemoveWatchedEpisode()
    {
        int index = GetIndexLatestWatchedEpisode();
        if (index >= 0)
        {
            DidWatch[index] = false;
        }
        else
        {
            throw new InvalidOperationException("No watched episodes to remove.");
        }
    }

    // Get the index of the last watched episode
    public int GetIndexLatestWatchedEpisode()
    {
        return LastTrue(0, DidWatch.Length - 1, DidWatch);
    }

    // Binary search for the last true in a boolean array
    private int LastTrue(int l, int r, bool[] array)
    {
        if (l > r)
        {
            return -1;
        }

        int m = l + (r - l) / 2;

        if (array[m])
        {
            int result = LastTrue(m + 1, r, array);
            return result == -1 ? m : result;
        }
        else
        {
            return LastTrue(l, m - 1, array);
        }
    }
}
