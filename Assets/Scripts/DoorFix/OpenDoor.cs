using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenDoor : MonoBehaviour, IEInteractable
{ private bool OpenIsEnabled = false;

    private bool _isOpening = false;


    [SerializeField] HingeJoint _joint;

    [SerializeField] GameObject OpenDoorText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OpenIsEnabled = true;

            GetInteractText();

            //_joint.useMotor = true;

            //_joint.useMotor = false;


            print("entro");
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OpenDoorText.SetActive(false);

            OpenIsEnabled = false;
        }

    }
    void Start()
    {
        //_joint = GetComponent<HingeJoint>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame && OpenIsEnabled == true && _isOpening == false)
        {
            Interact(this.transform);
            OpenDoorText.SetActive(false);
        }
        else if (OpenIsEnabled == false && _isOpening == false) _joint.useMotor = false;

    }

    public IEnumerator Timer()
    {
        _isOpening = true;

        _joint.useMotor = true;
        _joint.useLimits = true;


        yield return new WaitForSeconds(6f);

        _joint.useMotor = false;
        _joint.useLimits = false;

        _isOpening = false;


    }


  public void Interact(Transform interactorTransform)
  {
        StartCoroutine(Timer());


  }
  public string GetInteractText()
  {
        OpenDoorText.SetActive(true);
        return OpenDoorText.ToString();
  }
   public Transform GetTransform()
   {
     return this.transform; 
   }
}
   
