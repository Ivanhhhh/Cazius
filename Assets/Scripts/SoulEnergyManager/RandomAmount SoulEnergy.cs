using UnityEngine;

public class RandomAmountSoulEnergy : MonoBehaviour
{
    [SerializeField] private int MinSoulAmount;   
    [SerializeField] private int MaxSoulAmount;

    void OnEnable()
    {
      CalculateRandomSA();
    }

    public void CalculateRandomSA()
    {
       AddSoulEnergyAmount.SoulEnergyAmount  += Random.Range(MinSoulAmount,MaxSoulAmount);
       Debug.LogWarning(AddSoulEnergyAmount.SoulEnergyAmount);
    }
}
