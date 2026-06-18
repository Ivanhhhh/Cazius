using System.Collections.Generic;
using UnityEngine;

public class UIHideOnDialogOpen : MonoBehaviour
{
    [SerializeField] private List<GameObject> _objectsToHide;

    private void OnEnable()
    {
        DialogUIController.OnDialogOpened += Hide;
        DialogUIController.OnDialogClosed += Show;
    }

    private void OnDisable()
    {
        DialogUIController.OnDialogOpened -= Hide;
        DialogUIController.OnDialogClosed -= Show;
    }

    private void Hide()
    {
        foreach (var obj in _objectsToHide)
            obj.SetActive(false);
    }

    private void Show()
    {
        foreach (var obj in _objectsToHide)
            obj.SetActive(true);
    }
}
