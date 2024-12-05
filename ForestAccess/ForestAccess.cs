using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ForestAccess : MonoBehaviour
{

    [SerializeField] private GameObject LockScreen;
    [SerializeField] private int CoinsNeed = 0;
    [SerializeField] private TextMeshProUGUI CoinsTMP;

    private Player PlayerLink;
    private SoundsManager _soundManager;
    private void Awake()
    {
        _soundManager = GameObject.FindGameObjectWithTag("UIMain").GetComponent<SoundsManager>();
        PlayerLink = FindObjectOfType<Player>();
        if (CoinsTMP != null)
            CoinsTMP.text = CoinsNeed.ToString();
    }
    public void IsEnoughCoins()
    {
        if(Player.Money>=CoinsNeed)
        {
            _soundManager.PlaySound(2);
            Player.Money -= CoinsNeed;
            PlayerLink.MoneyUpdate();
            Destroy(LockScreen);
        }
    }
    
}
