using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIToggleObjects : MonoBehaviour
{
    [SerializeField] private List<GameObject> _objectsToToggle = new();

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Toggle);
    }

    private void Toggle()
    {
        foreach (var obj in _objectsToToggle)
        {
            if (obj != null)
            {
                obj.SetActive(!obj.activeSelf);
            }
        }
    }

}
