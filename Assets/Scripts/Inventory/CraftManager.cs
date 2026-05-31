using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [Header("Listas y Botones")]
    [SerializeField] GameObject[] _objectsList;
    [SerializeField] GameObject[] _buttonsToPush;
    [SerializeField] CraftTabButton[] _buttonsVisuals;

    private int _currentIndex = 0;

    void Start()
    {
        ActivateList(0);
    }
     
    public void ActivateList(int indexToActivate)
    {
        _currentIndex = indexToActivate;

        for (int i = 0; i < _objectsList.Length; i++)
        {
            _objectsList[i].SetActive(i == indexToActivate); 
        }

        UpdateButtonsVisuals();
    }

    private void UpdateButtonsVisuals()
    {
        for (int i = 0; i < _buttonsVisuals.Length; i++)
        {
            _buttonsVisuals[i].SetSelectedButton(i == _currentIndex);
        }
    }
}
