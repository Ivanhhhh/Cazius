using System.Collections;
using UnityEngine;

public class DoorHelper : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;

    [SerializeField] float _ForceApply;

   // IEnumerator myCoroutine;

    bool Run = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && Run == true)
        {
            StartCoroutine(Stopper());
            print("se hizo la fuerza");
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Stopper()
    {
        Run = false;
        print(Run);
        //myCoroutine = Stopper();
        _rb.AddForceAtPosition(new Vector3(0, 0, _ForceApply), this.transform.forward);

        yield return new WaitForSeconds(3);
        Run = true;
        print(Run);


        // print (myCoroutine);

    }
}
