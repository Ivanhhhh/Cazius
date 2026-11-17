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
        List<GameObject> validObjects = new List<GameObject>();

        foreach (GameObject obj in _objects)
        {
            if (obj == null) continue;

            obj.SetActive(false);
            validObjects.Add(obj);
        }

        if (validObjects.Count == 0) return;

        int chosen = Random.Range(0, validObjects.Count);

        validObjects[chosen].SetActive(true);
    }
}