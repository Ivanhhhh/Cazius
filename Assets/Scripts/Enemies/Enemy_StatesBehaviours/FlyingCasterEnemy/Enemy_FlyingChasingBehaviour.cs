using UnityEngine;

public abstract class Enemy_FlyingChasingBehaviour
{
    protected Transform _transform;
    protected Rigidbody _rb;
    protected Transform _playerTransform;
    
    protected Enemy_FieldOfViewBehaviour _fov;
    protected Enemy_ObstacleAvoidanceBehaviour _avoidanceHelper;
    protected FlyingEnemyStatsSO _stats; 

    protected float _randomWanderTimer = 0f;
    protected Vector3 _currentRandomWander = Vector3.zero;
    protected float _lastShootTime = -100f; 

    public Enemy_FlyingChasingBehaviour(Transform transform, Rigidbody rb, Transform playerTransform, 
        Enemy_FieldOfViewBehaviour fov, Enemy_ObstacleAvoidanceBehaviour avoidanceHelper, FlyingEnemyStatsSO stats)
    {
        _transform = transform;
        _rb = rb;
        _playerTransform = playerTransform;
        _fov = fov;
        _avoidanceHelper = avoidanceHelper;
        _stats = stats;
    }

    public virtual void EnterChase() => _rb.useGravity = false;
    public virtual void ExitChase() { }

    public void Tick() 
    {
        if (_playerTransform == null) return;

        Vector3 flatEnemyPos = new Vector3(_transform.position.x, 0, _transform.position.z);
        Vector3 flatPlayerPos = new Vector3(_playerTransform.position.x, 0, _playerTransform.position.z);
        float horizontalDistance = Vector3.Distance(flatEnemyPos, flatPlayerPos);
        bool canSeePlayer = _fov.CanseePlayer();

        RotateTowardsTarget();

        // 1. MOVIMIENTO EVASIVO Y ORBITAL
        Vector3 targetPosition = _playerTransform.position + (Vector3.up * _stats.hoverHeight);
        Vector3 directionToPlayer = (targetPosition - _transform.position).normalized;
        Vector3 idealMovement = Vector3.zero;

        if (horizontalDistance > _stats.attackRange || !canSeePlayer)
        {
            if (!canSeePlayer)
            {
                Vector3 searchOrbit = Vector3.Cross(directionToPlayer, Vector3.up).normalized;
                idealMovement = (directionToPlayer + searchOrbit).normalized;
            }
            else idealMovement = directionToPlayer;
        }
        else if (horizontalDistance < _stats.attackRange - _stats.retreatMargin) 
        {
            idealMovement = -directionToPlayer; 
        }
        else 
        {
            _randomWanderTimer -= Time.fixedDeltaTime;
            if (_randomWanderTimer <= 0f)
            {
                float randomStrafe = Random.Range(-_stats.wanderStrafeLimit, _stats.wanderStrafeLimit); 
                float randomForward = Random.Range(-_stats.wanderForwardLimit, _stats.wanderForwardLimit); 
                Vector3 rightOrbit = Vector3.Cross(directionToPlayer, Vector3.up).normalized;
                _currentRandomWander = (rightOrbit * randomStrafe) + (directionToPlayer * randomForward);
                _randomWanderTimer = Random.Range(_stats.minWanderTime, _stats.maxWanderTime);
            }
            float heightError = targetPosition.y - _transform.position.y;
            Vector3 finalWander = _currentRandomWander;
            finalWander.y = heightError * _stats.heightCorrectionMultiplier; 
            idealMovement = finalWander.normalized;
        }

        Vector3 avoidance = _avoidanceHelper.GetAvoidanceVector();
        Vector3 finalDirection = (idealMovement + avoidance).normalized;
        float reactionSpeed = avoidance.sqrMagnitude > 0.1f ? _stats.evasionReactionSpeed : _stats.normalReactionSpeed;
        _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, finalDirection * _stats.chaseSpeed, Time.fixedDeltaTime * reactionSpeed);

        // 2. DETECCIÓN DE ATAQUE
        if (horizontalDistance <= _stats.attackRange && canSeePlayer && Time.time >= _lastShootTime + _stats.shootCooldown)
        {
            _lastShootTime = Time.time;
            ExecuteAttack(); 
        }

        // 3. DEBUG RAYS
        Vector3 origin = _transform.position;
        if (_stats.showTargetingRay) Debug.DrawRay(origin, directionToPlayer * 3f, Color.red);
        if (_stats.showWanderRay && _currentRandomWander != Vector3.zero && horizontalDistance <= _stats.attackRange) Debug.DrawRay(origin, _currentRandomWander * 3f, Color.magenta);
        if (_stats.showIdealMovementRay) Debug.DrawRay(origin, idealMovement * 4f, Color.yellow);
        if (_stats.showAvoidanceRay && avoidance.sqrMagnitude > 0.01f) Debug.DrawRay(origin, avoidance * 3f, Color.cyan);
        if (_stats.showFinalDirectionRay) Debug.DrawRay(origin, finalDirection * 5f, Color.white);
        if (_stats.showVelocityRay) Debug.DrawRay(origin, _rb.linearVelocity.normalized * 5f, Color.green);
    }

    private void RotateTowardsTarget()
    {
        Vector3 targetCenter = _playerTransform.position + (Vector3.up * _stats.aimOffset);
        Vector3 directionToPlayer = (targetCenter - _transform.position).normalized;
        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.fixedDeltaTime * _stats.rotationSpeed);
        }
    }

    // Contrato para los hijos
    protected abstract void ExecuteAttack();
}