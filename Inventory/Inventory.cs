using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using Newtonsoft.Json;

public class Inventory : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public DataBase data;
    public List<ItemInventory> items = new List<ItemInventory>();

    public GameObject gameObjShow;
    public GameObject inventoryMainObject;
    public int maxCount;
    public Camera cam;
    public EventSystem es;
    public int currentID = -1;
    public ItemInventory currentItem;
    public RectTransform movingObject;
    public List<GameObject> UIs = new List<GameObject>();
    
    public Vector3 offset;
    public bool isOpen = false;
    
    public ScrollRect scrollRect;
    public Player player;
    public float scrollSpeed = 0.1f;
    private bool isPointerOver = false;
    public RectTransform InventoryRect;
    public GameObject Background;
    public List<int> current_items = new List<int>();
    public SpawnIngredients spawnIngredients;
    public SellMarketManager market;
    public Boolean onForest = false;

    private SoundsManager _soundManager;
    private string savePath;
    public void refresh()
    {
        CountItems();
        if (onForest == false && spawnIngredients!=null && market!=null)
        {
            spawnIngredients.refresh();
            market.refresh();
        }
        UpdateInventory();
        SaveInventory();
    }

    public void ClearInventory()
    {
        items.Clear();
        current_items.Clear();
        SaveInventory();
        LoadInventory();
    }
    
    public void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "inventory.json");
        if (items.Count == 0)
        {
            AddGraphics();
        }
        if (File.Exists(savePath))
        {
            LoadInventory();
        }
        
        refresh();
        Background.SetActive(isOpen);
        _soundManager = inventoryMainObject.GetComponent<SoundsManager>();
        
        // ClearInventory();
    }

    public void delete(int item_id, int count)
    {
        foreach (ItemInventory item in items)
        {
            if (item.id == item_id)
            {
                item.count -= count;
                if (item.count < 0)
                {
                    count = -item.count;
                    item.count = 0;
                }
                else
                {
                    if (item.count == 0)
                    {
                        item.id = 0;
                    }
                    break;
                }
            }
        }
        refresh();
    }

    public void CountItems()
    {
        List<int> current = new List<int>(new int[data.items.Count]);

        foreach (ItemInventory it in items)
        {
            if (it.id != 0)
            {
                current[it.id] += it.count;
            }
        }
        current_items = current;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        int selectedID = int.Parse(es.currentSelectedGameObject.name);
        if (items[selectedID].id != 0)
        {
            currentItem = CopyInventoryItem(items[selectedID]);
            currentID = selectedID;

            movingObject.gameObject.SetActive(true);
            movingObject.GetComponent<Image>().sprite = data.items[currentItem.id].item_icon;
            Clear(currentID);
        }
        refresh();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        RectTransform droppedSlotRect = null;
        int droppedID = -1;

        for (int i = 0; i < items.Count; i++)
        {
            RectTransform slotRect = items[i].itemGameObject.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(slotRect, Input.mousePosition, cam))
            {
                
                droppedSlotRect = slotRect;
                droppedID = i;
                break;
            }
        }

        if (currentItem != null && currentItem.id != 0 && currentID != -1)
        {
            if (!IsMouseOverUI(Background.GetComponent<RectTransform>()))
            {
                player.dropItem(currentItem.id, currentItem.count);
                currentItem = null;
            }
            else
            {
                if (droppedSlotRect != null && droppedID != -1)
                {
                    if (currentItem.id == items[droppedID].id)
                    {
                        int maxStack = data.items[currentItem.id].stack;
                        int availableSpace = maxStack - items[droppedID].count;

                        if (currentItem.count <= availableSpace)
                        {
                            items[droppedID].count += currentItem.count;
                            Clear(currentID);
                        }
                        else
                        {
                            items[droppedID].count = maxStack;
                            currentItem.count -= availableSpace;
                            AddInventoryItem(currentID, currentItem);
                        }
                        UpdateInventory();
                    }
                    else
                    {
                        AddInventoryItem(currentID, items[droppedID]);
                        AddInventoryItem(droppedID, currentItem);
                    }
                }
                else
                {
                    AddInventoryItem(currentID, currentItem);
                }
            }
            currentID = -1;
            movingObject.gameObject.SetActive(false);
        }
        refresh();
    }

    public void Update()
    {
        if (Input.GetMouseButton(0) && currentID != -1)
        {
            MoveObject();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool f = true;
            foreach (var ui in UIs)
            {
                if (ui.activeSelf)
                {
                    f = false;
                    break;
                }
            }

            if (f)
            {
                ToggleInventory();
            }
        }
        if (Input.mouseScrollDelta.y != 0 && isOpen)
        {
            if (IsMouseOverUI(InventoryRect))
            {
                scrollRect.verticalNormalizedPosition += Input.mouseScrollDelta.y * scrollSpeed;
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
            }
        }
    }
    
    private bool IsMouseOverUI(RectTransform rectTransform)
    {
        Vector2 localMousePosition;
        bool isOver = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            Input.mousePosition,
            Camera.main, 
            out localMousePosition);
        return isOver && rectTransform.rect.Contains(localMousePosition);
    }
    
    public void ToggleInventory()
    {
        isOpen = !isOpen;
        Background.SetActive(isOpen);
        if(_soundManager!=null)
            _soundManager.PlaySound(0, destroyed: !isOpen);
    }

    public void OffInventory()
    {
        isOpen = false;
        Background.SetActive(isOpen);
    }

    public void Clear(int id)
    {
        items[id].count = 0;
        items[id].id = 0;
        items[id].itemCountText.text = "";
        Image childImage = items[id].itemGameObject.transform.Find("Item").GetComponent<Image>();
        childImage.enabled = items[id].id != 0;
        refresh();
    }

    public bool check_space(int item_id)
    {
        foreach (ItemInventory it in items)
        {
            if (it.id == 0)
            {
                return true;
            }
            else if (it.id == item_id && it.count < data.items[item_id].stack)
            {
                return true;
            }
        }

        return false;
    }
    
    public int AddItem(int item_id, int count)
    {
        int st_count = count;
        
        foreach (ItemInventory it in items)
        {
            if (it.id == 0 && count > 0)
            {
                if (count > data.items[item_id].stack)
                {
                    it.count = data.items[item_id].stack;
                    count -= data.items[item_id].stack;
                }
                else
                {
                    it.count = count;
                    count = 0;
                }
                it.id = item_id;
                it.itemIconImage.sprite = data.items[item_id].item_icon;
                it.itemCountText.text = it.count > 1 ? it.count.ToString() : "";

                Image childImage = it.itemGameObject.transform.Find("Item").GetComponent<Image>();
                childImage.enabled = it.id != 0;
            }
            if (item_id == it.id)
            {
                int remainingSpace = data.items[it.id].stack - it.count;

                if (count <= remainingSpace)
                {
                    it.count += count;
                    count = 0;
                    refresh();
                    return st_count;
                }
                else
                {
                    it.count = data.items[it.id].stack;
                    it.itemCountText.text = it.count > 1 ? it.count.ToString() : "";
                    count -= remainingSpace;
                }
            }

            if (count == 0)
            {
                refresh();
                return st_count;
            }
        }
        refresh();
        return st_count-count;
    }


    public void AddInventoryItem(int id, ItemInventory invitem)
    {
        items[id].id = invitem.id;
        items[id].count = invitem.count;
        items[id].itemIconImage.sprite = data.items[invitem.id].item_icon;
        items[id].itemCountText.text = invitem.count > 1 && invitem.id != 0 ? invitem.count.ToString() : "";

        Image childImage = items[id].itemGameObject.transform.Find("Item").GetComponent<Image>();
        childImage.enabled = items[id].id != 0;
    }

    public void AddGraphics()
    {
        for (int i = 0; i < maxCount; i++)
        {
            GameObject newItem = Instantiate(gameObjShow, inventoryMainObject.transform);
            newItem.name = i.ToString();

            ItemInventory ii = new ItemInventory
            {
                itemGameObject = newItem,
                itemIconImage = newItem.transform.Find("Item").GetComponent<Image>(),
                itemCountText = newItem.GetComponentInChildren<TextMeshProUGUI>()
            };
            RectTransform rt = newItem.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;

            EventTrigger trigger = newItem.AddComponent<EventTrigger>();

            EventTrigger.Entry entryDown = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            entryDown.callback.AddListener((eventData) => OnPointerDown((PointerEventData)eventData));
            trigger.triggers.Add(entryDown);

            EventTrigger.Entry entryUp = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            entryUp.callback.AddListener((eventData) => OnPointerUp((PointerEventData)eventData));
            trigger.triggers.Add(entryUp);

            items.Add(ii);
        }
    }

    public void UpdateInventory()
    {
        for (int i = 0; i < maxCount; i++)
        {
            items[i].itemCountText.text = items[i].count > 1 ? items[i].count.ToString() : "";
            items[i].itemIconImage.sprite = data.items[items[i].id].item_icon;

            Image childImage = items[i].itemGameObject.transform.Find("Item").GetComponent<Image>();
            childImage.enabled = items[i].id != 0;
        }
    }

    public void MoveObject()
    {
        Vector3 pos = Input.mousePosition + offset;
        movingObject.position = new Vector3(cam.ScreenToWorldPoint(pos).x,cam.ScreenToWorldPoint(pos).y,0);
    }

    public ItemInventory CopyInventoryItem(ItemInventory old)
    {
        if (old == null) return null;

        return new ItemInventory
        {
            id = old.id,
            count = old.count,
            itemGameObject = old.itemGameObject,
            itemIconImage = old.itemIconImage,
            itemCountText = old.itemCountText
        };
    }
    
    ////////////
    public void SaveInventory()
    {
        List<SerializableItem> serializableItems = new List<SerializableItem>();

        int i = 0;
        foreach (var item in items)
        {
            if (item.id != 0 && item.count > 0)
            {
                serializableItems.Add(new SerializableItem
                {
                    slot = i,
                    id = item.id,
                    count = item.count
                });
            }
            i++;
        }

        string json = JsonConvert.SerializeObject(serializableItems, Formatting.Indented);
        File.WriteAllText(savePath, json);
    }

    public void LoadInventory()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            List<SerializableItem> serializableItems = JsonConvert.DeserializeObject<List<SerializableItem>>(json);
            
            foreach (var item in items)
            {
                item.id = 0;
                item.count = 0;
                item.itemIconImage.sprite = null;
                item.itemCountText.text = "";
            }
            
            foreach (var savedItem in serializableItems)
            {
                var it = new ItemInventory();
                it.id = savedItem.id;
                it.count = savedItem.count;
                AddInventoryItem(savedItem.slot, it);
            }
            
        }
    }

    [System.Serializable]
    public class SerializableItem
    {
        public int slot;
        public int id;
        public int count;
    }
}

[System.Serializable]
public class ItemInventory
{
    public int id;
    public GameObject itemGameObject;
    public int count;

    public Image itemIconImage;
    public TextMeshProUGUI itemCountText;
}
