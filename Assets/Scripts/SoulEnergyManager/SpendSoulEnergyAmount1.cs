using UnityEngine;
public class SpendSoulEnergyAmount : MonoBehaviour
{
    private static int _SoulEnergyAmount;

    public static void SpendSoulEnergy(int Amount)
    {
       AddSoulEnergyAmount.SoulEnergyAmount -= Amount;
    }
}
