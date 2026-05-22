using System.Collections;
using UnityEngine;

public class DoorHelper1 : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;

    [SerializeField] float _ForceApply;

    [SerializeField] Collider ColliderDoor;


  //  [SerializeField] GameObject ApplyForcePosition;


    bool Run = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && Run == true)
        {
            StartCoroutine(Stopper());
        }

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public IEnumerator Stopper()
    {
        Run = false;
        print(Run);

       //_rb.AddForceAtPosition(new Vector3(0, 0, _ForceApply), ApplyForcePosition.transform.position);

        _rb.AddForce(transform.forward * _ForceApply, ForceMode.Impulse);
        ColliderDoor.isTrigger = true;
        print("se hizo la fuerza de + transform.forward");

        yield return new WaitForSeconds(1);
        Run = true;
        print(Run);


    }
}
