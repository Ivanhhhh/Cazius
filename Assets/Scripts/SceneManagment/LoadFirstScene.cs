using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class LoadFirstScene : MonoBehaviour
{
    [SerializeField] private Image _loadBarImage;

    [SerializeField] private GameObject canvas;

    [SerializeField] private SceneField _persistentGameplayScene;
    [SerializeField] private SceneField _firstScene;
    [SerializeField] private SceneField _baseScene;
    [SerializeField] private SceneField _firstScene2;
    [SerializeField] private SceneField _baseScene2;

    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();


    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_baseScene, LoadSceneMode.Additive));
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_firstScene, LoadSceneMode.Additive));
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_baseScene2, LoadSceneMode.Additive));
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_firstScene2, LoadSceneMode.Additive));
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_persistentGameplayScene, LoadSceneMode.Additive));
        WorldChangeManager.Instance.AddSceneToList(_firstScene);
        WorldChangeManager.Instance.AddSceneToList(_firstScene2);
        StartCoroutine(LoadBar());

    }
    /*
    private IEnumerator LoadBar()
    {
        float loadprogress = 0f;


        foreach (AsyncOperation operation in _scenesToLoad)
        {
            while(!operation.isDone)
            {
                loadprogress = operation.progress / _scenesToLoad.Count;
                _loadBarImage.fillAmount = loadprogress;
                Debug.Log("Loading!");
                Debug.Log(operation.progress);
                yield return null;
            }

        }
        Debug.Log("Finished Load!");
    }*/

    private IEnumerator LoadBar()
    {
        bool allDone = false;

        while (!allDone)
        {
            float totalProgress = 0f;
            allDone = true;

            foreach (AsyncOperation operation in _scenesToLoad)
            {
                totalProgress += operation.progress;

                if (!operation.isDone)
                {
                    allDone = false;
                }
            }

            float averageProgress = totalProgress / _scenesToLoad.Count;
            _loadBarImage.fillAmount = averageProgress;

            yield return null;
        }

        Debug.Log("Finished Load!");

        // Set your gameplay base scene active
        Scene baseScene = SceneManager.GetSceneByName(_persistentGameplayScene);
        if (baseScene.IsValid() && baseScene.isLoaded)
        {
            SceneManager.SetActiveScene(baseScene);
        }

        // Remove the loading scene
        SceneManager.UnloadSceneAsync(gameObject.scene);
    }

}
