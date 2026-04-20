using Unity.Jobs;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.iOS;

public class Interaction : MonoBehaviour
{
    [SerializeField] private float maxDistance;
    [SerializeField] private float rotateVelocity;
    [SerializeField] private LayerMask interactiveLayerMask;

    [SerializeField] private GameObject currentObject;
    [SerializeField] private bool isInspecting;

    private GameObject inspectionInstance;

    [SerializeField] private Transform inspectionPoint;

    [SerializeField] private GameObject canvasUI;
    [SerializeField] private GameObject panelUI;

    [SerializeField] private Transform interactiveIcon;
    [SerializeField] private Vector3 iconOffset;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera inspectionCamera;

    [SerializeField] private bool hasKey;
    [SerializeField] private GameObject keyOffset;
    [SerializeField] private GameObject doorToUnlock;
    [SerializeField] private bool IsInsideTrigger = false;
    [SerializeField] private GameObject PivotDoor;


    [Header("Inputs")]
    public InputAction pickUp;
    public InputAction inspection;
    public InputAction interaction;
    public InputAction cancel;
    public InputAction mouse;

    void Start()
    {
        inspectionCamera.enabled = false;
        canvasUI.SetActive(false);
        panelUI.SetActive(false);
    }

    void OnEnable()
    {
        pickUp.Enable();
        inspection.Enable();
        interaction.Enable();
        cancel.Enable();
        mouse.Enable();
    }

    void OnDisable()
    {
        pickUp.Disable();
        inspection.Disable();
        interaction.Disable();
        cancel.Disable();
        mouse.Disable();
    }

    void Update()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * maxDistance, Color.red);

        if (!isInspecting)
        {
            if (!hasKey)
            {
                interactiveIcon.gameObject.SetActive(true);

                if (Physics.Raycast(origin, direction, out hit, maxDistance, interactiveLayerMask))
                {
                    currentObject = hit.transform.gameObject;
                    interactiveIcon.gameObject.SetActive(true);
                    interactiveIcon.position = currentObject.transform.position + iconOffset;
                }
                /*else
                {
                    currentObject = null;
                }*/
            }
            if (pickUp.WasPressedThisFrame() && currentObject != null && !isInspecting)
            {
                hasKey = true;
                currentObject.transform.SetParent(keyOffset.transform);
                currentObject.transform.localPosition = Vector3.zero;
                interactiveIcon.gameObject.SetActive(false);

            }
            if (interaction.WasPressedThisFrame() && IsInsideTrigger && hasKey)
            {
                Debug.Log("Unlock");
                currentObject.gameObject.SetActive(false);
                PivotDoor.transform.Rotate(0, -90, 0);
            }
        }

        if (inspection.WasPressedThisFrame() && currentObject != null && !isInspecting)
        {
            isInspecting = true;
            currentObject.SetActive(true);

            mainCamera.enabled = false;
            inspectionCamera.enabled = true;

            canvasUI.SetActive(true);
            panelUI.SetActive(true);

            inspectionInstance = Instantiate(currentObject, inspectionPoint.position, Quaternion.identity);
            inspectionInstance.layer = LayerMask.NameToLayer("Inspection");
        }

        if (isInspecting && inspectionInstance != null)
        {
            Vector2 mouseInput = mouse.ReadValue<Vector2>();
            float inputX = mouseInput.x * rotateVelocity;
            float inputY = mouseInput.y * rotateVelocity;

            inspectionInstance.transform.Rotate(Vector3.up, inputX, Space.World);
            inspectionInstance.transform.Rotate(Vector3.right, -inputY, Space.World);
        }

        if (cancel.WasCompletedThisFrame() && isInspecting)
        {
            isInspecting = false;

            Destroy(inspectionInstance);

            inspectionCamera.enabled = false;
            mainCamera.enabled = true;

            canvasUI.SetActive(false);
            panelUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            IsInsideTrigger = true;
            if (!hasKey) Debug.Log("You need the key");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            IsInsideTrigger = false;
        }
    }
}
