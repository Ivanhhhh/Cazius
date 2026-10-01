using UnityEngine;
using SmoothShakeFree;
using System.Collections;

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
    private Rigidbody[] rigidbodies;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (actionType == ActionType.Sound)
        {
            SFXManager.Instance.PlaySFXAtPositionAndPauseMusic(sfxType, transform.position);
            CameraShake.Instance.DamageShake();
        }
        else if (actionType == ActionType.Physics)
        {
            StartCoroutine(ActivateGravity());
        }
    }

    private IEnumerator ActivateGravity()
    {
        GameObject[] rocks = GameObject.FindGameObjectsWithTag(rockTag);

        foreach (GameObject rock in rocks)
        {
            Rigidbody rb = rock.GetComponent<Rigidbody>();

            if (rb == null) continue;

            rb.useGravity = true;
            rb.AddForce(Vector3.down * gravityMultipler, ForceMode.Acceleration);

            SFXManager.Instance.PlaySFXAtPosition(sfxType, transform.position);

            yield return new WaitForSeconds(0.5f);
        }

    }
}