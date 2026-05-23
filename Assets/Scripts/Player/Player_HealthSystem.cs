using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class Player_HealthSystem : MonoBehaviour, IPlayerHitable
{
    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private Color[] _healthColors;
    [SerializeField] private HealthStates _healthState;

    [Header("Invincibility Settings")]
    [SerializeField] private float _invincibilityDuration = 0.5f;
    private float _lastHitTime = -100f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _currentHealthText;
    [SerializeField] private Image _healthStateImage;

    [Header("Display Settings")]
    [SerializeField] private float _displayDuration = 2.5f;

    public float _currentHealth;
    private Coroutine _hideUICoroutine;

    [SerializeField] string LoseScreenScene = "loseScreen";

    // --- INTEGRACIÓN CON EL INVENTARIO (OBSERVER PATTERN) ---
    private void OnEnable()
    {
        // Nos suscribimos al evento cuando este script se activa
        InventoryInputHandler.OnInventoryToggled += HandleInventoryState;
    }

    private void OnDisable()
    {
        // Nos desuscribimos para evitar errores de memoria si el jugador se destruye
        InventoryInputHandler.OnInventoryToggled -= HandleInventoryState;
    }

    private void HandleInventoryState(bool isInventoryOpen)
    {
        if (isInventoryOpen)
        {
            UpdateHealthState();
            ModifyUI();
        }
        else
        {
            if (_hideUICoroutine != null)
            {
                StopCoroutine(_hideUICoroutine);
                _hideUICoroutine = null;
            }
            SetUIElementsVisibility(false);
        }
    }

    void Start()
    {
        _currentHealth = _maxHealth;
        SetUIElementsVisibility(false);
    }

    public void Heal(int amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        UpdateHealthState();
        ModifyUI();
    }

    public void Hit(int amount)
    {
        if (Time.time < _lastHitTime + _invincibilityDuration)
        {
            return;
        }

        Debug.Log("Daño aplicado y validado");
        _lastHitTime = Time.time;

        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        UpdateHealthState();
        ModifyUI();

        if (_currentHealth <= 0)
        {
            ManageDeath();
        }
    }

    private void UpdateHealthState()
    {
        float percentage = (_currentHealth / _maxHealth) * 100f;

        if (percentage > 80) _healthState = HealthStates.Amazing_100;
        else if (percentage > 50) _healthState = HealthStates.Great_80;
        else if (percentage > 30) _healthState = HealthStates.Regular_50;
        else if (percentage > 20) _healthState = HealthStates.Low_30;
        else _healthState = HealthStates.Critic_20;
    }

    private void ModifyUI()
    {
        int colorIndex = (int)_healthState;
        Color targetColor = Color.white;

        if (_healthColors != null && colorIndex >= 0 && colorIndex < _healthColors.Length)
        {
            targetColor = _healthColors[colorIndex];
        }

        if (_currentHealthText != null)
        {
            _currentHealthText.text = $"Vida Actual: {_currentHealth}";
        }

        if (_healthStateImage != null)
        {
            Color finalColor = new Color(targetColor.r, targetColor.g, targetColor.b, _healthStateImage.color.a);
            _healthStateImage.color = finalColor;
        }

        SetUIElementsVisibility(true);

        if (_hideUICoroutine != null)
        {
            StopCoroutine(_hideUICoroutine);
        }
        _hideUICoroutine = StartCoroutine(HideUIRoutine());
    }

    private IEnumerator HideUIRoutine()
    {
        yield return new WaitForSeconds(_displayDuration);
        SetUIElementsVisibility(false);
        _hideUICoroutine = null;
    }

    private void SetUIElementsVisibility(bool visible)
    {
        if (_currentHealthText != null) _currentHealthText.enabled = visible;
        if (_healthStateImage != null) _healthStateImage.enabled = visible;
    }

    private void ManageDeath()
    {
        //
        SceneManager.LoadScene(LoseScreenScene);

        Cursor.lockState = CursorLockMode.None;
        
        Debug.Log("Jugador eliminado");
        //
    }
}

public enum HealthStates
{
    Amazing_100 = 0,
    Great_80 = 1,
    Regular_50 = 2,
    Low_30 = 3,
    Critic_20 = 4
}

public interface IPlayerHitable
{
    void Hit(int damage);
}

