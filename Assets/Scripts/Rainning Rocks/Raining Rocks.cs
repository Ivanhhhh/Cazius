using UnityEngine;
using SmoothShakeFree;

public class RainingRocks : MonoBehaviour
{
    public enum ActionType
    {
        Sound,
        Physics
    }

    [SerializeField] private ActionType actionType;

    [Header("Sound")]
    [SerializeField] private SFXManager.SFXCategoryType sfxType;

    [Header("Physics")]
    [SerializeField] private string rockTag = "Rock";
    [SerializeField] private float gravityMultipler;

    [Header("Script Reference")]
    [SerializeField] private CameraShake cameraShake;

    private Rigidbody[] rigidbodies;

    private void Awake()
    {
        GameObject[] rocks = GameObject.FindGameObjectsWithTag(rockTag);

        rigidbodies = new Rigidbody[rocks.Length];

        for (int i = 0; i < rocks.Length; i++)
        {
            rigidbodies[i] = rocks[i].GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (actionType == ActionType.Sound)
        {
            SFXManager.Instance.PlaySFXAtPositionAndPauseMusic(sfxType, transform.position);
            cameraShake.ShakeCamera();
        }
        else if (actionType == ActionType.Physics)
        {
            ActivateGravity();
            cameraShake.ShakeCamera();
        }
    }

    public void ActivateGravity()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb == null) continue;

            rb.useGravity = true;

            rb.AddForce(Vector3.down * gravityMultipler, ForceMode.Acceleration);
        }
    }
}