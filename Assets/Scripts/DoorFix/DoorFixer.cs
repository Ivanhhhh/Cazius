using UnityEngine;

public class DoorFixer : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            JointSpring SpringValue = _joint.spring;
            SpringValue.spring = 0f;
            _joint.spring = SpringValue;

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            JointSpring SpringValue = _joint.spring;
            SpringValue.spring = 80f;
            _joint.spring = SpringValue;

        }
    }
    private HingeJoint _joint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       _joint = GetComponent<HingeJoint>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
