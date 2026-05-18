using UnityEngine;

public class InteractionUI : MonoBehaviour 
    // Lo tienen los "Door Icon Lock/Unlock" tienen q estar apagados y el Interactive Icon prendido
{
    [SerializeField] private Transform _target;

    void Update()
    {
        transform.LookAt(_target);
    }
}
