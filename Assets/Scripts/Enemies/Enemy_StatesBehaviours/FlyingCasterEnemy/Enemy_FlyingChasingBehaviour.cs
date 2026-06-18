using UnityEngine;

public class Enemy_FlyingChasingBehaviour
{
    private Transform _transform;
    private Rigidbody _rb;
    private Transform _playerTransform;
    
    private Enemy_FieldOfViewBehaviour _fov;
    private Enemy_ObstacleAvoidanceBehaviour _avoidanceHelper;
    private Enemy_FlyingCasterEnemyData _data; // Inyectado para efectos visuales

    private float _chaseSpeed;
    private float _rotationSpeed;
    private float _attackRange;
    private float _shootRange;  // Para el alcance del láser

    private float _shootDelay;
    private float _shootCooldown;
    private LayerMask _playerLayer;

    private float _hoverHeight;
    private float _retreatMargin;
    private float _normalReactionSpeed;
    private float _evasionReactionSpeed;
    private float _heightCorrectionMultiplier;
    
    private float _minWanderTime;
    private float _maxWanderTime;
    private float _wanderStrafeLimit;
    private float _wanderForwardLimit;

    // Variables de Debug
    private bool _showTargetingRay;
    private bool _showWanderRay;
    private bool _showIdealMovementRay;
    private bool _showAvoidanceRay;
    private bool _showFinalDirectionRay;
    private bool _showVelocityRay;
    private float _aimOffset;
    private float _randomWanderTimer = 0f;
    private Vector3 _currentRandomWander = Vector3.zero;
    private float _lastShootTime = -100f; 
    private bool _isPreparingShot = false;
    private float _prepareTimer = 0f;

    public Enemy_FlyingChasingBehaviour(
        Transform transform, Rigidbody rb, Transform playerTransform, 
        Enemy_FieldOfViewBehaviour fov, Enemy_ObstacleAvoidanceBehaviour avoidanceHelper, 
        float chaseSpeed, float rotationSpeed, float attackRange, float shootRange,
        float shootDelay, float shootCooldown, LayerMask playerLayer,
        float hoverHeight, float retreatMargin, float normalReactionSpeed, float evasionReactionSpeed, float heightCorrectionMultiplier,
        float minWanderTime, float maxWanderTime, float wanderStrafeLimit, float wanderForwardLimit,
        bool showTargetingRay, bool showWanderRay, bool showIdealMovementRay, bool showAvoidanceRay, bool showFinalDirectionRay, bool showVelocityRay, float aimOffset,
        Enemy_FlyingCasterEnemyData data)
    {
        _transform = transform;
        _rb = rb;
        _playerTransform = playerTransform;
        _fov = fov;
        _avoidanceHelper = avoidanceHelper;
        _chaseSpeed = chaseSpeed;
        _rotationSpeed = rotationSpeed;
        _attackRange = attackRange;
        _shootRange = shootRange;
        _shootDelay = shootDelay;
        _shootCooldown = shootCooldown;
        _playerLayer = playerLayer;
        
        _hoverHeight = hoverHeight;
        _retreatMargin = retreatMargin;
        _normalReactionSpeed = normalReactionSpeed;
        _evasionReactionSpeed = evasionReactionSpeed;
        _heightCorrectionMultiplier = heightCorrectionMultiplier;
        _minWanderTime = minWanderTime;
        _maxWanderTime = maxWanderTime;
        _wanderStrafeLimit = wanderStrafeLimit;
        _wanderForwardLimit = wanderForwardLimit;

        _showTargetingRay = showTargetingRay;
        _showWanderRay = showWanderRay;
        _showIdealMovementRay = showIdealMovementRay;
        _showAvoidanceRay = showAvoidanceRay;
        _showFinalDirectionRay = showFinalDirectionRay;
        _showVelocityRay = showVelocityRay;
        _aimOffset = aimOffset;
        _data = data;
    }

    public void EnterChase()
    {
        _rb.useGravity = false;
        _isPreparingShot = false;
        _prepareTimer = 0f;
    }

    public void Tick() 
    {
        if (_playerTransform == null) return;

        Vector3 flatEnemyPos = new Vector3(_transform.position.x, 0, _transform.position.z);
        Vector3 flatPlayerPos = new Vector3(_playerTransform.position.x, 0, _playerTransform.position.z);
        float horizontalDistance = Vector3.Distance(flatEnemyPos, flatPlayerPos);
        
        bool canSeePlayer = _fov.CanseePlayer();

        RotateTowardsTarget();

        // ==========================================
        // 1. CARGA DEL DISPARO Y ACTUALIZACIÓN DEL LÁSER
        // ==========================================
        if (_isPreparingShot)
        {
            _prepareTimer += Time.fixedDeltaTime;

            Vector3 targetCenter = _playerTransform.position + (Vector3.up * _aimOffset);
            Vector3 chargeDirection = (targetCenter - _transform.position).normalized;
            
            // Usamos _shootRange para qué tan lejos llega la mira
            Vector3 chargeEndPoint = _transform.position + (chargeDirection * _shootRange);

            LayerMask aimMask = _playerLayer;
            // Usamos _shootRange en el Raycast de la mira
            if (Physics.Raycast(_transform.position, chargeDirection, out RaycastHit hit, _shootRange, aimMask)) 
            {
                chargeEndPoint = hit.point;
            }

            if (_data != null) _data.SetCharging3DLaser(true, _transform.position, chargeEndPoint);

            if (_prepareTimer >= _shootDelay)
            {
                ExecuteRaycastShoot();
            }
        }

        // ==========================================
        // 2. CEREBRO DE MOVIMIENTO CONSTANTE
        // ==========================================
        float desiredHoverHeight = _hoverHeight;
        
        // Techo Dinámico (Solo usa la máscara de obstáculos interna si es posible, aquí usamos una capa temporal o puedes omitirlo si quieres)
        // Para simplificar, confiamos en la evasión de paredes.

        Vector3 targetPosition = _playerTransform.position + (Vector3.up * desiredHoverHeight);
        Vector3 directionToPlayer = (targetPosition - _transform.position).normalized;
        Vector3 idealMovement = Vector3.zero;

        if (_isPreparingShot)
        {
            idealMovement = Vector3.zero; 
        }
        else
        {
            if (horizontalDistance > _attackRange || !canSeePlayer)
            {
                if (!canSeePlayer)
                {
                    Vector3 searchOrbit = Vector3.Cross(directionToPlayer, Vector3.up).normalized;
                    idealMovement = (directionToPlayer + searchOrbit).normalized;
                }
                else
                {
                    idealMovement = directionToPlayer;
                }
            }
            else if (horizontalDistance < _attackRange - _retreatMargin) 
            {
                idealMovement = -directionToPlayer; 
            }
            else 
            {
                _randomWanderTimer -= Time.fixedDeltaTime;

                if (_randomWanderTimer <= 0f)
                {
                    float randomStrafe = Random.Range(-_wanderStrafeLimit, _wanderStrafeLimit); 
                    float randomForward = Random.Range(-_wanderForwardLimit, _wanderForwardLimit); 

                    Vector3 rightOrbit = Vector3.Cross(directionToPlayer, Vector3.up).normalized;
                    _currentRandomWander = (rightOrbit * randomStrafe) + (directionToPlayer * randomForward);
                    _randomWanderTimer = Random.Range(_minWanderTime, _maxWanderTime);
                }

                float heightError = targetPosition.y - _transform.position.y;
                Vector3 finalWander = _currentRandomWander;
                finalWander.y = heightError * _heightCorrectionMultiplier; 

                idealMovement = finalWander.normalized;
            }
        }

        // ==========================================
        // 3. EVASIÓN Y MOVIMIENTO FINAL
        // ==========================================
        Vector3 avoidance = _avoidanceHelper.GetAvoidanceVector();
        Vector3 finalDirection = (idealMovement + avoidance).normalized;

        float reactionSpeed = avoidance.sqrMagnitude > 0.1f ? _evasionReactionSpeed : _normalReactionSpeed;
        _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, finalDirection * _chaseSpeed, Time.fixedDeltaTime * reactionSpeed);

        if (horizontalDistance <= _attackRange && canSeePlayer && Time.time >= _lastShootTime + _shootCooldown && !_isPreparingShot)
        {
            StartPreparingShot();
        }

        // ==========================================
        // 4. VISUAL DEBUGGING CONTROLADO
        // ==========================================
        Vector3 origin = _transform.position;
        if (_showTargetingRay) Debug.DrawRay(origin, directionToPlayer * 3f, Color.red);
        if (_showWanderRay && _currentRandomWander != Vector3.zero && horizontalDistance <= _attackRange) Debug.DrawRay(origin, _currentRandomWander * 3f, Color.magenta);
        if (_showIdealMovementRay) Debug.DrawRay(origin, idealMovement * 4f, Color.yellow);
        if (_showAvoidanceRay && avoidance.sqrMagnitude > 0.01f) Debug.DrawRay(origin, avoidance * 3f, Color.cyan);
        if (_showFinalDirectionRay) Debug.DrawRay(origin, finalDirection * 5f, Color.white);
        if (_showVelocityRay) Debug.DrawRay(origin, _rb.linearVelocity.normalized * 5f, Color.green);
    }

    public void ExitChase()
    {
        _isPreparingShot = false;
        if (_data != null) _data.SetCharging3DLaser(false);
    }

    private void RotateTowardsTarget()
    {
        // Ahora el enemigo rota su cuerpo físicamente mirando hacia el pecho/centro del jugador
        Vector3 targetCenter = _playerTransform.position + (Vector3.up * _aimOffset);
        Vector3 directionToPlayer = (targetCenter - _transform.position).normalized;
        
        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
        }
    }

    private void StartPreparingShot()
    {
        _isPreparingShot = true;
        _prepareTimer = 0f;
    }
    private void ExecuteRaycastShoot()
    {
        _isPreparingShot = false;
        _lastShootTime = Time.time; 

        if (_data != null) _data.SetCharging3DLaser(false);

        Vector3 targetCenter = _playerTransform.position + (Vector3.up * _aimOffset);
        Vector3 shootDirection = (targetCenter - _transform.position).normalized;
        
        // 1. Si el disparo falla, por defecto llega hasta el rango máximo configurado
        Vector3 laserEndPoint = _transform.position + (shootDirection * _shootRange);

        // 2. Ejecutamos el Raycast de daño normal
        if (Physics.Raycast(_transform.position, shootDirection, out RaycastHit hit, _shootRange, _playerLayer))
        {
            Debug.Log($"PUM! Disparo acertado a: {hit.collider.name}");
            
            // 🎯 LA MAGIA PARA ATRAVESAR:
            // Definimos cuántos metros extra queremos que el láser se extienda más allá del jugador
            float extensionExtra = 8f; 
            
            // El punto final será el punto de impacto MÁS la dirección por la extensión extra
            laserEndPoint = hit.point + (shootDirection * extensionExtra); 
        }

        // 3. Enviamos los puntos al script Data. 
        // Como mantuvimos la división por 2 en el Data, el origen nacerá perfecto en el enemigo
        if (_data != null) _data.Trigger3DShootLaser(_transform.position, laserEndPoint);
    }
}
