using UnityEngine;

public class SoulUIManager : MonoBehaviour
{
    public static SoulUIManager Instance;
    [SerializeField] private GameObject[] _soulEnergyPanels;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateUI(int currentSoulEnergy)
    {
        foreach (GameObject panel in _soulEnergyPanels)
        {
            panel.SetActive(false);
        }

        if (currentSoulEnergy > 0 && currentSoulEnergy <= _soulEnergyPanels.Length)
        {
            _soulEnergyPanels[currentSoulEnergy - 1].SetActive(true);
        }
    }
}
