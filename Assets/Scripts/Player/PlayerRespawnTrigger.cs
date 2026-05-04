using UnityEngine;
using System.Collections;

public class PlayerRespawnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _respawnPos;

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
        Debug.Log("TOUCH");
        if (other.gameObject == _player)
        {
            _triggered = true;
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        _player.transform.position = _respawnPos.position;
        _player.transform.rotation = _respawnPos.rotation;
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