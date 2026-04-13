using System.IO;
using UnityEngine;

public class SaveEnemiesJSON : MonoBehaviour
{
    private EnemyDatabase enemyDatabase;
    private string _path;

    private void Start()
    {
        LoadData();
        //instancia para los metodos
        enemyDatabase = EnemyDatabase.Instance;
        //donde esta el archivo json
        _path = Path.Combine(Application.persistentDataPath, "EnemiesData.json");
    }

    //guardamos la data en el JSON
    public void SaveData()
    {
        string json = JsonUtility.ToJson(enemyDatabase, true);
        Debug.Log(json);

        using (StreamWriter writer = new StreamWriter(_path))
        {
            writer.Write(json);
        }
    }

    public void LoadData()
    {
        if (!File.Exists(_path))
        {
            Debug.Log("Creating new Enemy JSON save file");
            SaveData();
            return;
        }

        string json = string.Empty;

        using (StreamReader reader = new StreamReader(_path))
        {
            json = reader.ReadToEnd();
        }

        EnemyDatabase loadedData = JsonUtility.FromJson<EnemyDatabase>(json);

        if (loadedData != null && loadedData.Enemies != null)
        {
            enemyDatabase.SetAllData(loadedData.Enemies);
        }
        else
        {
            enemyDatabase.SetAllData(null);
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }
}