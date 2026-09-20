using UnityEngine;
using System;
using Patterns.Observer.EventManager_Delegates;

public class SoulEnergyManager : MonoBehaviour
{
    public static SoulEnergyManager Instance { get; private set; }

    [SerializeField] private int _maxSoulEnergyValue = 100;
    public int MaxSoulEnergyValue => _maxSoulEnergyValue;

    [Header("UI")]
    [Tooltip("Referencia al panel de Soul Energy. Lo podés asignar a mano " +
             "o dejar vacío si no querés mostrar el panel.")]
    [SerializeField] private SoulEnergyUI _soulEnergyUI;

    private int _currentSoulEnergy;
    public int CurrentSoulEnergy => _currentSoulEnergy;

    // ============================================================
    //  CICLO DE VIDA
    // ============================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ============================================================
    //  ENERGÍA
    // ============================================================

    public void AddSoulEnergy(int amount)
    {
        if (amount <= 0) return;

        int nuevo = Mathf.Min(_currentSoulEnergy + amount, _maxSoulEnergyValue);
        if (nuevo == _currentSoulEnergy) return;

        _currentSoulEnergy = nuevo;
        EventManager.TriggerEvent(EventsType.Event_SoulEnergyChanged, _currentSoulEnergy);
    }

    public void RemoveSoulEnergy(int amount)
    {
        if (amount <= 0) return;

        int nuevo = Mathf.Max(_currentSoulEnergy - amount, 0);
        if (nuevo == _currentSoulEnergy) return;

        _currentSoulEnergy = nuevo;
        EventManager.TriggerEvent(EventsType.Event_SoulEnergyChanged, _currentSoulEnergy);
    }

    // ============================================================
    //  VISIBILIDAD DEL UI (reenvío)
    // ============================================================

    /// El orbe llama a esto cuando entra en rango del player.
    /// No necesita conocer al UI, solo al manager.
    public void RequestShowUI(object source)
    {
        if (_soulEnergyUI != null)
            _soulEnergyUI.RequestShow(source);
    }

    /// El orbe llama a esto cuando sale de rango o se recoge.
    public void RequestHideUI(object source)
    {
        if (_soulEnergyUI != null)
            _soulEnergyUI.RequestHide(source);
    }

    /// Opcional: registro tardío por si el UI aparece después.
    public void RegisterUI(SoulEnergyUI ui)
    {
        _soulEnergyUI = ui;
    }
}