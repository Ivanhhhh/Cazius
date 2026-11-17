using System.Collections.Generic;
using UnityEngine;

// WARNING!!!!!!!! Do not put this script on one of the listed objects, or it can disable itself

// Once at Start, randomly picks which of the listed objects are enabled and disables the rest
// Empty slots are ignored. A null or empty list does nothing
//
// _allowMultipleEnabled | _allowAllDisabled | Result
// ----------------------|-------------------|----------------------------------------------------------
// off                   | off               | Exactly one enabled. A single object is always on
// off                   | on                | None or one enabled. A single object is 50/50
// on                    | off               | At least one enabled. Each object is 50/50, and one is forced on if none came up
// on                    | on                | Any combination, including all off. Each object is 50/50
//
// With _allowMultipleEnabled each object is an independent coin flip, so how many end up enabled varies from run to run

public class Enable1Random : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private List<GameObject> _objects = new List<GameObject>();

    [Header("Behavior")]
    [Tooltip("Allows more than one object to be enabled at the same time")]
    [SerializeField] private bool _allowMultipleEnabled = false;

    [Tooltip("Allows every object to be disabled")]
    [SerializeField] private bool _allowAllDisabled = false;

    private void Start()
    {
        PickRandom();
    }

    private void PickRandom()
    {
        if (_objects == null || _objects.Count == 0) return;

        List<GameObject> valid = new List<GameObject>();
        List<GameObject> unique = new List<GameObject>();
        HashSet<GameObject> seen = new HashSet<GameObject>();

        foreach (GameObject obj in _objects)
        {
            if (obj == null) continue;

            valid.Add(obj);

            if (seen.Add(obj))
                unique.Add(obj);
        }

        if (valid.Count == 0) return;

        if (_allowMultipleEnabled)
        {
            PickMultiple(unique);
        }
        else
        {
            PickSingle(valid, unique);
        }
    }

    private void PickSingle(
        List<GameObject> valid,
        List<GameObject> unique)
    {
        int outcomes = _allowAllDisabled
            ? valid.Count + 1
            : valid.Count;

        int chosen = Random.Range(0, outcomes);

        foreach (GameObject obj in unique)
            obj.SetActive(false);

        if (chosen < valid.Count)
            valid[chosen].SetActive(true);
    }

    private void PickMultiple(List<GameObject> unique)
    {
        bool[] shouldEnable = new bool[unique.Count];
        bool anyEnabled = false;

        for (int i = 0; i < unique.Count; i++)
        {
            shouldEnable[i] = Random.value < 0.5f;

            if (shouldEnable[i])
                anyEnabled = true;
        }

        if (!anyEnabled && !_allowAllDisabled)
        {
            int chosen = Random.Range(0, unique.Count);
            shouldEnable[chosen] = true;
        }

        for (int i = 0; i < unique.Count; i++)
        {
            unique[i].SetActive(shouldEnable[i]);
        }
    }
}