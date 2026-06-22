using UnityEngine;

public class EnableObjectOnDisable : MonoBehaviour
{
    [SerializeField] private GameObject objectToEnable;

    private void OnDisable()
    {
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }
    }
}