using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    protected new abstract bool DontDestroyOnLoad { get; }

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                var t = typeof(T);
                _instance = (T)FindObjectOfType(t);
                if (!_instance)
                {
                    Debug.LogError(t + " is nothing.");
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        if (DontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}