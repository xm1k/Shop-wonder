using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Loot : MonoBehaviour
{
    public DataBase db;
    public int item_id = 0;
    public int count = 0;
    public Transform player;
    public Inventory inventory;
    public float magnetDistance = 0.9f;
    public float magnetSpeed = 5f;
    private Rigidbody2D rb2d;

    public Vector2 direction = Vector2.zero;

    private SoundsManager _soundManager;

    private void Awake()
    {
        _soundManager = GetComponent<SoundsManager>();
    }
    void Start()
    {
        Init(item_id, count, db, inventory, player);
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Init(int item_id, int count, DataBase db, Inventory inventory, Transform player)
    {
        this.item_id = item_id;
        this.count = count;
        this.db = db;
        this.inventory = inventory;
        this.player = player;
        gameObject.GetComponent<SpriteRenderer>().sprite = db.items[item_id].item_icon;
    }
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (direction != Vector2.zero)
        {
            rb2d.MovePosition(rb2d.position + direction * 0.2f);
            direction -= direction * 0.2f; 
        }
        
        if (distanceToPlayer <= magnetDistance && inventory.check_space(item_id) && direction == Vector2.zero)
        {
            GetComponent<CircleCollider2D>().enabled = false;
            transform.position = Vector2.MoveTowards(transform.position, player.position, magnetSpeed * Time.deltaTime);
            if (distanceToPlayer < 0.1f)
            {
                int new_c = inventory.AddItem(item_id, count);
                // _soundManager.PlaySound(0, destroyed: true);
                count -= new_c;
                if (count <= 0)
                {
                    Destroy(gameObject);
                    
                    MapCreator.LootCount--;
                }
            }
        }
        else
        {
            GetComponent<CircleCollider2D>().enabled = true;
        }
    }
}