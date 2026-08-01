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

    public event Action _onParryActivated;

    void OnEnable()
    {
        GameInputManager.Instance.Controls.Player.Interact.started += MakeParry;
    }

    void OnDisable()
    {
        GameInputManager.Instance.Controls.Player.Interact.started -= MakeParry;
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

        float elapsed = 0f;
        while (elapsed < _timeToParrying)
        {
            CheckForParryTargets();
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isParrying = false;
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
