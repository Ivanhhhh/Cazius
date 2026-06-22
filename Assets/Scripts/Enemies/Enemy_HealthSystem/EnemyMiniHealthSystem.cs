using System;
using System.Collections;
using UnityEngine;

public class EnemyMiniHealthSystem : MonoBehaviour, Enemy_Interface_Damage
{
    [Header("Health")]
    [SerializeField] private float _maxHealth = 1f;
    [SerializeField] private float _currentHealth;

    [Header("SFX")]
    [SerializeField] private SFXManager.SFXCategoryType _onDamageSFX;
    [SerializeField] private SFXManager.SFXCategoryType _onDeathSFX;

    [Header("Item")]
    [SerializeField] GameObject _itemPrefabToDrop;
    [SerializeField] Transform _itemSpawnPoint;

    public Action OnDeath;
    public Action<float> OnDamaged;

    private SFXManager _sfxManager;
    private bool _isDead;

    void Start()
    {
        _sfxManager = SFXManager.Instance;
        _currentHealth = _maxHealth;
        _isDead = false;
    }

    public void TakeDamage(float amount)
    {
        if (_isDead) return;

        _currentHealth -= amount;
        _sfxManager.PlaySFXAtPosition(_onDamageSFX, transform.position);
        OnDamaged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
        {
            _isDead = true;
            OnDeath?.Invoke();
            StartCoroutine(DieRoutine());
        }
    }

    private IEnumerator DieRoutine()
    {
        _sfxManager.PlaySFXAtPosition(_onDeathSFX, transform.position);

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        if (_itemPrefabToDrop)
        {
            Vector3 spawnPos = _itemSpawnPoint != null ? _itemSpawnPoint.position : transform.position;
            Instantiate(_itemPrefabToDrop, spawnPos, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.15f);
        Destroy(gameObject);
    }

}
