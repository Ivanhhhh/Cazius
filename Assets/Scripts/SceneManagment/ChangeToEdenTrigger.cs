using UnityEngine;

public class ChangeToEdenTrigger : MonoBehaviour
{

    private GameObject _player;
    [SerializeField] private SceneField[] scenesToLoad;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _player)
        {
            StartCoroutine(WorldChangeManager.Instance.SwapToEden(scenesToLoad));
        }
    }

}
