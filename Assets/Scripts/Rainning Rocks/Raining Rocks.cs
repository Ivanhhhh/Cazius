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
    [SerializeField] private float gravityMultiplier = 10f;
    [SerializeField] private float timeBetweenRocks = 0.5f;

    [Header("Camera Shake")]
    [SerializeField] private float shakeStrength = 0.08f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (triggered)
            return;

        triggered = true;

        if (actionType == ActionType.Sound)
        {
            SFXManager.Instance.PlaySFXAtPositionAndPauseMusic(
                sfxType,
                transform.position
            );
        }
        else if (actionType == ActionType.Physics)
        {
            StartCoroutine(ActivateGravity());
        }
    }

    private IEnumerator ActivateGravity()
    {
        GameObject[] rocks = GameObject.FindGameObjectsWithTag(rockTag);

        CameraShake.Instance.StartShake(shakeStrength);

        foreach (GameObject rock in rocks)
        {
            if (rock == null)
                continue;

            Rigidbody rb = rock.GetComponent<Rigidbody>();

            if (rb == null)
                continue;

            rb.useGravity = true;

            rb.AddForce(
                Vector3.down * gravityMultiplier,
                ForceMode.Acceleration
            );

            SFXManager.Instance.PlaySFXAtPosition(
                sfxType,
                rock.transform.position
            );

            yield return new WaitForSeconds(timeBetweenRocks);
        }

        CameraShake.Instance.StopShake();
    }
}