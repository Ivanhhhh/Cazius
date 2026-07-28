using UnityEngine;
using System.Collections.Generic;
public class Player_CounterAttack : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private LayerMask _enemyLayerMask;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _executionAnimTrigger = "Execute";

    [Header("State")]
    [SerializeField] private bool _isExecuting;

    private List<Enemy_MeleeEnemy_Data> _enemiesInRange = new List<Enemy_MeleeEnemy_Data>();
    private Enemy_MeleeEnemy_Data _currentTarget;

    void OnTriggerEnter(Collider other)
    {
        // filtro por layer antes de buscar el componente
        if (!IsInEnemyLayer(other.gameObject)) return;

        Enemy_MeleeEnemy_Data enemyData = other.GetComponent<Enemy_MeleeEnemy_Data>();
        if (enemyData != null && !_enemiesInRange.Contains(enemyData))
        {
            _enemiesInRange.Add(enemyData);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsInEnemyLayer(other.gameObject)) return;

        Enemy_MeleeEnemy_Data enemyData = other.GetComponent<Enemy_MeleeEnemy_Data>();
        if (enemyData != null)
        {
            _enemiesInRange.Remove(enemyData);
        }
    }

    private bool IsInEnemyLayer(GameObject obj)
    {
        return (_enemyLayerMask.value & (1 << obj.layer)) != 0;
    }

    void Update()
    {
        if (_isExecuting || _enemiesInRange.Count == 0) return;

        for (int i = 0; i < _enemiesInRange.Count; i++)
        {
            if (_enemiesInRange[i]._isStunned)
            {
                StartExecution(_enemiesInRange[i]);
                break;
            }
        }
    }

    private void StartExecution(Enemy_MeleeEnemy_Data target)
    {
        _isExecuting = true;
        _currentTarget = target;
        _animator.SetTrigger(_executionAnimTrigger);
    }

    public void OnExecutionAnimationEnd()
    {
        if (_currentTarget != null)
        {
            _currentTarget._isStunned = false;
            _enemiesInRange.Remove(_currentTarget);
        }

        _isExecuting = false;
        _currentTarget = null;
    }
}
