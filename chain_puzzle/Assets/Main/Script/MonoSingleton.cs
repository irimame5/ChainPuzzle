using UnityEngine;

/// <summary>
/// 継承したクラスのStartでDontDestroyonLoad(gameObject)
/// をするとゲーム全体でのシングルトンができる．
/// しないとシーン内でのシングルトンができる．
/// 継承先ではAwake,OnDestroyは使えないので，
/// SubAwake,SubOnDestroyを使う.
/// public class 継承クラス:MonoSingleton<継承クラス>って感じで使う.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MonoSingleton<T> : 
    MonoBehaviour where T : MonoSingleton<T>
{

    /// <summary>
    /// すでにインスタンスがあれば破棄
    /// 値をシーンごとにコピーする場合はこれをオーバーライド
    /// </summary>
    protected virtual void Awake()
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

    protected virtual void OnDestroy()
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
        protected set
        {
            _Instance = value;
        }
    }

    private static T _Instance;
}
