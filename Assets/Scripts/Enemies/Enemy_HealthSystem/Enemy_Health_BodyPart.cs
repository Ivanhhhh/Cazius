using UnityEngine;

public class Enemy_Health_BodyPart : MonoBehaviour, Enemy_Interface_Damage
{
    [SerializeField] float _damageMultiplier;
    [SerializeField] private BodyPartType _bodyPart;
    private Enemy_HealthSystem _healthSystem;
        void Start()
    {
        _healthSystem = GetComponentInParent<Enemy_HealthSystem>();
    }

    public void TakeDamage(float amount)
    {
        _healthSystem.TakeDamageFromPart(amount * _damageMultiplier, _bodyPart);
    }
}
public enum BodyPartType
{
    Head,
    Chest,
    Arms,
    Legs
}