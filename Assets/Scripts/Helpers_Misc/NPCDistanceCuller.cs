
using UnityEngine;
using System.Collections;

public class NPCDistanceCuller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _npcContainer;

    [Header("Culling Distances")]
    [SerializeField] private float _enableDistance = 65f;
    [SerializeField] private float _disableDistance = 70f;

    [Header("Performance")]
    [Min(1)]
    [SerializeField] private int _checksPerFrame = 25;

    private Transform[] _npcs;
    private int _currentIndex;


    private bool _isInitialized = false;

    private void Start()
    {
        StartCoroutine(InitializeRoutine());
    }

    private IEnumerator InitializeRoutine()
    {
        yield return new WaitUntil(() =>
            GameManager.Instance != null &&
            GameManager.Instance.playerTransform != null
        );

        _player = GameManager.Instance.playerTransform;

        if (_npcContainer == null)
        {
            Debug.LogWarning("NPC Distance Culler: Missing NPC Container.");
            enabled = false;
            yield break;
        }

        int count = _npcContainer.childCount;
        _npcs = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            _npcs[i] = _npcContainer.GetChild(i);
        }

        _isInitialized = true;
    }

    private void Update()
    {

        if (!_isInitialized) return;

        if (_npcs.Length == 0) return;

        Vector3 playerPosition = _player.position;

        float enableSqr = _enableDistance * _enableDistance;
        float disableSqr = _disableDistance * _disableDistance;

        int checks = Mathf.Min(_checksPerFrame, _npcs.Length);

        for (int i = 0; i < checks; i++)
        {
            Transform npc = _npcs[_currentIndex];

            _currentIndex = (_currentIndex + 1) % _npcs.Length;

            if (npc == null) continue;

            float distanceSqr =
                (npc.position - playerPosition).sqrMagnitude;

            GameObject npcObject = npc.gameObject;

            if (npcObject.activeSelf)
            {
                // Disable distant NPCs.
                if (distanceSqr > disableSqr)
                    npcObject.SetActive(false);
            }
            else
            {
                // Reactivate nearby NPCs.
                if (distanceSqr < enableSqr)
                    npcObject.SetActive(true);
            }
        }
    }

    private void OnValidate()
    {
        _enableDistance = Mathf.Max(0f, _enableDistance);
        _disableDistance = Mathf.Max(
            _enableDistance + 1f,
            _disableDistance
        );

        _checksPerFrame = Mathf.Max(1, _checksPerFrame);
    }
}
