using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;

public class UserDataManager
{
    private const string PROGRESS_KEY = "Progress";

    public static UserProgressData Progress;

    public static void LoadFromLocal()
    {
        if (!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            Progress = new UserProgressData();
            Save(true);
        }
        else
        {
            string json = PlayerPrefs.GetString(PROGRESS_KEY);
            Progress = JsonUtility.FromJson<UserProgressData>(json);
        }
    }

    public static void Save(bool uploadToCloud = false)
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(PROGRESS_KEY, json);

        if (uploadToCloud)
        {
            AnalyticsManager.SetUserProperties("gold", Progress.Gold.ToString());
            
            byte[] data = Encoding.Default.GetBytes(json);
            StorageReference targetStorage = GetTargetCloudStorage();

            targetStorage.PutBytesAsync(data);
        }
    }

    public static bool HasResource(int index)
    {
        return index + 1 <= Progress.ResourcesLevels.Count;
    }

    public static IEnumerator LoadFromCloud(System.Action onComplete)
    {
        StorageReference targetStorage = GetTargetCloudStorage();

        bool isCompleted = false;
        bool isSuccessfull = false;
        const long maxAllowedSize = 1024 * 1024;

        targetStorage.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
        {
            if (!task.IsFaulted)
            {
                string json = Encoding.Default.GetString(task.Result);
                Progress = JsonUtility.FromJson<UserProgressData>(json);
                isSuccessfull = true;
            }

            isCompleted = true;
        });

        while (!isCompleted)
        {
            yield return null;
        }

        if (isSuccessfull)
        {
            Save();
        }
        else
        {
            LoadFromLocal();
        }

        onComplete?.Invoke();
    }

    private static StorageReference GetTargetCloudStorage()
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        return storage.GetReferenceFromUrl($"{storage.RootReference}/{deviceId}");
    }
}
