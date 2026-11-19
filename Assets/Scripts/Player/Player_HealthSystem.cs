using Patterns.Observer.EventManager_Delegates; // <-- esto arriba del todo
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

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
    [SerializeField] private TMP_Text[] _deathTmpTexts;
    [SerializeField] private Image[] _deathCanvasImages;
    [SerializeField] private float _fadeDuration = 2.0f;
    [SerializeField] private float _delayBeforeLoad = 2000.0f;

    [Header("Parry Integration")]
    [SerializeField] private Player_Parry _parryScript;

    private Coroutine _hideUICoroutine;
    [SerializeField] private CameraShake _cameraShakeScript;
    [SerializeField] private Player_Damage playerDamageScript;

    [Header("Death Visuals")]
    [SerializeField] private Animator _animator;
    [SerializeField] private MonoBehaviour[] _scriptsToDisable;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _cameraTilt;

    private bool _deathHappened = false;

    public static HealthStates CurrentHealthState { get; private set; } = HealthStates.Amazing_100;
    public static event Action<HealthStates> OnHealthStateChanged;

    // --- INTEGRACIÓN CON EL INVENTARIO (OBSERVER PATTERN) ---
    private void OnEnable()
    {
        InventoryInputHandler.OnInventoryToggled += HandleInventoryState;

        if (_parryScript == null) _parryScript = GetComponent<Player_Parry>();
    }

    private void OnDisable()
    {
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
        UpdateHealthState();
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
        playerDamageScript.SetHealthState(_healthState);
        ModifyUI();
    }

    // --- Interfaz ---
    // Sobrecarga por compatibilidad: si algo llama Hit(int) sin atacante, redirige.
    public void Hit(int amount) => Hit(amount, null);

    public void Hit(int amount, GameObject attacker)
    {
        // 1) Si el parry está activo, ejecutar parry y NO recibir daño.
        if (_parryScript != null && _parryScript.IsParrying)
        {
            _parryScript.TryExecuteParry(attacker);
            return;
        }

        // 2) Pausa global de gameplay al recibir golpe (tu comportamiento original)
        EventManager.TriggerEvent(EventsType.Event_PausePlayer);

        // 3) i-frames
        if (Time.time < _lastHitTime + _invincibilityDuration)
            return;

        _lastHitTime = Time.time;

        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        _cameraShakeScript.DamageShake();

        UpdateHealthState();
        playerDamageScript.SetHealthState(_healthState);
        playerDamageScript.TakeDamageEffect();
        ModifyUI();

        if (_currentHealth <= 0)
            HandleDeath();
    }

    private void UpdateHealthState()
    {
        float percentage = (_currentHealth / _maxHealth) * 100f;
        HealthStates previous = _healthState;

        if (percentage > 80) _healthState = HealthStates.Amazing_100;
        else if (percentage > 50) _healthState = HealthStates.Great_80;
        else if (percentage > 30) _healthState = HealthStates.Regular_50;
        else if (percentage > 20) _healthState = HealthStates.Low_30;
        else _healthState = HealthStates.Critic_20;

        // NUEVO: sincronizar el estado global y avisar
        CurrentHealthState = _healthState;
        if (previous != _healthState)
            OnHealthStateChanged?.Invoke(_healthState);
    }
    private void ModifyUI()
    {
        _healthBarUIManager.ChangeHealthBarPercentage(_currentHealth / _maxHealth);

        int colorIndex = (int)_healthState;
        Color targetColor = Color.white;

        if (_healthColors != null && colorIndex >= 0 && colorIndex < _healthColors.Length)
            targetColor = _healthColors[colorIndex];

        if (_currentHealthText != null)
            _currentHealthText.text = $"Health: {_currentHealth}";

        if (_healthStateImage != null)
        {
            Color finalColor = new Color(targetColor.r, targetColor.g, targetColor.b, _healthStateImage.color.a);
            _healthStateImage.color = finalColor;
        }

        SetUIElementsVisibility(true);

        if (_hideUICoroutine != null) StopCoroutine(_hideUICoroutine);
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
        if (_deathHappened) { return; }
        GameInputManager.Instance.Controls.Player.Disable();
        StartCoroutine(DeathSequenceRoutine());
        _animator.SetBool("Die", true);
        foreach (MonoBehaviour script in _scriptsToDisable)
        {
            script.enabled = false;
        }
    }

    private IEnumerator DeathSequenceRoutine()
    {
        _deathHappened = true;

        float elapsedTime = 0f;

        if (_deathFadeImage != null)
        {
            _deathFadeImage.enabled = true;
            Color initialColor = _deathFadeImage.color;

            float startAlpha = 0f;
            float endAlpha = 1f;

            float camOriginRotation = _camera.transform.rotation.x;
            float camFinalRotation = camOriginRotation + _cameraTilt;
            //Quaternion camTarget = Quaternion.Euler(camFinalRotation, _camera.transform.rotation.y, _camera.transform.rotation.z);
            Quaternion camTarget = Quaternion.Euler(camFinalRotation, -camFinalRotation, _camera.transform.rotation.z);

            Transform camOriginPos = _camera.GetComponent<Transform>();

            while (elapsedTime < _fadeDuration)
            {
                GameInputManager.Instance.Controls.Player.Disable();
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / _fadeDuration);
                _deathFadeImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, newAlpha);

                foreach (var img in _deathCanvasImages)
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);
                }
                foreach (var txt in _deathTmpTexts)
                {
                    txt.alpha = newAlpha;
                }

                _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, camTarget, Time.deltaTime * 0.2f);
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
    void Hit(int damage, GameObject attacker);
}