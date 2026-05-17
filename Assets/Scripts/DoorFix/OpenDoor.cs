using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenDoor : MonoBehaviour, IEInteractable
{ private bool OpenIsEnabled = false;

    private bool _isOpening = false;

    private bool _CanOpen = true;


    [SerializeField] HingeJoint _joint;

    [SerializeField] GameObject OpenDoorText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && _CanOpen)
        {
            OpenIsEnabled = true;

            GetInteractText();
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
        if (Keyboard.current.fKey.wasPressedThisFrame && OpenIsEnabled == true && _isOpening == false && _CanOpen)
        {
            Interact(this.transform);
            OpenDoorText.SetActive(false);
        }
        else if (OpenIsEnabled == false && _isOpening == false) _joint.useMotor = false;

    }

    public IEnumerator Timer()
    {
        _isOpening = true;

        _CanOpen = false;

        _joint.useMotor = true;
        _joint.useLimits = true;


        yield return new WaitForSeconds(4f);

        //_joint.useMotor = false;
        //_joint.useLimits = false;

        //_isOpening = false;

        yield return new WaitForSeconds(2f);
        _CanOpen = true;


    }


  public void Interact(Transform interactorTransform)
  {
        _CanOpen = false;
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
   
