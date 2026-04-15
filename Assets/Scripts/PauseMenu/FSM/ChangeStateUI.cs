using UnityEngine;

public class ChangeStateUI : MonoBehaviour
{
    [SerializeField] private MenuWithFSM menu; // mejor que Find

    public void GoToPause()
    {
        menu.ChangeState(AgentStates.Pause);
    }

    public void GoToUnpause()
    {
        menu.GetFSM().ChangeState(AgentStates.Unpause);
    }

    public void Quit()
    {
        menu.GetFSM().ChangeState(AgentStates.Quit);
    }
}