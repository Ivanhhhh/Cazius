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
    [SerializeField] private float _radiusVision;
    [SerializeField] private float _angleVision;
    [SerializeField] private LayerMask _lineOfSightLayerMask;

    [Header("First Attack")]
    [SerializeField] private float _firstAttackDuration;
    [SerializeField] private float _firsAttackPreparationTime;
    [SerializeField] private Collider _attackCollider;

    [Header("Second Attack")]
    [SerializeField] private float _secondAttackPreparationTime;
    [SerializeField] private float _spinTime;
    [SerializeField] private float _objectSpeedWhileSpinning;
    [SerializeField] private float _spinSpeed;

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
        _fieldOfView = new Enemy_FieldOfViewBehaviour(_playerTransform,_radiusVision,_selfObjectTransform, _angleVision, _lineOfSightLayerMask);
        _firstAttack = new Enemy_FirstAttackBehaviour(_firsAttackPreparationTime, _firstAttackDuration,_agent,_attackCollider);
        _secondAttack= new Enemy_SecondAttackBehaviour(_spinSpeed,_objectSpeedWhileSpinning,_secondAttackPreparationTime, _spinTime,_playerTransform,_selfObjectTransform,_agent);
    }

        private void OnDrawGizmosSelected()
    {
        // Radio de visión
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _radiusVision);

        // Ángulo de visión
        Vector3 angleA = DirFromAngle(-_angleVision / 2);
        Vector3 angleB = DirFromAngle(_angleVision / 2);
        Gizmos.DrawLine(transform.position, transform.position + angleA * _radiusVision);
        Gizmos.DrawLine(transform.position, transform.position + angleB * _radiusVision);

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
