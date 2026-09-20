using System.Collections;
using UnityEngine;

public class SpheresScan : MonoBehaviour
{

    [SerializeField] private MeshRenderer _sphere;
    [SerializeField] private Transform _pivot;
    [SerializeField] private bool _usePivot;

    private void Start()
    {
        _sphere = this.GetComponent<MeshRenderer>();
        _sphere.enabled = false;

        if (_pivot == null)
        {
            _pivot = GameObject.FindWithTag("Player").transform;
        }
    }

    private void Update()
    {
        if (_usePivot)
        {
            transform.position = _pivot.position;
        }
    }

    public void Grow(float growTime, float scaleAmount)
    {
        StartCoroutine(ScaleOverTime(growTime, scaleAmount, true));
    }

    public void Contract(float contractTime, float scaleAmount)
    {
        StartCoroutine(ScaleOverTime(contractTime, scaleAmount, false));
    }


    private IEnumerator ScaleOverTime(float time, float targetScale, bool appear)
    {
        if (appear)
        {
            _sphere.enabled = true;
        }

        Vector3 startScale = transform.localScale;

        Vector3 endScale = new Vector3(targetScale, targetScale, targetScale);

        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.unscaledDeltaTime;

            transform.localScale  = Vector3.Lerp(startScale, endScale, elapsedTime / time);

            yield return null;
        }

        transform.localScale = endScale;

        if (!appear)
        {
            _sphere.enabled = false;
        }
    }
}
