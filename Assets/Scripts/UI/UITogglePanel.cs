using UnityEngine;
using UnityEngine.UI;

public class UITogglePanel : MonoBehaviour
{
    [SerializeField] private GameObject _panel;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Toggle);
    }

    private void Toggle() => _panel.SetActive(!_panel.activeSelf);
}
