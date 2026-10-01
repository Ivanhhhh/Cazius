using System.Collections;
using UnityEngine;

public class ObjectsActivator : MonoBehaviour
{
    [SerializeField] private GameObject[] _ObjectsToActivate;

    [SerializeField] private SFXManager.SFXCategoryType sfxType;

    public void Activate()
    {
        StartCoroutine(ActivateObjects());
    }

    private IEnumerator ActivateObjects()
    {
        SFXManager.Instance.PlaySFXAtPositionAndPauseMusic(sfxType, transform.position);

        CameraShake.Instance.DamageShake();

        yield return new WaitForSeconds(5f);

        foreach (GameObject obj in _ObjectsToActivate)
        {
            obj.SetActive(true);
        }
    }
}
