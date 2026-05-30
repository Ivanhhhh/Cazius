using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform playerTransform;

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Save()
    {
        SaveData data = new SaveData();

        // --- Quests ---
        var questDict = QuestManager.Instance.GetSaveData();
        var questEntries = new SerializableQuestEntry[questDict.Count];
        int i = 0;
        foreach (var kvp in questDict)
        {
            questEntries[i++] = new SerializableQuestEntry
            {
                questID = kvp.Key,
                status = kvp.Value
            };
        }
        data.questStates = questEntries;

        // --- Inventory ---
        // data.inventoryItemIDs = Inventory.Instance.GetAllItemIDs();
        // data.keyItemIDs = Inventory.Instance.GetKeyItemIDs();

        // --- Player ---
        if (playerTransform != null)
        {
            data.playerX = playerTransform.position.x;
            data.playerY = playerTransform.position.y;
            data.playerZ = playerTransform.position.z;
        }
        data.currentScene = SceneManager.GetActiveScene().name;

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[SaveManager] Saved to {SavePath}");
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[SaveManager] No save file found.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // --- Quests ---
        var questDict = new Dictionary<string, QuestStatus>();
        if (data.questStates != null)
        {
            foreach (var entry in data.questStates)
                questDict[entry.questID] = entry.status;
        }
        QuestManager.Instance.LoadSaveData(questDict);

        // --- Inventory ---
        // Inventory.Instance.LoadSaveData(data.inventoryItemIDs, data.keyItemIDs);

        // --- Player position is applied by the scene loader, not here ---
        Debug.Log($"[SaveManager] Loaded from {SavePath}");
    }

    public Vector3 GetSavedPlayerPosition()
    {
        if (!File.Exists(SavePath)) return Vector3.zero;

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return new Vector3(data.playerX, data.playerY, data.playerZ);
    }
}