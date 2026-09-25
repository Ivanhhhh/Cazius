using UnityEngine;

public class NPCIdleState : NPCState
{
    private float _timer;

    public NPCIdleState(NPCController npc) : base(npc) { }

    public override void Enter()
    {
        _timer = _npc.WaitTime;
    }

    public override void Tick()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
            _npc.Machine.ChangeState(_npc.WalkingState);
    }

    public override void Exit()
    {
        _timer = 0f;
    }

}