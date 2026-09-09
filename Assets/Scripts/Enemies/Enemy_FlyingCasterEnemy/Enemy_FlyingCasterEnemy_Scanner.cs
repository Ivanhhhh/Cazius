using System.Collections;
using UnityEngine;

public class Enemy_FlyingCasterEnemy_Scanner : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Animator _scannerAnimator;
    [SerializeField] private GameObject _scannerMesh;

    void Start()
    {
        StartCoroutine(ScannerRoutine());
    }

    private IEnumerator ScannerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            _scannerMesh.SetActive(true);
            _scannerAnimator.Play("Enemy_FlyingCasterEnemy Scanner");
            SFXManager.Instance.PlaySFXAtPosition(SFXManager.SFXCategoryType.ScanningSFX, transform.position);

            yield return new WaitForSeconds(3);
            _scannerMesh.SetActive(false);
        }
    }




}
