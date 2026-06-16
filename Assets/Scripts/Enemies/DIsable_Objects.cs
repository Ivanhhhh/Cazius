using UnityEngine;

public class DIsable_Objects : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToDisable;

    private void OnDestroy()
    {
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}
