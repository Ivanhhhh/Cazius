using UnityEngine;

public class SoulRandomAmount : MonoBehaviour,IRandomSoul
{
    [SerializeField] private int _minDrop;
    [SerializeField] private int _maxDrop;

    public void RandomSoul()
    {
        int randomAmount = Random.Range(_minDrop, _maxDrop + 1);
        SoulEnergyManager.Instance.AddSoulEnergy(randomAmount);
        // Debug.LogError(randomAmount + "DROPPED");
        // Debug.LogWarning(SoulEnergyManager.Instance.CurrentSoulEnergy);
    }
}