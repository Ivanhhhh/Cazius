using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class WorldChangeManager : MonoBehaviour
{

    public static WorldChangeManager Instance;

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
        Debug.Log(LoadedScenes);
    }

    public void LoadSceneAsync(SceneField sceneToLoad)
    {
        if (sceneToLoad == null || LoadedScenes.Contains(sceneToLoad)) 
            return;
        SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        LoadedScenes.Add(sceneToLoad);
    }

    public IEnumerator SwapToEden(SceneField[] scenesToLoad)
    {
        Debug.Log("To Eden!");
        List<AsyncOperation> _operationsToBeDone = new List<AsyncOperation>();

        _operationsToBeDone.Add(SceneManager.UnloadSceneAsync(_purgatoryBaseScene));

        foreach (SceneField scene in LoadedScenes)
        {
            _operationsToBeDone.Add(SceneManager.UnloadSceneAsync(scene));
            //yield return null;
        }
        /*
        foreach (AsyncOperation operation in _operationsToBeDone)
        {
            while (!operation.isDone)
            {
                yield return null;
            }
        }*/

        LoadedScenes.Clear();
        _operationsToBeDone.Clear();

        _operationsToBeDone.Add(SceneManager.LoadSceneAsync(_edenBaseScene, LoadSceneMode.Additive));


        foreach (SceneField scen in scenesToLoad)
        {
            _operationsToBeDone.Add(SceneManager.LoadSceneAsync(scen, LoadSceneMode.Additive));
            LoadedScenes.Add(scen);
            //yield return null;
        }
        /*
        foreach (AsyncOperation operation in _operationsToBeDone)
        {
            while (!operation.isDone)
            {
                yield return null;
            }
        }*/

        yield break;
    }
    
    public IEnumerator SwapToPurgatory(SceneField[] scenesToLoad)
    {
        Debug.Log("To Eden!");
        List<AsyncOperation> _operationsToBeDone = new List<AsyncOperation>();

        _operationsToBeDone.Add(SceneManager.UnloadSceneAsync(_edenBaseScene));

        foreach (SceneField scene in LoadedScenes)
        {
            _operationsToBeDone.Add(SceneManager.UnloadSceneAsync(scene));
            //yield return null;
        }
        /*
        foreach (AsyncOperation operation in _operationsToBeDone)
        {
            while (!operation.isDone)
            {
                yield return null;
            }
        }*/

        LoadedScenes.Clear();
        _operationsToBeDone.Clear();

        _operationsToBeDone.Add(SceneManager.LoadSceneAsync(_purgatoryBaseScene, LoadSceneMode.Additive));


        foreach (SceneField scen in scenesToLoad)
        {
            _operationsToBeDone.Add(SceneManager.LoadSceneAsync(scen, LoadSceneMode.Additive));
            LoadedScenes.Add(scen);
            //yield return null;
        }
        /*
        foreach (AsyncOperation operation in _operationsToBeDone)
        {
            while (!operation.isDone)
            {
                yield return null;
            }
        }*/

        yield break;
    }
}
