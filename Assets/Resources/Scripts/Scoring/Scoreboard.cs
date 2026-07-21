using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// local solo leaderboard for Circuit_Solo, saved as json in persistentDataPath, sorted by score
public static class Scoreboard
{
    [Serializable]
    public class Entry
    {
        public float score;
        public float totalTime;
        public float bestLap;
        public string vehicle;
        public long dateTicks;
    }

    [Serializable]
    class Wrapper
    {
        public List<Entry> entries = new List<Entry>();
    }

    const int MaxStored = 50;

    static string FilePath
    {
        get { return Path.Combine(Application.persistentDataPath, "Scoreboard", "circuit_solo.json"); }
    }

    public static List<Entry> Load()
    {
        try
        {
            string path = FilePath;
            if (!File.Exists(path))
                return new List<Entry>();

            string json = File.ReadAllText(path);
            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);
            if (wrapper == null || wrapper.entries == null)
                return new List<Entry>();

            wrapper.entries.Sort(CompareByScoreDesc);
            return wrapper.entries;
        }
        catch (Exception e)
        {
            Debug.LogError("Scoreboard load failed: " + e);
            return new List<Entry>();
        }
    }

    // adds a result, saves, and gives back its rank (1-based)
    public static int Add(Entry entry)
    {
        List<Entry> entries = Load();
        entries.Add(entry);
        entries.Sort(CompareByScoreDesc);

        int rank = entries.IndexOf(entry) + 1;

        if (entries.Count > MaxStored)
            entries.RemoveRange(MaxStored, entries.Count - MaxStored);

        Save(entries);
        return rank;
    }

    public static List<Entry> Top(int count)
    {
        List<Entry> entries = Load();
        if (entries.Count > count)
            entries.RemoveRange(count, entries.Count - count);
        return entries;
    }

    static void Save(List<Entry> entries)
    {
        try
        {
            string path = FilePath;
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            Wrapper wrapper = new Wrapper { entries = entries };
            File.WriteAllText(path, JsonUtility.ToJson(wrapper, true));
        }
        catch (Exception e)
        {
            Debug.LogError("Scoreboard save failed: " + e);
        }
    }

    static int CompareByScoreDesc(Entry a, Entry b)
    {
        // best score first, ties broken by the faster time
        int byScore = b.score.CompareTo(a.score);
        if (byScore != 0) return byScore;
        return a.totalTime.CompareTo(b.totalTime);
    }
}
