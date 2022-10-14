/// Unity Modules - Singleton
/// Created by: Nicolas Capelier
/// Contact: capelier.nicolas@gmail.com
/// Version: 1.0.0
/// Version release date (dd/mm/yyyy): 29/07/2022

using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{

    /// <summary>
    /// The instance of the object.
    /// </summary>
    private static T instance;
    
    /// <summary>
    /// Get the instance of the Singleton.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    Debug.LogWarning($"Instance of type {typeof(T)} not found.");
                    return null;
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Make this object a Singleton.
    /// </summary>
    protected bool CreateSingleton()
    {
        if (instance == null)
        {
            instance = this as T;
            return true;
        }
        else
        {
            Debug.LogWarning("Destroyed a non-unique gameObject named " + gameObject.name);
            Destroy(gameObject);
            return false;
        }
    }

    /// <summary>
    /// Make the object a non-destroyable on load Singleton.
    /// </summary>
    protected bool CreateSingleton(bool isUndestroyableOnLoad)
    {
        if(CreateSingleton() && isUndestroyableOnLoad)
        {
            DontDestroyOnLoad(gameObject);
            return true;
        }
        return false;
    }
}