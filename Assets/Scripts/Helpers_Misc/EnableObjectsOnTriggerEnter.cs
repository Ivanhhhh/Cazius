using UnityEngine;

public class EnableObjectsOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private string _targetTag = "Player";
    [SerializeField] private bool _onlyTriggerOnce = true;

    [SerializeField] private GameObject[] _objectsToEnable;

    private bool _hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (_onlyTriggerOnce && _hasTriggered)
            return;

        if (!other.CompareTag(_targetTag))
            return;

        EnableObjects();

        _hasTriggered = true;
    }

    private void EnableObjects()
    {
        foreach (GameObject obj in _objectsToEnable)
        {
            if (obj == null)
                continue;

            obj.SetActive(true);
        }
    }
}