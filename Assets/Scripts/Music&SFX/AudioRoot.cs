using UnityEngine;

public class AudioRoot : MonoBehaviour
{
    private static AudioRoot _instance;

    public static AudioRoot Instance // Para acceder a 
    {
        get
        {
            if (_instance == null)
            {
                // Busca una instancia sino la crea
                _instance = FindFirstObjectByType<AudioRoot>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("AudioRoot");
                    _instance = go.AddComponent<AudioRoot>();
                }
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    private void OnEnable()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
