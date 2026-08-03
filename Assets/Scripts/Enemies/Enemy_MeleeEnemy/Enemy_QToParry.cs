using UnityEngine;

public class Enemy_QToParry : MonoBehaviour
{
    [SerializeField] private Enemy_MeleeEnemy_Data _enemyData;
    [SerializeField] private GameObject _QImage;

    private Camera _mainCamera;
    private Renderer _qImageRenderer;

    void Awake()
    {
        if (_enemyData == null) _enemyData = GetComponent<Enemy_MeleeEnemy_Data>();
        _mainCamera = Camera.main;

        if (_QImage != null) _qImageRenderer = _QImage.GetComponent<Renderer>();
    }

    void Start()
    {
        if (_qImageRenderer != null) _qImageRenderer.enabled = false;
    }

    void Update()
    {
        if (_enemyData == null || _qImageRenderer == null) return;

        // Activa/desactiva el render según el estado de stun
        if (_qImageRenderer.enabled != _enemyData._isStunned)
        {
            _qImageRenderer.enabled = _enemyData._isStunned;
        }

        // Billboard: siempre mirando a la cámara
        if (_qImageRenderer.enabled && _mainCamera != null)
        {
            _QImage.transform.rotation = _mainCamera.transform.rotation;
        }
    }
}