using System;
using GLTFast.Schema;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class Player_WhitoutGun : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Player_AimAndShoot aimAndShoot;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject[] Gameobjects;

    private void Start()
    {
        animator = GetComponent<Animator>();
        SetWithoutGun();
    }

    private void OnEnable()
    {
        WorldChangeManager.Instance.SwapToEdenEvent += SetWithoutGun;
        WorldChangeManager.Instance.SwapToPurgatoryEvent += SetWithGun;
    }

    private void OnDisable()
    {
        WorldChangeManager.Instance.SwapToEdenEvent -= SetWithoutGun;
        WorldChangeManager.Instance.SwapToPurgatoryEvent -= SetWithGun;
    }

    private void SetWithoutGun()
    {
        animator.SetFloat("HasGun", 0f);

        Gameobjects[0].SetActive(false);
        Gameobjects[1].SetActive(false);
        Gameobjects[2].SetActive(false);

        aimAndShoot.SetCanUseWeapon(false);

        playerMovement.SetCanAim(false);

        Debug.Log("Without Gun");
    }

    private void SetWithGun()
    {
        animator.SetFloat("HasGun", 1f);

        Gameobjects[0].SetActive(true);
        Gameobjects[1].SetActive(true);
        Gameobjects[2].SetActive(true);

        aimAndShoot.SetCanUseWeapon(true);

        playerMovement.SetCanAim(true);

        Debug.Log("With Gun");
    }
}
