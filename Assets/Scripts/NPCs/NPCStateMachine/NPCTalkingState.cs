using UnityEngine;

public class NPCTalkingState : NPCState
{
    private Transform _interactor;
    private string _wavingTrigger;
    private SFXManager.SFXCategoryType _interactSFX;

    public NPCTalkingState(NPCController npc) : base(npc) { }

    // Called by the interactable script right before ChangeState(TalkingState)
    public void BeginTalking(Transform interactor, string wavingTrigger, SFXManager.SFXCategoryType interactSFX)
    {
        _interactor = interactor;
        _wavingTrigger = wavingTrigger;
        _interactSFX = interactSFX;
    }

    public override void Enter()
    {
        _npc.Velocity = Vector3.zero;

        if (_npc.Animator != null && !string.IsNullOrEmpty(_wavingTrigger))
            _npc.Animator.SetTrigger(_wavingTrigger);

        if (SFXManager.Instance != null)
            SFXManager.Instance.PlaySFXAtPosition(_interactSFX, _npc.transform.position);
    }

    public override void Tick()
    {
        if (_interactor == null) return;

        Vector3 dir = _interactor.position - _npc.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        _npc.transform.rotation = Quaternion.RotateTowards(
            _npc.transform.rotation,
            Quaternion.LookRotation(dir),
            _npc.TurnSpeed * Time.deltaTime);
    }

    public override void Exit()
    {
        _interactor = null;
    }

}
