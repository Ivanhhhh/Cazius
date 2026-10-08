using System.Collections;
using UnityEngine;

public class DisableObjectEnable : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(57f);
        gameObject.SetActive(false);
    }

}
