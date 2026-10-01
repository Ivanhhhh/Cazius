using UnityEngine;
using SmoothShakeFree;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private SmoothShake shake;
    [SerializeField] private SmoothShakeFreePreset preset;

    private void Awake()
    {
        Instance = this;
    }

    public void DamageShake()
    {
        shake.StartShake(preset);

        Debug.Log("Damage Shake ON");
    }

    public void ShakeCamera()
    {
        shake.StartShake(preset);

        Debug.Log("Camera Shake ON");
    }
}