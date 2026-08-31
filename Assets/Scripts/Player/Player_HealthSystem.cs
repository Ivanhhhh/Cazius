using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Patterns.Observer.EventManager_Delegates; // <-- esto arriba del todo


public class Player_HealthSystem : MonoBehaviour, IPlayerHitable
{
    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private Color[] _healthColors;
    [SerializeField] private HealthStates _healthState;
    public float _currentHealth;

    [Header("Invincibility Settings")]
    [SerializeField] private float _invincibilityDuration = 0.5f;
    private float _lastHitTime = -100f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _currentHealthText;
    [SerializeField] private Image _healthStateImage;
    [SerializeField] private DiageticHealthBarUIManager _healthBarUIManager;

    [Header("Display Settings")]
    [SerializeField] private float _displayDuration = 2.5f;

    [Header("Death UI Settings")]
    [SerializeField] private string _sceneToLoadAfterDeath = "MainMenu";
    [SerializeField] private GameObject _deathPanel;
    [SerializeField] private Image _deathFadeImage;
    [SerializeField] private float _fadeDuration = 2.0f;
    [SerializeField] private float _delayBeforeLoad = 2000.0f;
    [Header("Parry Integration")]
    [SerializeField] private Player_Parry _parryScript;
    private bool _isInvulnerableByParry = false;

    private Coroutine _hideUICoroutine;
    [SerializeField] private CameraShake _cameraShakeScript;
    [SerializeField] private Player_Damage playerDamageScript;

    // --- INTEGRACIÓN CON EL INVENTARIO (OBSERVER PATTERN) ---
    private void OnEnable()
    {
        InventoryInputHandler.OnInventoryToggled += HandleInventoryState;

        if (_parryScript == null) _parryScript = GetComponent<Player_Parry>();
        if (_parryScript != null)
        {
            _parryScript._onParryActivated += HandleParryStarted;
            _parryScript._onParryEnded += HandleParryEnded;
        }
    }

    private void OnDisable()
    {
        InventoryInputHandler.OnInventoryToggled -= HandleInventoryState;

        if (_parryScript != null)
        {
            _parryScript._onParryActivated -= HandleParryStarted;
            _parryScript._onParryEnded -= HandleParryEnded;
        }
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

        if (_deathPanel != null) { _deathPanel.SetActive(false); }
        if (_deathFadeImage != null)
        {
            _deathFadeImage.enabled = false;
            Color c = _deathFadeImage.color;
            c.a = 0f;
            _deathFadeImage.color = c;
        }
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
        if (_isInvulnerableByParry) return;
        EventManager.TriggerEvent(EventsType.Event_PausePlayer);


        if (Time.time < _lastHitTime + _invincibilityDuration)
        {
            return;
        }

        _lastHitTime = Time.time;

        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        _cameraShakeScript.DamageShake();
        playerDamageScript.TakeDamageEffect();

        UpdateHealthState();
        ModifyUI();

        if (_currentHealth <= 0)
        {
            HandleDeath();
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

        _healthBarUIManager.ChangeHealthBarPercentage(_currentHealth / _maxHealth);

        int colorIndex = (int)_healthState;
        Color targetColor = Color.white;

        if (_healthColors != null && colorIndex >= 0 && colorIndex < _healthColors.Length)
        {
            targetColor = _healthColors[colorIndex];
        }

        if (_currentHealthText != null)
        {
            _currentHealthText.text = $"Health: {_currentHealth}";
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

    private void HandleDeath()
    {
        this.enabled = false;

        if (_deathPanel != null) { _deathPanel.SetActive(true); }

        StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        float elapsedTime = 0f;

        if (_deathFadeImage != null)
        {
            _deathFadeImage.enabled = true;
            Color initialColor = _deathFadeImage.color;

            float startAlpha = 0f;
            float endAlpha = 1f;

            while (elapsedTime < _fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / _fadeDuration);

                _deathFadeImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, newAlpha);
                yield return null;
            }
            _deathFadeImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, endAlpha);
        }
        else
        {
            yield return new WaitForSeconds(_fadeDuration);
        }

        WorldChangeManager.DestroyCurrentInstance();

        yield return new WaitForSeconds(_delayBeforeLoad);
        SceneManager.LoadScene(_sceneToLoadAfterDeath);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HandleParryStarted()
    {
        _isInvulnerableByParry = true;
    }

    private void HandleParryEnded()
    {
        _isInvulnerableByParry = false;
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