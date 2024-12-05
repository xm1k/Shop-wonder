using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
public class Potion : MonoBehaviour
{
    private float rotationAngle = 0.0f;
    private float rotationSpeed = 0.3f;
    private float rotationRange = 10.0f;
    private float scaleSpeed = 20.0f;
    private Vector3 maxScale = new Vector3(800.0f, 800.0f, 0.0f);
    
    public Inventory inventory;
    public Item item;

    private int timer = 0;
    
    void Start()
    {
        inventory.AddItem(item.item_id, 1);
        transform.localScale = Vector3.zero;
        if (item.item_id == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = item.item_icon;
        }
    }

    void Update()
    {
        if (transform.localScale.x < maxScale.x && timer == 0)
        {
            transform.localScale += Vector3.one * scaleSpeed;
            if (transform.localScale.x > maxScale.x)
            {
                transform.localScale = maxScale;
            }
        }
        else
        {
            timer++;
            if (rotationSpeed > 0.0f && timer > 50)
            {
                rotationSpeed *= 0.90f;
                if (transform.localScale.x > 0)
                {
                    transform.localScale -= new Vector3(scaleSpeed, scaleSpeed, 0.0f);
                    transform.position += new Vector3(0, -0.05f, 0.0f);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
        
        rotationAngle += rotationSpeed;
        float rotationZ = Mathf.Sin(rotationAngle) * rotationRange;
        transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }
}