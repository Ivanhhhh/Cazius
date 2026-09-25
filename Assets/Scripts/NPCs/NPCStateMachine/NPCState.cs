public abstract class NPCState
{
    protected readonly NPCController _npc;

    protected NPCState(NPCController npc)
    {
        _npc = npc;
    }

    public virtual void Enter() { }
    public virtual void Tick() { }
    public virtual void Exit() { }

}
