using System.Collections.Generic;
using UnityEngine;

// WARNING!!!!!!!! Do not put this script on one of the listed objects, or it can disable itself

public class Enable1Random : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private List<GameObject> _objects = new List<GameObject>();

    private void Start()
    {
        PickRandom();
    }

    private void PickRandom()
    {
        int validCount = 0;
        foreach (GameObject obj in _objects)
        {
            if (obj != null) validCount++;
        }

        if (validCount == 0) return;

        int chosen = Random.Range(0, validCount);
        int index = 0;

        foreach (GameObject obj in _objects)
        {
            if (obj == null) continue;

            obj.SetActive(index == chosen);
            index++;
        }
    }

}
