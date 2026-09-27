using System;

public class NPCStateMachine
{
    public NPCState Current { get; private set; }
    public NPCState Previous { get; private set; }

    // OldState to NewState - OldState is null on the first transition
    public event Action<NPCState, NPCState> OnStateChanged;

    public void ChangeState(NPCState next)
    {
        if (next == null || next == Current) return;

        Current?.Exit();
        Previous = Current;
        Current = next;
        Current.Enter();

        OnStateChanged?.Invoke(Previous, Current);
    }

    public void Tick()
    {
        Current?.Tick();
    }

}
