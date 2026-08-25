using UnityEngine;

public interface IEInteractable
{
    void Interact(Transform interactorTransform);
    string GetInteractText();
    Transform GetTransform();

    Transform GetInteractionUIPoint();
}
