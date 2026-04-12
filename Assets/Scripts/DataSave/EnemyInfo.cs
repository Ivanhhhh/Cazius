using System;
using UnityEngine;

[Serializable]
public class EnemyInfo
{
    public string EnemyID;
    public int EnemyHealth;
    public Vector3 EnemyPos;
    public bool IsEnemyDead;

    public EnemyInfo(string enemyID, int enemyHealth, Vector3 enemyPos, bool isEnemyDead)
    {
        EnemyID = enemyID;
        EnemyHealth = enemyHealth;
        EnemyPos = enemyPos;
        IsEnemyDead = isEnemyDead;
    }
}