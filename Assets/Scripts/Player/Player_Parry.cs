using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player_Parry : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] float _timeToParrying;
    [SerializeField] bool _isParrying;

    [Header("Detection")]
    [SerializeField] private float _parryRadius;
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private Transform _detectionOrigin; // podría ser el propio transform o un punto adelante del jugador

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    private static readonly int ParryTrigger = Animator.StringToHash("IsBlocking");
    [SerializeField] private ParryCounterVisuals _combatVisuals;

    public event Action _onParryActivated;
    public event Action _onParryEnded; 

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
        if (!_isParrying)
        {
            Debug.Log("Parry tried");
            StartCoroutine(ParryWindow());
        }
    }


    private IEnumerator ParryWindow()
    {
        _isParrying = true;
        _onParryActivated?.Invoke();
        _animator.SetBool(ParryTrigger, true);

        float elapsed = 0f;
        while (elapsed < _timeToParrying)
        {
            CheckForParryTargets();
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isParrying = false;
        _animator.SetBool(ParryTrigger, false);
        _onParryEnded?.Invoke(); // NUEVO
    }  
    private void CheckForParryTargets()
    {
        Collider[] hits = Physics.OverlapSphere(_detectionOrigin.position, _parryRadius, _enemyLayerMask);

        foreach (var hit in hits)
        {
            Enemy_Parry scriptEnemigo = hit.GetComponent<Enemy_Parry>();
            if (scriptEnemigo != null)
            {
                Debug.Log("parry encontrado");
                _animator.SetTrigger("TakeDamage");
                //aca meter particulas
                _combatVisuals.PlayParryVisuals();
                scriptEnemigo.Execute();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_detectionOrigin == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_detectionOrigin.position, _parryRadius);
    }
}
