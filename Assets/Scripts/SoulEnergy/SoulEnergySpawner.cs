using UnityEngine;
using System.Collections.Generic;

public class SoulSpawner : MonoBehaviour
{
    [Header("Standard Prefab")]
    [Tooltip("Soul that is always spawned on every cycle.")]
    [SerializeField] private SoulEnergyMovement _soulPrefab;

    [Header("Spawn Frequency")]
    [Tooltip("Seconds between spawns. A random value is picked within this range.")]
    [SerializeField] private float _intervalMin = 0.5f;
    [SerializeField] private float _intervalMax = 2f;

    [Tooltip("Maximum number of alive souls at once. Spawn pauses when reached.")]
    [SerializeField] private int _maxAliveSouls = 12;

    [Header("Spawn Limit")]
    [Tooltip("Total number of standard spawns. -1 = unlimited.")]
    [SerializeField] private int _maxTotalSpawns = -1;
    [SerializeField] private bool _disableOnFinish = true;
    [SerializeField] private bool _destroyOnFinish = false;
    [SerializeField] private UnityEngine.Events.UnityEvent _onFinish;

    [Header("Initial Impulse")]
    [SerializeField] private float _impulseMin = 1f;
    [SerializeField] private float _impulseMax = 4f;
    [SerializeField] private bool _impulseHorizontal = true;

    [Header("Spawn Point")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _spawnRadius = 0.3f;

    // ============================================================
    //  BONUS · LOW HEALTH
    // ============================================================
    [Header("Bonus · Low Health")]
    [SerializeField] private bool _enableHealthBonusDrop = true;

    [Tooltip("State equal to or WORSE than this is considered 'low health'. Amazing_100=0 ... Critic_20=4.")]
    [SerializeField] private HealthStates _healthStateThreshold = HealthStates.Low_30;

    [SerializeField] private SoulEnergyMovement _healthBonusPrefab;

    [Tooltip("How many extra souls per spawn when the condition is met.")]
    [SerializeField] private int _healthBonusAmountPerSpawn = 1;

    [Tooltip("Chance (0-1) that the bonus appears when the condition is met. 1 = always.")]
    [SerializeField, Range(0f, 1f)] private float _healthBonusChance = 1f;

    [Tooltip("If the player ALREADY has any of these itemIDs, the bonus is not added.")]
    [SerializeField] private string[] _healthItemIDsToCheck = new string[] { };

    // ============================================================
    //  BONUS · LOW AMMO
    // ============================================================
    [Header("Bonus · Low Ammo")]
    [SerializeField] private bool _enableAmmoBonusDrop = true;

    [Tooltip("Total ammo at or below this value triggers the bonus.")]
    [SerializeField] private int _ammoThreshold = 5;

    [SerializeField] private SoulEnergyMovement _ammoBonusPrefab;

    [Tooltip("How many extra souls per spawn when the condition is met.")]
    [SerializeField] private int _ammoBonusAmountPerSpawn = 1;

    [SerializeField, Range(0f, 1f)] private float _ammoBonusChance = 1f;

    [Tooltip("If the player ALREADY has any of these itemIDs, the bonus is not added.")]
    [SerializeField] private string[] _ammoItemIDsToCheck = new string[] { };

    [Header("Debug")]
    [SerializeField] private bool _drawGizmos = true;

    // --- Internal state ---
    private float _nextSpawnTimer;
    private int _completedSpawns;
    private bool _finished;
    private readonly List<SoulEnergyMovement> _aliveSouls = new List<SoulEnergyMovement>();
    private HealthStates _cachedHealthState = HealthStates.Amazing_100;

    // One-shot flags: each bonus can only trigger once per spawner lifetime
    private bool _healthBonusGiven;
    private bool _ammoBonusGiven;

    public bool IsFinished => _finished;
    public int CompletedSpawns => _completedSpawns;

    // -------------------- Lifecycle --------------------

    private void OnEnable()
    {
        _finished = false;
        _healthBonusGiven = false;
        _ammoBonusGiven = false;

        _cachedHealthState = Player_HealthSystem.CurrentHealthState;
        Player_HealthSystem.OnHealthStateChanged += HandleHealthStateChanged;

        ScheduleNextSpawn();
    }

    private void OnDisable()
    {
        Player_HealthSystem.OnHealthStateChanged -= HandleHealthStateChanged;
    }

    private void HandleHealthStateChanged(HealthStates newState)
    {
        _cachedHealthState = newState;
    }

    private void Start()
    {
        ScheduleNextSpawn();
    }

    private void Update()
    {
        if (_finished) return;
        if (_soulPrefab == null) return;

        _aliveSouls.RemoveAll(s => s == null || !s.gameObject.activeInHierarchy);

        if (_maxTotalSpawns >= 0 && _completedSpawns >= _maxTotalSpawns)
        {
            Finish();
            return;
        }

        if (_aliveSouls.Count >= _maxAliveSouls) return;

        _nextSpawnTimer -= Time.deltaTime;
        if (_nextSpawnTimer <= 0f)
        {
            SpawnSoul();
            ScheduleNextSpawn();
        }
    }

    private void ScheduleNextSpawn()
    {
        _nextSpawnTimer = Random.Range(_intervalMin, _intervalMax);
    }

    // -------------------- MAIN SPAWN --------------------

    private void SpawnSoul()
    {
        Vector3 origin = _spawnPoint != null ? _spawnPoint.position : transform.position;

        // 1) Standard soul (always)
        Vector3 offset = Random.insideUnitSphere * _spawnRadius;
        offset.y = 0f;

        SoulEnergyMovement soul = Instantiate(_soulPrefab, origin + offset, Quaternion.identity);
        _aliveSouls.Add(soul);
        soul.AplicarImpulsoAleatorio(_impulseMin, _impulseMax, _impulseHorizontal);
        _completedSpawns++;

        // 2) Conditional bonuses (one-shot each)
        TrySpawnBonusDrops(origin);
    }

    // -------------------- CONDITIONAL BONUSES --------------------

    private void TrySpawnBonusDrops(Vector3 origin)
    {
        // --- HEALTH ---
        if (_enableHealthBonusDrop && !_healthBonusGiven
            && _healthBonusPrefab != null && _healthBonusAmountPerSpawn > 0)
        {
            bool isLowHealth = _cachedHealthState >= _healthStateThreshold;
            bool alreadyHasItem = HasAnyItem(_healthItemIDsToCheck);

            if (isLowHealth && !alreadyHasItem && Random.value <= _healthBonusChance)
            {
                SpawnBonusSouls(_healthBonusPrefab, _healthBonusAmountPerSpawn, origin);
                _healthBonusGiven = true;
                Debug.Log("[SoulSpawner] Low health bonus granted (one-shot).");
            }
        }

        // --- AMMO ---
        if (_enableAmmoBonusDrop && !_ammoBonusGiven
            && _ammoBonusPrefab != null && _ammoBonusAmountPerSpawn > 0)
        {
            int currentAmmo = Inventory.Instance != null
                ? Inventory.Instance.GetTotalAmmo()
                : int.MaxValue;

            bool isLowAmmo = currentAmmo <= _ammoThreshold;
            bool alreadyHasItem = HasAnyItem(_ammoItemIDsToCheck);

            if (isLowAmmo && !alreadyHasItem && Random.value <= _ammoBonusChance)
            {
                SpawnBonusSouls(_ammoBonusPrefab, _ammoBonusAmountPerSpawn, origin);
                _ammoBonusGiven = true;
                Debug.Log("[SoulSpawner] Low ammo bonus granted (one-shot).");
            }
        }
    }

    private void SpawnBonusSouls(SoulEnergyMovement prefab, int amount, Vector3 origin)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 offset = Random.insideUnitSphere * _spawnRadius;
            offset.y = 0f;

            SoulEnergyMovement bonus = Instantiate(prefab, origin + offset, Quaternion.identity);
            bonus.AplicarImpulsoAleatorio(_impulseMin, _impulseMax, _impulseHorizontal);
            _aliveSouls.Add(bonus);
        }
    }

    private bool HasAnyItem(string[] itemIDs)
    {
        if (itemIDs == null || itemIDs.Length == 0) return false;
        if (Inventory.Instance == null) return false;

        foreach (var id in itemIDs)
            if (!string.IsNullOrEmpty(id) && Inventory.Instance.HasItem(id))
                return true;

        return false;
    }

    // -------------------- Finish / API --------------------

    private void Finish()
    {
        _finished = true;
        _onFinish?.Invoke();

        if (_destroyOnFinish) { Destroy(gameObject); return; }
        if (_disableOnFinish) enabled = false;
    }

    public void ForceFinish() => Finish();

    public void Restart()
    {
        _completedSpawns = 0;
        _finished = false;
        _healthBonusGiven = false;
        _ammoBonusGiven = false;
        enabled = true;
        ScheduleNextSpawn();
    }

    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmos) return;

        Vector3 origin = _spawnPoint != null ? _spawnPoint.position : transform.position;

        Gizmos.color = new Color(1f, 0.6f, 0f, 0.4f);
        Gizmos.DrawWireSphere(origin, _spawnRadius);

        Gizmos.color = Color.yellow;
        const int steps = 16;
        for (int i = 0; i < steps; i++)
        {
            float angle = i * (360f / steps) * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Gizmos.DrawRay(origin, dir * 1.2f);
        }
    }
}