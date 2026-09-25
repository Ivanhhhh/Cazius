using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NPCController))]
public class NPCWalkerDialog : MonoBehaviour, IEInteractable
{
    [Header("Dialog")]
    [SerializeField] private string[] _dialogPages;

    [Header("NPC Behavior")]
    [SerializeField] private string _interactText = "F to talk";
    [SerializeField] private string _wavingTrigger = "Waving";
    [SerializeField] private SFXManager.SFXCategoryType _interactSFX = SFXManager.SFXCategoryType.MaleHeySFX;

    [SerializeField] private Transform _interactionUIPoint;
    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }

    private NPCController _npc;

    private void Awake()
    {
        _npc = GetComponent<NPCController>();
    }

    public void Interact(Transform interactorTransform)
    {
        _npc.TalkingState.BeginTalking(interactorTransform, _wavingTrigger, _interactSFX);
        _npc.Machine.ChangeState(_npc.TalkingState);

        DialogUIController.Instance.OpenDialog(
            pages: Translate(_dialogPages),
            onAccept: null,
            onClose: () => _npc.Machine.ChangeState(_npc.IdleState)
        );
    }

    public string GetInteractText() { return _interactText; }
    public Transform GetTransform() { return transform; }

    private string[] Translate(string[] ids)
    {
        return ids.Select(id => LocalizationManager.Instance.GetTranslate(id)).ToArray();
    }

    public bool IsLocked()
    {
        return false;
    }

}
