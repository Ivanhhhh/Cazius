using UnityEngine;

public class QuestCompletedObjectEnabler : MonoBehaviour
{
    [SerializeField] private string _questID;
    [SerializeField] private GameObject _gameObjectToEnable;

    private void OnEnable()
    {
        if (QuestManager.Instance == null || _gameObjectToEnable == null) return;

        bool isCompleted = QuestManager.Instance.GetStatus(_questID) == QuestStatus.Completed;
        _gameObjectToEnable.SetActive(isCompleted);
    }
}