using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class ChangeToPurgatoryTrigger : MonoBehaviour
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
        Debug.Log("triggered soemthing");
        if (other.gameObject == _player && _swapped == false)
        {
            Debug.Log("SWAP!");
            _swapped = true;
            WorldChangeManager.Instance.SwapToPurgatory(scenesToLoad);
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

