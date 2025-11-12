using UnityEngine;

public abstract class BaseDontDestoryManager<T> : MonoBehaviour where T : BaseDontDestoryManager<T>
{
    static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject(typeof(T).Name);
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<T>();
                _instance.InitManager();
            }

            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    public abstract void InitManager();
}
