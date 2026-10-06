using System.Collections;
using UnityEngine;

public class DisableObjectEnable : MonoBehaviour
{
    private void OnEnable()
    {
        DisableObject();
    }

    private void DisableObject()
    {
        if (!gameObject.activeSelf) return;

        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(61f);
        gameObject.SetActive(false);
    }
}
