using Unity.VisualScripting;
using UnityEngine;

public class ChangeToPurgatoryTrigger : MonoBehaviour
{

    private GameObject _player;
    [SerializeField] private SceneField[] scenesToLoad;
    private bool _swapped = false;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
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
            WorldChangeManager.Instance.SwapToPurgatory(scenesToLoad);
        }
    }

}

