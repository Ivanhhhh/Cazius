using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VFX;

public class LoadFirstScene : MonoBehaviour
{
    [SerializeField] private Image _loadBarImage;

    [SerializeField] private GameObject canvas;

    [SerializeField] private SceneField _persistentGameplayScene;
    [SerializeField] private SceneField _firstScene;
    [SerializeField] private SceneField _baseScene;
    [SerializeField] private SceneField _firstScene2;
    [SerializeField] private SceneField _baseScene2;

    [SerializeField] private GameObject[] _vfxPrefabsToWarmUp;
    [SerializeField] private Transform _vfxWarmupPoint;
    [SerializeField] private float _secondsPerVFX = 0.15f;

    [SerializeField] private PSOWarmupManager _psoWarmupManager;

    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();


    void Start()
    {
        //StartGame();
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        StartGame();

        yield return StartCoroutine(WarmUpVFX());

        yield return StartCoroutine(_psoWarmupManager.WarmUp());

        StartCoroutine(LoadBar());
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
        //StartCoroutine(LoadBar());
        //IMPORTANTE LA LOADBAR, PROBANDO HACER COROUTINA PARA WARMUP VFX PRIMERO ARRIBA

    }

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

        Scene baseScene = SceneManager.GetSceneByName(_persistentGameplayScene);
        if (baseScene.IsValid() && baseScene.isLoaded)
        {
            SceneManager.SetActiveScene(baseScene);
        }

        SceneManager.UnloadSceneAsync(gameObject.scene);
    }


    private IEnumerator WarmUpVFX()
    {
        if (_vfxPrefabsToWarmUp == null || _vfxPrefabsToWarmUp.Length == 0)
            yield break;

        foreach (GameObject prefab in _vfxPrefabsToWarmUp)
        {
            if (prefab == null)
                continue;

            Vector3 spawnPosition = _vfxWarmupPoint != null
                ? _vfxWarmupPoint.position
                : Vector3.zero;

            Quaternion spawnRotation = _vfxWarmupPoint != null
                ? _vfxWarmupPoint.rotation
                : Quaternion.identity;

            GameObject instance = Instantiate(prefab, spawnPosition, spawnRotation);
            instance.SetActive(true);

            VisualEffect[] vfxGraphs = instance.GetComponentsInChildren<VisualEffect>(true);
            foreach (VisualEffect vfx in vfxGraphs)
            {
                vfx.gameObject.SetActive(true);
                vfx.Reinit();
                vfx.Play();

                vfx.SendEvent("OnPlay");
            }

            ParticleSystem[] particleSystems = instance.GetComponentsInChildren<ParticleSystem>(true);
            foreach (ParticleSystem ps in particleSystems)
            {
                ps.gameObject.SetActive(true);
                ps.Play(true);
            }

            yield return new WaitForSecondsRealtime(_secondsPerVFX);

            Destroy(instance);

            yield return null;
        }
    }

}
