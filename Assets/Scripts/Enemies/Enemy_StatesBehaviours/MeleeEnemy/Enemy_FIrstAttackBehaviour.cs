using UnityEngine;
using UnityEngine.AI;

public class Enemy_FirstAttackBehaviour
{
    private float _preparationTime;
    private float _currentPreparationTime;
    
    // Nueva variable para determinar cuánto tiempo estará activo el ataque
    private float _attackDuration; 
    private float _currentAttackTime;

    private float _stunDuration;
    private float _currentStunTime;
    private Collider _attackCollider;
    private NavMeshAgent _agent;
    private Enemy_MeleeEnemy_Data _data;

    // Cambiamos "Charging" por "Attacking" para que tenga más sentido
    private enum AttackPhase { Preparing, Attacking, Stunned, Done }
    private AttackPhase _currentPhase = AttackPhase.Preparing;
    
    public bool IsDone => _currentPhase == AttackPhase.Done;

    // Actualizamos el constructor. Quitamos speed y playerTransform, y añadimos attackDuration
    public Enemy_FirstAttackBehaviour(float preparationTime, float attackDuration, float stunDuration, NavMeshAgent agent, Collider attackCollider,Enemy_MeleeEnemy_Data data)
    {
        _preparationTime = preparationTime;
        _currentPreparationTime = preparationTime;
        
        _attackDuration = attackDuration;
        _currentAttackTime = attackDuration;

        _stunDuration = stunDuration;
        _currentStunTime = stunDuration;
        
        _agent = agent;
        _attackCollider = attackCollider;
        
        _data = data;
    }

    public void Tick()
    {
        if (_data._isStunned && _currentPhase != AttackPhase.Stunned && _currentPhase != AttackPhase.Done)
        {
            Debug.Log("Enemy Stunned");
            _currentStunTime = _stunDuration;
            _attackCollider.enabled = false;
            _currentPhase = AttackPhase.Stunned;
        }
        switch (_currentPhase)
        {
            case AttackPhase.Preparing: UpdatePreparing(); break;
            case AttackPhase.Attacking: UpdateAttacking(); break;
            case AttackPhase.Stunned: updateStunned(); break;
        }
    }

    void UpdatePreparing()
    {
        _currentPreparationTime -= Time.deltaTime;
        if (_currentPreparationTime <= 0)
        {
            // 1. Nos aseguramos de que el enemigo se quede quieto
            if (_agent.isOnNavMesh)
            {
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero; // Frenado inmediato
            }

            // 2. Activamos el ataque y pasamos a la fase de ataque
            _attackCollider.enabled = true;
            _currentPhase = AttackPhase.Attacking;
        }
    }

    void UpdateAttacking()
    {
        // 3. Simplemente restamos tiempo hasta que el ataque termine
        _currentAttackTime -= Time.deltaTime;
        if (_currentAttackTime <= 0)
        {
            _attackCollider.enabled = false;
            _currentPhase = AttackPhase.Done;
        }
    }

    void updateStunned()
    {
        _currentStunTime -= Time.deltaTime;
        _attackCollider.enabled = false;

        if (_agent.isOnNavMesh)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero; // Frenado inmediato
        }

        if(_currentStunTime <= 0)
        {
            _attackCollider.enabled = false;
            _currentPhase = AttackPhase.Done;
            _data._isStunned = false;
        }
    }
    public void Reset() 
    {
        _currentPreparationTime = _preparationTime;
        _currentAttackTime = _attackDuration;
        _attackCollider.enabled = false;
        _currentPhase = AttackPhase.Preparing;

        // Si tu lógica requiere que el NavMeshAgent vuelva a moverse tras resetear:
        if (_agent.isOnNavMesh)
        {
            _agent.isStopped = false;
        }
    }
}