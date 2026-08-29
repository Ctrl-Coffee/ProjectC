using UnityEngine;

public static class Utils
{
    public static T ParseEnum<T>(string value, T defaultValue = default) where T : struct, System.Enum
    {
        if (System.Enum.TryParse(value, true, out T result) && System.Enum.IsDefined(typeof(T), result))
        {
            return result;
        }

        Logger.LogError($"{typeof(T).Name} 값을 해석할 수 없습니다. value: {value}");

        return defaultValue;
    }

    public static string FormatClock(float seconds)
    {
        if (seconds <= 0f)
        {
            return "00:00";
        }

        int totalSeconds = (int)seconds;
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int remainSeconds = totalSeconds % 60;

        if (hours > 0)
        {
            return $"{hours}:{minutes:00}:{remainSeconds:00}";
        }

        return $"{minutes:00}:{remainSeconds:00}";
    }

    public static string FormatDuration(float seconds)
    {
        int totalSeconds = (int)seconds;
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;

        if (hours <= 0)
        {
            return $"{minutes}분";
        }

        if (minutes <= 0)
        {
            return $"{hours}시간";
        }

        return $"{hours}시간 {minutes}분";
    }

    public static T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        if (null == obj) return null;

        T component = obj.GetComponent<T>();
        if (null == component) component = obj.AddComponent<T>();

        return component;
    }

    public static T GetOrAddComponentInChild<T>(GameObject obj, string name) where T : Component
    {
        if (null == obj) return null;

        T component = FindChild<T>(obj, name);
        if (null == component)
        {
            GameObject newGameObject = new GameObject(name);
            component = newGameObject.AddComponent<T>();
            newGameObject.transform.SetParent(obj.transform);
        }

        return component;
    }

    public static T FindChild<T>(GameObject obj, string name = null, bool recursive = false) where T : Object
    {
        if (null == obj) return null;

        if (!recursive)
        {
            Transform transform = obj.transform.Find(name);
            if (null != transform) return transform.GetComponent<T>();
        }
        else
        {
            foreach (T component in obj.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static GameObject CreateEmptyGameObject(string name, Transform parent = null)
    {
        GameObject newGameObject = new GameObject(name);
        if (null != parent) newGameObject.transform.SetParent(parent);
        return newGameObject;
    }

    public static T ResourcesLoad<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public static bool IsNullOrWhiteSpace(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            Logger.LogError("string이 null이거나 빈 문자열입니다.");
            return true;
        }

        return false;
    }

    public static string GetEquipmentLevelDataId(string grade, int level)
    {
        return $"equipment_level_{grade.ToLower()}_{level}";
    }
}
