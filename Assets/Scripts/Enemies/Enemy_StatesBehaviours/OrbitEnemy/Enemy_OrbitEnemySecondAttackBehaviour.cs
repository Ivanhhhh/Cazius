using System.Data.Common;
using UnityEngine;

public class Enemy_OrbitEnemySecondAttackBehaviour 
{
    private readonly Enemy_OrbitEnemyData _orbitData;

    public Enemy_OrbitEnemySecondAttackBehaviour(Enemy_OrbitEnemyData data)
    {
        _orbitData = data;
    }

    public void SpawnEnemies()
    {
        if (_orbitData.OrbitManager == null) return;

        // El boost de velocidad de las piedras arranca YA y dura _secondAttackDuration
        _orbitData.OrbitManager.BoostSharedSpeed(
            _orbitData.SpeedBoostMultiplier,
            _orbitData.SecondAttackDuration
        );

        // El spawn de enemigos se dispara recién cuando termine esa misma duración
        _orbitData.StartSecondAttackSequence();
    }
}