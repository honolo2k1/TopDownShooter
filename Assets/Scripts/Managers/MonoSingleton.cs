using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    protected static T m_Instance = null;

    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindFirstObjectByType<T>();

                if (m_Instance == null)
                {
                    GameObject newGO = new GameObject(typeof(T).ToString());
                    m_Instance = newGO.AddComponent<T>();
                }
                else
                {
                    m_Instance.Initiate();
                }
            }
            return m_Instance;
        }
    }

    protected virtual void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this as T;
            Initiate();
        }
        else if (m_Instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Initiate() { }

    private void OnApplicationQuit()
    {
        m_Instance = null;
    }
}