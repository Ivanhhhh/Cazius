using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEndTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _player;

    [Header("Scene")]
    [SerializeField] private string _sceneToLoad = "GameEnd";

    private bool _triggered = false;

    private void Awake()
    {
        if (_player == null)
        {
            StartCoroutine(FindPlayerRoutine());
        }
    }

    private void OnEnable()
    {
        _triggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered)
            return;

        if (other.gameObject == _player)
        {
            _triggered = true;
            SceneManager.LoadScene(_sceneToLoad);
        }
    }

    private IEnumerator FindPlayerRoutine()
    {
        while (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }
    }
}