using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking.PlayerConnection;
using System.IO;

public class PSOTraceRecorder : MonoBehaviour
{
    [Header("Tracing")]
    [SerializeField] private bool traceOnStart = true;
    [SerializeField] private KeyCode saveKey = KeyCode.F10;

    [Header("Output")]
    [SerializeField] private string collectionName = "FirstGameplay_DX12";

    private GraphicsStateCollection _collection;
    private bool _hasSaved;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (traceOnStart)
            BeginTrace();
    }

    private void Update()
    {
        if (Input.GetKeyDown(saveKey))
        {
            EndTraceAndSave();
        }
    }

    private void BeginTrace()
    {
        _collection = new GraphicsStateCollection();

        bool started = _collection.BeginTrace();

        Debug.Log(started
            ? "PSO tracing started."
            : "PSO tracing failed to start.");
    }

    public void EndTraceAndSave()
    {
        if (_hasSaved)
            return;

        if (_collection == null)
        {
            Debug.LogWarning("No PSO collection exists.");
            return;
        }

        if (_collection.isTracing)
            _collection.EndTrace();

        _hasSaved = true;

        string fileName = collectionName + ".graphicsstate";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        bool savedToDisk = _collection.SaveToFile(path);

        Debug.Log(savedToDisk
            ? "PSO collection saved to: " + path
            : "Failed to save PSO collection to disk.");

        if (PlayerConnection.instance != null && PlayerConnection.instance.isConnected)
        {
            bool sent = _collection.SendToEditor(collectionName);

            Debug.Log(sent
                ? "PSO collection sent to Editor: " + collectionName
                : "Could not send PSO collection to Editor.");
        }

        Debug.Log(
            $"PSO Trace Finished. Variants: {_collection.variantCount}, Graphics States: {_collection.totalGraphicsStateCount}, API: {_collection.graphicsDeviceType}, Platform: {_collection.runtimePlatform}"
        );
    }

    private void OnApplicationQuit()
    {
        EndTraceAndSave();
    }
}