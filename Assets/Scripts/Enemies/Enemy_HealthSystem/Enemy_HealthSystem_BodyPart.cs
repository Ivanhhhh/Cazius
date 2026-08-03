using UnityEngine;

public class Enemy_HealthSystem_BodyPart : MonoBehaviour, Enemy_Interface_Damage
{
    [SerializeField] float _damageMultiplier;
    [SerializeField] private BodyPartType _bodyPart;
    [SerializeField] private Enemy_HealthSystem_Base _healthSystem;
 
    void Start()
    {
        if (_healthSystem == null)
            _healthSystem = GetComponentInParent<Enemy_HealthSystem_Base>();
    }
 
    public void TakeDamage(float amount)
    {
        _healthSystem.TakeDamageFromPart(amount * _damageMultiplier, _bodyPart);
    }
}

