using System;
using TMPro;
using UnityEngine;

public class Player_Recharge : MonoBehaviour
{
    [SerializeField] int _maxBullets;
    [SerializeField] TextMeshProUGUI _maxBulletsUI;
    [SerializeField] TextMeshProUGUI _pressR;

    private int _remainingBullets;
    private TextMeshProUGUI _remainingBulletsUI;
    public bool _hasBullets => _remainingBullets > 0;
    public Action OnShoot;

    void Start()
    {
        _remainingBullets = _maxBullets;
        _maxBulletsUI.text = $"{_maxBullets}";
    }
    void Update()
    {
        if (GameInputManager.Instance.Controls.Player.Recharge.IsPressed())
        {
            OnShoot();
        }
    }

    void ManageShoot()
    {
        _remainingBullets --;
        _remainingBulletsUI.text = $"{_remainingBullets}";
    }
}
