using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellMarketManager : MonoBehaviour
{
    [SerializeField] List<Slot> MarketSlots = new List<Slot>();
    private Player PlayerLink;
    [SerializeField] private DataBase db;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject desc;
    SoundsManager _soundManager;

    public void refresh()
    {
        foreach (Slot slot in MarketSlots)
        {
            Destroy(slot.SlotObject);
        }
        desc.SetActive(true);
        MarketSlots.Clear();
        for (int i = 0; i < inventory.current_items.Count; i++)
        {
            if (inventory.current_items[i] != 0)
            {
                desc.SetActive(false);
            }
            int it = inventory.current_items[i];
            if (it > 0)
            {
                GameObject slot_object = Instantiate(slotPrefab, transform);
                Slot new_slot = new Slot
                {
                    SlotObject = slot_object,
                    Item_ID = i,
                    StartCost = db.items[i].start_cost,
                    Item_Count = it,
                    Item_Image = db.items[i].item_icon,
                    BuyCount = db.items[i].buy_count
                    
                };
                MarketSlots.Add(new_slot);
            }
        }
        Init();
        PlayerLink = FindObjectOfType<Player>();
        _soundManager = GameObject.FindGameObjectWithTag("UIMain").GetComponent<SoundsManager>();
    }
    private void Init()
    {
        for (int i = 0; i < MarketSlots.Count; i++)
        {
            int currentIndex = i;
            MarketSlots[currentIndex].SlotObject.transform.Find("ItemImage").GetComponent<Image>().sprite = MarketSlots[currentIndex].Item_Image;
            // MarketSlots[currentIndex].SlotObject.transform.Find("CostBack/CostText").GetComponent<TextMeshProUGUI>().text = MarketSlots[currentIndex].StartCost.ToString();
            
            MarketSlots[currentIndex].SlotObject.transform.Find("CountText").GetComponent<TextMeshProUGUI>().text = "X" + MarketSlots[currentIndex].Item_Count.ToString();
            MarketSlots[currentIndex].SlotObject.transform.Find("SellButton").GetComponent<Button>().onClick.AddListener(() => Sell(MarketSlots[currentIndex]));
            MarketSlots[currentIndex].SlotObject.transform.Find("CostBack/CostText").GetComponent<TextMeshProUGUI>().text = GetCost(MarketSlots[currentIndex]).ToString();
        }
    }
    public void Sell(Slot Target)
    {
        if (Target.Item_Count > 0)
        {
            Player.Money += GetCost(Target);
            PlayerLink.MoneyUpdate();
            
            if (db.items[Target.Item_ID].buy_count <= 100)
            {
                db.items[Target.Item_ID].buy_count++;
                UpdateCost(Target);
            }

            UpdateOtherItems(Target);
            inventory.delete(Target.Item_ID, 1);
        }
    }
    private void UpdateOtherItems(Slot SlotIgnore)
    {
        for(int i = 0; i < MarketSlots.Count;i++)
        {
            if(MarketSlots[i]!=SlotIgnore)
            {
                MarketSlots[i].Pointer++;
                if (MarketSlots[i].Pointer% MarketSlots[i].RecoveryCount==0)
                {
                    MarketSlots[i].Pointer = 0;
                    if (MarketSlots[i].BuyCount > 0)
                    {
                        MarketSlots[i].BuyCount--;
                        db.items[MarketSlots[i].Item_ID].buy_count--;
                        UpdateCost(MarketSlots[i]);
                    }
                }
            }
        }
    }
    private void UpdateCost(Slot Target)
    {
        Target.SlotObject.transform.Find("CostBack/CostText").GetComponent<TextMeshProUGUI>().text = GetCost(Target).ToString();
    }
    private int GetCost(Slot Target)
    {
        float Multiplier = Target.BuyCount / Target.CollapseCount * 0.1f;
        //Debug.Log(Multiplier);
        if (Multiplier >= 0.5f)
            Multiplier = 0.5f;
        return (int)(Target.StartCost * (1f- Multiplier));
    }
    private void UpdateText(Slot Target)
    {
        Target.SlotObject.transform.Find("CountText").GetComponent<TextMeshProUGUI>().text = "X" + Target.Item_Count.ToString();
    }
}

[Serializable]
public class Slot
{
    public GameObject SlotObject;
    public int Item_ID;
    public Sprite Item_Image;
    public int Item_Count;
    public int StartCost;

    public int CollapseCount = 4;
    public int RecoveryCount = 20;

    public int BuyCount=0;
    public int Pointer=0;
}