using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class ConfirmationPopup : MonoBehaviour
{
    public static ConfirmationPopup Instance { get; private set; }
    [SerializeField] GameObject _panel;
    [SerializeField] TextMeshProUGUI _messageText;
    [SerializeField] TextMeshProUGUI _yesText, _noText;
    [SerializeField] string _defaultMessage;
    [SerializeField] string _defaultYes;
    [SerializeField] string _defaultNo;
    [SerializeField] Button _yesButton, _noButton;
    Action _onYes, _onNo;

    private void Awake()
    {
        Instance = this;
        _yesButton.onClick.AddListener(Confirm);
        _noButton.onClick.AddListener(Cancel);
        _panel.SetActive(false);
    }
    public void Show(Action yesAction, string message = "", Action noAction = null, string yes = "", string no = "")
    {
        _messageText.text = message != "" ? message : _defaultMessage;
        _yesText.text = yes != "" ? yes : _defaultYes;
        _noText.text = no != "" ? no : _defaultNo;

        _onYes = yesAction;
        _onNo = noAction;

        _panel.SetActive(true);
    }
    void Confirm()
    {
        _panel.SetActive(false);

        if (_onYes != null)
            _onYes();
    }

    void Cancel()
    {
        _panel.SetActive(false);

        if (_onNo != null)
            _onNo();
    }
}
