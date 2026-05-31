using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [Header("Listas y Botones")]
    [SerializeField] GameObject[] _objectsList;
    [SerializeField] GameObject[] _buttonsToPush;
     
    void Start()
    {
        ActivateList(0);
    }
     
    public void ActivateList(int indexToActivate)
    {
        for (int i = 0; i < _objectsList.Length; i++)
        {
            _objectsList[i].SetActive(i == indexToActivate); 
        }
    }
}
