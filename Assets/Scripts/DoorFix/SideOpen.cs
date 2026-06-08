using UnityEngine;

public class SideOpen : MonoBehaviour
{
    [SerializeField] Collider _SideOpen;
    [SerializeField] Animator _Anim;
    public bool Opened;

    private void OnTriggerEnter(Collider other)
    {
        Opened = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Opened = false;
    }

    private void OnTriggerStay(Collider other)
    {
        Opened = true;
    }
}
