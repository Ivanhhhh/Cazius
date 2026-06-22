using System;

[Serializable]
public class SaveData
{
    public SerializableQuestEntry[] questStates;
    public string[] keyItemIDs;
    public string[] inventoryItemIDs;
    public float playerX;
    public float playerY;
    public float playerZ;
    public string currentScene;
}

// Dictionary isn't directly serializable with JsonUtility so we flatten to an array
[Serializable]
public class SerializableQuestEntry
{
    public string questID;
    public QuestStatus status;
}