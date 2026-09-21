using System.Collections;
using UnityEngine;

public class ShaderPreloader : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        StartCoroutine(Load());
    }

    [SerializeField] ShaderVariantCollection variants;

    private IEnumerator Load()
    {

        variants.WarmUp();

        yield return null;

        Destroy(gameObject);
    }

}
