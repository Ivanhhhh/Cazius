using UnityEngine;

public class ObjectsActivator : MonoBehaviour
{
    [SerializeField] private GameObject[] _ObjectsToActivate;

    public void Activate()
    {
        foreach (GameObject objects in _ObjectsToActivate)
        {
            objects.SetActive(true);
        }
    }
}
