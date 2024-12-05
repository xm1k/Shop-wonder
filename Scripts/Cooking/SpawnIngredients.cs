using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnIngredients : MonoBehaviour
{
    public GameObject ingredient;
    public Inventory inventory;
    public DataBase db;
    public GameManager blockpuzzleManager;

    public void refresh()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 1; i < inventory.current_items.Count; i++)
        {
            if (inventory.current_items[i] > 0 && db.items[i].Is_potion == false)
            {
                Ingredient cur_ing = Instantiate(ingredient, transform).GetComponent<Ingredient>();
                cur_ing.db = db;
                cur_ing.item_id = i;
                cur_ing.amount = inventory.current_items[i];
            }
        }
    }

    public void SpawnBlock(int item_id)
    {
        blockpuzzleManager.SpawnBlock(db.items[item_id].BlockPositions, db.items[item_id].figure_color, item_id);
        inventory.delete(item_id, 1);
    }
}
