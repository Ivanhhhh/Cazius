using UnityEngine;

public class QuestLocationTrigger : MonoBehaviour
{
    [Tooltip("Unique ID matching the locationID in ReachLocationCondition")]
    [SerializeField] private string locationID;

    [Tooltip("Tag used to identify the player")]
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        QuestManager.Instance.RegisterLocation(locationID);
        Debug.Log($"[QuestLocationTrigger] Location reached: {locationID}");
    }
}