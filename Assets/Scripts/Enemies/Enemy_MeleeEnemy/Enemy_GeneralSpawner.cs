    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class Enemy_GeneralSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject _objectToSpawn;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private int _amountPerSpawn = 1;
        [SerializeField] private int _maxSpawnedObjects = -1;

        [Header("Proximity Detection")]
        [SerializeField] private PlayerMovement _targetPlayer;
        [SerializeField] private float _proximityRadius = 5f;

        [Header("Spawning Mode")]
        [SerializeField] private SpawnMode _spawnMode = SpawnMode.Once;
        [SerializeField] private float _repeatInterval = 2f;

        public enum SpawnMode
        {
            Once,
            RepeatWhileNear,
            RepeatAlways
        }

        private bool _hasSpawnedOnce = false;
        private Coroutine _repeatCoroutine;
        private List<GameObject> _spawnedObjects = new List<GameObject>();

        private void Start()
        {
            if (_targetPlayer == null)
                _targetPlayer = GameManager.Instance.Player;
        }

        private void OnEnable()
        {
            if (_spawnMode == SpawnMode.RepeatAlways)
                _repeatCoroutine = StartCoroutine(RepeatSpawnCoroutine());
        }

        private void OnDisable()
        {
            if (_repeatCoroutine != null)
                StopCoroutine(_repeatCoroutine);
        }

        private void Update()
        {
            switch (_spawnMode)
            {
                case SpawnMode.Once:
                    if (!_hasSpawnedOnce && IsTargetNearby())
                    {
                        SpawnObjects();
                        _hasSpawnedOnce = true;
                    }
                    break;

                case SpawnMode.RepeatWhileNear:
                    if (_repeatCoroutine == null && IsTargetNearby())
                        _repeatCoroutine = StartCoroutine(RepeatSpawnCoroutine());
                    else if (_repeatCoroutine != null && !IsTargetNearby())
                    {
                        StopCoroutine(_repeatCoroutine);
                        _repeatCoroutine = null;
                    }
                    break;
            }
        }

        private IEnumerator RepeatSpawnCoroutine()
        {
            while (true)
            {
                SpawnObjects();
                yield return new WaitForSeconds(_repeatInterval);
            }
        }

        private bool IsTargetNearby()
        {
            if (_targetPlayer == null) return false;
            Vector3 checkPos = _spawnPoint != null ? _spawnPoint.position : transform.position;
            return Vector3.Distance(_targetPlayer.transform.position, checkPos) <= _proximityRadius;
        }

        private void SpawnObjects()
        {
            if (_objectToSpawn == null) return;

            _spawnedObjects.RemoveAll(obj => obj == null);

            if (_maxSpawnedObjects > 0 && _spawnedObjects.Count >= _maxSpawnedObjects)
                return;

            Vector3 spawnPos = _spawnPoint != null ? _spawnPoint.position : transform.position;

            for (int i = 0; i < _amountPerSpawn; i++)
            {
                if (_maxSpawnedObjects > 0 && _spawnedObjects.Count >= _maxSpawnedObjects)
                    break;

                GameObject newObj = Instantiate(_objectToSpawn, spawnPos, Quaternion.identity);
                _spawnedObjects.Add(newObj);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 checkPos = _spawnPoint != null ? _spawnPoint.position : transform.position;
            Gizmos.DrawWireSphere(checkPos, _proximityRadius);
        }
    }
