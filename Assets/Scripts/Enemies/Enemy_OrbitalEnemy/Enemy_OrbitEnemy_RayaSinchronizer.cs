using UnityEngine;
using System.Collections.Generic;
public class Enemy_OrbitEnemy_RayaSinchronizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Enemy_OrbitEnemyData _orbitData;
    [SerializeField] private LineRenderer _linkPrefab; // el prefab ahora debe tener un LineRenderer

    [Header("Offset Settings")]
    [Tooltip("Cuánto se aleja la punta de la línea del centro del enemigo en la lista.")]
    [SerializeField] private float _startOffset = 0.5f;
    [Tooltip("Cuánto se aleja la punta de la línea del centro de este objeto.")]
    [SerializeField] private float _endOffset = 0.5f;

    private readonly Dictionary<GameObject, LineRenderer> _activeLinks = new Dictionary<GameObject, LineRenderer>();
    private readonly List<GameObject> _staleKeys = new List<GameObject>();

    void Awake()
    {
        if (_orbitData == null) _orbitData = GetComponent<Enemy_OrbitEnemyData>();
    }

    void Update()
    {
        if (_orbitData == null || _linkPrefab == null) return;

        SyncLinkInstances();
        UpdateLinkPositions();
    }

    private void SyncLinkInstances()
    {
        var enemiesAlive = _orbitData.EnemiesAlive;

        for (int i = 0; i < enemiesAlive.Count; i++)
        {
            GameObject enemy = enemiesAlive[i];
            if (enemy == null) continue;

            if (!_activeLinks.ContainsKey(enemy))
            {
                LineRenderer newLink = Instantiate(_linkPrefab);
                _activeLinks.Add(enemy, newLink);
            }
        }

        _staleKeys.Clear();
        foreach (var kvp in _activeLinks)
        {
            bool stillAlive = kvp.Key != null && enemiesAlive.Contains(kvp.Key);
            if (!stillAlive) _staleKeys.Add(kvp.Key);
        }

        for (int i = 0; i < _staleKeys.Count; i++)
        {
            GameObject key = _staleKeys[i];
            if (_activeLinks[key] != null) Destroy(_activeLinks[key].gameObject);
            _activeLinks.Remove(key);
        }
    }

    private void UpdateLinkPositions()
    {
        foreach (var kvp in _activeLinks)
        {
            GameObject enemy = kvp.Key;
            LineRenderer link = kvp.Value;

            if (enemy == null || link == null) continue;

            Vector3 centerA = enemy.transform.position;
            Vector3 centerB = transform.position;

            Vector3 fullDirection = centerB - centerA;
            float fullDistance = fullDirection.magnitude;

            if (fullDistance < 0.0001f)
            {
                link.gameObject.SetActive(false);
                continue;
            }

            link.gameObject.SetActive(true);
            Vector3 direction = fullDirection / fullDistance;

            // Mismo clamp que antes: nunca deja que los offsets se crucen entre sí
            float halfDistance = fullDistance * 0.5f;
            float clampedStartOffset = Mathf.Min(_startOffset, halfDistance);
            float clampedEndOffset = Mathf.Min(_endOffset, halfDistance);

            Vector3 adjustedStart = centerA + direction * clampedStartOffset;
            Vector3 adjustedEnd = centerB - direction * clampedEndOffset;

            link.SetPosition(0, adjustedStart);
            link.SetPosition(1, adjustedEnd);
        }
    }

    void OnDisable()
    {
        foreach (var kvp in _activeLinks)
        {
            if (kvp.Value != null) Destroy(kvp.Value.gameObject);
        }
        _activeLinks.Clear();
    }
}