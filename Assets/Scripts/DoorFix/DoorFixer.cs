using System.Collections;
using UnityEngine;

public class DoorFixer : MonoBehaviour
{
    [SerializeField] HingeJoint _joint;

    [SerializeField] float SpringAmount;

    [SerializeField] float LerpDuration;

    [SerializeField] int DamperObjetiveAmount;

    [SerializeField] int SpringObjetiveAmount;

  //  [SerializeField] Collider StopFreining;

    private JointSpring SpringValue;

    bool Flag = false;

    bool Stop;

  // [SerializeField] DoorStop _DoorStop;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
           
            StopDamper();

            // StartCoroutine(SpringLerp());
            // StartCoroutine(DamperLerp());

            print("empezaron todas las corrutinas");

            // SpringValue = _joint.spring;
            // SpringValue.spring = SpringAmount*0;
            // _joint.spring = SpringValue;



        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {

            StopDamper();

            // StartCoroutine(SpringLerp());
            // StartCoroutine(DamperLerp());

            print("empezaron todas las corrutinas");

            // SpringValue = _joint.spring;
            // SpringValue.spring = SpringAmount*0;
            // _joint.spring = SpringValue;



        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {

            StartDamper();
           // Stop = true;

           // if (_DoorStop.Stop == true)
           // {
               

           // }
          //  Flag = false;
            //StopAllCoroutines();
           // _joint.useSpring = false;
           // SpringValue.damper = 0f;
           // _joint.spring = SpringValue;

            print("pararon todas las corrutinas");
        }
    }
    void Start()
    {
       //_joint = GetComponent<HingeJoint>(); 
    }

    void Update()
    {
        //float angulo = Mathf.Abs(_joint.angle);

        //&& Stop == false && Flag == true


        //if (_DoorStop.Stop == false )
        //{

        //    // on colicion exit
        //}


        //if (_DoorStop.Stop == true && Stop == true && Flag == false)
        //{
           
        //}


        //  print("el angulo es" + angulo);

        // SpringValue.spring = 0;
        // SpringValue.spring = SpringAmount * 0;
    }

    private IEnumerator SpringLerp()
    {
        SpringValue = _joint.spring;  // ← sincronizar antes de modificar

        for (float t = 0; t < 1; t += Time.deltaTime/LerpDuration)
        {
            _joint.useSpring = true;
            float resultado = Mathf.Lerp(0, SpringObjetiveAmount, t);
            SpringAmount = resultado;
            SpringValue.spring = resultado;
            _joint.spring = SpringValue;

           
            yield return null;
        }
        //yield return new WaitForSeconds(2);
       // _joint.useSpring = false;
       // StopAllCoroutines();
        //print("Spring Terminado");
        //SpringValue.targetPosition = 0f;  // atrae hacia el centro

    }

    private IEnumerator DamperLerp()
    {
        SpringValue = _joint.spring;  // ← sincronizar antes de modificar

        for (float t = 0; t < 1; t += Time.deltaTime / LerpDuration)
        {
            float resultado = Mathf.Lerp(0, DamperObjetiveAmount, t);
            SpringValue.damper = resultado;
            _joint.spring = SpringValue;

           
            yield return null;
        }
        //yield return new WaitForSeconds(2);

       // SpringValue.damper = 0f;
        //StopAllCoroutines();

    }

    public void StopDamper()
    {
        _joint.useSpring = false;
        SpringValue.damper = 0f;

        Stop = false;


        print("entro");
        Flag = true;

        DamperObjetiveAmount = 0;
        SpringObjetiveAmount = 0;

        SpringValue.spring = 0;
    }


    public void StartDamper()
    {
        DamperObjetiveAmount = 120;
        SpringObjetiveAmount = 400;
        StartCoroutine(DamperLerp());
        StartCoroutine(SpringLerp());
    }
}

