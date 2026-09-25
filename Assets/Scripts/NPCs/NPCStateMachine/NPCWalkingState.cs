using UnityEngine;

public class NPCWalkingState : NPCState
{
    public NPCWalkingState(NPCController npc) : base(npc) { }

    public override void Enter()
    {
        _npc.SetWalking(true);
    }

    public override void Exit()
    {
        _npc.SetWalking(false);
    }

    public override void Tick()
    {
        Transform t = _npc.transform;
        Vector3 toTarget = _npc.CurrentWaypoint.position - t.position;
        toTarget.y = 0f;

        if (toTarget.magnitude <= _npc.ArriveDistance)
        {
            // PROGRESS LIVES IN CONTROLLER, IT SURVIVES INTERRUPTIONS
            _npc.AdvanceWaypoint();
            _npc.Machine.ChangeState(_npc.IdleState);
            return;
        }

        // Seek steering
        Vector3 desiredVelocity = toTarget.normalized * _npc.MoveSpeed;
        _npc.Velocity = Vector3.MoveTowards(
            _npc.Velocity,
            desiredVelocity,
            _npc.Acceleration * Time.deltaTime);

        t.position += _npc.Velocity * Time.deltaTime;

        // Facing follows velocity (where it's moving)
        if (_npc.Velocity.sqrMagnitude > 0.01f)
        {
            t.rotation = Quaternion.RotateTowards(
                t.rotation,
                Quaternion.LookRotation(_npc.Velocity.normalized),
                _npc.TurnSpeed * Time.deltaTime);
        }
    }

}
