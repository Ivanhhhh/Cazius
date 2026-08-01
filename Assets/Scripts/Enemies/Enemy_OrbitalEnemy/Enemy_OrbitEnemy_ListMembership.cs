using UnityEngine;

public class EnemyListMembership : MonoBehaviour
{
    private Enemy_OrbitEnemyData _owner;

    public void Initialize(Enemy_OrbitEnemyData owner)
    {
        _owner = owner;
    }

    private void OnDestroy()
    {
        if (_owner != null)
            _owner.RemoveEnemyAlive(gameObject);
    }
}