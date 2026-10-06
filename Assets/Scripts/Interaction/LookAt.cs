using UnityEngine;

public class LookAt : MonoBehaviour
{
    private GameObject target;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target.transform.position, Vector3.up);
        }
    }
}
