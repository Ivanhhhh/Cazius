using UnityEngine;
using System.Collections;

public class AngelDemonAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Slow Effect")]
    [SerializeField] private float _slowAmount = 2f;
    [SerializeField] private float _slowDuration = 1.5f;
    [SerializeField] private float _flyDistanceThreshold;

    private Enemy_MeleeEnemy_Data _enemyData;
    private Coroutine _slowCoroutine;
    private Transform _playerTransform;

    void Start()
    {
        StartCoroutine(WaitForPlayer());
        _enemyData = GetComponent<Enemy_MeleeEnemy_Data>();

        if (_enemyData == null)
            Debug.LogError($"{nameof(AngelDemonAnim)}: No se encontró Enemy_MeleeEnemy_Data en el mismo GameObject.");

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }
    private IEnumerator WaitForPlayer()
    {
        while (_playerTransform == null)
        {
            var player = FindFirstObjectByType<PlayerMovement>();
            if (player != null)
            {
                _playerTransform = player.transform;
                yield break;
            }

            yield return null;
        }
    }

    // ====== ANIMACIONES ======

    public void WalkAnim(float speed)
    {
        // (Opcional) Aquí podrías controlar el blend tree de caminata usando 'speed'
        // animator.SetFloat("Speed", speed);
    }

    public void AttackAnim()
    {
        animator.SetBool("Attacking", true);
        animator.SetTrigger("Attack");
    }

    public void AttackFalse()
    {
        animator.SetBool("Attacking", false);
    }

    public void HeadshotAnim()
    {
        SlowChaseSpeed();
        animator.SetTrigger("Headshot");
    }

    public void ChestAnim()
    {
        SlowChaseSpeed();
        animator.SetTrigger("Chest");
    }

    public void LeftArmAnim()
    {
        SlowChaseSpeed();
        animator.SetTrigger("LeftArm");
    }

    public void RightArmAnim()
    {
        SlowChaseSpeed();
        animator.SetTrigger("RightArm");
    }

    public void RightLegAnim()
    {
        SlowChaseSpeed();
        animator.SetTrigger("RightLeg");
    }

    public void LeftLegAnim()
    {
        SlowChaseSpeed();
        animator.SetTrigger("LeftLeg");
    }

    public void DieAnim()
    {
        SlowChaseSpeed();
        animator.SetBool("Dead", true);
        animator.SetTrigger("Die");
    }

    // ====== SLOW ======

    private void SlowChaseSpeed()
    {
        if (_enemyData == null) return;

        // Si ya hay un slow activo, lo reiniciamos
        if (_slowCoroutine != null)
            StopCoroutine(_slowCoroutine);

        _slowCoroutine = StartCoroutine(SlowChaseSpeedCoroutine());
    }

    private IEnumerator SlowChaseSpeedCoroutine()
    {
        if (_playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            bool shouldBoost = distance > _flyDistanceThreshold;

            if (shouldBoost == false)
            {
                // Registrar el modificador aditivo negativo identificado por 'this'
                _enemyData.AddSpeedAdditive(this, -_slowAmount);

                yield return new WaitForSeconds(_slowDuration);

                // Quitar el modificador (el enemy data recalcula la velocidad final)
                _enemyData.RemoveSpeedAdditive(this);
            }
        }
        _slowCoroutine = null;
    }

    private void OnDisable()
    {
        // Seguridad: si el objeto se desactiva con un slow activo, lo limpiamos
        if (_enemyData != null)
            _enemyData.RemoveSpeedAdditive(this);

        if (_slowCoroutine != null)
        {
            StopCoroutine(_slowCoroutine);
            _slowCoroutine = null;
        }
    }
}