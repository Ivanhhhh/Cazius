
using UnityEngine;

public class TriggerEnemySequence : MonoBehaviour
{
    [SerializeField] private FlyingEnemyPath _enemy;

    [SerializeField] private bool _triggerOnlyOnce = true;

    private bool _triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggerOnlyOnce && _triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (_enemy == null)
            return;

        _triggered = true;

        _enemy.Play();
    }
}
