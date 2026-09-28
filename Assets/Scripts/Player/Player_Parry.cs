using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player_Parry : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] float _timeToParrying;
    [SerializeField] bool _isParrying;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    private static readonly int ParryTrigger = Animator.StringToHash("IsBlocking");
    [SerializeField] private ParryCounterVisuals _combatVisuals;

    public event Action _onParryActivated;
    public event Action _onParryEnded;

    // Propiedad pública para que Player_HealthSystem consulte el estado
    public bool IsParrying => _isParrying;

    void OnEnable()
    {
        GameInputManager.Instance.Controls.Player.Parry.started += MakeParry;
    }

    void OnDisable()
    {
        GameInputManager.Instance.Controls.Player.Parry.started -= MakeParry;
    }

    void MakeParry(InputAction.CallbackContext context)
    {
        if (_animator.GetBool("IsAiming")) return;

        if (!_isParrying)
            StartCoroutine(ParryWindow());
    }

    private IEnumerator ParryWindow()
    {
        _isParrying = true;
        _onParryActivated?.Invoke();
        _animator.SetBool(ParryTrigger, true);

        yield return new WaitForSeconds(_timeToParrying);

        _isParrying = false;
        _animator.SetBool(ParryTrigger, false);
        _onParryEnded?.Invoke();
    }

    /// <summary>
    /// Se llama desde Player_HealthSystem.Hit() cuando llega un golpe
    /// mientras el parry está activo. Devuelve true si se paró.
    /// </summary>
    public bool TryExecuteParry(GameObject attacker)
    {
        if (!_isParrying) return false;

        _animator.SetTrigger("TakeDamage");
        _combatVisuals.PlayParryVisuals();

        if (attacker != null)
        {
            Enemy_Parry enemyParry = attacker.GetComponent<Enemy_Parry>();
            if (enemyParry != null)
            {
                enemyParry.Execute();
                _combatVisuals.PlayCounterattackVisuals(); // opcional
            }
        }

        return true;
    }
}