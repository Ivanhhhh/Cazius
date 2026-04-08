using UnityEngine;
using UnityEngine.AI;

public class Enemy_SecondAttackBehaviour
{
    private float _preparationTime;
    private float _spinTIme;
    private float _currentSpinTime;
    private float _currentPreparationTime;
    private Transform _playerTransform;
    private Transform _objectTransform;
    private NavMeshAgent _agent;
    private float _previousSpeed;
    private float _objectSpeed;
    private float _spinSpeed;
    private float _attackDuration;
    private enum AttackPhase {Preparing,Spinning,Done};
    private AttackPhase _currentPhase = AttackPhase.Preparing;
    public bool IsDone => _currentPhase == AttackPhase.Done;
    public Enemy_SecondAttackBehaviour()
    {
        
    }
    void Tick()
    {
        switch (_currentPhase)
        {
            case AttackPhase.Preparing: UpdatePreparing(); break;
            case AttackPhase.Spinning: UpdateSpinning(); break;
        }


    }
    void UpdatePreparing()
    {
        _currentPreparationTime -= Time.deltaTime;
        if (_currentPreparationTime <= 0)
        {
            _agent.speed = _objectSpeed;
            _agent.updateRotation = false;
            _objectTransform.Rotate(Vector3.up * _spinSpeed * Time.deltaTime);
            _currentPhase = AttackPhase.Spinning;
        }
    }
    void UpdateSpinning()
    {
        _currentSpinTime -= Time.deltaTime;
        if (_currentSpinTime <= 0)
        {
            _agent.speed = _previousSpeed;
            _currentPhase = AttackPhase.Done;
            _agent.updateRotation = false;
        }
    }
    void Reset()
    {
        _currentPreparationTime = _preparationTime;
        _currentSpinTime = _spinTIme;
        _currentPhase = AttackPhase.Preparing;
    }




}
