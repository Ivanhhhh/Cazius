using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyDatabase
{
    public List<EnemyInfo> Enemies = new List<EnemyInfo>();

    private static EnemyDatabase _instance = null;

    //hacemos una instancia
    public static EnemyDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EnemyDatabase();
            }

            return _instance;
        }
    }

    //guardamos la data del enemigo. Buscamos si ya esta guardada y la "overrideamos" y si no la agregamos por primera vez a la lista
    public void SaveEnemyData(string enemyID, int enemyHealth, Vector3 enemyPos, bool isEnemyDead)
    {
        bool found = false;

        for (int i = 0; i < Enemies.Count; i++)
        {
            if (Enemies[i].EnemyID == enemyID)
            {
                Enemies[i].EnemyHealth = enemyHealth;
                Enemies[i].EnemyPos = enemyPos;
                Enemies[i].IsEnemyDead = isEnemyDead;
                found = true;
                break;
            }
        }

        if (!found)
        {
            Enemies.Add(new EnemyInfo(enemyID, enemyHealth, enemyPos, isEnemyDead));
        }

    }

    //vemos si el data existe
    public bool TryGetEnemyData(string enemyID, out EnemyInfo enemyInfo)
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            if (Enemies[i].EnemyID == enemyID)
            {
                enemyInfo = Enemies[i];
                return true;
            }
        }

        enemyInfo = null;
        return false;
    }

    public void SetAllData(List<EnemyInfo> loadedEnemies)
    {
        Enemies = loadedEnemies ?? new List<EnemyInfo>();
    }
}