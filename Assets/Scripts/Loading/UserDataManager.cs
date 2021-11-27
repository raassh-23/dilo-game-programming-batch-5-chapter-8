using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager
{
    private const string PROGRESS_KEY = "Progress";
    
    public static UserProgressData Progress;

    public static void Load()
    {
        if (!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            Progress = new UserProgressData();
            Save();
        }
        else
        {
            string json = PlayerPrefs.GetString(PROGRESS_KEY);
            Progress = JsonUtility.FromJson<UserProgressData>(json);
        }
    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(PROGRESS_KEY, json);
    }

    public static bool HasResource(int index) {
        return index + 1 <= Progress.ResourcesLevels.Count;
    }
}
