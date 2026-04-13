using UnityEngine;
using UnityEngine.AI;

public class Enemy_SecondAttackBehaviour
{
    private float _preparationTime;
    private float _spinTime;
    private float _currentSpinTime;
    private float _currentPreparationTime;
    private Transform _playerTransform;
    private Transform _objectTransform;
    private NavMeshAgent _agent;
    private float _objectSpeedWhileSpinning;
    private float _spinSpeed;
    private enum AttackPhase {Preparing,Spinning,Done};
    private AttackPhase _currentPhase = AttackPhase.Preparing;
    public bool IsDone => _currentPhase == AttackPhase.Done;
    public Enemy_SecondAttackBehaviour(float spinSpeed, float objectSpeedWhileSpinning, float preparationTime,float spinTime,
    Transform playerTransform, Transform objectTransform, NavMeshAgent agent)
    {
        _spinSpeed = spinSpeed;
        _objectSpeedWhileSpinning = objectSpeedWhileSpinning;
        _preparationTime = preparationTime;
        _spinTime = spinTime;
        _currentPreparationTime = preparationTime;
        _currentSpinTime = _spinTime;
        _playerTransform = playerTransform;
        _objectTransform = objectTransform;
        _agent = agent;
    }
    public void Tick()
    {
        switch (_currentPhase)
        {
            case AttackPhase.Preparing: UpdatePreparing(); break;
            case AttackPhase.Spinning: UpdateSpinning(); break;
        }
    }
    void UpdatePreparing()
    {
        Debug.Log("Preparing Spin");
        _currentPreparationTime -= Time.deltaTime;
        if (_currentPreparationTime <= 0)
        {
            Debug.Log("Spin");
            _agent.speed = _objectSpeedWhileSpinning;
            _agent.updateRotation = false;
            _currentPhase = AttackPhase.Spinning;
        }
    }
    void UpdateSpinning()
    {
        _currentSpinTime -= Time.deltaTime;
        _objectTransform.Rotate(Vector3.up * _spinSpeed * Time.deltaTime);
        _agent.SetDestination(_playerTransform.position);

        if (_currentSpinTime <= 0)
        {
            _currentPhase = AttackPhase.Done;
            _agent.updateRotation = true;
        }
    }
    public void Reset()
    {
        _currentPreparationTime = _preparationTime;
        _currentSpinTime = _spinTime;
        _currentPhase = AttackPhase.Preparing;
    }
}
