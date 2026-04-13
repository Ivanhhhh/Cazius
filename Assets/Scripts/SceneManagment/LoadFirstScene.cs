using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;

public class LoadFirstScene : MonoBehaviour
{
    [SerializeField] private Image _loadBarImage;

    [SerializeField] private SceneField _persistentGameplayScene;
    [SerializeField] private SceneField _firstScene;
    [SerializeField] private SceneField _baseScene;

    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();


    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_persistentGameplayScene));
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_baseScene, LoadSceneMode.Additive));
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_firstScene, LoadSceneMode.Additive));
        WorldChangeManager.Instance.AddSceneToList(_firstScene);
        StartCoroutine(LoadBar());

    }

    private IEnumerator LoadBar()
    {
        float loadprogress = 0f;


        foreach (AsyncOperation operation in _scenesToLoad)
        {
            while(!operation.isDone)
            {
                loadprogress += operation.progress;
                _loadBarImage.fillAmount = loadprogress;
                Debug.Log("Loading!");
                Debug.Log(operation.progress);
                yield return null;
            }

        }
        Debug.Log("Finished Load!");
    }
}
