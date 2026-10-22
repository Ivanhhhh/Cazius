using UnityEngine;

public class ShieldAnimation : MonoBehaviour
{
    [SerializeField] private Animator animatior;

    private bool isEnable = false;

    void OnTriggerEnter(Collider other)
    {
        if (isEnable) return;

        if (other.CompareTag("Player"))
        {
            isEnable = true;

            animatior.SetTrigger("Enable");
        }
    }
}
