using UnityEngine;
using UnityEngine.UI;


public class ButtonSelfRegister : MonoBehaviour
{
    [SerializeField] private ChangeOption _changeOption;
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => _changeOption.ButtonToggle(this._button));
    }

    private void Update()
    {
       
    }
}