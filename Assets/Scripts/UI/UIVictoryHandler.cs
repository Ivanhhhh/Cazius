using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIVictoryHandler : MonoBehaviour
{
    [Header("Target Enemy Reference")]
    [SerializeField] private Enemy_SUPERHEALTHSYSTEM _targetEnemy;

    [Header("Victory UI Settings")]
    [SerializeField] private string _sceneToLoadAfterVictory = "MainMenu";
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private Image _victoryFadeImage;
    [SerializeField] private float _fadeDuration = 2.0f;
    [SerializeField] private float _delayBeforeLoad = 2000.0f;

    private bool _sequenceStarted = false;

    void OnEnable()
    {
        if (_targetEnemy != null) { _targetEnemy.OnDeath += HandleEnemyDeath; }
    }

    void OnDisable()
    {
        if (_targetEnemy != null) { _targetEnemy.OnDeath -= HandleEnemyDeath; }
    }

    void Start()
    {
        if (_victoryPanel != null) { _victoryPanel.SetActive(false); }
        if (_victoryFadeImage != null)
        {
            _victoryFadeImage.enabled = false;
            Color c = _victoryFadeImage.color;
            c.a = 0f;
            _victoryFadeImage.color = c;
        }
    }

    private void HandleEnemyDeath()
    {
        if (_sequenceStarted) return;
        _sequenceStarted = true;

        if (_targetEnemy != null) { _targetEnemy.OnDeath -= HandleEnemyDeath; }
        if (_victoryPanel != null) { _victoryPanel.SetActive(true); }

        StartCoroutine(VictorySequenceRoutine());
    }

    private IEnumerator VictorySequenceRoutine()
    {
        float elapsedTime = 0f;

        if (_victoryFadeImage != null)
        {
            _victoryFadeImage.enabled = true;
            Color initialColor = _victoryFadeImage.color;

            float startAlpha = 0f;
            float endAlpha = 1f;

            while (elapsedTime < _fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / _fadeDuration);

                _victoryFadeImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, newAlpha);
                yield return null;
            }
            _victoryFadeImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, endAlpha);
        }
        else
        {
            yield return new WaitForSeconds(_fadeDuration);
        }

        WorldChangeManager.DestroyCurrentInstance();
        
        yield return new WaitForSeconds(_delayBeforeLoad);
        SceneManager.LoadScene(_sceneToLoadAfterVictory);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetupTargetEnemy(Enemy_SUPERHEALTHSYSTEM enemy)
    {
        // Si ya hab�a uno, nos desuscribimos primero
        if (_targetEnemy != null) _targetEnemy.OnDeath -= HandleEnemyDeath;

        _targetEnemy = enemy;
        if (_targetEnemy != null) _targetEnemy.OnDeath += HandleEnemyDeath;
    }
}
