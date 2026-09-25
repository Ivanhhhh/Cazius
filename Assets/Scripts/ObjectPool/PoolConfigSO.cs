using UnityEngine;

[CreateAssetMenu(fileName = "PoolConfigSO", menuName = "Scriptable Objects/PoolConfigSO")]
public class PoolConfigSO : ScriptableObject
{
    [Tooltip("ID único para este pool. Ej: 'Bullet_Caster'")]
    public string id;

    [Tooltip("Prefab que debe implementar IPoolable")]
    public MonoBehaviour prefab;

    [Tooltip("Cuántos objetos se crean al inicio")]
    public int initialAmount = 20;

    [Tooltip("Tamaño máximo opcional (0 = sin límite)")]
    public int maxSize = 0;
}
