using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Color light_green;
    public Color dark_green;
    public Color red;
    
    public void refresh(int x)
    {
        int i = 0;
        if (x >= transform.childCount)
        {
            foreach (Transform child in transform)
            { 
                child.gameObject.GetComponent<Image>().color = red;
            }
        }
        else{
            foreach (Transform child in transform)
            {
                if (i < x)
                {
                    child.gameObject.GetComponent<Image>().color = light_green;
                }
                else
                {
                    child.gameObject.GetComponent<Image>().color = dark_green;
                }

                i++;
            }
        }
    }
}