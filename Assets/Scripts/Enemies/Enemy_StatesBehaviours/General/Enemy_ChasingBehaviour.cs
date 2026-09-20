using UnityEngine;
using UnityEngine.AI;

public class Enemy_ChasingBehaviour
{
    private Transform _playerTransform;
    private Transform _selfTransform;
    private float _attackRadius;
    private float _chaseSpeed;
    private NavMeshAgent _agent;

    // --- NUEVAS VARIABLES PARA EL COOLDOWN ---
    private float _attackCooldown; 
    private float _lastAttackTime = -100f; // Inicializado en negativo para que pueda atacar al instante la primera vez

    // Mantenemos la distancia original
    public bool _isNearToAttack => Vector3.Distance(_selfTransform.position, _playerTransform.position) <= _attackRadius;
    
    // NUEVA PROPIEDAD: Chequea la distancia Y el tiempo de espera
    public bool CanAttack => _isNearToAttack && (Time.time >= _lastAttackTime + _attackCooldown);

    // Agregamos el cooldown al constructor
    public Enemy_ChasingBehaviour(Transform playerTransform, float attackRadius, NavMeshAgent agent, Transform selfTransform, float chaseSpeed, float attackCooldown)
    {
        _playerTransform = playerTransform;
        _selfTransform = selfTransform;
        _attackRadius = attackRadius;
        _agent = agent;
        _chaseSpeed = chaseSpeed;
        _attackCooldown = attackCooldown;
    }   

    public void EnterChase()
    {
        _agent.speed = _chaseSpeed;
        _agent.acceleration = 50f;
    }

    public void Tick()
    {
        _agent.speed = _chaseSpeed;
        _agent.SetDestination(_playerTransform.position);
    }

    public void SetChaseSpeed(float newSpeed)
    {
        _chaseSpeed = newSpeed;
    }

    // --- MÉTODO PARA REINICIAR EL COOLDOWN ---
    // Esto lo vas a llamar justo cuando termine de atacar
    public void ResetAttackCooldown()
    {
        _lastAttackTime = Time.time;
    }
}
