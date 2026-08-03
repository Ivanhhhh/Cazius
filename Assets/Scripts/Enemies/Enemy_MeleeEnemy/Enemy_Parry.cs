using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy_Parry : MonoBehaviour
{
    [SerializeField] private Enemy_MeleeEnemy_Data _enemyData;
    public void Execute()
    {
        _enemyData._isStunned = true;
    }
}
