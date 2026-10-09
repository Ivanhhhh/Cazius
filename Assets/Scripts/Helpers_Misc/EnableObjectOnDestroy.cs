using UnityEngine;

public class EnableObjectOnDestroy : MonoBehaviour
{
    [SerializeField] private GameObject _obj;

    private void OnDestroy()
    {
        if (_obj != null)
        {
            _obj.SetActive(true);
        }
    }
}
