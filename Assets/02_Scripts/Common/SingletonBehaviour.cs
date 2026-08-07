using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    private static T _instance;

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (null == _instance)
        {
            _instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static T Instance
    {
        get
        {
            return _instance;
        }
    }
}
