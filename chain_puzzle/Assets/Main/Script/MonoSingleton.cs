using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour
    where T : MonoSingleton<T>
{

    void Awake()
    {
        if (_Instance != null)
        {
            print(typeof(T) + "was instantiated");
            Destroy(gameObject);
            return;
        }
        Instance = (T)this;
        SubAwake();
    }

    protected virtual void SubAwake()
    {
    }

    void OnDestroy()
    {
        SubOnDestroy();
        if (Instance == this)
        {
            Instance = null;
        }
    }

    protected virtual void SubOnDestroy()
    {
    }

    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                Debug.LogWarning(typeof(T) + "is not instantiated");
            }
            return _Instance;
        }
        private set
        {
            _Instance = value;
        }
    }

    private static T _Instance;
}
