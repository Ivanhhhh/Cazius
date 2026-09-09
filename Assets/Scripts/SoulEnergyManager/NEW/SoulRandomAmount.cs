using UnityEngine;

public class SoulRandomAmount : MonoBehaviour
{
    [SerializeField] private int _minDrop;
    [SerializeField] private int _maxDrop;

    public void OnEnemyDeath()
    {
        int randomAmount = Random.Range(_minDrop, _maxDrop + 1);
        long amountToGive = randomAmount;

        SoulEnergyManager.Instance.AddSoulEnergy(amountToGive);
        Debug.LogWarning(amountToGive);
    }
}