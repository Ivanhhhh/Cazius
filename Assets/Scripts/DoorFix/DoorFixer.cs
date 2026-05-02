using System.Collections;
using UnityEngine;

public class DoorFixer : MonoBehaviour
{
    private HingeJoint _joint;

    [SerializeField] float SpringAmount;

    [SerializeField] float LerpDuration;

    [SerializeField] int DamperObjetiveAmount;

    [SerializeField] int SpringObjetiveAmount;

    private JointSpring SpringValue;

    bool Flag = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _joint.useSpring = false;

            SpringValue = _joint.spring;
            SpringValue.spring = SpringAmount*0;
            _joint.spring = SpringValue;

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
       // {
            

       // }
    }
    void Start()
    {
       _joint = GetComponent<HingeJoint>(); 
    }

    void Update()
    {
        float angulo = Mathf.Abs(_joint.angle);

        if (angulo < 20f)  // 20 grados del centro
        {
            DamperObjetiveAmount = 300;
            SpringObjetiveAmount = 200;

            if (Flag == true)
            {
                StartCoroutine(SpringLerp());
                StartCoroutine(DamperLerp());
            }
            Flag = false;
           
        }

        if (angulo > 20f)
        {
            StopAllCoroutines();
            Flag = true;

            DamperObjetiveAmount = 0;
            SpringObjetiveAmount = 0;
        }

    }

    private IEnumerator SpringLerp()
    {
        for (float t = 0; t < 1; t += Time.deltaTime/LerpDuration)
        {
            _joint.useSpring = true;
            float resultado = Mathf.Lerp(0, SpringObjetiveAmount, t);
            SpringAmount = resultado;
            SpringValue.spring = resultado;
            yield return null;
        }
    }

    private IEnumerator DamperLerp()
    {
        for (float t = 0; t < 1; t += Time.deltaTime / LerpDuration)
        {
            float resultado = Mathf.Lerp(0, DamperObjetiveAmount, t);
            SpringValue.damper = resultado;
            _joint.spring = SpringValue; 
            yield return null;
        }
    }

    public void StopDoor()
    {
        _joint.useSpring = true;
        StartCoroutine(SpringLerp());
        SpringValue = _joint.spring;
        SpringValue.spring = SpringAmount;
        _joint.spring = SpringValue;
    }
}

