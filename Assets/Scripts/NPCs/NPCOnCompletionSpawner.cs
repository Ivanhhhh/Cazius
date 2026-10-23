using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCQuestGiver))]
public class NPCOnCompletionSpawner : MonoBehaviour
{
    [Tooltip("Objects in the same scene as this NPC, disabled by default")]
    [SerializeField] private List<GameObject> _objectsToEnable = new List<GameObject>();

    private NPCQuestGiver _questGiver;

    private void Awake()
    {
        _questGiver = GetComponent<NPCQuestGiver>();
    }

    private void OnEnable()
    {
        _questGiver.OnCompletionApplied += HandleCompletionApplied;
    }

    private void OnDisable()
    {
        _questGiver.OnCompletionApplied -= HandleCompletionApplied;
    }

    private void HandleCompletionApplied()
    {
        foreach (GameObject obj in _objectsToEnable)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

}
