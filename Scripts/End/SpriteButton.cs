using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteButton : MonoBehaviour
{
    private Menu _menu;
    private UnityAnalytics UnityAnalytics;
    private void Awake()
    {
        UnityAnalytics = GetComponent<UnityAnalytics>();
        _menu = GetComponent<Menu>();
    }
    private void OnMouseDown()
    {
        if (UnityAnalytics != null) UnityAnalytics.PlayerClickEndTGChannel();
        _menu.EndToChannel();
    }
}
