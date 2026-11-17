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

        // Ignore empty slots
        List<GameObject> valid = new List<GameObject>();
        foreach (GameObject obj in _objects)
        {
            if (obj != null) valid.Add(obj);
        }

        if (valid.Count == 0) return;

        bool[] shouldEnable = _allowMultipleEnabled
            ? PickMultiple(valid.Count)
            : PickSingle(valid.Count);

        for (int i = 0; i < valid.Count; i++)
            valid[i].SetActive(shouldEnable[i]);
    }

    // One object enabled, or none when allowed (one extra outcome)
    private bool[] PickSingle(int count)
    {
        bool[] result = new bool[count];

        int outcomes = _allowAllDisabled ? count + 1 : count;
        int chosen = Random.Range(0, outcomes);

        if (chosen < count)
            result[chosen] = true;

        return result;
    }

    // Each object is independently on or off (50/50)
    private bool[] PickMultiple(int count)
    {
        bool[] result = new bool[count];
        bool anyEnabled = false;

        for (int i = 0; i < count; i++)
        {
            result[i] = Random.value < 0.5f;
            anyEnabled |= result[i];
        }

        // Nothing came up enabled and that is not allowed: force one random object on
        if (!anyEnabled && !_allowAllDisabled)
            result[Random.Range(0, count)] = true;

        return result;
    }

}
