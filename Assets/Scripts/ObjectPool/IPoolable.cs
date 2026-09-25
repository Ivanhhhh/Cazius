using UnityEngine;

public interface IPoolable
{
    string PoolId { get; set; }

    void OnGetFromPool();

    void OnReturnToPool();
}
