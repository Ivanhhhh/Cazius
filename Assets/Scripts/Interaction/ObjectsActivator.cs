using System.Collections;
using UnityEngine;

public class ObjectsActivator : MonoBehaviour
{
    [SerializeField] private GameObject[] _ObjectsToActivate;

    [Header("Sound")]
    [SerializeField] private SFXManager.SFXCategoryType sfxType;

    [Header("Camera Shake")]
    [SerializeField] private float shakeStrength = 0.08f;
    [SerializeField] private float duration = 5f;

    public void Activate()
    {
        StartCoroutine(ActivateObjects());
    }

    private IEnumerator ActivateObjects()
    {
        SFXManager.Instance.PlaySFXAtPositionAndPauseMusic(
            sfxType,
            transform.position
        );

        CameraShake.Instance.StartShake(shakeStrength);

        yield return new WaitForSeconds(duration);

        foreach (GameObject obj in _ObjectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        CameraShake.Instance.StopShake();
    }
}
