using UnityEngine;

public class KeyInventorySystem : MonoBehaviour // Lo tiene Inventory
{
    [Header("Keys")]
    public bool HasEdenKey { get; private set; }
    public bool HasPurgatoryKey { get; private set; }

    [Header("Soul Energy")]
    public int CurrentSoulEnergy { get; private set; }
    public int MaxSoulEnergy { get; private set; }

    public static KeyInventorySystem Instance { get; private set; }

    public PlayerControls _controls;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _controls = GameInputManager.Instance.Controls;
    }

    private void OnEnable() => _controls.Player.Consume.performed += _ => ConsumeSoulEnergy();

    private void OnDisable() => _controls.Player.Consume.performed += _ => ConsumeSoulEnergy();

    public void AddEdenKey()
    {
        HasEdenKey = true;
    }
    public void AddPurgatoryKey()
    {
        HasPurgatoryKey = true;
    }
    public void AddSoulEnergy()
    {
        CurrentSoulEnergy++;
        Debug.Log("Soul Energy" + CurrentSoulEnergy);
    }
    public void RemoveSoulEnergy()
    {
        if (CurrentSoulEnergy > 0)
        {
            CurrentSoulEnergy--;
            Debug.Log("Soul Energy has consumed" + CurrentSoulEnergy);
        }
    }

    private void ConsumeSoulEnergy()
    {
        if (CurrentSoulEnergy > 0)
        {
            RemoveSoulEnergy();
            SoulUIManager.Instance.UpdateUI(CurrentSoulEnergy);

            Debug.Log("Soul Energy consumed");
        }
    }

}
