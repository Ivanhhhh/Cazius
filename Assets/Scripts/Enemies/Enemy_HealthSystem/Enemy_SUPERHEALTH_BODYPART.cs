using UnityEngine;

public class Enemy_SUPERHEALTH_BODYPART : MonoBehaviour, Enemy_Interface_Damage
{
    [SerializeField] float _damageMultiplier;
    [SerializeField] private BodyPartType _bodyPart;
    private Enemy_SUPERHEALTHSYSTEM _healthSystem;
    void Start()
    {
        _healthSystem = GetComponentInParent<Enemy_SUPERHEALTHSYSTEM>();
    }

    public void TakeDamage(float amount)
    {
        _healthSystem.TakeDamageFromPart(amount * _damageMultiplier, _bodyPart);
    }
}
public enum SUPERBodyPartType
{
    Head,
    Chest,
    Left_Arm,
    Right_Arm,
    Left_Leg,
    Right_Leg
}
