using UnityEngine;
using System.Collections;

public class AngelDemonAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Slow Effect")]
    [SerializeField] private float _slowAmount = 2f;
    [SerializeField] private float _slowDuration = 1.5f;

    private Enemy_MeleeEnemy_Data _enemyData;
    private Coroutine _slowCoroutine;
    private Transform _playerTransform;

    void Start()
    {
        StartCoroutine(WaitForPlayer());
        _enemyData = GetComponent<Enemy_MeleeEnemy_Data>();

        if (_enemyData == null)
            Debug.LogError($"{nameof(AngelDemonAnim)}: No se encontró Enemy_MeleeEnemy_Data.");
        else
            _enemyData._onBoostActivated += CancelSlow;   // suscripción al evento

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void OnDestroy()
    {
        if (_enemyData != null)
            _enemyData._onBoostActivated -= CancelSlow;   // desuscripción
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
        // (Opcional) blend tree de caminata
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

    public void HeadshotAnim() { SlowChaseSpeed(); animator.SetTrigger("Headshot"); }
    public void ChestAnim() { SlowChaseSpeed(); animator.SetTrigger("Chest"); }
    public void LeftArmAnim() { SlowChaseSpeed(); animator.SetTrigger("LeftArm"); }
    public void RightArmAnim() { SlowChaseSpeed(); animator.SetTrigger("RightArm"); }
    public void RightLegAnim() { SlowChaseSpeed(); animator.SetTrigger("RightLeg"); }
    public void LeftLegAnim() { SlowChaseSpeed(); animator.SetTrigger("LeftLeg"); }

    public void DieAnim()
    {
        animator.SetBool("Dead", true);
        animator.SetTrigger("Die");
    }

    public void DeactivateEnemy()
    {
        _enemyData.AttackCollider.enabled = false;
        _enemyData.SetChaseSpeed(0f);
    }

    // ====== SLOW ======

    private void SlowChaseSpeed()
    {
        if (_enemyData == null) return;

        // ✅ Chequeo con el flag local, sin depender de _speedBoost
        if (_enemyData.IsBoostActive)
        {
            Debug.Log("[Slow] Ignorado: boost activo");
            return;
        }

        if (_slowCoroutine != null)
            StopCoroutine(_slowCoroutine);

        _slowCoroutine = StartCoroutine(SlowChaseSpeedCoroutine());
    }

    private IEnumerator SlowChaseSpeedCoroutine()
    {
        // El diccionario de Enemy_MeleeEnemy_Data rechaza el negativo si hay boost,
        // pero por las dudas ya chequeamos arriba.
        _enemyData.AddSpeedAdditive(this, -_slowAmount);

        yield return new WaitForSeconds(_slowDuration);

        _enemyData.RemoveSpeedAdditive(this);
        _slowCoroutine = null;
    }

    /// <summary>
    /// Cancela el slow actual. Es llamado por el evento _onBoostActivated.
    /// Idempotente: se puede llamar sin slow activo.
    /// </summary>
    public void CancelSlow()
    {
        if (_slowCoroutine != null)
        {
            StopCoroutine(_slowCoroutine);
            _slowCoroutine = null;
        }

        if (_enemyData != null)
            _enemyData.RemoveSpeedAdditive(this);
    }

    private void OnDisable()
    {
        if (_enemyData != null)
            _enemyData.RemoveSpeedAdditive(this);

        if (_slowCoroutine != null)
        {
            StopCoroutine(_slowCoroutine);
            _slowCoroutine = null;
        }
    }
}