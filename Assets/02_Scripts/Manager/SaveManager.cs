using System;
using System.IO;
using UnityEngine;

public class SaveManager
{
    private const string SAVE_FILE_NAME = "user.json";

    public UserData User { get; private set; } = new();

    public static string SavePath
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        }
    }

    public void Load()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                string json = File.ReadAllText(SavePath);
                UserData loaded = JsonUtility.FromJson<UserData>(json);

                User = null != loaded ? loaded : new UserData();
            }
            catch (Exception exception)
            {
                Logger.LogError($"세이브 로드 실패, 새로 시작합니다. {exception.Message}");
                User = new UserData();
            }
        }
        else
        {
            User = new UserData();
        }

        User.EnsureDefaults();
    }

    public void Save()
    {
        try
        {
            File.WriteAllText(SavePath, JsonUtility.ToJson(User));
        }
        catch (Exception exception)
        {
            Logger.LogError($"세이브 저장 실패. {exception.Message}");
        }
    }

    public void DeleteAll()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        User = new UserData();
        User.EnsureDefaults();
    }
}
