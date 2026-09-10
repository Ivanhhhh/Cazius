using UnityEngine;

public class InventoryFadeController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    public void SetFade(float fade)
    {
        fade = Mathf.Clamp01(fade);

        _canvasGroup.alpha = fade;
    }
}