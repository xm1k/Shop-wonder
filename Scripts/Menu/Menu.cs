using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    private UnityAnalytics UnityAnalytics;
    private void Start()
    {
        UnityAnalytics = FindObjectOfType<UnityAnalytics>();
    }
    public void MenuComment()
    {
        if(UnityAnalytics!=null) UnityAnalytics.PlayerClickMenuTGComment();
        GoToChannel();
    }
    public void EndToChannel()
    {
        if (UnityAnalytics != null)  UnityAnalytics.PlayerClickEndTGChannel();
        GoToChannel();
    }
    public void MenuToChannel()
    {
        if (UnityAnalytics != null)  UnityAnalytics.PlayerClickMenuTGChannel();
        GoToChannel();
    }
    public void GoToChannel()
    {
        Application.OpenURL("https://t.me/ilure_games");
    }

    public void GoToPrevGame()
    {
        if (UnityAnalytics != null)  UnityAnalytics.PlayerClickMenuOtherGames();
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.ILUREGames.OceanDepthsClicker");
    }
    public void LeaveGame()
    {
        Application.Quit();
    }
}
