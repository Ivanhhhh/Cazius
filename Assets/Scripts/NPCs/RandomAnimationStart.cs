using UnityEngine;

public class RandomAnimationStart : MonoBehaviour
{
    [SerializeField] private string _animationName;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Play _animationName state on layer 0, at a random normalized progress between 0% and 100%
        float randomOffset = Random.Range(0f, 1f);
        animator.Play(_animationName, 0, randomOffset);
    }
}
