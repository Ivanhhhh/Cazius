using UnityEngine;
using SmoothShakeFree;
using UnityEngine.Rendering.Universal;

public class Enemy_ShakeDamage : MonoBehaviour
{
    [SerializeField] private SmoothShake preset;
    public void OnDamage()
    {
        preset.StartShake();
    }
}
