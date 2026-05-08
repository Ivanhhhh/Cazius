using UnityEngine;
using System.Collections;

public class AngelDemonAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private float _slowAmount = 2f;
    [SerializeField] private float _slowDuration = 1.5f;

    private Enemy_MeleeEnemy_Data _enemyData;
    private Coroutine _slowCoroutine;
    private float originalSpeed;


    void Start()
    {
        _enemyData = GetComponent<Enemy_MeleeEnemy_Data>();
        originalSpeed = _enemyData.GetChaseSpeed();
        //animator = GetComponentInChildren<Animator>();
    }

    public void WalkAnim(float speed)
    {
        /*animator.SetBool("IsAttacking", false);
        animator.SetBool("IsHeadshot", false);
        animator.SetBool("IsDead", false);*/
    }

    public void AttackAnim()
    {/*
        animator.SetBool("IsAttacking", true);
        animator.SetBool("IsHeadshot", false);
        animator.SetBool("IsDead", false);*/
        animator.SetBool("Attacking", true);
        animator.SetTrigger("Attack");
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

    public void AttackFalse()
    {
        animator.SetBool("Attacking", false);
    }

    private void SlowChaseSpeed()
    {
        if (_enemyData == null) return;

        if (_slowCoroutine != null)
        {
            StopCoroutine(_slowCoroutine);
        }

        _slowCoroutine = StartCoroutine(SlowChaseSpeedCoroutine());
    }

    private IEnumerator SlowChaseSpeedCoroutine()
    {

        float slowedSpeed = Mathf.Max(0f, originalSpeed - _slowAmount);

        _enemyData.SetChaseSpeed(slowedSpeed);

        yield return new WaitForSeconds(_slowDuration);

        _enemyData.SetChaseSpeed(originalSpeed);

        _slowCoroutine = null;
    }
}
