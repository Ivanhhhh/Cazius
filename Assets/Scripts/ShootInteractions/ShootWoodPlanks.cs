using System.Collections.Generic;
using UnityEngine;

public class ShootWoodPlanks : MonoBehaviour, IShootable
{

    [SerializeField] GameObject[] _woodPlanks;
    [SerializeField] float _planksForce = 5f;
    [SerializeField] Collider[] _colliders;

    [SerializeField] GameObject[] _particles;
    
    private List<Rigidbody> _woodRb = new List<Rigidbody>();

    private void Start()
    {
        foreach (var par in _particles)
        {
            par.SetActive(false);
        }

        foreach (GameObject go in _woodPlanks)
        {
            _woodRb.Add(go.GetComponent<Rigidbody>());
        }

        foreach (Rigidbody rb in _woodRb)
        {
            rb.isKinematic = true;
        }
    }
    public void TakeDamage(float damage)
    {
        Vector3 direction = (transform.position - GameManager.Instance.playerModelTransform.position).normalized;

        foreach (Rigidbody rb in _woodRb)
        {
            rb.isKinematic = false;

            rb.AddForce(direction * _planksForce, ForceMode.Impulse);
        }

        foreach (Collider collider in _colliders)
        {
            collider.enabled = false;
        }

        foreach (var par in _particles)
        {
            par.SetActive(true);
            if (par.GetComponent<ParticleSystem>() != null) { par.GetComponent<ParticleSystem>().Play(); }
        }
    }
}
