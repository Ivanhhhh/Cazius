using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy_MeleeEnemy_Data : MonoBehaviour
{
 [Header("Patrolling")]
    [SerializeField] private LayerMask _nodes;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _patrolSpeed;
    [SerializeField] private int _MaximumAmountOfNodes;

    [Header("Chasing")]
    [SerializeField] private float _attackRadius;
    [SerializeField] private float _chaseSpeed;
    [SerializeField] private float _attackCooldown;

    [Header("Field of View")]
    [SerializeField] private float _radiusVision = 15f;
    [Range(0, 360)] [SerializeField] private float _horizontalAngleVision = 90f;
    [Range(0, 360)] [SerializeField] private float _verticalAngleVision = 180f;
    [SerializeField] private LayerMask _lineOfSightLayerMask;
    [SerializeField] private float _aimOffset;

    [Header("First Attack")]
    [SerializeField] private float _firstAttackDuration;
    [SerializeField] private float _firsAttackPreparationTime;
    [SerializeField] private Collider _attackCollider;

    [Header("Second Attack")]
    [SerializeField] private float _secondAttackPreparationTime;
    [SerializeField] private float _spinTime;
    [SerializeField] private float _objectSpeedWhileSpinning;
    [SerializeField] private float _spinSpeed;
    
    [Header("Stun")]
    [SerializeField] private float _stunDuration;
    public bool _isStunned;

    [Header("General")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _selfObjectTransform;
    [SerializeField] private NavMeshAgent _agent;
    [Header("Health")]
    [SerializeField] public Enemy_HealthSystem _healthSystem;

    [Header("Behaviours")]
    public Enemy_PatrollingBehaviour _patrolling { get; private set; }
    public Enemy_ChasingBehaviour _chasing { get; private set; }
    public Enemy_FieldOfViewBehaviour _fieldOfView { get; private set; }
    public Enemy_FirstAttackBehaviour _firstAttack { get; private set; }
    public Enemy_SecondAttackBehaviour _secondAttack { get; private set; }
    void Awake()
    {
        //_playerTransform = FindFirstObjectByType<PlayerMovement>().transform;
        //InitializeBehaviours(); 
        StartCoroutine(WaitForPlayer());
    }
    private IEnumerator WaitForPlayer()
    {
        while (_playerTransform == null)
        {
            var player = FindFirstObjectByType<PlayerMovement>();
            if (player != null)
            {
                _playerTransform = player.transform;
                InitializeBehaviours();
                yield break;
            }

            yield return null;
        }
    }

        void InitializeBehaviours()
    {
        _patrolling = new Enemy_PatrollingBehaviour(_nodes,_detectionRadius,_MaximumAmountOfNodes,_patrolSpeed,_selfObjectTransform,_agent,_healthSystem);
        _chasing = new Enemy_ChasingBehaviour(_playerTransform,_attackRadius,_agent,_selfObjectTransform,_chaseSpeed,_attackCooldown);
        _fieldOfView = new Enemy_FieldOfViewBehaviour(_playerTransform, _radiusVision, _selfObjectTransform, _horizontalAngleVision, _verticalAngleVision, _lineOfSightLayerMask,_aimOffset);        
        _firstAttack = new Enemy_FirstAttackBehaviour(_firsAttackPreparationTime, _firstAttackDuration, _stunDuration,_agent,_attackCollider, this);
        _secondAttack= new Enemy_SecondAttackBehaviour(_spinSpeed,_objectSpeedWhileSpinning,_secondAttackPreparationTime, _spinTime,_playerTransform,_selfObjectTransform,_agent);
    }

    private void OnDrawGizmosSelected()
    {
        // Radio de visión
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _radiusVision);

        // ==========================================
        // DIBUJAR EL CONO DE VISIÓN 3D REAL
        // ==========================================
        
        // Aristas Horizontales (Usamos Quaternion en el eje Y)
        Vector3 angleLeft = transform.rotation * Quaternion.Euler(0, -_horizontalAngleVision / 2, 0) * Vector3.forward;
        Vector3 angleRight = transform.rotation * Quaternion.Euler(0, _horizontalAngleVision / 2, 0) * Vector3.forward;

        // Aristas Verticales (Usamos Quaternion en el eje X)
        Vector3 angleUp = transform.rotation * Quaternion.Euler(-_verticalAngleVision / 2, 0, 0) * Vector3.forward;
        Vector3 angleDown = transform.rotation * Quaternion.Euler(_verticalAngleVision / 2, 0, 0) * Vector3.forward;

        // Dibujamos las líneas Horizontales
        Gizmos.DrawLine(transform.position, transform.position + angleLeft * _radiusVision);
        Gizmos.DrawLine(transform.position, transform.position + angleRight * _radiusVision);

        // Dibujamos las líneas Verticales con un color más suave
        Gizmos.color = new Color(1, 1, 1, 0.4f);
        Gizmos.DrawLine(transform.position, transform.position + angleUp * _radiusVision);
        Gizmos.DrawLine(transform.position, transform.position + angleDown * _radiusVision);

        // Radio de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);

        // Radio de detección de nodos
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
    private Vector3 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.y;
        return new Vector3(
            Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),
            0,
            Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)
        );
    }

    public float GetChaseSpeed()
    {
        return _chaseSpeed;
    }

    public void SetChaseSpeed(float newSpeed)
    {
        _chaseSpeed = newSpeed;

        if (_chasing != null)
        {
            _chasing.SetChaseSpeed(newSpeed);
        }
    }
}
