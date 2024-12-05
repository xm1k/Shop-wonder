using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Book : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Goto(int x)
    {
        foreach (Transform child in transform)
        {
            GameObject obj = child.gameObject;
            if (int.TryParse(obj.name, out int objNumber))
            {
                Image image = obj.GetComponent<Image>();

                if (image != null)
                {
                    image.enabled = (objNumber == x);
                }
            }
        }
    }
    
    void Update()
    {
        
    }
}
