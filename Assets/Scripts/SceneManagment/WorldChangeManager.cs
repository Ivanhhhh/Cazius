using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.CompilerServices;

public class WorldChangeManager : MonoBehaviour
{

    public static WorldChangeManager Instance;

    public int scenesLoaded = 0;

     private  SceneField _edenBaseScene;
     private  SceneField _purgatoryBaseScene;

    [SerializeField] private SceneField edenBaseScene;
    [SerializeField] private SceneField purgatoryBaseScene;

    public List<SceneField> LoadedScenes { get; private set; } = new List<SceneField>(); 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);


        _edenBaseScene = edenBaseScene;
        _purgatoryBaseScene = purgatoryBaseScene;
    }

    public void AddSceneToList(SceneField scene)
    {
        LoadedScenes.Add(scene);
        scenesLoaded++;
    }

    public void SwapToEden(SceneField[] scenes)
    {
        StartCoroutine(SwapToEdenCoroutine(scenes));
    }

    public void SwapToPurgatory(SceneField[] scenes)
    {
        StartCoroutine(SwapToPurgatoryCoroutine(scenes));
    }

    public void LoadSceneAsync(SceneField sceneToLoad)
    {
        if (sceneToLoad == null || LoadedScenes.Contains(sceneToLoad)) 
            return;
        SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        LoadedScenes.Add(sceneToLoad);
    }

    private IEnumerator SwapToEdenCoroutine(SceneField[] scenesToLoad)
    {
        //Debug.Log("To Eden!");
        List<AsyncOperation> _operationsToBeDone = new List<AsyncOperation>();

        _operationsToBeDone.Add(SceneManager.UnloadSceneAsync(_purgatoryBaseScene));

        foreach (SceneField scene in LoadedScenes)
        {
            _operationsToBeDone.Add(SceneManager.UnloadSceneAsync(scene));
            yield return null;
        }
        
        foreach (AsyncOperation operation in _operationsToBeDone)
        {
            while (!operation.isDone)
            {
                yield return null;
            }
        }

        LoadedScenes.Clear();
        _operationsToBeDone.Clear();

        _operationsToBeDone.Add(SceneManager.LoadSceneAsync(_edenBaseScene, LoadSceneMode.Additive));

        yield return null;

        foreach (SceneField scen in scenesToLoad)
        {
            _operationsToBeDone.Add(SceneManager.LoadSceneAsync(scen, LoadSceneMode.Additive));
            AddSceneToList(scen);
            yield return null;
        }
        
        foreach (AsyncOperation operation in _operationsToBeDone)
        {
            while (!operation.isDone)
            {
                yield return null;
            }
        }
        //Debug.Log("End of Load!");
    }
    
    private IEnumerator SwapToPurgatoryCoroutine(SceneField[] scenesToLoad)
    {
        //Debug.Log("To Purgatory!");
        List<AsyncOperation> _operationsToBeDone = new List<AsyncOperation>();

        _operationsToBeDone.Add(SceneManager.UnloadSceneAsync(_edenBaseScene));

        foreach (SceneField scene in LoadedScenes)
        {
            _operationsToBeDone.Add(SceneManager.UnloadSceneAsync(scene));
            yield return null;
        }
        
        foreach (AsyncOperation operation in _operationsToBeDone)
        {
            while (!operation.isDone)
            {
                yield return null;
            }
        }

        LoadedScenes.Clear();
        _operationsToBeDone.Clear();

        _operationsToBeDone.Add(SceneManager.LoadSceneAsync(_purgatoryBaseScene, LoadSceneMode.Additive));

        yield return null;

        foreach (SceneField scen in scenesToLoad)
        {
            _operationsToBeDone.Add(SceneManager.LoadSceneAsync(scen, LoadSceneMode.Additive));
            AddSceneToList(scen);
            yield return null;
        }
        
        foreach (AsyncOperation operation in _operationsToBeDone)
        {
            if (operation != null)
            {
                while (!operation.isDone)
                {
                    yield return null;
                }

            }
        }
        //Debug.Log("End of load!");
    }
}
