using UnityEngine;

public interface IEInteractable
{
    void Interact(Transform interactorTransform);
    string GetInteractText();

    bool IsLocked();
    Transform GetTransform();

    Transform GetInteractionUIPoint();
}
