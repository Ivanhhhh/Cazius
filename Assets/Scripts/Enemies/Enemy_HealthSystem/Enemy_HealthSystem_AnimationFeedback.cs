using UnityEngine;

/// <summary>
/// Conecta los eventos de daño por parte y muerte del health system
/// con las animaciones de AngelDemonAnim. Solo se agrega en los enemigos
/// que tengan ese sistema de animacion.
/// </summary>
[RequireComponent(typeof(Enemy_HealthSystem_Base))]
public class Enemy_HealthSystem_AnimationFeedback : MonoBehaviour
{
    [SerializeField] private AngelDemonAnim _anim;

    private Enemy_HealthSystem_Base _healthSystem;

    void Awake()
    {
        _healthSystem = GetComponent<Enemy_HealthSystem_Base>();
        if (_anim == null) _anim = GetComponent<AngelDemonAnim>();
    }

    void OnEnable()
    {
        _healthSystem.OnPartHit += HandlePartHit;
        _healthSystem.OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        _healthSystem.OnPartHit -= HandlePartHit;
        _healthSystem.OnDeath -= HandleDeath;
    }

    private void HandlePartHit(BodyPartType part)
    {
        if (_anim == null) return;

        switch (part)
        {
            case BodyPartType.Head:
                _anim.HeadshotAnim();
                break;
            case BodyPartType.Chest:
                _anim.ChestAnim();
                break;
            case BodyPartType.Left_Arm:
                _anim.LeftArmAnim();
                break;
            case BodyPartType.Right_Arm:
                _anim.RightArmAnim();
                break;
            case BodyPartType.Left_Leg:
                _anim.LeftLegAnim();
                break;
            case BodyPartType.Right_Leg:
                _anim.RightLegAnim();
                break;
        }
    }

    private void HandleDeath()
    {
        if (_anim != null)
            _anim.DieAnim();
    }
}