using System.Collections;
using System.Linq;
using UnityEngine;

public class NPCShopOwner : MonoBehaviour, IEInteractable
{
    [Header("Shop")]
    [SerializeField] private ShopCatalog _shopCatalog;

    [Header("Dialog - Localization IDs")]
    [SerializeField] private string[] _greetingDialog;
    [SerializeField] private string[] _goodbyePurchaseDialog;
    [SerializeField] private string[] _goodbyeNoPurchaseDialog;

    [Header("NPC Behavior")]
    [SerializeField] private string _interactText = "F";
    [SerializeField] private string _wavingTrigger = "Waving";
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private bool _rotateTowardsPlayer = true;
    [SerializeField] private SFXManager.SFXCategoryType _interactSFX = SFXManager.SFXCategoryType.MaleHeySFX;

    [Header("Interaction UI")]
    [SerializeField] private Transform _interactionUIPoint;

    private Animator _animator;
    private NPCController _npcController;
    private Coroutine _rotateCoroutine;

    private void Awake()
    {
        _npcController = GetComponent<NPCController>();
        _animator = GetComponentInChildren<Animator>();
    }

    // --- IEInteractable ---

    public string GetInteractText() => _interactText;
    public Transform GetTransform() => transform;
    public bool IsLocked() => false;

    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null ? _interactionUIPoint : transform;
    }

    public void Interact(Transform interactorTransform)
    {
        if (_shopCatalog == null)
        {
            Debug.LogWarning($"[NPCShopOwner] No ShopCatalog assigned on {gameObject.name}");
            return;
        }

        if (_npcController != null && _npcController.enabled)
            _npcController.BeginTalking();

        if (_rotateTowardsPlayer)
            RotateTowardsPlayer(interactorTransform);

        if (_animator != null && !string.IsNullOrEmpty(_wavingTrigger))
            _animator.SetTrigger(_wavingTrigger);

        if (SFXManager.Instance != null)
            SFXManager.Instance.PlaySFXAtPosition(_interactSFX, transform.position);

        if (_greetingDialog != null && _greetingDialog.Length > 0)
        {
            DialogUIController.Instance.OpenDialog(
                pages: Translate(_greetingDialog),
                onAccept: null,
                onClose: OpenShop
            );
        }
        else
        {
            OpenShop();
        }
    }

    private void OpenShop()
    {
        ShopUI.Instance.OpenShop(_shopCatalog, OnShopClosed);
    }

    private void OnShopClosed(bool madePurchase)
    {
        string[] goodbyeIDs = madePurchase ? _goodbyePurchaseDialog : _goodbyeNoPurchaseDialog;

        if (goodbyeIDs != null && goodbyeIDs.Length > 0)
        {
            DialogUIController.Instance.OpenDialog(
                pages: Translate(goodbyeIDs),
                onAccept: null,
                onClose: FinishInteraction
            );
        }
        else
        {
            FinishInteraction();
        }
    }

    private void FinishInteraction()
    {
        if (_npcController != null && _npcController.enabled)
            _npcController.EndTalking();
    }

    // --- Helpers ---

    private string[] Translate(string[] ids)
    {
        if (ids == null || ids.Length == 0) return new string[0];
        return ids.Select(id => LocalizationManager.Instance.GetTranslate(id)).ToArray();
    }

    private void RotateTowardsPlayer(Transform interactorTransform)
    {
        Vector3 dir = interactorTransform.position - transform.position;
        dir.y = 0;
        if (dir == Vector3.zero) return;

        if (_rotateCoroutine != null)
            StopCoroutine(_rotateCoroutine);

        _rotateCoroutine = StartCoroutine(RotateCoroutine(Quaternion.LookRotation(dir)));
    }

    private IEnumerator RotateCoroutine(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.rotation = targetRotation;
        _rotateCoroutine = null;
    }

}
