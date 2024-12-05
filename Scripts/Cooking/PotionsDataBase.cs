using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionsDataBase : MonoBehaviour
{
    public List<PotionData> PotionsOneElement = new List<PotionData>();
    public List<PotionData> PotionsTwoElement = new List<PotionData>();
    public List<PotionData> PotionsThreeElement = new List<PotionData>();
    public List<PotionData> PotionsFourElement = new List<PotionData>();
    public readonly Dictionary<ElementType, string> elementIdentifiers = new Dictionary<ElementType, string>
    {
        { ElementType.Вода, "WATER" },
        { ElementType.Огонь, "FIRE" },
        { ElementType.Земля, "EARTH" },
        { ElementType.Воздух, "AIR" },
        { ElementType.Природа, "NATURE" },
        { ElementType.Металл, "METAL" },
        { ElementType.Свет, "LIGHT" },
        { ElementType.Тьма, "DARK" }
    };
    public string GetElementIdentifier(ElementType elementType)
    {
        if (elementIdentifiers.TryGetValue(elementType, out string identifier))
        {
            return identifier;
        }
        else
        {
            return null;
        }
    }
}
[Serializable]
public class PotionData
{
    [Header("Main")]
    public int Potion_Id;
    public Sprite item_icon;
    public string item_name;
    public List<ElementAttribute> attribute = new List<ElementAttribute>();
    [Header("Market")]
    public bool CanBeSold = true;
    public int start_cost = 0;
    // [Space]
    // public int CollapseCount = 4;
    // public int RecoveryCount = 20;
}