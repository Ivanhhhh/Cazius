using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.CompilerServices;
using System;

public class WorldChangeManager : MonoBehaviour
{

    public static WorldChangeManager Instance;

    public event Action SwapToEdenEvent;
    public event Action SwapToPurgatoryEvent;

    public int scenesLoaded = 0;

    private SceneField _edenBaseScene;
    private SceneField _purgatoryBaseScene;

    [SerializeField] private SceneField edenBaseScene;
    [SerializeField] private SceneField purgatoryBaseScene;

    public bool IsInEden { get; private set; } = true;

    public List<SceneField> LoadedScenes { get; private set; } = new List<SceneField>();


    //Visual Refs
    [SerializeField] Material _purgatorySwapFullscreenShader;
    [SerializeField] Material _edenSwapFullscreenShader;
    [SerializeField] float _shaderTransitionLength = 0.5f;
    [SerializeField] float _waitForStartLoad = 1;

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

        _edenSwapFullscreenShader.SetFloat("_Intensity", 0f);
        _purgatorySwapFullscreenShader.SetFloat("_Intensity", 0f);
    }

    public void AddSceneToList(SceneField scene)
    {
        LoadedScenes.Add(scene);
        scenesLoaded++;
    }

    public void SwapToEden(SceneField[] scenes)
    {
        Debug.Log("SWAP TO EDEN");
        StartCoroutine(SwapToEdenCoroutine(scenes));
    }

    public void SwapToPurgatory(SceneField[] scenes)
    {
        Debug.Log("SWAP TO PURGATORY");
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

        yield return StartCoroutine(FadeShader(_edenSwapFullscreenShader, "_Intensity", 0f, 1f, _shaderTransitionLength));

        IsInEden = true;

        SwapToEdenEvent?.Invoke();

        yield return new WaitForSeconds(_waitForStartLoad);

        yield return StartCoroutine(FadeShader(_edenSwapFullscreenShader, "_Intensity", 1f, 0f, _shaderTransitionLength));
        _edenSwapFullscreenShader.SetFloat("_Intensity", 0f);

        //Debug.Log("End of Load!");
    }

    private IEnumerator SwapToPurgatoryCoroutine(SceneField[] scenesToLoad)
    {
        //Debug.Log("To Purgatory!");

        yield return StartCoroutine(FadeShader(_purgatorySwapFullscreenShader, "_Intensity", 0f, 1f, _shaderTransitionLength));

        IsInEden = false;

        SwapToPurgatoryEvent?.Invoke();

        yield return new WaitForSeconds(_waitForStartLoad);

        yield return StartCoroutine(FadeShader(_purgatorySwapFullscreenShader, "_Intensity", 1f, 0f, _shaderTransitionLength));
        _purgatorySwapFullscreenShader.SetFloat("_Intensity", 0f);
        //Debug.Log("End of load!");
    }

    private bool IsSceneLoaded(SceneField sceneField)
    {
        Scene scene = SceneManager.GetSceneByName(sceneField.SceneName);
        return scene.IsValid() && scene.isLoaded;
    }

    private IEnumerator FadeShader(Material mat, string property, float start, float end, float duration)
    {
        Debug.Log("FadeShaderStart");
        float elapsed = 0f;
        mat.SetFloat(property, start);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float value = Mathf.Lerp(start, end, elapsed / duration);
            mat.SetFloat(property, value);
            yield return null;
        }

        mat.SetFloat(property, end);
        Debug.Log("FadeShaderEnd");
    }


    /*
    private IEnumerator SwapToEdenCoroutine(SceneField[] scenesToLoad)
    {
        //Debug.Log("To Eden!");

        yield return StartCoroutine(FadeShader(_edenSwapFullscreenShader, "_Intensity", 0f, 1f, _shaderTransitionLength));

        List<AsyncOperation> _operationsToBeDone = new List<AsyncOperation>();

        //_operationsToBeDone.Add(SceneManager.UnloadSceneAsync(_purgatoryBaseScene));
        if (IsSceneLoaded(_purgatoryBaseScene))
        {
            _operationsToBeDone.Add(SceneManager.UnloadSceneAsync(_purgatoryBaseScene));
        }

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

        yield return new WaitForSeconds(_waitForStartLoad);

        yield return StartCoroutine(FadeShader(_edenSwapFullscreenShader, "_Intensity", 1f, 0f, _shaderTransitionLength));
        _edenSwapFullscreenShader.SetFloat("_Intensity", 0f);

        //Debug.Log("End of Load!");
    }
    */
    /*
    private IEnumerator SwapToPurgatoryCoroutine(SceneField[] scenesToLoad)
    {
        //Debug.Log("To Purgatory!");

        yield return StartCoroutine(FadeShader(_purgatorySwapFullscreenShader, "_Intensity", 0f, 1f, _shaderTransitionLength));

        List<AsyncOperation> _operationsToBeDone = new List<AsyncOperation>();

        //_operationsToBeDone.Add(SceneManager.UnloadSceneAsync(_edenBaseScene));
        if (IsSceneLoaded(_edenBaseScene))
        {
            _operationsToBeDone.Add(SceneManager.UnloadSceneAsync(_edenBaseScene));
        }

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

        yield return new WaitForSeconds(_waitForStartLoad);

        yield return StartCoroutine(FadeShader(_purgatorySwapFullscreenShader, "_Intensity", 1f, 0f, _shaderTransitionLength));
        _purgatorySwapFullscreenShader.SetFloat("_Intensity", 0f);
        //Debug.Log("End of load!");
    }
    */
}