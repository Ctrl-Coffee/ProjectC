using UnityEngine;

public static class Utils
{
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
}
