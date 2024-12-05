using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DataBase : MonoBehaviour
{
    public List<Item>items = new List<Item>();
    public int itemCount = 0;
    public PotionsDataBase pdb;
    public int potions_stack = 6;
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

    private void Awake()
    {
        itemCount = items.Count;
        HashSet<int> existingIds = new HashSet<int>(items.Select(i => i.item_id));
        AddPotionsToItems(pdb.PotionsOneElement, existingIds);
        AddPotionsToItems(pdb.PotionsTwoElement, existingIds);
        AddPotionsToItems(pdb.PotionsThreeElement, existingIds);
        AddPotionsToItems(pdb.PotionsFourElement, existingIds);
    }

    private void AddPotionsToItems(List<PotionData> potions, HashSet<int> existingIds)
    {
        foreach (var potion in potions)
        {
            int newId = items.Count;
            while (existingIds.Contains(newId))
            {
                newId++;
            }
            Item item = new Item
            {
                item_id = newId,
                stack = potions_stack,
                item_name = potion.item_name,
                item_icon = potion.item_icon,
                attributes = potion.attribute,
                Is_potion = true,
                start_cost = potion.start_cost,
            };
            items.Add(item);
            existingIds.Add(newId);
        }
    }

}

[System.Serializable]
public class ElementAttribute
{
    public ElementType elementType;
    public int amount = 1;
}

public enum ElementType
{
    Вода,
    Огонь,
    Земля,
    Воздух,
    Природа,
    Металл,
    Свет,
    Тьма
}

[System.Serializable]
public class Item
{
    public int item_id = 0;
    public int stack;
    public string item_name;
    public Sprite item_icon;
    public int start_cost;
    public int buy_count = 0;
    public List<ElementAttribute> attributes = new List<ElementAttribute>();
    public List<Vector2Int> BlockPositions;
    public Color figure_color;
    public Boolean Is_potion = false;
}