using System.Collections;
using UnityEngine;

public class DisableObjectEnable : MonoBehaviour
{
    [SerializeField] private bool _useOnce;
    private bool _activated;
    private void OnEnable()
    {
        if (_useOnce && _activated) 
        {
            gameObject.SetActive(false);
            return; 
        }
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        _activated = true;
        yield return new WaitForSeconds(57f);
        gameObject.SetActive(false);
    }

}
