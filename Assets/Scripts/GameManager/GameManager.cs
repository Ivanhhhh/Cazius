using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Reference")]
    public PlayerMovement Player { get; private set; }

    public PlayerRigManager playerRig;

    public CameraShake cameraShake;

    public bool gunActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void RegisterPlayer(PlayerMovement player)
    {
        Player = player;
    }
}
