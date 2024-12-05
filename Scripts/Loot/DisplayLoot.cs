using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayLoot : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI DisplayText;
    
    void Update() //Change this System
    {
        DisplayText.text = "Осталось " + MapCreator.LootCount;
    }
}
