using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GraphController : MonoBehaviour
{
    [SerializeField] private float LevelHeight = 25f;
    [SerializeField] List<GraphElement> Elements = new List<GraphElement>();
    [SerializeField] private DataBase db;
    [SerializeField] private TextMeshProUGUI Title;

    public int ElementLevel=4;
    void Start()
    {
    }

    public Item getPotion()
{
    Dictionary<ElementType, int> requiredAttributes = new Dictionary<ElementType, int>();

    // 1. Рассчитываем необходимые атрибуты
    int max = 0;
    foreach (var el in Elements)
    {
        if (Enum.TryParse(el.name, true, out ElementType elementType))
        {
            if (el.Value > 0)
            {
                int v = 0;
                int n = el.Value;
                while (n >= ElementLevel)
                {
                    v++;
                    n -= ElementLevel;
                }
                requiredAttributes[elementType] = v;
                if (v > max)
                {
                    max = v;
                }
            }
        }
    }

    var keys = new List<ElementType>(requiredAttributes.Keys);
    var keysToRemove = new List<ElementType>();

    foreach (var key in keys)
    {
        if (requiredAttributes[key] != max)
        {
            keysToRemove.Add(key);
        }
        else
        {
            requiredAttributes[key] = 1;
        }
    }
    
    foreach (var key in keysToRemove)
    {
        requiredAttributes.Remove(key);
    }

    
    List<Item> matchingItems = new List<Item>();
    
    foreach (var item in db.items)
    {
        if (!item.Is_potion)
        {
            continue;
        }
        
        bool matches = true;
        
        foreach (var required in requiredAttributes)
        {
            var matchingAttribute = item.attributes.Find(attr => attr.elementType == required.Key);
            
            if (matchingAttribute == null || matchingAttribute.amount != required.Value)
            {
                matches = false;
                break;
            }
        }
        
        if (matches)
        {
            if (item.attributes.Count != requiredAttributes.Count)
            {
                matches = false;
            }
        }

        if (matches)
        {
            matchingItems.Add(item);
        }
    }
    
    if (matchingItems.Count == 0)
    {
        print("Не нашлось зелья с нужным рецептом");
        return new Item();
    }
    else
    {
        return matchingItems[0];
    }
}

    
    public void ResolveGraph(List<Block> blocks = null)
    {
        foreach (var el in Elements)
        {
            el.Value = 0;
        }
        foreach (var block in blocks)
        {
            var item = db.items[block.item_id];
            foreach (var el in Elements)
            {
                foreach (var element in item.attributes)
                {
                    if (element.elementType.ToString() == el.name)
                    {
                        el.Value += element.amount;
                    }
                }
            }
        }
        

        string desc = "";
        for(int i=0;i<Elements.Count;i++)
        {
            if (Elements[i].Value > 0)
            {
                string str = Elements[i].name;
                Enum.TryParse(str, true, out ElementType result);
                desc += Elements[i].Value + " " + Elements[i].name + " <sprite name=" +
                        db.GetElementIdentifier(result) +
                        ">\n";
            }
            Elements[i].Object.GetComponent<RectTransform>().sizeDelta = new Vector2(Elements[i].Object.GetComponent<RectTransform>().rect.width,LevelHeight+ LevelHeight * Elements[i].Value);
        }
        Title.text = desc;
    }
}
[Serializable]
public class GraphElement
{
    public GameObject Object;
    public string name;
    public int Value;
}
