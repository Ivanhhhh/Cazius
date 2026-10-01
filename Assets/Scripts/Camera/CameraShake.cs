using System.Collections;
using UnityEngine;
using SmoothShakeFree;
using System;
using UnityEngine.Rendering.Universal;
public class CameraShake : MonoBehaviour
{
    [SerializeField] private SmoothShake shake;
    [SerializeField] private SmoothShakeFreePreset preset;

    void Start()
    {
        shake.StartShake(preset);
    }
    public void DamageShake()
    {
        shake.StartShake(preset);

        Debug.Log("Damage Shake ON");
    }

    public void ShakeCamera()
    {
        shake.StartShake(preset);

        Debug.Log("Shake camera ON");
    }
}
