using UnityEngine;
using System.Collections;

public class ChangeToEdenTrigger : MonoBehaviour
{

    private GameObject _player;
    [SerializeField] private SceneField[] scenesToLoad;
    private bool _swapped = false;

    private void Awake()
    {
        StartCoroutine(FindPlayerRoutine());
    }

    private void OnEnable()
    {
        _swapped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _player && _swapped == false)
        {
            _swapped = true;
            WorldChangeManager.Instance.SwapToEden(scenesToLoad);
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