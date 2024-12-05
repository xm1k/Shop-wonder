using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Ingredient : MonoBehaviour, IPointerDownHandler
{
    public int item_id = 1;
    public int amount = 0;
    public DataBase db;
    public GameObject figure_piece;

    private void Start()
    {
        transform.Find("Image").GetComponent<Image>().sprite = db.items[item_id].item_icon;
        transform.Find("Count").GetComponent<TextMeshProUGUI>().text = amount.ToString();
        string attributes = "";
        foreach (var atribute in db.items[item_id].attributes)
        {
            attributes += atribute.amount.ToString() + " " + atribute.elementType.ToString() + " <sprite name=" + db.GetElementIdentifier(atribute.elementType) + ">\n";
        }
        transform.Find("Structure").GetComponent<TextMeshProUGUI>().text = attributes;
        List<Vector2Int> blockPositionsCopy = new List<Vector2Int>(db.items[item_id].BlockPositions);
    
        foreach (var block in blockPositionsCopy)
        {
            GameObject block_piece = Instantiate(figure_piece, transform.Find("Figure").transform);
            block_piece.transform.position = transform.Find("Figure").transform.TransformPoint(14 * new Vector3(block.x, block.y, 0));
            block_piece.GetComponent<Image>().color = db.items[item_id].figure_color;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(0))
        {
            transform.parent.GetComponent<SpawnIngredients>().SpawnBlock(item_id);
        }
    }
    
}