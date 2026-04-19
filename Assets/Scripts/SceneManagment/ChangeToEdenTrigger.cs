using UnityEngine;

public class ChangeToEdenTrigger : MonoBehaviour
{

    private GameObject _player;
    [SerializeField] private SceneField[] scenesToLoad;
    private bool _swapped = false;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _player && _swapped == false)
        {
            _swapped = true;
            WorldChangeManager.Instance.SwapToEden(scenesToLoad);
        }
    }

}
