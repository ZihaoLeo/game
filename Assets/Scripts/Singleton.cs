using System;
using UnityEngine;

public class Singleton<T> where T : new()
{
    private static T _instance = default(T);

    protected Singleton() { }

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }
}

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = default(T);

    protected MonoSingleton() { }

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
            }
            return _instance;
        }
    }
}