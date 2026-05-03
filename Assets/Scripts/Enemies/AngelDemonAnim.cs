using UnityEngine;

public class AngelDemonAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;

 

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void WalkAnim(float speed)
    {
        /*animator.SetBool("IsAttacking", false);
        animator.SetBool("IsHeadshot", false);
        animator.SetBool("IsDead", false);*/
    }

    public void AttackAnim()
    {
        animator.SetBool("IsAttacking", true);
        animator.SetBool("IsHeadshot", false);
        animator.SetBool("IsDead", false);
    }

    public void HeadshotAnim()
    {
        animator.SetTrigger("Headshot");
    }

    public void DieAnim()
    {
        //animator.SetBool("IsAttacking", true);
    }

}
